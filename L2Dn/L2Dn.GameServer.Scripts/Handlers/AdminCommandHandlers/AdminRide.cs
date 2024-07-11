using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * @author
 */
public class AdminRide: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
	{
		"admin_ride_horse",
		"admin_ride_bike",
		"admin_ride_wyvern",
		"admin_ride_strider",
		"admin_unride_wyvern",
		"admin_unride_strider",
		"admin_unride",
		"admin_ride_wolf",
		"admin_unride_wolf",
	};
	
	private static int PURPLE_MANED_HORSE_TRANSFORMATION_ID = 106;
	
	private static int JET_BIKE_TRANSFORMATION_ID = 20001;
	
	public bool useAdminCommand(string command, Player activeChar)
	{
		Player player = getRideTarget(activeChar);
		if (player == null)
		{
			return false;
		}
		
		if (command.startsWith("admin_ride"))
		{
			if (player.isMounted() || player.hasSummon())
			{
				BuilderUtil.sendSysMessage(activeChar, "Target already have a summon.");
				return false;
			}
			
			int petRideId;
			if (command.startsWith("admin_ride_wyvern"))
			{
				petRideId = 15955;
			}
			else if (command.startsWith("admin_ride_strider"))
			{
				petRideId = 12526;
			}
			else if (command.startsWith("admin_ride_wolf"))
			{
				petRideId = 16041;
			}
			else if (command.startsWith("admin_ride_horse")) // handled using transformation
			{
				if (player.isTransformed())
				{
					activeChar.sendPacket(SystemMessageId.YOU_ALREADY_POLYMORPHED_AND_CANNOT_POLYMORPH_AGAIN);
				}
				else
				{
					player.transform(PURPLE_MANED_HORSE_TRANSFORMATION_ID, true);
				}
				
				return true;
			}
			else if (command.startsWith("admin_ride_bike")) // handled using transformation
			{
				if (player.isTransformed())
				{
					activeChar.sendPacket(SystemMessageId.YOU_ALREADY_POLYMORPHED_AND_CANNOT_POLYMORPH_AGAIN);
				}
				else
				{
					player.transform(JET_BIKE_TRANSFORMATION_ID, true);
				}
				
				return true;
			}
			else
			{
				BuilderUtil.sendSysMessage(activeChar, "Command '" + command + "' not recognized");
				return false;
			}
			
			player.mount(petRideId, 0, false);
			return false;
		}
		else if (command.startsWith("admin_unride"))
		{
			if (player.getTransformationId() == PURPLE_MANED_HORSE_TRANSFORMATION_ID)
			{
				player.untransform();
			}
			
			if (player.getTransformationId() == JET_BIKE_TRANSFORMATION_ID)
			{
				player.untransform();
			}
			else
			{
				player.dismount();
			}
		}
		return true;
	}
	
	private Player getRideTarget(Player activeChar)
	{
		Player player = null;
		if ((activeChar.getTarget() == null) || (activeChar.getTarget().getObjectId() == activeChar.getObjectId()) || !activeChar.getTarget().isPlayer())
		{
			player = activeChar;
		}
		else
		{
			player = (Player) activeChar.getTarget();
		}
		return player;
	}
	
	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}
