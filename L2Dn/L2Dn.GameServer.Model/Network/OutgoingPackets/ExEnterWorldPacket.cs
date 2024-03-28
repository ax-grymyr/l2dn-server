using L2Dn.Extensions;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExEnterWorldPacket: IOutgoingPacket
{
    private readonly int _zoneIdOffsetSeconds;
    private readonly int _epochInSeconds;
    private readonly int _daylight;
	
    public ExEnterWorldPacket()
    {
        DateTime now = DateTime.UtcNow;
        _epochInSeconds = now.getEpochSecond();
        TimeZoneInfo timeZoneInfo = TimeZoneInfo.Local;
        _zoneIdOffsetSeconds = (int)timeZoneInfo.GetUtcOffset(now).TotalSeconds;
        _daylight = 0; // TODO: Daylight savings: rules.getDaylightSavings(now).toMillis() / 1000;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ENTER_WORLD);
        
        writer.WriteInt32(_epochInSeconds);
        writer.WriteInt32(-_zoneIdOffsetSeconds);
        writer.WriteInt32(_daylight);
        writer.WriteInt32(Config.MAX_FREE_TELEPORT_LEVEL);
    }
}