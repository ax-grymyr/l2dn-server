using L2Dn.GameServer.Db;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using NLog;
using Clan = L2Dn.GameServer.Model.Clans.Clan;

namespace L2Dn.GameServer.Scripts.Handlers.UserCommandHandlers;

/**
 * Clan War Start, Under Attack List, War List user commands.
 * @author Tempy
 */
public class ClanWarsList: IUserCommandHandler
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(ClanWarsList));
	private static readonly int[] COMMAND_IDS =
	{
		88,
		89,
		90
	};
	
	public bool useUserCommand(int id, Player player)
	{
		if ((id != COMMAND_IDS[0]) && (id != COMMAND_IDS[1]) && (id != COMMAND_IDS[2]))
		{
			return false;
		}
		
		Clan clan = player.getClan();
		if (clan == null)
		{
			player.sendPacket(SystemMessageId.NOT_JOINED_IN_ANY_CLAN);
			return false;
		}

		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int clanId = clan.getId();

			var query = id switch
			{
				88 => from c in ctx.Clans
					from cw in ctx.ClanWars
					where cw.Clan1Id == clanId && c.Id == cw.Clan2Id &&
					      !ctx.ClanWars.Where(r => r.Clan2Id == clanId).Select(r => r.Clan1Id).Contains(cw.Clan2Id)
					select new
					{
						c.Name,
						c.Id,
						c.AllyId,
						c.AllyName
					},

				89 => from c in ctx.Clans
					from cw in ctx.ClanWars
					where cw.Clan2Id == clanId && c.Id == cw.Clan1Id &&
					      !ctx.ClanWars.Where(r => r.Clan1Id == clanId).Select(r => r.Clan2Id).Contains(cw.Clan1Id)
					select new
					{
						c.Name,
						c.Id,
						c.AllyId,
						c.AllyName
					},

				_ => from c in ctx.Clans
					from cw in ctx.ClanWars
					where cw.Clan1Id == clanId && c.Id == cw.Clan2Id &&
					      ctx.ClanWars.Where(r => r.Clan2Id == clanId).Select(r => r.Clan1Id).Contains(cw.Clan2Id)
					select new
					{
						c.Name,
						c.Id,
						c.AllyId,
						c.AllyName
					}
			};

			// Attack List
			switch (id)
			{
				case 88:
					player.sendPacket(SystemMessageId.CLANS_YOU_VE_DECLARED_WAR_ON);
					break;
				case 89:
					player.sendPacket(SystemMessageId.CLANS_THAT_HAVE_DECLARED_WAR_ON_YOU);
					break;
				default:
					player.sendPacket(SystemMessageId.CLAN_WAR_LIST);
					break;
			}

			SystemMessagePacket sm;
			foreach (var record in query)
			{
				string clanName = record.Name;
				int? allyId = record.AllyId;
				if (allyId != null)
				{
					// Target With Ally
					sm = new SystemMessagePacket(SystemMessageId.S1_S2_ALLIANCE);
					sm.Params.addString(clanName);
					sm.Params.addString(record.AllyName ?? string.Empty);
				}
				else
				{
					// Target Without Ally
					sm = new SystemMessagePacket(SystemMessageId.S1_NO_ALLIANCE);
					sm.Params.addString(clanName);
				}

				player.sendPacket(sm);
			}

			player.sendPacket(SystemMessageId.EMPTY_3);
		}
		catch (Exception e)
		{
			_logger.Warn("", e);
		}

		return true;
	}
	
	public int[] getUserCommandList()
	{
		return COMMAND_IDS;
	}
}