using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Reflection;

using Flee.ExpressionElements.Base;

using Flee.InternalTypes;
using Flee.PublicTypes;
using Flee.Resources;

namespace Flee.ExpressionElements
{
    internal sealed class InElement : ExpressionElement
    {
        // Element we will search for
        private readonly ExpressionElement _myOperand;
        // Elements we will compare against
        private readonly List<ExpressionElement> _myArguments;
        // Collection to look in
        private readonly ExpressionElement _myTargetCollectionElement;
        // Type of the collection
        private Type _myTargetCollectionType;

        private static readonly MethodInfo s_InMethodInfo;

        static InElement()
        {
            s_InMethodInfo = typeof(CustomMethods).GetMethod("IsIn", BindingFlags.Public | BindingFlags.Static);
        }

        // Initialize for searching a list of values
        public InElement(ExpressionElement operand, IList listElements)
        {
            _myOperand = operand;

            ExpressionElement[] arr = new ExpressionElement[listElements.Count];
            listElements.CopyTo(arr, 0);

            _myArguments = new List<ExpressionElement>(arr);
        }

        // Initialize for searching a collection
        public InElement(ExpressionElement operand, ExpressionElement targetCollection)
        {
            _myOperand = operand;
            _myTargetCollectionElement = targetCollection;
            ResolveForCollectionSearch();
        }

        private void ResolveForCollectionSearch()
        {
            // Try to find a collection type
            _myTargetCollectionType = this.GetTargetCollectionType();

            if (_myTargetCollectionType == null)
            {
                ThrowCompileException(CompileErrorResourceKeys.SearchArgIsNotKnownCollectionType, CompileExceptionReason.TypeMismatch, _myTargetCollectionElement.ResultType.Name);
            }

            // Validate that the operand type is compatible with the collection
            MethodInfo mi = GetCollectionContainsMethod();
            ParameterInfo p1 = mi.GetParameters()[0];

            if (ImplicitConverter.EmitImplicitConvert(_myOperand.ResultType, p1.ParameterType, null) == false)
            {
                ThrowCompileException(CompileErrorResourceKeys.OperandNotConvertibleToCollectionType, CompileExceptionReason.TypeMismatch, _myOperand.ResultType.Name, p1.ParameterType.Name);
            }
        }

        private Type GetTargetCollectionType()
        {
            Type collType = _myTargetCollectionElement.ResultType;

            // Try to see if the collection is a generic ICollection or IDictionary
            Type[] interfaces = collType.GetInterfaces();

            foreach (Type interfaceType in interfaces)
            {
                if (interfaceType.IsGenericType == false)
                {
                    continue;
                }

                Type genericTypeDef = interfaceType.GetGenericTypeDefinition();

                if (ReferenceEquals(genericTypeDef, typeof(ICollection<>)) | ReferenceEquals(genericTypeDef, typeof(IDictionary<,>)))
                {
                    return interfaceType;
                }
            }

            // Try to see if it is a regular IList or IDictionary
            if (typeof(IList<>).IsAssignableFrom(collType))
            {
                return typeof(IList<>);
            }

            if (typeof(IDictionary<,>).IsAssignableFrom(collType))
            {
                return typeof(IDictionary<,>);
            }

            // Not a known collection type
            return null;
        }

        public override void Emit(FleeILGenerator ilg, IServiceProvider services)
        {
            if ((_myTargetCollectionType != null))
            {
                this.EmitCollectionIn(ilg, services);

            }
            else
            {
                EmitCustomInMethod(ilg, services);
                //return;

                //BranchManager bm = new BranchManager();
                //bm.GetLabel("endLabel", ilg);
                //bm.GetLabel("trueTerminal", ilg);

                //// Do a fake emit to get branch positions
                //FleeILGenerator ilgTemp = this.CreateTempFleeILGenerator(ilg);
                //Utility.SyncFleeILGeneratorLabels(ilg, ilgTemp);

                //this.EmitListIn(ilgTemp, services, bm);

                //bm.ComputeBranches();

                //// Do the real emit
                //this.EmitListIn(ilg, services, bm);
            }
        }

        private void EmitCollectionIn(FleeILGenerator ilg, IServiceProvider services)
        {
            // Get the contains method
            MethodInfo mi = this.GetCollectionContainsMethod();
            ParameterInfo p1 = mi.GetParameters()[0];

            // Load the collection
            _myTargetCollectionElement.Emit(ilg, services);
            // Load the argument
            _myOperand.Emit(ilg, services);
            // Do an implicit convert if necessary
            ImplicitConverter.EmitImplicitConvert(_myOperand.ResultType, p1.ParameterType, ilg);
            // Call the contains method
            ilg.Emit(OpCodes.Callvirt, mi);
        }

        private MethodInfo GetCollectionContainsMethod()
        {
            string methodName = "Contains";

            if (_myTargetCollectionType.IsGenericType && ReferenceEquals(_myTargetCollectionType.GetGenericTypeDefinition(), typeof(IDictionary<,>)))
            {
                methodName = "ContainsKey";
            }

            return _myTargetCollectionType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        }

        private void EmitCustomInMethod(FleeILGenerator ilg, IServiceProvider services)
        {
            int arraySize = _myArguments.Count;
            Type arrayElementType = _myOperand.ResultType;
            Type arrayType = arrayElementType.MakeArrayType();

            // Load the array length
            LdcI4(ilg, arraySize);

            // Create the array
            ilg.Emit(OpCodes.Newarr, arrayElementType);

            // Store the new array in a unique local and remember the index
            LocalBuilder local = ilg.DeclareLocal(arrayType);
            int arrayLocalIndex = local.LocalIndex;
            Utility.EmitStoreLocal(ilg, arrayLocalIndex);
           
            for (int i = 0; i < _myArguments.Count; i++)
            {
                ExpressionElement argumentElement = _myArguments[i];

                // Load the array
                Utility.EmitLoadLocal(ilg, arrayLocalIndex);

                // Load the index
                LdcI4(ilg, i);

                EmitChild(argumentElement, arrayElementType, ilg, services);

                // Store it into the array
                Utility.EmitArrayStore(ilg, arrayElementType);
            }

            EmitChild(_myOperand, arrayElementType, ilg, services);

            // Load the array
            Utility.EmitLoadLocal(ilg, arrayLocalIndex);

            MethodInfo genericMethod = s_InMethodInfo.MakeGenericMethod(arrayElementType);

            ilg.Emit(OpCodes.Call, genericMethod);
        }

        private static void EmitChild(ExpressionElement child, Type resultType, FleeILGenerator ilg, IServiceProvider services)
        {
            child.Emit(ilg, services);
            bool converted = ImplicitConverter.EmitImplicitConvert(child.ResultType, resultType, ilg);
            Debug.Assert(converted, "convert failed");
        }

        private static void LdcI4(FleeILGenerator ilg, int index)
        {
            switch (index)
            {
                case 0: ilg.Emit(OpCodes.Ldc_I4_0); break;
                case 1: ilg.Emit(OpCodes.Ldc_I4_1); break;
                case 2: ilg.Emit(OpCodes.Ldc_I4_2); break;
                case 3: ilg.Emit(OpCodes.Ldc_I4_3); break;
                case 4: ilg.Emit(OpCodes.Ldc_I4_4); break;
                case 5: ilg.Emit(OpCodes.Ldc_I4_5); break;
                case 6: ilg.Emit(OpCodes.Ldc_I4_6); break;
                case 7: ilg.Emit(OpCodes.Ldc_I4_7); break;
                case 8: ilg.Emit(OpCodes.Ldc_I4_8); break;
                default:
                    if (index < 256)
                    {
                        ilg.Emit(OpCodes.Ldc_I4_S, (byte)index);
                    }
                    else
                    {
                        ilg.Emit(OpCodes.Ldc_I4, (ushort)index);
                    }
                    break;
            }
        }

        public override Type ResultType => typeof(bool);
    }
}
