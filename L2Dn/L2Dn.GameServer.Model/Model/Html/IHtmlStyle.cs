namespace L2Dn.GameServer.Model.Html;

public interface IHtmlStyle
{
    string applyBypass(string bypass, string name, bool isEnabled);
    string applySeparator();
}