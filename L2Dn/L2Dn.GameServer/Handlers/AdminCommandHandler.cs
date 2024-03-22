using System.Runtime.CompilerServices;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;
using L2Dn.GameServer.Utilities;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Handlers;

/**
 * @author UnAfraid
 */
public class AdminCommandHandler: IHandler<IAdminCommandHandler, String>
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(AdminCommandHandler));
	
	private readonly Map<String, IAdminCommandHandler> _datatable;
	
	protected AdminCommandHandler()
	{
		_datatable = new();
		
		// Admin Command Handlers
		registerHandler(new AdminAdmin());
		registerHandler(new AdminAnnouncements());
		registerHandler(new AdminBBS());
		registerHandler(new AdminBuffs());
		registerHandler(new AdminCamera());
		registerHandler(new AdminChangeAccessLevel());
		registerHandler(new AdminClan());
		registerHandler(new AdminClanHall());
		registerHandler(new AdminCastle());
		registerHandler(new AdminPcCondOverride());
		registerHandler(new AdminCreateItem());
		registerHandler(new AdminCursedWeapons());
		registerHandler(new AdminDelete());
		registerHandler(new AdminDestroyItems());
		registerHandler(new AdminDisconnect());
		registerHandler(new AdminDoorControl());
		registerHandler(new AdminEditChar());
		registerHandler(new AdminEffects());
		registerHandler(new AdminElement());
		registerHandler(new AdminEnchant());
		registerHandler(new AdminEvents());
		registerHandler(new AdminExpSp());
		registerHandler(new AdminFakePlayers());
		registerHandler(new AdminFence());
		registerHandler(new AdminFightCalculator());
		registerHandler(new AdminFortSiege());
		registerHandler(new AdminGeodata());
		registerHandler(new AdminGm());
		registerHandler(new AdminGmChat());
		registerHandler(new AdminGmSpeed());
		registerHandler(new AdminGraciaSeeds());
		registerHandler(new AdminGrandBoss());
		registerHandler(new AdminHeal());
		registerHandler(new AdminHide());
		registerHandler(new AdminHtml());
		registerHandler(new AdminHwid());
		registerHandler(new AdminInstance());
		registerHandler(new AdminInstanceZone());
		registerHandler(new AdminInvul());
		registerHandler(new AdminKick());
		registerHandler(new AdminKill());
		registerHandler(new AdminLevel());
		//registerHandler(new AdminLogin());
		registerHandler(new AdminManor());
		registerHandler(new AdminMenu());
		registerHandler(new AdminMessages());
		registerHandler(new AdminMissingHtmls());
		registerHandler(new AdminMobGroup());
		registerHandler(new AdminOlympiad());
		registerHandler(new AdminOnline());
		registerHandler(new AdminPathNode());
		registerHandler(new AdminPcCafePoints());
		registerHandler(new AdminPetition());
		//registerHandler(new AdminPForge());
		registerHandler(new AdminPledge());
		//registerHandler(new AdminZones());
		//registerHandler(new AdminPremium());
		registerHandler(new AdminPrimePoints());
		registerHandler(new AdminPunishment());
		//registerHandler(new AdminQuest());
		registerHandler(new AdminReload());
		registerHandler(new AdminRepairChar());
		registerHandler(new AdminRes());
		registerHandler(new AdminRide());
		registerHandler(new AdminScan());
		registerHandler(new AdminServerInfo());
		registerHandler(new AdminShop());
		registerHandler(new AdminShowQuests());
		registerHandler(new AdminShutdown());
		registerHandler(new AdminSkill());
		registerHandler(new AdminSpawn());
		registerHandler(new AdminSummon());
		registerHandler(new AdminSuperHaste());
		registerHandler(new AdminTarget());
		registerHandler(new AdminTargetSay());
		registerHandler(new AdminTeleport());
		registerHandler(new AdminTest());
		registerHandler(new AdminTransform());
		registerHandler(new AdminVitality());
		registerHandler(new AdminZone());
	}
	
	public void registerHandler(IAdminCommandHandler handler)
	{
		foreach (String id in handler.getAdminCommandList())
		{
			_datatable.put(id, handler);
		}
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public void removeHandler(IAdminCommandHandler handler)
	{
		foreach (String id in handler.getAdminCommandList())
		{
			_datatable.remove(id);
		}
	}
	
	/**
	 * WARNING: Please use {@link #useAdminCommand(Player, String, bool)} instead.
	 */
	public IAdminCommandHandler getHandler(String adminCommand)
	{
		String command = adminCommand;
		if (adminCommand.Contains(" "))
		{
			command = adminCommand.Substring(0, adminCommand.IndexOf(' '));
		}
		
		return _datatable.get(command);
	}
	
	public void useAdminCommand(Player player, String fullCommand, bool useConfirm)
	{
		if (!player.isGM())
		{
			return;
		}
		
		String command = fullCommand.Split(" ")[0];
		String commandNoPrefix = command.Substring(6);
		IAdminCommandHandler handler = getHandler(command);
		if (handler == null)
		{
			player.sendMessage("The command '" + commandNoPrefix + "' does not exist!");
			LOGGER.Warn("No handler registered for admin command '" + command + "'");
			return;
		}
		
		if (!AdminData.getInstance().hasAccess(command, player.getAccessLevel()))
		{
			player.sendMessage("You don't have the access rights to use this command!");
			LOGGER.Warn(player + " tried to use admin command '" + command + "', without proper access level!");
			return;
		}

		if (useConfirm && AdminData.getInstance().requireConfirm(command))
		{
			player.setAdminConfirmCmd(fullCommand);
			ConfirmDialogPacket dlg =
				new ConfirmDialogPacket("Are you sure you want execute command '" + commandNoPrefix + "' ?");
			
			player.addAction(PlayerAction.ADMIN_COMMAND);
			player.sendPacket(dlg);
		}
		else
		{
			// Admin Commands must run through a long running task, otherwise a command that takes too much time will freeze the server, this way you'll feel only a minor spike.
			ThreadPool.execute(() =>
			{
				DateTime begin = DateTime.UtcNow;
				try
				{
					if (Config.GMAUDIT)
					{
						WorldObject target = player.getTarget();
						// TODO: GMAudit 
						//GMAudit.auditGMAction(player.getName() + " [" + player.getObjectId() + "]", fullCommand, (target != null ? target.getName() : "no-target"));
					}
					
					handler.useAdminCommand(fullCommand, player);
				}
				catch (Exception e)
				{
					player.sendMessage("Exception during execution of  '" + fullCommand + "': " + e);
					LOGGER.Warn("Exception during execution of " + fullCommand + ": "  + e);
				}
				finally
				{
					TimeSpan runtime = DateTime.UtcNow - begin;
					if (runtime > TimeSpan.FromSeconds(5))
					{
						player.sendMessage("The execution of '" + fullCommand + "' took " + runtime + ".");
					}
				}
			});
		}
	}
	
	public int size()
	{
		return _datatable.size();
	}
	
	public static AdminCommandHandler getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly AdminCommandHandler INSTANCE = new AdminCommandHandler();
	}
}