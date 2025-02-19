using System.Globalization;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * Allows Game Masters to test System Messages.<br>
 * admin_msg display the raw message.<br>
 * admin_msgx is an extended version that allows to set parameters.
 * @author Zoey76
 */
public class AdminMessages: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
    [
        "admin_msg",
		"admin_msgx",
    ];
	
	public bool useAdminCommand(string command, Player activeChar)
	{
		if (command.startsWith("admin_msg "))
		{
			try
			{
				activeChar.sendPacket(new SystemMessagePacket((SystemMessageId)int.Parse(command.Substring(10).Trim())));
				return true;
			}
			catch (Exception e)
			{
				BuilderUtil.sendSysMessage(activeChar, "Command format: //msg <SYSTEM_MSG_ID>");
			}
		}
		else if (command.startsWith("admin_msgx "))
		{
			string[] tokens = command.Split(" ");
			if ((tokens.Length <= 2) || !int.TryParse(tokens[1], CultureInfo.InvariantCulture, out int token1))
			{
				BuilderUtil.sendSysMessage(activeChar, "Command format: //msgx <SYSTEM_MSG_ID> [item:Id] [skill:Id] [npc:Id] [zone:x,y,x] [castle:Id] [str:'text']");
				return false;
			}
			
			SystemMessagePacket sm = new SystemMessagePacket((SystemMessageId)token1);
			string val;
			int lastPos = 0;
			for (int i = 2; i < tokens.Length; i++)
			{
				try
				{
					val = tokens[i];
					if (val.startsWith("item:"))
					{
						sm.Params.addItemName(int.Parse(val.Substring(5)));
					}
					else if (val.startsWith("skill:"))
					{
						sm.Params.addSkillName(int.Parse(val.Substring(6)));
					}
					else if (val.startsWith("npc:"))
					{
						sm.Params.addNpcName(int.Parse(val.Substring(4)));
					}
					else if (val.startsWith("zone:"))
					{
						int x = int.Parse(val.Substring(5, val.IndexOf(',') - 5));
						int y = int.Parse(val.Substring(val.IndexOf(',') + 1, val.LastIndexOf(',') - (val.IndexOf(',') + 1)));
						int z = int.Parse(val.Substring(val.LastIndexOf(',') + 1, val.Length - (val.LastIndexOf(',') + 1)));
						sm.Params.addZoneName(x, y, z);
					}
					else if (val.startsWith("castle:"))
					{
						sm.Params.addCastleId(int.Parse(val.Substring(7)));
					}
					else if (val.startsWith("str:"))
					{
						int pos = command.IndexOf("'", lastPos + 1);
						lastPos = command.IndexOf("'", pos + 1);
						sm.Params.addString(command.Substring(pos + 1, lastPos - (pos + 1)));
					}
				}
				catch (Exception e)
				{
					BuilderUtil.sendSysMessage(activeChar, "Exception: " + e);
				}
			}
			activeChar.sendPacket(sm);
		}
		return false;
	}
	
	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}