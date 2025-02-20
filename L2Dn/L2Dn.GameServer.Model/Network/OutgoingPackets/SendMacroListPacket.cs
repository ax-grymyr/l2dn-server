using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct SendMacroListPacket: IOutgoingPacket
{
    private readonly int _count;
    private readonly Macro _macro;
    private readonly MacroUpdateType _updateType;
	
    public SendMacroListPacket(int count, Macro macro, MacroUpdateType updateType)
    {
        _count = count;
        _macro = macro;
        _updateType = updateType;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.MACRO_LIST);
        
        writer.WriteByte((byte)_updateType);
        writer.WriteInt32(_updateType != MacroUpdateType.LIST ? _macro.getId() : 0); // modified, created or deleted macro's id
        writer.WriteByte((byte)_count); // count of Macros
        writer.WriteByte(_macro != null); // unknown
        if (_macro != null && _updateType != MacroUpdateType.DELETE)
        {
            writer.WriteInt32(_macro.getId()); // Macro ID
            writer.WriteString(_macro.getName()); // Macro Name
            writer.WriteString(_macro.getDescr()); // Desc
            writer.WriteString(_macro.getAcronym()); // acronym
            writer.WriteInt32(_macro.getIcon() ?? 0); // icon
            writer.WriteByte((byte)_macro.getCommands().Count); // count
            byte i = 1;
            foreach (MacroCmd cmd in _macro.getCommands())
            {
                writer.WriteByte(i++); // command count
                writer.WriteByte((byte)cmd.getType()); // type 1 = skill, 3 = action, 4 = shortcut
                writer.WriteInt32(cmd.getD1()); // skill id
                writer.WriteByte((byte)cmd.getD2()); // shortcut id
                writer.WriteString(cmd.getCmd()); // command name
            }
        }
    }
}