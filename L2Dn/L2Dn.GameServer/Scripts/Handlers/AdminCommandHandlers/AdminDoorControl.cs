using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * This class handles following admin commands: - open1 = open coloseum door 24190001 - open2 = open coloseum door 24190002 - open3 = open coloseum door 24190003 - open4 = open coloseum door 24190004 - openall = open all coloseum door - close1 = close coloseum door 24190001 - close2 = close coloseum
 * door 24190002 - close3 = close coloseum door 24190003 - close4 = close coloseum door 24190004 - closeall = close all coloseum door - open = open selected door - close = close selected door
 * @version $Revision: 1.2.4.5 $ $Date: 2005/04/11 10:06:06 $
 */
public class AdminDoorControl: IAdminCommandHandler
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(AdminDoorControl));
	
	private static DoorData _doorTable = DoorData.getInstance();
	
	private static readonly string[] ADMIN_COMMANDS =
	{
		"admin_open",
		"admin_close",
		"admin_openall",
		"admin_closeall",
		"admin_showdoors"
	};
	
	public bool useAdminCommand(String command, Player activeChar)
	{
		try
		{
			if (command.startsWith("admin_open "))
			{
				int doorId = int.Parse(command.Substring(11));
				if (_doorTable.getDoor(doorId) != null)
				{
					_doorTable.getDoor(doorId).openMe();
				}
				else
				{
					foreach (Castle castle in CastleManager.getInstance().getCastles())
					{
						if (castle.getDoor(doorId) != null)
						{
							castle.getDoor(doorId).openMe();
						}
					}
				}
			}
			else if (command.startsWith("admin_close "))
			{
				int doorId = int.Parse(command.Substring(12));
				if (_doorTable.getDoor(doorId) != null)
				{
					_doorTable.getDoor(doorId).closeMe();
				}
				else
				{
					foreach (Castle castle in CastleManager.getInstance().getCastles())
					{
						if (castle.getDoor(doorId) != null)
						{
							castle.getDoor(doorId).closeMe();
						}
					}
				}
			}
			else if (command.equals("admin_closeall"))
			{
				foreach (Door door in _doorTable.getDoors())
				{
					door.closeMe();
				}
				foreach (Castle castle in CastleManager.getInstance().getCastles())
				{
					foreach (Door door in castle.getDoors())
					{
						door.closeMe();
					}
				}
			}
			else if (command.equals("admin_openall"))
			{
				foreach (Door door in _doorTable.getDoors())
				{
					door.openMe();
				}
				foreach (Castle castle in CastleManager.getInstance().getCastles())
				{
					foreach (Door door in castle.getDoors())
					{
						door.openMe();
					}
				}
			}
			else if (command.equals("admin_open"))
			{
				WorldObject target = activeChar.getTarget();
				if ((target != null) && target.isDoor())
				{
					((Door) target).openMe();
				}
				else
				{
					BuilderUtil.sendSysMessage(activeChar, "Incorrect target.");
				}
			}
			else if (command.equals("admin_close"))
			{
				WorldObject target = activeChar.getTarget();
				if ((target != null) && target.isDoor())
				{
					((Door) target).closeMe();
				}
				else
				{
					BuilderUtil.sendSysMessage(activeChar, "Incorrect target.");
				}
			}
			else if (command.equals("admin_showdoors"))
			{
				World.getInstance().forEachVisibleObject<Door>(activeChar, door =>
				{
					ExServerPrimitivePacket packet = new ExServerPrimitivePacket("door" + door.getId(), activeChar.getX(), activeChar.getY(), -16000);
					Color color = door.isOpen() ? Colors.GREEN : Colors.RED;
					// box 1
					packet.addLine(color, door.getX(0), door.getY(0), door.getZMin(), door.getX(1), door.getY(1), door.getZMin());
					packet.addLine(color, door.getX(1), door.getY(1), door.getZMin(), door.getX(2), door.getY(2), door.getZMax());
					packet.addLine(color, door.getX(2), door.getY(2), door.getZMax(), door.getX(3), door.getY(3), door.getZMax());
					packet.addLine(color, door.getX(3), door.getY(3), door.getZMax(), door.getX(0), door.getY(0), door.getZMin());
					// box 2
					packet.addLine(color, door.getX(0), door.getY(0), door.getZMax(), door.getX(1), door.getY(1), door.getZMax());
					packet.addLine(color, door.getX(1), door.getY(1), door.getZMax(), door.getX(2), door.getY(2), door.getZMin());
					packet.addLine(color, door.getX(2), door.getY(2), door.getZMin(), door.getX(3), door.getY(3), door.getZMin());
					packet.addLine(color, door.getX(3), door.getY(3), door.getZMin(), door.getX(0), door.getY(0), door.getZMax());
					// diagonals
					packet.addLine(color, door.getX(0), door.getY(0), door.getZMin(), door.getX(1), door.getY(1), door.getZMax());
					packet.addLine(color, door.getX(2), door.getY(2), door.getZMin(), door.getX(3), door.getY(3), door.getZMax());
					packet.addLine(color, door.getX(0), door.getY(0), door.getZMax(), door.getX(1), door.getY(1), door.getZMin());
					packet.addLine(color, door.getX(2), door.getY(2), door.getZMax(), door.getX(3), door.getY(3), door.getZMin());
					activeChar.sendPacket(packet);
					// send message
					BuilderUtil.sendSysMessage(activeChar, "Found door " + door.getId());
				});
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Problem with AdminDoorControl: " + e);
		}
		
		return true;
	}
	
	public String[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}
