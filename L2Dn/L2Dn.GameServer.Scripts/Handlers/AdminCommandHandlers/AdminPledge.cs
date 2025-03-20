using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * <b>Pledge Manipulation:</b><br>
 * <li>With target in a character without clan:<br>
 * //pledge create clanname
 * <li>With target in a clan leader:<br>
 * //pledge info<br>
 * //pledge dismiss<br>
 * //pledge setlevel level<br>
 * //pledge rep reputation_points
 */
public class AdminPledge: IAdminCommandHandler
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(AdminPledge));
	private static readonly string[] ADMIN_COMMANDS =
    [
        "admin_pledge",
    ];

	public bool useAdminCommand(string command, Player activeChar)
	{
		StringTokenizer st = new StringTokenizer(command);
		string cmd = st.nextToken();
		WorldObject? target = activeChar.getTarget();
		Player? targetPlayer = target != null && target.isPlayer() ? (Player) target : null;
		Clan? clan = targetPlayer != null ? targetPlayer.getClan() : null;
		if (targetPlayer == null)
		{
			activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
			showMainPage(activeChar);
			return false;
		}
		switch (cmd)
		{
			case "admin_pledge":
			{
				if (!st.hasMoreTokens())
				{
					BuilderUtil.sendSysMessage(activeChar, "Missing parameters!");
					break;
				}
				string action = st.nextToken();
				if (!st.hasMoreTokens())
				{
					BuilderUtil.sendSysMessage(activeChar, "Missing parameters!");
					break;
				}
				string param = st.nextToken();

				switch (action)
				{
					case "create":
					{
						if (clan != null)
						{
							BuilderUtil.sendSysMessage(activeChar, "Target player has clan!");
							break;
						}

						DateTime? penalty = targetPlayer.getClanCreateExpiryTime();
						targetPlayer.setClanCreateExpiryTime(null);
						clan = ClanTable.getInstance().createClan(targetPlayer, param);
						if (clan != null)
						{
							BuilderUtil.sendSysMessage(activeChar, "Clan " + param + " created. Leader: " + targetPlayer.getName());
						}
						else
						{
							targetPlayer.setClanCreateExpiryTime(penalty);
							BuilderUtil.sendSysMessage(activeChar, "There was a problem while creating the clan.");
						}
						break;
					}
					case "dismiss":
					{
						if (clan == null)
						{
							BuilderUtil.sendSysMessage(activeChar, "Target player has no clan!");
							break;
						}

						if (!targetPlayer.isClanLeader())
						{
							SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_IS_NOT_A_CLAN_LEADER);
							sm.Params.addString(targetPlayer.getName());
							activeChar.sendPacket(sm);
							showMainPage(activeChar);
							return false;
						}

						ClanTable.getInstance().destroyClan(targetPlayer.getClanId() ?? 0);
						clan = targetPlayer.getClan();
						if (clan == null)
						{
							BuilderUtil.sendSysMessage(activeChar, "Clan disbanded.");
						}
						else
						{
							BuilderUtil.sendSysMessage(activeChar, "There was a problem while destroying the clan.");
						}
						break;
					}
					case "info":
					{
						if (clan == null)
						{
							BuilderUtil.sendSysMessage(activeChar, "Target player has no clan!");
							break;
						}

						activeChar.sendPacket(new GMViewPledgeInfoPacket(clan, targetPlayer));
						break;
					}
					case "setlevel":
					{
						if (clan == null)
						{
							BuilderUtil.sendSysMessage(activeChar, "Target player has no clan!");
							break;
						}

                        if (param == null)
                        {
                            BuilderUtil.sendSysMessage(activeChar, "Usage: //pledge <setlevel|rep> <number>");
                            break;
                        }

                        int level = int.Parse(param);
						if (level >= 0 && level <= ClanLevelData.getInstance().getMaxLevel())
						{
							clan.changeLevel(level);
							clan.setExp(activeChar.ObjectId, ClanLevelData.getInstance().getLevelExp(level));
							foreach (Player member in clan.getOnlineMembers(0))
							{
								member.broadcastUserInfo(UserInfoType.RELATION, UserInfoType.CLAN);
							}
							BuilderUtil.sendSysMessage(activeChar, "You set level " + level + " for clan " + clan.getName());
						}
						else
						{
							BuilderUtil.sendSysMessage(activeChar, "Level incorrect.");
						}
						break;
					}
					case "rep":
					{
						if (clan == null)
						{
							BuilderUtil.sendSysMessage(activeChar, "Target player has no clan!");
							break;
						}

                        if (clan.getLevel() < 5)
                        {
                            BuilderUtil.sendSysMessage(activeChar, "Only clans of level 5 or above may receive reputation points.");
                            showMainPage(activeChar);
                            return false;
                        }

                        try
						{
							int points = int.Parse(param);
							clan.addReputationScore(points);
							BuilderUtil.sendSysMessage(activeChar, "You " + (points > 0 ? "add " : "remove ") + Math.Abs(points) + " points " + (points > 0 ? "to " : "from ") + clan.getName() + "'s reputation. Their current score is " + clan.getReputationScore());
						}
						catch (Exception e)
						{
                            _logger.Error(e);
							BuilderUtil.sendSysMessage(activeChar, "Usage: //pledge <rep> <number>");
						}
						break;
					}
					case "arena":
					{
						if (clan == null)
						{
							BuilderUtil.sendSysMessage(activeChar, "Target player has no clan!");
							break;
						}

						try
						{
							int stage = int.Parse(param);
							GlobalVariablesManager.getInstance().Set(GlobalVariablesManager.MONSTER_ARENA_VARIABLE + clan.Id, stage);
							BuilderUtil.sendSysMessage(activeChar, "You set " + stage + " Monster Arena stage for clan " + clan.getName() + "");
						}
						catch (Exception e)
						{
                            _logger.Error(e);
							BuilderUtil.sendSysMessage(activeChar, "Usage: //pledge arena <number>");
						}
						break;
					}
				}
				break;
			}
		}
		showMainPage(activeChar);
		return true;
	}

	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}

	private void showMainPage(Player activeChar)
	{
		AdminHtml.showAdminHtml(activeChar, "game_menu.htm");
	}
}