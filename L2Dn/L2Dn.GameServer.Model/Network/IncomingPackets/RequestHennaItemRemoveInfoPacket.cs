using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Henna;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestHennaItemRemoveInfoPacket: IIncomingPacket<GameSession>
{
    private int _symbolId;

    public void ReadContent(PacketBitReader reader)
    {
        _symbolId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null || _symbolId == 0)
            return ValueTask.CompletedTask;
		
        Henna henna = HennaData.getInstance().getHenna(_symbolId);
        if (henna == null)
        {
            PacketLogger.Instance.Warn("Invalid Henna Id: " + _symbolId + " from " + player);
            player.sendPacket(ActionFailedPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }
		
        player.sendPacket(new HennaItemRemoveInfoPacket(henna, player));
        return ValueTask.CompletedTask;
    }
}