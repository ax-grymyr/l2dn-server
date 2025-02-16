using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.TaskManagers;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct CharacterSelectedPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly int _sessionId;
    
    public CharacterSelectedPacket(Player player, int sessionId)
    {
        _player = player;
        _sessionId = sessionId;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.CHARACTER_SELECTED);
        
        writer.WriteString(_player.getName());
        writer.WriteInt32(_player.ObjectId);
        writer.WriteString(_player.getTitle());
        writer.WriteInt32(_sessionId);
        writer.WriteInt32(_player.getClanId() ?? 0);
        writer.WriteInt32(0); // ??
        writer.WriteInt32((int)_player.getAppearance().getSex());
        writer.WriteInt32((int)_player.getRace());
        writer.WriteInt32((int)_player.getClassId());
        writer.WriteInt32(1); // active ??
        writer.WriteInt32(_player.getX());
        writer.WriteInt32(_player.getY());
        writer.WriteInt32(_player.getZ());
        writer.WriteDouble(_player.getCurrentHp());
        writer.WriteDouble(_player.getCurrentMp());
        writer.WriteInt64(_player.getSp());
        writer.WriteInt64(_player.getExp());
        writer.WriteInt32(_player.getLevel());
        writer.WriteInt32(_player.getReputation());
        writer.WriteInt32(_player.getPkKills());
        writer.WriteInt32(GameTimeTaskManager.getInstance().getGameTime() % (24 * 60)); // "reset" on 24th hour
        writer.WriteInt32(0);
        writer.WriteInt32((int)_player.getClassId());
        writer.WriteZeros(16);
        writer.WriteInt32(0);
        writer.WriteInt32(0);
        writer.WriteInt32(0);
        writer.WriteInt32(0);
        writer.WriteInt32(0);
        writer.WriteInt32(0);
        writer.WriteInt32(0);
        writer.WriteInt32(0);
        writer.WriteInt32(0);
        writer.WriteZeros(28);
        writer.WriteInt32(0);
    }
}