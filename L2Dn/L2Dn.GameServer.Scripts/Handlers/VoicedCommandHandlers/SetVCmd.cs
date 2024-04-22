using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.VoicedCommandHandlers;

/**
 * @author Zoey76
 */
public class SetVCmd: IVoicedCommandHandler
{
	private static readonly String[] VOICED_COMMANDS =
	{
		"set name",
		"set home",
		"set group"
	};
	
	public bool useVoicedCommand(String command, Player activeChar, String @params)
	{
		if (command.equals("set"))
		{
			WorldObject target = activeChar.getTarget();
			if ((target == null) || !target.isPlayer())
			{
				return false;
			}
			
			Player player = activeChar.getTarget().getActingPlayer();
			if ((activeChar.getClan() == null) || (player.getClan() == null) || (activeChar.getClan().getId() != player.getClan().getId()))
			{
				return false;
			}
			
			if (@params.startsWith("privileges"))
			{
				String val = @params.Substring(11);
				if (!Util.isDigit(val))
				{
					return false;
				}

				int n = int.Parse(val);
				if ((activeChar.getClanPrivileges() <= (ClanPrivilege)n) || !activeChar.isClanLeader())
				{
					return false;
				}
				
				player.setClanPrivileges((ClanPrivilege)n);
				activeChar.sendMessage("Your clan privileges have been set to " + n + " by " + activeChar.getName() + ".");
			}
			else if (@params.startsWith("title"))
			{
				// TODO why is this empty?
			}
		}

        return true;
	}
	
	public String[] getVoicedCommandList()
	{
		return VOICED_COMMANDS;
	}
}