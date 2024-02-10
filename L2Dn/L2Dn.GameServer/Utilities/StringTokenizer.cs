namespace L2Dn.GameServer.Utilities;

public struct StringTokenizer
{
    private readonly string _input;
    private readonly string _delimiters;

    public StringTokenizer(string input, string delimiters)
    {
        _input = input;
        _delimiters = delimiters;
    }

    public string nextToken()
    {
        throw new NotImplementedException();
    }

    public bool hasMoreTokens()
    {
        throw new NotImplementedException();
    }
}