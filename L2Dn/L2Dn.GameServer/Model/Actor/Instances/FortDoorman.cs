using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Model.Actor.Instances;

public class FortDoorman: Doorman
{
    public FortDoorman(NpcTemplate template): base(template)
    {
        setInstanceType(InstanceType.FortDoorman);
    }

    public override void showChatWindow(Player player)
    {
        player.sendPacket(ActionFailedPacket.STATIC_PACKET);

        NpcHtmlMessage html = new NpcHtmlMessage(getObjectId());
        if (!isOwnerClan(player))
        {
            html.setFile(player, "data/html/doorman/" + getTemplate().getId() + "-no.htm");
        }
        else if (isUnderSiege())
        {
            html.setFile(player, "data/html/doorman/" + getTemplate().getId() + "-busy.htm");
        }
        else
        {
            html.setFile(player, "data/html/doorman/" + getTemplate().getId() + ".htm");
        }

        html.replace("%objectId%", getObjectId().ToString());
        player.sendPacket(html);
    }

    protected override void openDoors(Player player, String command)
    {
        StringTokenizer st = new StringTokenizer(command.substring(10), ", ");
        st.nextToken();

        while (st.hasMoreTokens())
        {
            getFort().openDoor(player, int.Parse(st.nextToken()));
        }
    }

    protected override void closeDoors(Player player, String command)
    {
        StringTokenizer st = new StringTokenizer(command.substring(11), ", ");
        st.nextToken();

        while (st.hasMoreTokens())
        {
            getFort().closeDoor(player, int.Parse(st.nextToken()));
        }
    }

    protected sealed override bool isOwnerClan(Player player)
    {
        return (player.getClan() != null) && (getFort() != null) && (getFort().getOwnerClan() != null) &&
               (player.getClanId() == getFort().getOwnerClan().getId());
    }

    protected sealed override bool isUnderSiege()
    {
        return getFort().getZone().isActive();
    }
}