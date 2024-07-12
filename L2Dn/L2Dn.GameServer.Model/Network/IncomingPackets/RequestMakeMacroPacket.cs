using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestMakeMacroPacket: IIncomingPacket<GameSession>
{
    private const int MAX_MACRO_LENGTH = 20;

    private Macro _macro;
    private int _commandsLength;

    public void ReadContent(PacketBitReader reader)
    {
        int id = reader.ReadInt32();
        string name = reader.ReadString();
        string desc = reader.ReadString();
        string acronym = reader.ReadString();
        int icon = reader.ReadInt32();
        int count = reader.ReadByte();
        if (count > MAX_MACRO_LENGTH)
        {
            count = MAX_MACRO_LENGTH;
        }
		
        List<MacroCmd> commands = new(count);
        for (int i = 0; i < count; i++)
        {
            int entry = reader.ReadByte();
            int type = reader.ReadByte(); // 1 = skill, 3 = action, 4 = shortcut
            int d1 = reader.ReadInt32(); // skill or page number for shortcuts
            int d2 = reader.ReadByte();
            string command = reader.ReadString();
            _commandsLength += command.Length;

            commands.Add(new MacroCmd(entry, (MacroType)(type < 1 || type > 6 ? 0 : type), d1, d2, command));
        }
        
        _macro = new Macro(id, icon, name, desc, acronym, commands);
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (_commandsLength > 255)
        {
            // Invalid macro. Refer to the Help file for instructions.
            player.sendPacket(SystemMessageId.INVALID_MACRO_REFER_TO_THE_HELP_FILE_FOR_INSTRUCTIONS);
            return ValueTask.CompletedTask;
        }
        
        if (player.getMacros().getAllMacroses().Count > 48)
        {
            // You may create up to 48 macros.
            player.sendPacket(SystemMessageId.YOU_MAY_CREATE_UP_TO_48_MACROS);
            return ValueTask.CompletedTask;
        }

        if (string.IsNullOrEmpty(_macro.getName()))
        {
            // Enter the name of the macro.
            player.sendPacket(SystemMessageId.ENTER_THE_NAME_OF_THE_MACRO);
            return ValueTask.CompletedTask;
        }

        if (_macro.getDescr().Length > 32)
        {
            // Macro descriptions may contain up to 32 characters.
            player.sendPacket(SystemMessageId.MACRO_DESCRIPTIONS_MAY_CONTAIN_UP_TO_32_CHARACTERS);
            return ValueTask.CompletedTask;
        }

        player.registerMacro(_macro);

        return ValueTask.CompletedTask;
    }
}