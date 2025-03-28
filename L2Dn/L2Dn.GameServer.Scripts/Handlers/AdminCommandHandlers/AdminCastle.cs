using System.Globalization;
using L2Dn.Extensions;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Html;
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
    [
        "admin_castlemanage",
    ];

	public bool useAdminCommand(string command, Player activeChar)
	{
		StringTokenizer st = new StringTokenizer(command, " ");
		string actualCommand = st.nextToken();
		if (actualCommand.equals("admin_castlemanage"))
		{
			if (st.hasMoreTokens())
			{
				string param = st.nextToken();
				Castle? castle;
				if (int.TryParse(param, CultureInfo.InvariantCulture, out int castleId))
				{
					castle = CastleManager.getInstance().getCastleById(castleId);
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
					string action = st.nextToken();
					switch (action)
					{
						case "showRegWindow":
						{
							castle.getSiege().listRegisterClan(activeChar);
							break;
						}
						case "addAttacker":
						{
							if (CheckTarget(activeChar, out Player? target) && target != null)
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
                            if (CheckTarget(activeChar, out Player? target) && target != null)
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
                            if (CheckTarget(activeChar, out Player? target) && target != null)
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
                            if (CheckTarget(activeChar, out Player? target) && target != null)
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
							if (castle.getSiege().getAttackerClans().Count != 0)
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
							if (!CheckTarget(activeChar, out Player? target) || target == null)
							{
								activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
							}
							else if (target.getClan()?.getCastleId() > 0)
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
                                if (Enum.TryParse(st.nextToken(), true, out CastleSide side))
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
							Clan? clan = ClanTable.getInstance().getClan(castle.getOwnerId());
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
				HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/castlemanage.htm", activeChar);
				NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(null, 1, htmlContent);
				activeChar.sendPacket(html);
			}
		}
		return true;
	}

	private void showCastleMenu(Player player, int castleId)
	{
		Castle? castle = CastleManager.getInstance().getCastleById(castleId);
		if (castle != null)
		{
			Clan? ownerClan = castle.getOwner();

			HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/castlemanage_selected.htm", player);
			NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(null, 1, htmlContent);
			htmlContent.Replace("%castleId%", castle.getResidenceId().ToString());
			htmlContent.Replace("%castleName%", castle.getName());
			htmlContent.Replace("%ownerName%", ownerClan != null ? ownerClan.getLeaderName() : "NPC");
			htmlContent.Replace("%ownerClan%", ownerClan != null ? ownerClan.getName() : "NPC");
			htmlContent.Replace("%castleSide%", castle.getSide().ToString().toLowerCase().CapitalizeFirstLetter());
			player.sendPacket(html);
		}
	}

	private static bool CheckTarget(Player player, out Player? target)
	{
        WorldObject? target1 = player.getTarget();
        if (target1 != null && target1.isPlayer() && ((Player)target1).getClan() != null)
        {
            target = (Player)target1;
            return true;
        }

        target = null;
        return false;
    }

	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}