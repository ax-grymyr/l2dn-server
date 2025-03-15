using FluentAssertions;
using L2Dn.Parsing;

namespace L2Dn.Common.Tests.Parsing;

public class ExpressionParserTests
{
    [Fact]
    public void ParseResult()
    {
        Parse.Char('1')("1").Success.Should().BeTrue();
        Parse.Int32("123").Success.Should().BeTrue();
        ExpressionParser.Parser("123").Success.Should().BeTrue();
        ExpressionParser.Parser("a").Success.Should().BeTrue();
        ExpressionParser.Parser("a + b").Success.Should().BeTrue();
        ExpressionParser.Parser("-15 + (125 * 89 + a * 2 * b) - (---123)").Success.Should().BeTrue();
    }
}