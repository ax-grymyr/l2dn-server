using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExSetCompassZoneCodePacket: IOutgoingPacket
{
    public const int ALTEREDZONE = 7;
    public const int SIEGEWARZONE = 10;
    public const int PEACEZONE = 11;
    public const int SEVENSIGNSZONE = 12;
    public const int NOPVPZONE = 13;
    public const int PVPZONE = 14;
    public const int GENERALZONE = 15;
	
    private readonly int _zoneType;
	
    public ExSetCompassZoneCodePacket(int value)
    {
        _zoneType = value;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SET_COMPASS_ZONE_CODE);
        
        writer.WriteInt32(_zoneType);
    }
}