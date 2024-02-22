using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Fishing;

public readonly struct ExUserInfoFishingPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly bool _isFishing;
    private readonly ILocational? _baitLocation;
	
    public ExUserInfoFishingPacket(Player player, bool isFishing, ILocational baitLocation)
    {
        _player = player;
        _isFishing = isFishing;
        _baitLocation = baitLocation;
    }
	
    public ExUserInfoFishingPacket(Player player, bool isFishing)
    {
        _player = player;
        _isFishing = isFishing;
        _baitLocation = null;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_USER_INFO_FISHING);
        
        writer.WriteInt32(_player.getObjectId());
        writer.WriteByte(_isFishing);
        if (_baitLocation == null)
        {
            writer.WriteInt32(0);
            writer.WriteInt32(0);
            writer.WriteInt32(0);
        }
        else
        {
            writer.WriteInt32(_baitLocation.getX());
            writer.WriteInt32(_baitLocation.getY());
            writer.WriteInt32(_baitLocation.getZ());
        }
    }
}