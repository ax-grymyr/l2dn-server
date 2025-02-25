using L2Dn.GameServer.Data;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Actor.Instances;

public class FortDoorman: Doorman
{
    public FortDoorman(NpcTemplate template): base(template)
    {
        InstanceType = InstanceType.FortDoorman;
    }

    public override void showChatWindow(Player player)
    {
        player.sendPacket(ActionFailedPacket.STATIC_PACKET);

        HtmlContent htmlContent;
        if (!isOwnerClan(player))
        {
            htmlContent = HtmlContent.LoadFromFile("html/doorman/" + getTemplate().getId() + "-no.htm", player);
        }
        else if (isUnderSiege())
        {
            htmlContent = HtmlContent.LoadFromFile("html/doorman/" + getTemplate().getId() + "-busy.htm", player);
        }
        else
        {
            htmlContent = HtmlContent.LoadFromFile("html/doorman/" + getTemplate().getId() + ".htm", player);
        }

        htmlContent.Replace("%objectId%", ObjectId.ToString());

        NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(ObjectId, 0, htmlContent);
        player.sendPacket(html);
    }

    protected override void openDoors(Player player, string command)
    {
        StringTokenizer st = new StringTokenizer(command.Substring(10), ", ");
        st.nextToken();

        Fort fort = getFort() ?? throw new InvalidOperationException("fort is null in FortDoorman.openDoors");
        while (st.hasMoreTokens())
        {
            fort.openDoor(player, int.Parse(st.nextToken()));
        }
    }

    protected override void closeDoors(Player player, string command)
    {
        StringTokenizer st = new StringTokenizer(command.Substring(11), ", ");
        st.nextToken();

        Fort fort = getFort() ?? throw new InvalidOperationException("fort is null in FortDoorman.openDoors");
        while (st.hasMoreTokens())
        {
            fort.closeDoor(player, int.Parse(st.nextToken()));
        }
    }

    protected sealed override bool isOwnerClan(Player player)
    {
        Fort? fort = getFort();
        Clan? ownerClan = fort?.getOwnerClan();
        return player.getClan() != null && fort != null && ownerClan != null &&
               player.getClanId() == ownerClan.getId();
    }

    protected sealed override bool isUnderSiege()
    {
        return getFort()?.getZone().isActive() == true;
    }
}