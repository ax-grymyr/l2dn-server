using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExSpawnEmitterPacket: IOutgoingPacket
{
    private readonly int _playerObjectId;
    private readonly int _npcObjectId;
	
    public ExSpawnEmitterPacket(int playerObjectId, int npcObjectId)
    {
        _playerObjectId = playerObjectId;
        _npcObjectId = npcObjectId;
    }
	
    public ExSpawnEmitterPacket(Player player, Npc npc): this(player.getObjectId(), npc.getObjectId())
    {
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SPAWN_EMITTER);
        
        writer.WriteInt32(_npcObjectId);
        writer.WriteInt32(_playerObjectId);
        writer.WriteInt32(0); // ?
    }
}