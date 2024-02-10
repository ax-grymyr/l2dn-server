namespace L2Dn.GameServer.Model.ClientStrings;

public class BuilderText: Builder
{
    private readonly String _text;

    public BuilderText(String text)
    {
        _text = text;
    }

    public override String toString(Object param)
    {
        return ToString();
    }

    public override String toString(params Object[] @params)
    {
        return ToString();
    }

    public override int getIndex()
    {
        return -1;
    }

    public override String ToString()
    {
        return _text;
    }
}