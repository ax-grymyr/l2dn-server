namespace L2Dn.GameServer.Model.ClientStrings;

public class BuilderText: Builder
{
    private readonly string _text;

    public BuilderText(string text)
    {
        _text = text;
    }

    public override string toString(object param)
    {
        return ToString();
    }

    public override string toString(params object[] @params)
    {
        return ToString();
    }

    public override int getIndex()
    {
        return -1;
    }

    public override string ToString()
    {
        return _text;
    }
}