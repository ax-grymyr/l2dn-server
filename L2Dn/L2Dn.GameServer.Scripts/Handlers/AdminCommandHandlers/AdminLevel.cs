using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

public class AdminLevel: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
    [
        "admin_add_level",
		"admin_set_level",
    ];
	
	public bool useAdminCommand(string command, Player activeChar)
	{
		WorldObject targetChar = activeChar.getTarget();
		StringTokenizer st = new StringTokenizer(command, " ");
		string actualCommand = st.nextToken(); // Get actual command
		string val = "";
		if (st.countTokens() >= 1)
		{
			val = st.nextToken();
		}
		
		if (actualCommand.equalsIgnoreCase("admin_add_level"))
		{
			try
			{
				if ((targetChar != null) && targetChar.isPlayable())
				{
					((Playable) targetChar).getStat().addLevel(int.Parse(val));
				}
			}
			catch (FormatException e)
			{
				BuilderUtil.sendSysMessage(activeChar, "Wrong Number Format");
			}
		}
		else if (actualCommand.equalsIgnoreCase("admin_set_level"))
		{
			if ((targetChar == null) || !targetChar.isPlayer())
			{
				activeChar.sendPacket(SystemMessageId.THAT_IS_AN_INCORRECT_TARGET); // incorrect target!
				return false;
			}
			
			Player targetPlayer = targetChar.getActingPlayer();
			int maxLevel = ExperienceData.getInstance().getMaxLevel();
			if (targetPlayer.isSubClassActive() && !targetPlayer.isDualClassActive() && (Config.MAX_SUBCLASS_LEVEL < maxLevel))
			{
				maxLevel = Config.MAX_SUBCLASS_LEVEL;
			}
			
			try
			{
				int level = int.Parse(val);
				if ((level >= 1) && (level <= maxLevel))
				{
					long pXp = targetPlayer.getExp();
					long tXp = ExperienceData.getInstance().getExpForLevel(level);
					if (pXp > tXp)
					{
						targetPlayer.getStat().setLevel(level);
						targetPlayer.removeExpAndSp(pXp - tXp, 0);
						BuilderUtil.sendSysMessage(activeChar, "Removed " + (pXp - tXp) + " exp.");
					}
					else if (pXp < tXp)
					{
						targetPlayer.addExpAndSp(tXp - pXp, 0);
						BuilderUtil.sendSysMessage(activeChar, "Added " + (tXp - pXp) + " exp.");
					}
					targetPlayer.setCurrentHpMp(targetPlayer.getMaxHp(), targetPlayer.getMaxMp());
					targetPlayer.setCurrentCp(targetPlayer.getMaxCp());
					targetPlayer.broadcastUserInfo();
				}
				else
				{
					BuilderUtil.sendSysMessage(activeChar, "You must specify level between 1 and " + maxLevel + ".");
					return false;
				}
			}
			catch (FormatException e)
			{
				BuilderUtil.sendSysMessage(activeChar, "You must specify level between 1 and " + maxLevel + ".");
				return false;
			}
		}
		return true;
	}
	
	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}
