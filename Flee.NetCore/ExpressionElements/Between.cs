namespace Flee.ExpressionElements
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Reflection.Emit;

    using Flee.ExpressionElements.Base;
    using Flee.InternalTypes;

    internal sealed class BetweenElement : ExpressionElement
    {
        // Element we will search for
        private readonly ExpressionElement _operand;
        // Element we will compare from
        private readonly ExpressionElement _from;
        // Element we will compare to
        private readonly ExpressionElement _to;

        private static readonly MethodInfo _betweenMethodInfo;

        static BetweenElement()
        {
            _betweenMethodInfo = typeof(CustomMethods).GetMethod("IsBetween", BindingFlags.Public | BindingFlags.Static);
        }

        // Initialize for between two expressions
        public BetweenElement(ExpressionElement operand, ExpressionElement from, ExpressionElement to)
        {
            _operand = operand;
            _from = from;
            _to = to;

            // TODO: Validate parameter types
        }

        public override void Emit(FleeILGenerator ilg, IServiceProvider services)
        {
            EmitChild(_operand, _operand.ResultType, ilg, services);
            EmitChild(_from, _operand.ResultType, ilg, services);
            EmitChild(_to, _operand.ResultType, ilg, services);

            ilg.Emit(OpCodes.Call, _betweenMethodInfo.MakeGenericMethod(_operand.ResultType));
        }

        private static void EmitChild(ExpressionElement child, Type resultType, FleeILGenerator ilg, IServiceProvider services)
        {
            child.Emit(ilg, services);
            bool converted = ImplicitConverter.EmitImplicitConvert(child.ResultType, resultType, ilg);
            Debug.Assert(converted, "convert failed");
        }

        public override Type ResultType => typeof(bool);
    }
}