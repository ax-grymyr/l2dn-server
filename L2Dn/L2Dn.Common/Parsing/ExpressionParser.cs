namespace L2Dn.Parsing;

public enum ExpressionType
{
    Number,
    Variable,
    UnaryOperator,
    BinaryOperator
}

public enum UnaryOperator
{
    Plus,
    Minus
}

public enum BinaryOperator
{
    Add,
    Subtract,
    Multiply,
    Divide
}

public abstract record Expression(ExpressionType Type)
{
    public abstract double Evaluate(IDictionary<string, double> variables);
}

public sealed record NumberExpression(double Number): Expression(ExpressionType.Number)
{
    public override double Evaluate(IDictionary<string, double> variables) => Number;
}

public sealed record VariableExpression(string Variable): Expression(ExpressionType.Variable)
{
    public override double Evaluate(IDictionary<string, double> variables) =>
        variables.TryGetValue(Variable, out double value)
            ? value
            : throw new InvalidOperationException($"Variable '{Variable}' is not defined");
}

public sealed record UnaryOperatorExpression(UnaryOperator Operator, Expression Argument): Expression(ExpressionType.UnaryOperator)
{
    public override double Evaluate(IDictionary<string, double> variables) =>
        Operator switch
        {
            UnaryOperator.Plus => Argument.Evaluate(variables),
            UnaryOperator.Minus => -Argument.Evaluate(variables),
            _ => throw new InvalidOperationException("Unknown unary operator")
        };
}

public sealed record BinaryOperatorExpression(BinaryOperator Operator, Expression Argument1, Expression Argument2): Expression(ExpressionType.BinaryOperator)
{
    public override double Evaluate(IDictionary<string, double> variables) =>
        Operator switch
        {
            BinaryOperator.Add => Argument1.Evaluate(variables) + Argument2.Evaluate(variables),
            BinaryOperator.Subtract => Argument1.Evaluate(variables) - Argument2.Evaluate(variables),
            BinaryOperator.Multiply => Argument1.Evaluate(variables) * Argument2.Evaluate(variables),
            BinaryOperator.Divide => Argument1.Evaluate(variables) / Argument2.Evaluate(variables),
            _ => throw new InvalidOperationException("Unknown binary operator")
        };
}

public static class ExpressionParser
{
    private static readonly Parser<Expression> _expressionParser;

    static ExpressionParser()
    {
        Parser<Expression> numberParser = Parse.Double.Select(x => (Expression)new NumberExpression(x));

        Parser<Expression> variableParser = Parse.Ident.Select(x => (Expression)new VariableExpression(x));

        Parser<Expression> subExprParser = Parse.Char('(')
            .Then(Parse.Ref(() => _expressionParser ?? Parse.Fail<Expression>("Parser not assigned")))
            .Then(Parse.Char(')'))
            .Select(t => t.Item1.Item2);

        Parser<Expression> simpleExprParser = numberParser.Or(variableParser, subExprParser).Trim();

        Parser<UnaryOperator> unaryOperation = Parse.Char('+').Select(_ => UnaryOperator.Plus)
            .Or(Parse.Char('-').Select(_ => UnaryOperator.Minus));

        Parser<Expression> unaryExprParser = unaryOperation.Many().Then(simpleExprParser).Select(tuple =>
            tuple.Item1.Reverse().Aggregate(tuple.Item2,
                (left, right) => new UnaryOperatorExpression(right, left)));
        
        Parser<BinaryOperator> productOperation = Parse.Char('*').Select(_ => BinaryOperator.Multiply)
            .Or(Parse.Char('/').Select(_ => BinaryOperator.Divide));

        Parser<Expression> productParser =
            unaryExprParser.Then(productOperation.Then(unaryExprParser).Many()).Select(tuple =>
                tuple.Item2.Aggregate(tuple.Item1,
                    (left, right) => new BinaryOperatorExpression(right.Item1, left, right.Item2)));

        Parser<BinaryOperator> sumOperation = Parse.Char('+').Select(_ => BinaryOperator.Add)
            .Or(Parse.Char('-').Select(_ => BinaryOperator.Subtract));

        Parser<Expression> sumParser =
            productParser.Then(sumOperation.Then(productParser).Many()).Select(tuple =>
                tuple.Item2.Aggregate(tuple.Item1,
                    (left, right) => new BinaryOperatorExpression(right.Item1, left, right.Item2)));

        _expressionParser = sumParser;
    }

    public static Parser<Expression> Parser => _expressionParser;
}