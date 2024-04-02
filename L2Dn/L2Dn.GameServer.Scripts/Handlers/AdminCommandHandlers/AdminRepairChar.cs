using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * This class handles following admin commands: - delete = deletes target
 * @version $Revision: 1.1.2.6.2.3 $ $Date: 2005/04/11 10:05:59 $
 */
public class AdminRepairChar: IAdminCommandHandler
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(AdminRepairChar));
	
	private static readonly string[] ADMIN_COMMANDS =
	{
		"admin_restore",
		"admin_repair"
	};
	
	public bool useAdminCommand(String command, Player activeChar)
	{
		handleRepair(command);
		return true;
	}
	
	public String[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
	
	private void handleRepair(String command)
	{
		String[] parts = command.Split(" ");
		if (parts.Length != 2)
		{
			return;
		}
		
		try
		{
			string charName = parts[1];
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();

			int? charId = ctx.Characters.Where(c => c.Name == charName).Select(c => (int?)c.Id).FirstOrDefault();
			if (charId is null)
			{
				return;
			}
			
			ctx.Characters.Where(c => c.Id == charId).ExecuteUpdate(s =>
				s.SetProperty(r => r.X, -84318).SetProperty(r => r.Y, 244579).SetProperty(r => r.Z, -3730));


			ctx.CharacterShortCuts.Where(r => r.CharacterId == charId).ExecuteDelete();
			ctx.Items.Where(r => r.OwnerId == charId)
				.ExecuteUpdate(s => s.SetProperty(r => r.Location, (int)ItemLocation.INVENTORY));
		}
		catch (Exception e)
		{
			LOGGER.Warn("could not repair char: " + e);
		}
	}
}
