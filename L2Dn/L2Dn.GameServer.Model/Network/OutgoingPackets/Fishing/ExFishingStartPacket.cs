using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Fishing;

public readonly struct ExFishingStartPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly int _fishType;
    private readonly Location3D _baitLocation;
	
    /**
     * @param player
     * @param fishType
     * @param baitLocation
     */
    public ExFishingStartPacket(Player player, int fishType, Location3D baitLocation)
    {
        _player = player;
        _fishType = fishType;
        _baitLocation = baitLocation;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_FISHING_START);
        
        writer.WriteInt32(_player.getObjectId());
        writer.WriteByte((byte)_fishType);
        writer.WriteLocation3D(_baitLocation);
        writer.WriteByte(1); // 0 = newbie, 1 = normal, 2 = night
    }
}