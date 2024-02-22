using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Fishing;

public readonly struct ExFishingEndPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly FishingEndReason _reason;
	
    public ExFishingEndPacket(Player player, FishingEndReason reason)
    {
        _player = player;
        _reason = reason;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_FISHING_END);
        
        writer.WriteInt32(_player.getObjectId());
        writer.WriteByte((byte)_reason);
    }
}