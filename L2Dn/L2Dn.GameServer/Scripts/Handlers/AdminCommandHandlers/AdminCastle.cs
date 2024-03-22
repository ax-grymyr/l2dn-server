using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using Clan = L2Dn.GameServer.Model.Clans.Clan;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * Admin Castle manage admin commands.
 * @author St3eT
 */
public class AdminCastle: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
	{
		"admin_castlemanage",
	};
	
	public bool useAdminCommand(String command, Player activeChar)
	{
		StringTokenizer st = new StringTokenizer(command, " ");
		String actualCommand = st.nextToken();
		if (actualCommand.equals("admin_castlemanage"))
		{
			if (st.hasMoreTokens())
			{
				String param = st.nextToken();
				Castle castle;
				if (Util.isDigit(param))
				{
					castle = CastleManager.getInstance().getCastleById(int.Parse(param));
				}
				else
				{
					castle = CastleManager.getInstance().getCastle(param);
				}
				
				if (castle == null)
				{
					BuilderUtil.sendSysMessage(activeChar, "Invalid parameters! Usage: //castlemanage <castleId[1-9] / castleName>");
					return false;
				}
				
				if (!st.hasMoreTokens())
				{
					showCastleMenu(activeChar, castle.getResidenceId());
				}
				else
				{
					String action = st.nextToken();
					Player target = checkTarget(activeChar) ? activeChar.getTarget().getActingPlayer() : null;
					switch (action)
					{
						case "showRegWindow":
						{
							castle.getSiege().listRegisterClan(activeChar);
							break;
						}
						case "addAttacker":
						{
							if (checkTarget(activeChar))
							{
								castle.getSiege().registerAttacker(target, true);
							}
							else
							{
								activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
							}
							break;
						}
						case "removeAttacker":
						{
							if (checkTarget(activeChar))
							{
								castle.getSiege().removeSiegeClan(activeChar);
							}
							else
							{
								activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
							}
							break;
						}
						case "addDeffender":
						{
							if (checkTarget(activeChar))
							{
								castle.getSiege().registerDefender(target, true);
							}
							else
							{
								activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
							}
							break;
						}
						case "removeDeffender":
						{
							if (checkTarget(activeChar))
							{
								castle.getSiege().removeSiegeClan(target);
							}
							else
							{
								activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
							}
							break;
						}
						case "startSiege":
						{
							if (!castle.getSiege().getAttackerClans().isEmpty())
							{
								castle.getSiege().startSiege();
							}
							else
							{
								BuilderUtil.sendSysMessage(activeChar, "There is currently not registered any clan for castle siege!");
							}
							break;
						}
						case "stopSiege":
						{
							if (castle.getSiege().isInProgress())
							{
								castle.getSiege().endSiege();
							}
							else
							{
								BuilderUtil.sendSysMessage(activeChar, "Castle siege is not currently in progress!");
							}
							showCastleMenu(activeChar, castle.getResidenceId());
							break;
						}
						case "setOwner":
						{
							if ((target == null) || !checkTarget(activeChar))
							{
								activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
							}
							else if (target.getClan().getCastleId() > 0)
							{
								BuilderUtil.sendSysMessage(activeChar, "This clan already have castle!");
							}
							else if (castle.getOwner() != null)
							{
								BuilderUtil.sendSysMessage(activeChar, "This castle is already taken by another clan!");
							}
							else if (!st.hasMoreTokens())
							{
								BuilderUtil.sendSysMessage(activeChar, "Invalid parameters!!");
							}
							else
							{
								CastleSide side = Enum.Parse<CastleSide>(st.nextToken().toUpperCase());
								if (side != null)
								{
									castle.setSide(side);
									castle.setOwner(target.getClan());
								}
							}
							showCastleMenu(activeChar, castle.getResidenceId());
							break;
						}
						case "takeCastle":
						{
							Model.Clans.Clan clan = ClanTable.getInstance().getClan(castle.getOwnerId());
							if (clan != null)
							{
								castle.removeOwner(clan);
							}
							else
							{
								BuilderUtil.sendSysMessage(activeChar, "Error during removing castle!");
							}
							showCastleMenu(activeChar, castle.getResidenceId());
							break;
						}
						case "switchSide":
						{
							if (castle.getSide() == CastleSide.DARK)
							{
								castle.setSide(CastleSide.LIGHT);
							}
							else if (castle.getSide() == CastleSide.LIGHT)
							{
								castle.setSide(CastleSide.DARK);
							}
							else
							{
								BuilderUtil.sendSysMessage(activeChar, "You can't switch sides when is castle neutral!");
							}
							showCastleMenu(activeChar, castle.getResidenceId());
							break;
						}
					}
				}
			}
			else
			{
				NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(0, 1, HtmCache.getInstance().getHtm(activeChar, "html/admin/castlemanage.htm"));
				activeChar.sendPacket(html);
			}
		}
		return true;
	}
	
	private void showCastleMenu(Player player, int castleId)
	{
		Castle castle = CastleManager.getInstance().getCastleById(castleId);
		if (castle != null)
		{
			Clan ownerClan = castle.getOwner();
			
			HtmlPacketHelper helper =
				new HtmlPacketHelper(HtmCache.getInstance().getHtm(player, "html/admin/castlemanage_selected.htm"));
			
			NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(0, 1, helper);
			helper.Replace("%castleId%", castle.getResidenceId().ToString());
			helper.Replace("%castleName%", castle.getName());
			helper.Replace("%ownerName%", ownerClan != null ? ownerClan.getLeaderName() : "NPC");
			helper.Replace("%ownerClan%", ownerClan != null ? ownerClan.getName() : "NPC");
			helper.Replace("%castleSide%", CommonUtil.capitalizeFirst(castle.getSide().ToString().toLowerCase()));
			player.sendPacket(html);
		}
	}
	
	private bool checkTarget(Player player)
	{
		return ((player.getTarget() != null) && player.getTarget().isPlayer() && (((Player) player.getTarget()).getClan() != null));
	}
	
	public String[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}