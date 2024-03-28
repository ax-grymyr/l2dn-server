using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Settings;

public readonly struct ExItemAnnounceSettingPacket: IOutgoingPacket
{
    private readonly bool _announceEnabled;
	
    public ExItemAnnounceSettingPacket(bool announceEnabled)
    {
        _announceEnabled = announceEnabled;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ITEM_ANNOUNCE_SETTING);
        
        writer.WriteByte(_announceEnabled);
    }
}