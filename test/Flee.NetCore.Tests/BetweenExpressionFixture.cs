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
            ExpressionContext context = new ExpressionContext();
            context.ParserOptions.DecimalSeparator = '.';
            context.ParserOptions.FunctionArgumentSeparator = ',';
            context.Options.ParseCulture = CultureInfo.InvariantCulture;

            var e1 = context.CompileGeneric<bool>("5 between 2 , 10 and 1 = 1");
            var result = e1.Evaluate();

            Assert.AreEqual(true, result);
        }
    }
}
