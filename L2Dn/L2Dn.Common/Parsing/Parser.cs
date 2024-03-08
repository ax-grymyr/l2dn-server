using System.Collections.Immutable;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace L2Dn.Parsing;

public readonly record struct ParserInput(string Input, int Position)
{
    public char? Current => Input.Length > Position ? Input[Position] : null;

    public string? Substring(int length) =>
        Input.Length >= Position + length ? Input.Substring(Position, length) : null;
    
    public ParserInput Next() => this with { Position = Position + 1 };

    public static implicit operator ParserInput(string input) => new(input, 0);
}

public readonly record struct ParserError(string Error, ParserInput Input);

public readonly record struct ParserResult<T>(bool Success, T Result, ParserInput Input, string Error)
{
    public static implicit operator ParserResult<T>(ParserError error) => new(false, default, error.Input, error.Error);
}

public delegate ParserResult<T> Parser<T>(ParserInput input);

public static class Parse
{
    private static readonly Parser<string> _ident =
        Char(c => char.IsLetter(c) || c == '_', "Identifier expected")
            .Then(Char(c => char.IsLetterOrDigit(c) || c == '_', string.Empty).Many())
            .Select(t => t.Item1 + t.Item2);

    private static readonly Parser<string> _stringLiteral =
        Char('"').Then(Char(c => c != '"', "End of string expected").Many()).Select(t => t.Item2)
            .Then(Char('"')).Select(t => t.Item1).AtLeastOnce().Select(c => string.Join("\"", c));

    private static readonly Parser<string> _whitespaces =
        Char(char.IsWhiteSpace, "Whitespace expected").AtLeastOnce();

    private static readonly Parser<string> _digits =
        Char(c => c is >= '0' and <= '9', "Digit expected").AtLeastOnce();

    private static readonly Parser<string> _plusOrMinusSign =
        Char('-').Or(Char('+')).Select(c => c.ToString()).Optional(string.Empty);

    private static readonly Parser<string> _integer =
        _plusOrMinusSign.Then(Char(c => c is >= '0' and <= '9', "Digit expected").AtLeastOnce())
            .Select(t => t.Item1 + t.Item2);

    private static readonly Parser<string> _fixedPointNumber =
        _integer.Then(Char('.').Then(_digits).Select(t => t.Item1 + t.Item2).Optional(string.Empty))
            .Select(t => t.Item1 + t.Item2);

    private static readonly Parser<string> _floatingPointNumber =
        _fixedPointNumber.Then(Char('e', true).Then(_integer).Select(t => t.Item1 + t.Item2).Optional(string.Empty))
            .Select(t => t.Item1 + t.Item2);

    private static readonly Parser<float> _float = _floatingPointNumber
        .Select(f => float.Parse(f, CultureInfo.InvariantCulture)).Catch("Floating point number overflow");

    private static readonly Parser<double> _double = _floatingPointNumber
        .Select(f => double.Parse(f, CultureInfo.InvariantCulture)).Catch("Floating point number overflow");

    private static readonly Parser<int> _int32 =
        _integer.Select(integer => int.Parse(integer, CultureInfo.InvariantCulture))
            .WithErrorMessage("Integer expected").Catch("Integer overflow");

    private static readonly Parser<long> _int64 =
        _integer.Select(integer => long.Parse(integer, CultureInfo.InvariantCulture))
            .WithErrorMessage("Integer expected").Catch("Integer overflow");

    private static readonly Parser<BigInteger> _bigint =
        _integer.Select(integer => BigInteger.Parse(integer, CultureInfo.InvariantCulture))
            .WithErrorMessage("Integer expected").Catch("Integer overflow");

    private static readonly Parser<string> _newLine = String("\x000D\x000A").Or(String("\x000D"),
        String("\x000A"), String("\x0085"), String("\x2028"), String("\x2029"));

    public static Parser<string> Ident => _ident;
    public static Parser<string> StringLiteral => _stringLiteral;
    public static Parser<string> Whitespaces => _whitespaces;
    public static Parser<float> Float => _float;
    public static Parser<double> Double => _double;
    public static Parser<int> Int32 => _int32;
    public static Parser<long> Int64 => _int64;
    public static Parser<BigInteger> BigInt => _bigint;
    public static Parser<string> NewLine => _newLine;

    public static Parser<char> Char(char c) => input =>
        input.Current == c ? Success(c, input.Next()) : Error($"'{c}' expected", input);

    public static Parser<char> Char(char c, bool ignoreCase, CultureInfo? cultureInfo = null)
    {
        if (!ignoreCase)
            return Char(c);

        CultureInfo culture = cultureInfo ?? CultureInfo.CurrentCulture;

        char lowChar = char.ToLower(c, culture);
        char highChar = char.ToUpper(c, culture);
        if (c == lowChar && c == highChar)
            return Char(c);

        return input =>
        {
            char? current = input.Current;
            return current == c || current == lowChar || current == highChar
                ? Success(current.Value, input.Next())
                : Error($"'{c}' expected", input);
        };
    }

    public static Parser<string> String(string str) => input =>
        string.Equals(input.Substring(str.Length), str, StringComparison.Ordinal)
            ? Success(str, input with { Position = input.Position + str.Length })
            : Error($"'{str}' expected", input);

    public static Parser<string> String(string str, bool ignoreCase)
    {
        if (!ignoreCase)
            return String(str);

        return input =>
        {
            string? subStr = input.Substring(str.Length);
            return subStr != null && string.Equals(subStr, str, StringComparison.Ordinal)
                ? Success(subStr, input with { Position = input.Position + str.Length })
                : Error($"'{str}' expected", input);
        };
    }

    public static Parser<char> Char(Func<char, bool> predicate, string errorMessage) => input =>
    {
        char? current = input.Current;
        return current != null && predicate(current.Value)
            ? Success(current.Value, input.Next())
            : Error(errorMessage, input);
    };

    public static Parser<TResult> Fail<TResult>(string errorMessage) => input => Error(errorMessage, input);

    public static Parser<TResult> WithErrorMessage<TResult>(this Parser<TResult> parser, string errorMessage) =>
        input => parser(input) with { Error = errorMessage };

    public static Parser<TResult> Success<TResult>(TResult value) => input => Success(value, input);

    public static Parser<TResult> Catch<TResult>(this Parser<TResult> parser, string? errorMessage = null) => input =>
    {
        try
        {
            return parser(input);
        }
        catch (Exception exception)
        {
            return Error(errorMessage ?? exception.Message, input);
        }
    };

    public static Parser<TResult> Select<T, TResult>(this Parser<T> parser, Func<T, TResult> selector) => input => 
    {
        ParserResult<T> result = parser(input);
        return result.Success
            ? Success(selector(result.Result), result.Input)
            : Error(result.Error, input);
    };

    public static Parser<(T1, T2)> Then<T1, T2>(this Parser<T1> parser, Parser<T2> otherParser)
    {
        return input =>
        {
            ParserResult<T1> result = parser(input);
            if (!result.Success)
                return Error(result.Error, input);

            ParserResult<T2> otherResult = otherParser(result.Input);
            if (!otherResult.Success)
                return Error(otherResult.Error, input);

            return Success((result.Result, otherResult.Result), otherResult.Input,
                !string.IsNullOrEmpty(otherResult.Error) ? otherResult.Error : result.Error);
        };
    }

    public static Parser<TResult> SelectMany<T, TOther, TResult>(
        this Parser<T> parser, Func<T, Parser<TOther>> selectFunc,
        Func<T, TOther, TResult> resultFunc)
    {
        return input =>
        {
            ParserResult<T> result = parser(input);
            if (!result.Success)
                return Error(result.Error, input);

            ParserResult<TOther> otherResult = selectFunc(result.Result)(result.Input);
            if (!otherResult.Success)
                return Error(otherResult.Error, input);

            return Success(resultFunc(result.Result, otherResult.Result), otherResult.Input,
                !string.IsNullOrEmpty(otherResult.Error) ? otherResult.Error : result.Error);
        };
    }

    public static Parser<TResult> Or<TResult>(this Parser<TResult> parser, Parser<TResult> other) => input =>
    {
        ParserResult<TResult> result = parser(input);
        if (result.Success)
            return result;

        // loop for other parsers
        ParserResult<TResult> result1 = other(input);
        if (result1.Success)
            return result1;

        return result1.Input.Position > result.Input.Position ? result1 : result;
    };

    public static Parser<TResult> Or<TResult>(this Parser<TResult> parser, Parser<TResult> other1,
        Parser<TResult> other2) => input =>
    {
        ParserResult<TResult> result = parser(input);
        if (result.Success)
            return result;

        // loop for other parsers
        ParserResult<TResult> result1 = other1(input);
        if (result1.Success)
            return result1;

        if (result1.Input.Position > result.Input.Position)
            result = result1;

        result1 = other2(input);
        if (result1.Success)
            return result1;

        if (result1.Input.Position > result.Input.Position)
            result = result1;

        // end of loop
        return result;
    };

    public static Parser<TResult> Or<TResult>(this Parser<TResult> parser, params Parser<TResult>[] others) => input =>
    {
        ParserResult<TResult> result = parser(input);
        if (result.Success)
            return result;

        // loop for other parsers
        foreach (Parser<TResult> other in others)
        {
            ParserResult<TResult> result1 = other(input);
            if (result1.Success)
                return result1;

            if (result1.Input.Position > result.Input.Position)
                result = result1;
        }

        // end of loop
        return result;
    };

    public static Parser<ImmutableList<TResult>> Many<TResult>(this Parser<TResult> parser) => input =>
    {
        ParserResult<TResult> result = parser(input);
        if (!result.Success)
            return Success(ImmutableList<TResult>.Empty, input, result.Error);

        List<TResult> list = new();
        list.Add(result.Result);

        result = parser(result.Input);
        while (result.Success)
        {
            list.Add(result.Result);
            result = parser(result.Input);
        }

        return Success(list.ToImmutableList(), result.Input, result.Error);
    };

    public static Parser<string> Many(this Parser<char> parser) => input =>
    {
        ParserResult<char> result = parser(input);
        if (!result.Success)
            return Success(string.Empty, input, result.Error);

        StringBuilder sb = new();
        sb.Append(result.Result);

        result = parser(result.Input);
        while (result.Success)
        {
            sb.Append(result.Result);
            result = parser(result.Input);
        }

        return Success(sb.ToString(), result.Input, result.Error);
    };

    public static Parser<string> Many(this Parser<string> parser) => input =>
    {
        ParserResult<string> result = parser(input);
        if (!result.Success)
            return Success(string.Empty, input, result.Error);

        StringBuilder sb = new();
        sb.Append(result.Result);

        result = parser(result.Input);
        while (result.Success)
        {
            sb.Append(result.Result);
            result = parser(result.Input);
        }

        return Success(sb.ToString(), result.Input, result.Error);
    };

    public static Parser<ImmutableList<TResult>> Many<TResult, TSeparator>(this Parser<TResult> parser,
        Parser<TSeparator> separatorParser) => input =>
    {
        ParserResult<TResult> result = parser(input);
        if (!result.Success)
            return Success(ImmutableList<TResult>.Empty, input, result.Error);

        List<TResult> list = new();
        list.Add(result.Result);

        ParserResult<TSeparator> separatorResult = separatorParser(result.Input);
        while (separatorResult.Success)
        {
            result = parser(separatorResult.Input);
            if (!result.Success)
                return Success(list.ToImmutableList(), result.Input, result.Error);

            list.Add(result.Result);
            separatorResult = separatorParser(result.Input);
        }

        return Success(list.ToImmutableList(), result.Input, separatorResult.Error);
    };

    public static Parser<ImmutableList<TResult>> ManyWhen<TResult, TSeparator>(
        this Parser<TResult> parser, Parser<TSeparator> separatorParser) => input =>
    {
        ParserResult<TResult> result = parser(input);
        if (!result.Success)
            return Success(ImmutableList<TResult>.Empty, input, result.Error);

        List<TResult> list = new();
        list.Add(result.Result);

        ParserResult<TSeparator> separatorResult = separatorParser(result.Input);
        while (separatorResult.Success)
        {
            result = parser(separatorResult.Input);
            if (!result.Success)
                return Error(result.Error, input);

            list.Add(result.Result);
            separatorResult = separatorParser(result.Input);
        }

        return Success(list.ToImmutableList(), result.Input, separatorResult.Error);
    };

    public static Parser<TResult> AtLeastOnce<TResult, TSeparator>(
        this Parser<TResult> parser, Parser<TSeparator> separatorParser,
        Func<TResult, TSeparator, TResult, TResult> aggregationFunction) => input =>
    {
        ParserResult<TResult> result = parser(input);
        if (!result.Success)
            return result;

        TResult value = result.Result;
        ParserResult<TSeparator> separatorResult = separatorParser(result.Input);
        while (separatorResult.Success)
        {
            result = parser(separatorResult.Input);
            if (!result.Success)
                return Success(value, result.Input, result.Error);

            value = aggregationFunction(value, separatorResult.Result, result.Result);
            separatorResult = separatorParser(result.Input);
        }

        return Success(value, result.Input, separatorResult.Error);
    };

    public static Parser<TResult> AtLeastOnceWhen<TResult, TSeparator>(
        this Parser<TResult> parser, Parser<TSeparator> separatorParser,
        Func<TResult, TSeparator, TResult, TResult> aggregationFunction) => input =>
    {
        ParserResult<TResult> result = parser(input);
        if (!result.Success)
            return result;

        TResult value = result.Result;
        ParserResult<TSeparator> separatorResult = separatorParser(result.Input);
        while (separatorResult.Success)
        {
            result = parser(separatorResult.Input);
            if (!result.Success)
                return result;

            value = aggregationFunction(value, separatorResult.Result, result.Result);
            separatorResult = separatorParser(result.Input);
        }

        return Success(value, result.Input, separatorResult.Error);
    };

    public static Parser<IEnumerable<TResult>> AtLeastOnce<TResult>(
        this Parser<TResult> parser) =>
        parser.Then(parser.Many()).Select(t => t.Item2.Prepend(t.Item1));

    public static Parser<string> AtLeastOnce(this Parser<char> parser) =>
        parser.Then(parser.Many()).Select(t => t.Item1 + t.Item2);

    public static Parser<ImmutableList<TResult>> AtLeastOnce<TResult, TSeparator>(
        this Parser<TResult> parser, Parser<TSeparator> separatorParser) =>
        parser.Then(separatorParser.Then(parser).Select(t => t.Item2).Many())
            .Select(t => t.Item2.Insert(0, t.Item1));

    public static Parser<ImmutableList<TResult>> AtLeastOnceWhen<TResult, TSeparator>(
        this Parser<TResult> parser, Parser<TSeparator> separatorParser) => input =>
    {
        ParserResult<TResult> result = parser(input);
        if (!result.Success)
            return Error(result.Error, input);

        List<TResult> list = new List<TResult>();
        list.Add(result.Result);

        ParserResult<TSeparator> separatorResult = separatorParser(result.Input);
        while (separatorResult.Success)
        {
            result = parser(separatorResult.Input);
            if (!result.Success)
                return Error(result.Error, input);

            list.Add(result.Result);
            separatorResult = separatorParser(result.Input);
        }

        return Success(list.ToImmutableList(), result.Input, separatorResult.Error);
    };

    public static Parser<string> AsText(this Parser<IEnumerable<char>> parser) => parser.Select(CollectChars);

    public static Parser<TResult> Optional<TResult>(this Parser<TResult> parser,
        TResult defaultValue = default) => input =>
    {
        ParserResult<TResult> result = parser(input);
        return result.Success ? result : Success(defaultValue, input, result.Error);
    };

    public static Parser<T> Ref<T>(Func<Parser<T>> parserRef) => input => parserRef()(input);

    public static Parser<TResult> End<TResult>(this Parser<TResult> parser,
        string? errorMessage = null) => input =>
    {
        ParserResult<TResult> result = parser(input);
        return result.Input.Current is null ? result : Error("End of input expected", input);
    };

    public static Parser<TResult> Where<TResult>(this Parser<TResult> parser,
        Func<TResult, bool> predicate, string? errorMessage = null) => input =>
    {
        ParserResult<TResult> result = parser(input);
        return result.Success && !predicate(result.Result) ? Error(errorMessage ?? "Condition fails", input) : result;
    };

    public static Parser<T> Enclose<T, TLeft, TRight>(this Parser<T> parser,
        Parser<TLeft> leftParser, Parser<TRight> rightParser) =>
        leftParser.Then(parser).Select(t => t.Item2).Then(rightParser).Select(t => t.Item1);

    public static Parser<T> Enclose<T, TEnclosing>(this Parser<T> parser,
        Parser<TEnclosing> enclosingParser) => parser.Enclose(enclosingParser, enclosingParser);

    public static Parser<TResult> Trim<TResult>(this Parser<TResult> parser) =>
        _whitespaces.Optional().Then(parser).Select(t => t.Item2).Then(_whitespaces.Optional())
            .Select(t => t.Item1);

    private static ParserResult<T> Success<T>(T result, ParserInput next, string? errorMessage = null) =>
        new(true, result, next, errorMessage ?? string.Empty);
    
    private static ParserError Error(string error, ParserInput input) => new(error, input);

    private static string CollectChars(IEnumerable<char> chars)
    {
        StringBuilder sb = new();
        foreach (char c in chars)
            sb.Append(c);

        return sb.ToString();
    }

}