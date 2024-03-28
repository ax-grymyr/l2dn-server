using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets.Settings;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Settings;

public struct ExSaveItemAnnounceSettingPacket: IIncomingPacket<GameSession>
{
    private bool _announceType;

    public void ReadContent(PacketBitReader reader)
    {
        _announceType = reader.ReadByte() == 1;
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        player.getClientSettings().setAnnounceEnabled(_announceType);
        player.sendPacket(new ExItemAnnounceSettingPacket(_announceType));
        
        return ValueTask.CompletedTask;
    }
}