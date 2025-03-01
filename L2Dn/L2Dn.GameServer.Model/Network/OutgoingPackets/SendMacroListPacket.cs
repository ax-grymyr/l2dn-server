using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct SendMacroListPacket(int count, Macro? macro, MacroUpdateType updateType): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.MACRO_LIST);

        writer.WriteByte((byte)updateType);
        writer.WriteInt32(updateType != MacroUpdateType.LIST ? macro?.getId() ?? 0 : 0); // modified, created or deleted macro's id
        writer.WriteByte((byte)count); // count of Macros
        writer.WriteByte(macro != null); // unknown
        if (macro != null && updateType != MacroUpdateType.DELETE)
        {
            writer.WriteInt32(macro.getId()); // Macro ID
            writer.WriteString(macro.getName()); // Macro Name
            writer.WriteString(macro.getDescr()); // Desc
            writer.WriteString(macro.getAcronym()); // acronym
            writer.WriteInt32(macro.getIcon() ?? 0); // icon
            writer.WriteByte((byte)macro.getCommands().Count); // count
            byte i = 1;
            foreach (MacroCmd cmd in macro.getCommands())
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