using L2Dn.GameServer.Model;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.AutoPlay;

public readonly struct ExActivateAutoShortcutPacket: IOutgoingPacket
{
    private readonly int _position;
    private readonly bool _active;
	
    public ExActivateAutoShortcutPacket(Shortcut shortcut, bool active)
    {
        _position = shortcut.getSlot() + shortcut.getPage() * ShortCuts.MAX_SHORTCUTS_PER_BAR;
        _active = active;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ACTIVATE_AUTO_SHORTCUT);
        
        writer.WriteInt16((short)_position);
        writer.WriteByte(_active);
    }
}