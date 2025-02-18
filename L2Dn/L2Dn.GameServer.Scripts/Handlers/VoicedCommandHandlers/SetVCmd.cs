using System.Globalization;
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
	private static readonly string[] VOICED_COMMANDS =
	{
		"set name",
		"set home",
		"set group"
	};

	public bool useVoicedCommand(string command, Player activeChar, string @params)
	{
		if (command.equals("set"))
		{
			WorldObject? target = activeChar.getTarget();
			if (target == null || !target.isPlayer())
			{
				return false;
			}

			Player? player = target.getActingPlayer();
            if (player is null)
                return false;

            Clan? playerClan = player.getClan();
            Clan? activeCharClan = activeChar.getClan();
			if (activeCharClan == null || playerClan == null || activeCharClan.getId() != playerClan.getId())
			{
				return false;
			}

			if (@params.startsWith("privileges"))
			{
				string val = @params.Substring(11);
				if (!int.TryParse(val, CultureInfo.InvariantCulture, out int n))
				{
					return false;
				}

				if (activeChar.getClanPrivileges() <= (ClanPrivilege)n || !activeChar.isClanLeader())
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

	public string[] getVoicedCommandList()
	{
		return VOICED_COMMANDS;
	}
}