using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * This class handles following admin commands: - heal = restores HP/MP/CP on target, name or radius
 * @version $Revision: 1.2.4.5 $ $Date: 2005/04/11 10:06:06 $ Small typo fix by Zoey76 24/02/2011
 */
public class AdminHeal: IAdminCommandHandler
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(AdminHeal));
	private static readonly string[] ADMIN_COMMANDS =
	{
		"admin_heal"
	};
	
	public bool useAdminCommand(string command, Player activeChar)
	{
		if (command.equals("admin_heal"))
		{
			handleHeal(activeChar);
		}
		else if (command.startsWith("admin_heal"))
		{
			try
			{
				string healTarget = command.Substring(11);
				handleHeal(activeChar, healTarget);
			}
			catch (IndexOutOfRangeException e)
			{
				if (Config.DEVELOPER)
				{
					LOGGER.Warn("Heal error: " + e);
				}
				BuilderUtil.sendSysMessage(activeChar, "Incorrect target/radius specified.");
			}
		}
		return true;
	}
	
	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
	
	private void handleHeal(Player activeChar)
	{
		handleHeal(activeChar, null);
	}
	
	private void handleHeal(Player activeChar, string player)
	{
		WorldObject obj = activeChar.getTarget();
		if (player != null)
		{
			Player plyr = World.getInstance().getPlayer(player);
			if (plyr != null)
			{
				obj = plyr;
			}
			else
			{
				try
				{
					int radius = int.Parse(player);
					World.getInstance().forEachVisibleObject<Creature>(activeChar, character =>
					{
						character.setCurrentHpMp(character.getMaxHp(), character.getMaxMp());
						if (character.isPlayer())
						{
							character.setCurrentCp(character.getMaxCp());
						}
					});
					
					BuilderUtil.sendSysMessage(activeChar, "Healed within " + radius + " unit radius.");
					return;
				}
				catch (FormatException nbe)
				{
				}
			}
		}
		if (obj == null)
		{
			obj = activeChar;
		}
		if (obj.isCreature())
		{
			Creature target = (Creature) obj;
			target.setCurrentHpMp(target.getMaxHp(), target.getMaxMp());
			if (target.isPlayer())
			{
				target.setCurrentCp(target.getMaxCp());
			}
		}
		else
		{
			activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
		}
	}
}
