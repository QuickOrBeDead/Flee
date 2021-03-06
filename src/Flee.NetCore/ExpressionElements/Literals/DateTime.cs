﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Globalization;
using Flee.ExpressionElements.Base.Literals;

using Flee.InternalTypes;
using Flee.PublicTypes;
using Flee.Resources;

namespace Flee.ExpressionElements.Literals
{
    internal class DateTimeLiteralElement : LiteralElement
    {
        private DateTime _myValue;

        private static readonly ConstructorInfo _dateTimeConstructorInfo;

        static DateTimeLiteralElement()
        {
            _dateTimeConstructorInfo = typeof(DateTime).GetConstructor(new[] { typeof(long) });
        }

        public DateTimeLiteralElement(string image, ExpressionContext context)
        {
            ExpressionParserOptions options = context.ParserOptions;

            if (DateTime.TryParseExact(image, options.DateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out _myValue) == false)
            {
                base.ThrowCompileException(CompileErrorResourceKeys.CannotParseType, CompileExceptionReason.InvalidFormat, typeof(DateTime).Name);
            }
        }

        public override void Emit(FleeILGenerator ilg, IServiceProvider services)
        {
            int index = ilg.GetTempLocalIndex(typeof(DateTime));

            Utility.EmitLoadLocalAddress(ilg, index);

            LiteralElement.EmitLoad(_myValue.Ticks, ilg);

            ilg.Emit(OpCodes.Call, _dateTimeConstructorInfo);

            Utility.EmitLoadLocal(ilg, index);
        }

        public override System.Type ResultType => typeof(DateTime);
    }
}
