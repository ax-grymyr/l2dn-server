namespace L2Dn.GameServer.Utilities;

public struct StringTokenizer
{
    private readonly string _input;
    private readonly string _delimiters;
    private readonly string[] _tokens;
    private int _index = 0;

    public StringTokenizer(string input, string delimiters = " \t\n\r\f")
    {
        _input = input;
        _delimiters = delimiters;
        _tokens = input.Split(delimiters.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        if (_tokens.Length == 1 && string.IsNullOrEmpty(_tokens[0]))
            _tokens = Array.Empty<string>();
    }

    public string nextToken()
    {
        return _index < _tokens.Length ? _tokens[_index++] : string.Empty;
    }

    public bool hasMoreTokens()
    {
        return _index < _tokens.Length;
    }

    public int countTokens()
    {
        return _tokens.Length;
    }

    public bool hasMoreElements()
    {
        return hasMoreTokens();
    }
}