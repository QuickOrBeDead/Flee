using System;
using System.Reflection;
using System.Reflection.Emit;

using Flee.ExpressionElements.Base.Literals;
using Flee.InternalTypes;
using Flee.PublicTypes;
using Flee.Resources;

namespace Flee.ExpressionElements.Literals
{
    internal class TimeSpanLiteralElement : LiteralElement
    {
        private TimeSpan _myValue;

        private static readonly ConstructorInfo _timeSpanConstructor = typeof(TimeSpan).GetConstructor(new[] { typeof(long) });

        public TimeSpanLiteralElement(string image)
        {
            if (TimeSpan.TryParse(image, out _myValue) == false)
            {
                ThrowCompileException(CompileErrorResourceKeys.CannotParseType, CompileExceptionReason.InvalidFormat, typeof(TimeSpan).Name);
            }
        }

        public override void Emit(FleeILGenerator ilg, IServiceProvider services)
        {
            int index = ilg.GetTempLocalIndex(typeof(TimeSpan));

            Utility.EmitLoadLocalAddress(ilg, index);

            LiteralElement.EmitLoad(_myValue.Ticks, ilg);

            ilg.Emit(OpCodes.Call, _timeSpanConstructor);

            Utility.EmitLoadLocal(ilg, index);
        }

        public override Type ResultType => typeof(TimeSpan);
    }
}
