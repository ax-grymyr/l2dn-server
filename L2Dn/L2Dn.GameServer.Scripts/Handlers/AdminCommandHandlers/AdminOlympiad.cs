using System.Globalization;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Olympiads;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * @author UnAfraid
 */
public class AdminOlympiad: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
	{
		"admin_olympiad_game",
		"admin_addolypoints",
		"admin_removeolypoints",
		"admin_setolypoints",
	};
	
	public bool useAdminCommand(string command, Player activeChar)
	{
		StringTokenizer st = new StringTokenizer(command);
		string cmd = st.nextToken();
		switch (cmd)
		{
			case "admin_olympiad_game":
			{
				if (!st.hasMoreTokens())
				{
					BuilderUtil.sendSysMessage(activeChar, "Syntax: //olympiad_game <player name>");
					return false;
				}
				
				Player player = World.getInstance().getPlayer(st.nextToken());
				if (player == null)
				{
					activeChar.sendPacket(SystemMessageId.YOUR_TARGET_CANNOT_BE_FOUND);
					return false;
				}
				
				if (player == activeChar)
				{
					activeChar.sendPacket(SystemMessageId.YOU_CANNOT_USE_THIS_ON_YOURSELF);
					return false;
				}
				
				if (!checkplayer(player, activeChar) || !checkplayer(activeChar, activeChar))
				{
					return false;
				}
				
				for (int i = 0; i < OlympiadGameManager.getInstance().getNumberOfStadiums(); i++)
				{
					OlympiadGameTask task = OlympiadGameManager.getInstance().getOlympiadTask(i);
					if (task != null)
					{
						lock (task)
						{
							if (!task.isRunning())
							{
								Participant[] players = new Participant[2];
								players[0] = new Participant(activeChar, 1);
								players[1] = new Participant(player, 2);
								task.attachGame(new OlympiadGameNonClassed(i, players));
								return true;
							}
						}
					}
				}
				break;
			}
			case "admin_addolypoints":
			{
				WorldObject target = activeChar.getTarget();
				Player player = target != null ? target.getActingPlayer() : null;
				if (player != null)
				{
					int val = parseInt(st);
					if (val == -1)
					{
						BuilderUtil.sendSysMessage(activeChar, "Syntax: //addolypoints <points>");
						return false;
					}
					
					if (player.isNoble())
					{
						NobleData statDat = getPlayerSet(player);
						int oldpoints = Olympiad.getInstance().getNoblePoints(player);
						int points = Math.Max(oldpoints + val, 0);
						if (points > 1000)
						{
							BuilderUtil.sendSysMessage(activeChar, "You can't set more than 1000 or less than 0 Olympiad points!");
							return false;
						}
						
						statDat.OlympiadPoints = points;
						BuilderUtil.sendSysMessage(activeChar, "Player " + player.getName() + " now has " + points + " Olympiad points.");
					}
					else
					{
						BuilderUtil.sendSysMessage(activeChar, "This player is not noblesse!");
						return false;
					}
				}
				else
				{
					BuilderUtil.sendSysMessage(activeChar, "Usage: target a player and write the amount of points you would like to add.");
					BuilderUtil.sendSysMessage(activeChar, "Example: //addolypoints 10");
					BuilderUtil.sendSysMessage(activeChar, "However, keep in mind that you can't have less than 0 or more than 1000 points.");
				}
				break;
			}
			case "admin_removeolypoints":
			{
				WorldObject target = activeChar.getTarget();
				Player player = target != null ? target.getActingPlayer() : null;
				if (player != null)
				{
					int val = parseInt(st);
					if (val == -1)
					{
						BuilderUtil.sendSysMessage(activeChar, "Syntax: //removeolypoints <points>");
						return false;
					}
					
					if (player.isNoble())
					{
						NobleData nobleData = Olympiad.getNobleStats(player.ObjectId);
						if (nobleData == null)
						{
							BuilderUtil.sendSysMessage(activeChar, "This player hasn't played on Olympiad yet!");
							return false;
						}
						
						int oldpoints = Olympiad.getInstance().getNoblePoints(player);
						int points = Math.Max(oldpoints - val, 0);
						nobleData.OlympiadPoints += points;
						BuilderUtil.sendSysMessage(activeChar, "Player " + player.getName() + " now has " + points + " Olympiad points.");
					}
					else
					{
						BuilderUtil.sendSysMessage(activeChar, "This player is not noblesse!");
						return false;
					}
				}
				else
				{
					BuilderUtil.sendSysMessage(activeChar, "Usage: target a player and write the amount of points you would like to remove.");
					BuilderUtil.sendSysMessage(activeChar, "Example: //removeolypoints 10");
					BuilderUtil.sendSysMessage(activeChar, "However, keep in mind that you can't have less than 0 or more than 1000 points.");
				}
				break;
			}
			case "admin_setolypoints":
			{
				WorldObject target = activeChar.getTarget();
				Player player = target != null ? target.getActingPlayer() : null;
				if (player != null)
				{
					int val = parseInt(st);
					if (val == -1)
					{
						BuilderUtil.sendSysMessage(activeChar, "Syntax: //setolypoints <points>");
						return false;
					}
					
					if (player.isNoble())
					{
						NobleData statDat = getPlayerSet(player);
						int oldpoints = Olympiad.getInstance().getNoblePoints(player);
						int points = oldpoints - val;
						if ((points < 1) && (points > 1000))
						{
							BuilderUtil.sendSysMessage(activeChar, "You can't set more than 1000 or less than 0 Olympiad points! or lower then 0");
							return false;
						}
						
						statDat.OlympiadPoints = points;
						BuilderUtil.sendSysMessage(activeChar, "Player " + player.getName() + " now has " + points + " Olympiad points.");
					}
					else
					{
						BuilderUtil.sendSysMessage(activeChar, "This player is not noblesse!");
						return false;
					}
				}
				else
				{
					BuilderUtil.sendSysMessage(activeChar, "Usage: target a player and write the amount of points you would like to set.");
					BuilderUtil.sendSysMessage(activeChar, "Example: //setolypoints 10");
					BuilderUtil.sendSysMessage(activeChar, "However, keep in mind that you can't have less than 0 or more than 1000 points.");
				}
				break;
			}
		}
		return false;
	}
	
	private int parseInt(StringTokenizer st)
	{
		string token = st.nextToken();
		if (!int.TryParse(token, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int value))
		{
			return -1;
		}

		return value;
	}
	
	private NobleData getPlayerSet(Player player)
	{
		NobleData statDat = Olympiad.getNobleStats(player.ObjectId);
		if (statDat == null)
		{
			statDat = new NobleData();
			statDat.Class = player.getBaseClass();
			statDat.CharacterName = player.getName();
			statDat.OlympiadPoints = Olympiad.DEFAULT_POINTS;
			statDat.CompetitionsDone = 0;
			statDat.CompetitionsWon = 0;
			statDat.CompetitionsLost = 0;
			statDat.CompetitionsDrawn = 0;
			statDat.CompetitionsDoneWeek = 0;
			//statDat.set("to_save", true);
			Olympiad.addNobleStats(player.ObjectId, statDat);
		}
		
		return statDat;
	}
	
	private bool checkplayer(Player player, Player activeChar)
	{
		if (player.isSubClassActive())
		{
			BuilderUtil.sendSysMessage(activeChar, "Player " + player + " subclass active.");
			return false;
		}
		else if (player.getClassId().GetLevel() < 3)
		{
			BuilderUtil.sendSysMessage(activeChar, "Player " + player + " has not 3rd class.");
			return false;
		}
		else if (Olympiad.getInstance().getNoblePoints(player) <= 0)
		{
			BuilderUtil.sendSysMessage(activeChar, "Player " + player + " has 0 oly points (add them with (//addolypoints).");
			return false;
		}
		else if (OlympiadManager.getInstance().isRegistered(player))
		{
			BuilderUtil.sendSysMessage(activeChar, "Player " + player + " registered to oly.");
			return false;
		}
		return true;
	}
	
	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}
