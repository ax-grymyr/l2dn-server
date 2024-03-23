using L2Dn.GameServer.Data;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Model.Actor.Instances;

public class BroadcastingTower: Npc
{
    public BroadcastingTower(NpcTemplate template): base(template)
    {
        setInstanceType(InstanceType.BroadcastingTower);
    }

    public override void showChatWindow(Player player, int value)
    {
        String filename = null;
        if (isInsideRadius2D(-79884, 86529, 0, 50) || isInsideRadius2D(-78858, 111358, 0, 50) ||
            isInsideRadius2D(-76973, 87136, 0, 50) || isInsideRadius2D(-75850, 111968, 0, 50))
        {
            if (value == 0)
            {
                filename = "html/observation/" + getId() + "-Oracle.htm";
            }
            else
            {
                filename = "html/observation/" + getId() + "-Oracle-" + value + ".htm";
            }
        }
        else if (value == 0)
        {
            filename = "html/observation/" + getId() + ".htm";
        }
        else
        {
            filename = "html/observation/" + getId() + "-" + value + ".htm";
        }

        HtmlContent htmlContent = HtmlContent.LoadFromFile(filename, player);
        htmlContent.Replace("%objectId%", getObjectId().ToString());
       
        NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(getObjectId(), 0, htmlContent);
        player.sendPacket(html);
    }
}