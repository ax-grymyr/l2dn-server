using L2Dn.GameServer.Data;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Html;
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

        if (!session.HtmlActionValidator.IsValidAction("link " + _link, out int? htmlObjectId))
        {
            PacketLogger.Instance.Warn(player + " sent non cached html link: link " + _link);
            return ValueTask.CompletedTask;
        }

        if (htmlObjectId != null && !Util.isInsideRangeOfObjectId(player, htmlObjectId.Value, Npc.INTERACTION_DISTANCE))
        {
            // No logging here, this could be a common case
            return ValueTask.CompletedTask;
        }

        HtmlContent htmlContent = HtmlContent.LoadFromFile("html/" + _link, player);
        NpcHtmlMessagePacket msg = new NpcHtmlMessagePacket(htmlObjectId, 0, htmlContent);
        player.sendPacket(msg);
        return ValueTask.CompletedTask;
    }
}