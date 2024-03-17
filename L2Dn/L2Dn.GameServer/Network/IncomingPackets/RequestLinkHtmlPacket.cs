using L2Dn.GameServer.Data;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestLinkHtmlPacket: IIncomingPacket<GameSession>
{
    private string _link;

    public void ReadContent(PacketBitReader reader)
    {
        _link = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
        if (_link.isEmpty())
        {
            PacketLogger.Instance.Warn(player + " sent empty html link!");
            return ValueTask.CompletedTask;
        }
		
        if (_link.contains(".."))
        {
            PacketLogger.Instance.Warn(player + " sent invalid html link: link " + _link);
            return ValueTask.CompletedTask;
        }
		
        int htmlObjectId = player.validateHtmlAction("link " + _link);
        if (htmlObjectId == -1)
        {
            PacketLogger.Instance.Warn(player + " sent non cached html link: link " + _link);
            return ValueTask.CompletedTask;
        }
		
        if ((htmlObjectId > 0) && !Util.isInsideRangeOfObjectId(player, htmlObjectId, Npc.INTERACTION_DISTANCE))
        {
            // No logging here, this could be a common case
            return ValueTask.CompletedTask;
        }
		
        HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data, "html/" + _link);
        NpcHtmlMessagePacket msg = new NpcHtmlMessagePacket(htmlObjectId, helper);
        player.sendPacket(msg);
        return ValueTask.CompletedTask;
    }
}