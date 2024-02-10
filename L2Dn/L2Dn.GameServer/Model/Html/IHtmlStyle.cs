namespace L2Dn.GameServer.Model.Html;

public interface IHtmlStyle
{
    String applyBypass(String bypass, String name, bool isEnabled);
    String applySeparator();
}