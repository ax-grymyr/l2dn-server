using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Model.Actor.Instances;

public class BroadcastingTower: Npc
{
    public BroadcastingTower(NpcTemplate template): base(template)
    {
        InstanceType = InstanceType.BroadcastingTower;
    }

    public override void showChatWindow(Player player, int value)
    {
        string filename;
        if (this.IsInsideRadius2D(new Location2D(-79884, 86529), 50) ||
            this.IsInsideRadius2D(new Location2D(-78858, 111358), 50) ||
            this.IsInsideRadius2D(new Location2D(-76973, 87136), 50) ||
            this.IsInsideRadius2D(new Location2D(-75850, 111968), 50))
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
        htmlContent.Replace("%objectId%", ObjectId.ToString());
       
        NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(ObjectId, 0, htmlContent);
        player.sendPacket(html);
    }
}