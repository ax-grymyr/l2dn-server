using L2Dn.GameServer.Db;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * Change access level command handler.
 */
public class AdminChangeAccessLevel: IAdminCommandHandler
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(AdminChangeAccessLevel));
    private static readonly string[] ADMIN_COMMANDS = ["admin_changelvl"];

	public bool useAdminCommand(string command, Player activeChar)
	{
		string[] parts = command.Split(" ");
		if (parts.Length == 2)
		{
			try
			{
				int lvl = int.Parse(parts[1]);
				WorldObject? target = activeChar.getTarget();
				if (target == null || !target.isPlayer())
				{
					activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
				}
				else
				{
					onlineChange(activeChar, (Player) target, lvl);
				}
			}
			catch (Exception e)
			{
                _logger.Error(e);
				BuilderUtil.sendSysMessage(activeChar, "Usage: //changelvl <target_new_level> | <player_name> <new_level>");
			}
		}
		else if (parts.Length == 3)
		{
			string name = parts[1];
			int lvl = int.Parse(parts[2]);
			Player? player = World.getInstance().getPlayer(name);
			if (player != null)
			{
				onlineChange(activeChar, player, lvl);
			}
			else
			{
				try
				{
					using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
					int count = ctx.Characters.Where(c => c.Name == name)
						.ExecuteUpdate(s => s.SetProperty(r => r.AccessLevel, lvl));

					if (count == 0)
					{
						BuilderUtil.sendSysMessage(activeChar, "Character not found or access level unaltered.");
					}
					else
					{
						BuilderUtil.sendSysMessage(activeChar, "Character's access level is now set to " + lvl);
					}
				}
				catch (Exception se)
				{
                    _logger.Error(se);
					BuilderUtil.sendSysMessage(activeChar, "SQLException while changing character's access level");
				}
			}
		}
		return true;
	}

	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}

	/**
	 * @param activeChar the active GM
	 * @param player the online target
	 * @param lvl the access level
	 */
	private void onlineChange(Player activeChar, Player player, int lvl)
	{
		if (lvl >= 0)
		{
			AccessLevel accessLevel = AccessLevelData.Instance.GetAccessLevel(lvl);
			player.setAccessLevel(lvl, true, true);
			player.sendMessage("Your access level has been changed to " + accessLevel.Name + " (" + accessLevel.Level + ").");
			activeChar.sendMessage(player.getName() + "'s access level has been changed to " + accessLevel.Name + " (" + accessLevel.Level + ").");
		}
		else
		{
			player.setAccessLevel(-1, false, true);
			player.sendMessage("Your character has been banned. Bye.");
			Disconnection.of(player).defaultSequence(LeaveWorldPacket.STATIC_PACKET);
		}
	}
}