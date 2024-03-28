using L2Dn.GameServer.InstanceManagers.Events;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExLetterCollectorUiPacket: IOutgoingPacket
{
    private readonly int _minimumLevel;
	
    public ExLetterCollectorUiPacket(Player player)
    {
        _minimumLevel = player.getLevel() <= LetterCollectorManager.getInstance().getMaxLevel() ? LetterCollectorManager.getInstance().getMinLevel() : Config.PLAYER_MAXIMUM_LEVEL;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_LETTER_COLLECTOR_UI_LAUNCHER);
        
        writer.WriteByte(1); // enabled (0x00 - no, 0x01 -yes)
        writer.WriteInt32(_minimumLevel); // Minimum Level
    }
}