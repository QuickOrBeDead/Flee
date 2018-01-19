namespace Flee.NetCore.Tests
{
    using System.Globalization;

    using Flee.PublicTypes;

    using NUnit.Framework;

    [TestFixture]
    public class BetweenExpressionFixture
    {
        [Test]
        public void T()
        {
            ExpressionContext context2 = new ExpressionContext();
            IGenericExpression<decimal> ge1 = context2.CompileGeneric<decimal>("1/2");
            decimal result3 = ge1.Evaluate();

            ExpressionContext context = new ExpressionContext();
            context.ParserOptions.DecimalSeparator = '.';
            context.ParserOptions.FunctionArgumentSeparator = ',';
            context.Options.ParseCulture = CultureInfo.InvariantCulture;

            var e1 = context.CompileGeneric<bool>("#01/01/2011# < #02/02/2011#");
            var result = e1.Evaluate();

            Assert.AreEqual(true, result);
        }
    }
}
