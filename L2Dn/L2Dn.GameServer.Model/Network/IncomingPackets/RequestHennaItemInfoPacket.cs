using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Henna;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestHennaItemInfoPacket: IIncomingPacket<GameSession>
{
    private int _symbolId;

    public void ReadContent(PacketBitReader reader)
    {
        _symbolId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
        Henna henna = HennaData.getInstance().getHennaByDyeId(_symbolId);
        if (henna == null)
        {
            if (_symbolId != 0)
            {
                PacketLogger.Instance.Warn(GetType().Name + ": Invalid Henna Id: " + _symbolId + " from " + player);
            }
            
            player.sendPacket(ActionFailedPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }
		
        player.sendPacket(new HennaItemDrawInfoPacket(henna, player));

        return ValueTask.CompletedTask;
    }
}