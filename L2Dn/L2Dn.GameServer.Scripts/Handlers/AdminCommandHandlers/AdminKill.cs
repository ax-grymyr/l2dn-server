using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * This class handles following admin commands: - kill = kills target Creature - kill_monster = kills target non-player - kill <radius> = If radius is specified, then ALL players only in that radius will be killed. - kill_monster <radius> = If radius is specified, then ALL non-players only in that
 * radius will be killed.
 * @version $Revision: 1.2.4.5 $ $Date: 2007/07/31 10:06:06 $
 */
public class AdminKill: IAdminCommandHandler
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(AdminKill));
	private static readonly string[] ADMIN_COMMANDS =
    [
        "admin_kill",
		"admin_kill_monster",
    ];

	public bool useAdminCommand(string command, Player activeChar)
	{
		if (command.startsWith("admin_kill"))
		{
			StringTokenizer st = new StringTokenizer(command, " ");
			st.nextToken(); // skip command
			if (st.hasMoreTokens())
			{
				string firstParam = st.nextToken();
				Player? plyr = World.getInstance().getPlayer(firstParam);
				if (plyr != null)
				{
					if (st.hasMoreTokens())
					{
						try
						{
							int radius = int.Parse(st.nextToken());
							World.getInstance().forEachVisibleObjectInRange<Creature>(plyr, radius, knownChar =>
							{
								if (knownChar is ControllableMob || knownChar is FriendlyNpc || knownChar == activeChar)
								{
									return;
								}

								kill(activeChar, knownChar);
							});

							BuilderUtil.sendSysMessage(activeChar, "Killed all characters within a " + radius + " unit radius.");
							return true;
						}
						catch (FormatException e)
						{
                            _logger.Error(e);
							BuilderUtil.sendSysMessage(activeChar, "Invalid radius.");
							return false;
						}
					}
					kill(activeChar, plyr);
				}
				else
				{
					try
					{
						int radius = int.Parse(firstParam);
						World.getInstance().forEachVisibleObjectInRange<Creature>(activeChar, radius, wo =>
						{
							if (wo is ControllableMob || wo is FriendlyNpc)
							{
								return;
							}
							kill(activeChar, wo);
						});

						BuilderUtil.sendSysMessage(activeChar, "Killed all characters within a " + radius + " unit radius.");
						return true;
					}
					catch (FormatException e)
					{
                        _logger.Error(e);
						BuilderUtil.sendSysMessage(activeChar, "Usage: //kill <player_name | radius>");
						return false;
					}
				}
			}
			else
			{
				WorldObject? obj = activeChar.getTarget();
				if (obj == null || obj is ControllableMob || !obj.isCreature())
				{
					activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
				}
				else
				{
					kill(activeChar, (Creature) obj);
				}
			}
		}
		return true;
	}

	private void kill(Player activeChar, Creature target)
	{
		if (target is Player player)
		{
			if (!player.isGM())
			{
				target.stopAllEffects(); // e.g. invincibility effect
			}
			target.reduceCurrentHp(target.getMaxHp() + target.getMaxCp() + 1, activeChar, null);
		}
		else if (Config.CHAMPION_ENABLE && target.isChampion())
		{
			target.reduceCurrentHp(target.getMaxHp() * Config.CHAMPION_HP + 1, activeChar, null);
		}
		else
		{
			bool targetIsInvul = false;
			if (target.isInvul())
			{
				targetIsInvul = true;
				target.setInvul(false);
			}

			target.reduceCurrentHp(target.getMaxHp() + 1, activeChar, null);
			if (targetIsInvul)
			{
				target.setInvul(true);
			}
		}
	}

	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}