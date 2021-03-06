﻿using Flee.CalcEngine.PublicTypes;
using Flee.PublicTypes;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Flee.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            //Sample Scenario 1
            ExpressionContext context = new ExpressionContext();
            context.ParserOptions.DecimalSeparator = '.';
            context.ParserOptions.FunctionArgumentSeparator = ',';
            VariableCollection variables = context.Variables;
            variables.Add("a", 1);
            variables.Add("b", 1);

            IGenericExpression<bool> e = context.CompileGeneric<bool>("a=1 AND b=0");
            bool result = e.Evaluate();

            //Sample Scenario 2
            ExpressionContext context2 = new ExpressionContext();
            VariableCollection variables2 = context2.Variables;
            variables2.Add("a", 100);
            variables2.Add("b", 1);
            variables2.Add("c", 24);

            IGenericExpression<bool> ge = context2.CompileGeneric<bool>("(a = 100 OR b > 0) AND c <> 2");
            bool result2 = ge.Evaluate();

            IGenericExpression<decimal> ge1 = context2.CompileGeneric<decimal>("1/2");
            decimal result3 = ge1.Evaluate();

            System.Console.ReadKey();
        }
    }
}
