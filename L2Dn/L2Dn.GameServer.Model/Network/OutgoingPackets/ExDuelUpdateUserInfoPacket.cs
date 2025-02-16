using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExDuelUpdateUserInfoPacket: IOutgoingPacket
{
    private readonly Player _player;
	
    public ExDuelUpdateUserInfoPacket(Player player)
    {
        _player = player;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_DUEL_UPDATE_USER_INFO);
        writer.WriteString(_player.getName());
        writer.WriteInt32(_player.ObjectId);
        writer.WriteInt32((int)_player.getClassId());
        writer.WriteInt32(_player.getLevel());
        writer.WriteInt32((int) _player.getCurrentHp());
        writer.WriteInt32(_player.getMaxHp());
        writer.WriteInt32((int) _player.getCurrentMp());
        writer.WriteInt32(_player.getMaxMp());
        writer.WriteInt32((int) _player.getCurrentCp());
        writer.WriteInt32(_player.getMaxCp());
    }
}