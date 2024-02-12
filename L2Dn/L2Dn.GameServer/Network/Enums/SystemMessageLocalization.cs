using L2Dn.GameServer.Model.ClientStrings;

namespace L2Dn.GameServer.Network.Enums;

public readonly struct SystemMessageLocalization(string text)
{
    private readonly Builder _builder = Builder.newBuilder(text);

    public string getLocalisation(params object[] parameters)
    {
        return _builder.toString(parameters);
    }
}