using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.TaskManagers;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * This class handles following admin commands: - res = resurrects target Creature
 * @version $Revision: 1.2.4.5 $ $Date: 2005/04/11 10:06:06 $
 */
public class AdminRes: IAdminCommandHandler
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(AdminRes));
	private static readonly string[] ADMIN_COMMANDS =
    [
        "admin_res",
		"admin_res_monster",
    ];

	public bool useAdminCommand(string command, Player activeChar)
	{
		if (command.startsWith("admin_res "))
		{
			HandleRes(activeChar, command.Split(" ")[1]);
		}
		else if (command.equals("admin_res"))
		{
			HandleRes(activeChar);
		}
		else if (command.startsWith("admin_res_monster "))
		{
			handleNonPlayerRes(activeChar, command.Split(" ")[1]);
		}
		else if (command.equals("admin_res_monster"))
		{
			handleNonPlayerRes(activeChar);
		}
		return true;
	}

	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}

	private static void HandleRes(Player activeChar, string? resParam = null)
	{
		WorldObject? obj = activeChar.getTarget();
		if (resParam != null)
		{
			// Check if a player name was specified as a param.
			Player? plyr = World.getInstance().getPlayer(resParam);
			if (plyr != null)
			{
				obj = plyr;
			}
			else
			{
				// Otherwise, check if the param was a radius.
				try
				{
					int radius = int.Parse(resParam);
					World.getInstance().forEachVisibleObjectInRange<Player>(activeChar, radius, doResurrect);
					BuilderUtil.sendSysMessage(activeChar, "Resurrected all players within a " + radius + " unit radius.");
					return;
				}
				catch (FormatException e)
				{
                    _logger.Error(e);
					BuilderUtil.sendSysMessage(activeChar, "Enter a valid player name or radius.");
					return;
				}
			}
		}

		if (obj == null)
		{
			obj = activeChar;
		}

		if (obj is ControllableMob)
		{
			activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
			return;
		}

		doResurrect((Creature) obj);
	}

	private void handleNonPlayerRes(Player activeChar)
	{
		handleNonPlayerRes(activeChar, "");
	}

	private void handleNonPlayerRes(Player activeChar, string radiusStr)
	{
		WorldObject? obj = activeChar.getTarget();
		try
		{
			int radius = 0;
			if (!string.IsNullOrEmpty(radiusStr))
			{
				radius = int.Parse(radiusStr);
				World.getInstance().forEachVisibleObjectInRange<Creature>(activeChar, radius, knownChar =>
				{
					if (!knownChar.isPlayer() && !(knownChar is ControllableMob))
					{
						doResurrect(knownChar);
					}
				});

				BuilderUtil.sendSysMessage(activeChar, "Resurrected all non-players within a " + radius + " unit radius.");
			}
		}
		catch (FormatException e)
		{
            _logger.Error(e);
			BuilderUtil.sendSysMessage(activeChar, "Enter a valid radius.");
			return;
		}

		if (obj == null || obj.isPlayer() || obj is ControllableMob)
		{
			activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
			return;
		}

		doResurrect((Creature) obj);
	}

	private static void doResurrect(Creature targetChar)
	{
		if (!targetChar.isDead())
		{
			return;
		}

		// If the target is a player, then restore the XP lost on death.
		if (targetChar.isPlayer())
		{
			((Player) targetChar).restoreExp(100.0);
		}
		else
		{
			DecayTaskManager.getInstance().cancel(targetChar);
		}

		targetChar.doRevive();
	}
}