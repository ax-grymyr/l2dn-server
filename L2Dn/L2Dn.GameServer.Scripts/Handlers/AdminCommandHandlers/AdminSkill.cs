using System.Text;
using L2Dn.GameServer.Data;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * This class handles following admin commands:
 * <ul>
 * <li>show_skills</li>
 * <li>remove_skills</li>
 * <li>skill_list</li>
 * <li>skill_index</li>
 * <li>add_skill</li>
 * <li>remove_skill</li>
 * <li>get_skills</li>
 * <li>reset_skills</li>
 * <li>give_all_skills</li>
 * <li>give_all_skills_fs</li>
 * <li>admin_give_all_clan_skills</li>
 * <li>remove_all_skills</li>
 * <li>add_clan_skills</li>
 * <li>admin_setskill</li>
 * </ul>
 * @version 2012/02/26 Small fixes by Zoey76 05/03/2011
 */
public class AdminSkill: IAdminCommandHandler
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(AdminSkill));

	private static readonly string[] ADMIN_COMMANDS =
    [
        "admin_show_skills",
		"admin_remove_skills",
		"admin_skill_list",
		"admin_skill_index",
		"admin_add_skill",
		"admin_remove_skill",
		"admin_get_skills",
		"admin_reset_skills",
		"admin_give_all_skills",
		"admin_give_all_skills_fs",
		"admin_give_clan_skills",
		"admin_give_all_clan_skills",
		"admin_remove_all_skills",
		"admin_add_clan_skill",
		"admin_setskill",
		"admin_cast",
		"admin_castnow",
    ];

	private static Skill[]? _adminSkills;

	public bool useAdminCommand(string commandValue, Player activeChar)
	{
		string command = commandValue;
		if (command.equals("admin_show_skills"))
		{
			showMainPage(activeChar);
		}
		else if (command.startsWith("admin_remove_skills"))
		{
			try
			{
				string val = command.Substring(20);
				removeSkillsPage(activeChar, int.Parse(val));
			}
			catch (IndexOutOfRangeException e)
			{
                LOGGER.Error(e);
				// Not important.
			}
		}
		else if (command.startsWith("admin_skill_list"))
		{
			AdminHtml.showAdminHtml(activeChar, "skills.htm");
		}
		else if (command.startsWith("admin_skill_index"))
		{
			try
			{
				string val = command.Substring(18);
				AdminHtml.showAdminHtml(activeChar, "skills/" + val + ".htm");
			}
			catch (IndexOutOfRangeException e)
			{
                LOGGER.Error(e);
				// Not important.
			}
		}
		else if (command.startsWith("admin_add_skill"))
		{
			try
			{
				string val = command.Substring(15);
				adminAddSkill(activeChar, val);
			}
			catch (Exception e)
			{
                LOGGER.Error(e);
				BuilderUtil.sendSysMessage(activeChar, "Usage: //add_skill <skill_id> <level>");
			}
		}
		else if (command.startsWith("admin_remove_skill"))
		{
			try
			{
				string id = command.Substring(19);
				int idval = int.Parse(id);
				adminRemoveSkill(activeChar, idval);
			}
			catch (Exception e)
			{
                LOGGER.Error(e);
				BuilderUtil.sendSysMessage(activeChar, "Usage: //remove_skill <skill_id>");
			}
		}
		else if (command.equals("admin_get_skills"))
		{
			adminGetSkills(activeChar);
		}
		else if (command.equals("admin_reset_skills"))
		{
			adminResetSkills(activeChar);
		}
		else if (command.equals("admin_give_all_skills"))
		{
			adminGiveAllSkills(activeChar, false, false);
		}
		else if (command.equals("admin_give_all_skills_fs"))
		{
			adminGiveAllSkills(activeChar, true, true);
		}
		else if (command.equals("admin_give_clan_skills"))
		{
			adminGiveClanSkills(activeChar, false);
		}
		else if (command.equals("admin_give_all_clan_skills"))
		{
			adminGiveClanSkills(activeChar, true);
		}
		else if (command.equals("admin_remove_all_skills"))
		{
			WorldObject? target = activeChar.getTarget();
            Player? player = target?.getActingPlayer();
			if (target == null || !target.isPlayer() || player == null)
			{
				activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
				return false;
			}

            foreach (Skill skill in player.getAllSkills())
			{
				player.removeSkill(skill);
			}
			BuilderUtil.sendSysMessage(activeChar, "You have removed all skills from " + player.getName() + ".");
			player.sendMessage("Admin removed all skills from you.");
			player.sendSkillList();
			player.broadcastUserInfo();
			player.sendPacket(new AcquireSkillListPacket(player));
		}
		else if (command.startsWith("admin_add_clan_skill"))
		{
			try
			{
				string[] val = command.Split(" ");
				adminAddClanSkill(activeChar, int.Parse(val[1]), int.Parse(val[2]));
			}
			catch (Exception e)
			{
                LOGGER.Error(e);
				BuilderUtil.sendSysMessage(activeChar, "Usage: //add_clan_skill <skill_id> <level>");
			}
		}
		else if (command.startsWith("admin_setskill"))
		{
			string[] Split = command.Split(" ");
			int id = int.Parse(Split[1]);
			int level = int.Parse(Split[2]);
			Skill? skill = SkillData.Instance.GetSkill(id, level);
			if (skill != null)
			{
				activeChar.addSkill(skill);
				activeChar.sendSkillList();
				BuilderUtil.sendSysMessage(activeChar, "You added yourself skill " + skill.Name + "(" + id + ") level " + level);
				activeChar.sendPacket(new AcquireSkillListPacket(activeChar));
			}
			else
			{
				BuilderUtil.sendSysMessage(activeChar, "No such skill found. Id: " + id + " Level: " + level);
			}
		}
		else if (command.startsWith("admin_cast"))
		{
			StringTokenizer st = new StringTokenizer(command, " ");
			command = st.nextToken();
			if (!st.hasMoreTokens())
			{
				BuilderUtil.sendSysMessage(activeChar, "Skill Id and level are not specified.");
				BuilderUtil.sendSysMessage(activeChar, "Usage: //cast <skillId> <skillLevel>");
				return false;
			}

			try
			{
				int skillId = int.Parse(st.nextToken());
				int skillLevel = st.hasMoreTokens() ? int.Parse(st.nextToken()) : SkillData.Instance.GetMaxLevel(skillId);
				Skill? skill = SkillData.Instance.GetSkill(skillId, skillLevel);
				if (skill == null)
				{
					BuilderUtil.sendSysMessage(activeChar, "Skill with id: " + skillId + ", level: " + skillLevel + " not found.");
					return false;
				}

				if (command.equalsIgnoreCase("admin_castnow"))
				{
					BuilderUtil.sendSysMessage(activeChar, "Admin instant casting " + skill.Name + " (" + skillId + "," + skillLevel + ")");
					WorldObject? target = skill.GetTarget(activeChar, true, false, true);
					if (target != null)
					{
						skill.ForEachTargetAffected<Creature>(activeChar, target, o =>
						{
							if (o.isCreature())
							{
								skill.ActivateSkill(activeChar, [o]);
							}
						});
					}
				}
				else
				{
					BuilderUtil.sendSysMessage(activeChar, "Admin casting " + skill.Name + " (" + skillId + "," + skillLevel + ")");
					activeChar.doCast(skill);
				}

				return true;
			}
			catch (Exception e)
			{
				BuilderUtil.sendSysMessage(activeChar, "Failed casting: " + e);
				BuilderUtil.sendSysMessage(activeChar, "Usage: //cast <skillId> <skillLevel>");
				return false;
			}
		}
		return true;
	}

	/**
	 * This function will give all the skills that the target can learn at his/her level
	 * @param activeChar the player
	 * @param includeByFs if {@code true} Forgotten Scroll skills will be delivered.
	 * @param includeRequiredItems if {@code true} skills that have required items will be added
	 */
	private void adminGiveAllSkills(Player activeChar, bool includeByFs, bool includeRequiredItems)
	{
		WorldObject? target = activeChar.getTarget();
        Player? player = target?.getActingPlayer();
		if (target == null || !target.isPlayer() || player == null)
		{
			activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
			return;
		}

        // Notify player and admin
		BuilderUtil.sendSysMessage(activeChar, "You gave " + player.giveAvailableSkills(includeByFs, true, includeRequiredItems) + " skills to " + player.getName());
		player.sendSkillList();
		player.sendPacket(new AcquireSkillListPacket(player));
	}

	/**
	 * This function will give all the skills that the target's clan can learn at it's level.<br>
	 * If the target is not the clan leader, a system message will be sent to the Game Master.
	 * @param activeChar the player, probably a Game Master.
	 * @param includeSquad if Squad skills is included
	 */
	private void adminGiveClanSkills(Player activeChar, bool includeSquad)
	{
		WorldObject? target = activeChar.getTarget();
        Player? player = target?.getActingPlayer();
		if (target == null || !target.isPlayer() || player == null)
		{
			activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
			return;
		}

		Clan? clan = player.getClan();
		if (clan == null)
		{
			activeChar.sendPacket(SystemMessageId.THE_TARGET_MUST_BE_A_CLAN_MEMBER);
			return;
		}

		if (!player.isClanLeader())
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_IS_NOT_A_CLAN_LEADER);
			sm.Params.addString(player.getName());
			activeChar.sendPacket(sm);
		}

		Map<int, SkillLearn> skills = SkillTreeData.getInstance().getMaxPledgeSkills(clan, includeSquad);
		foreach (SkillLearn s in skills.Values)
        {
            Skill? skillData = SkillData.Instance.GetSkill(s.getSkillId(), s.getSkillLevel());
            if (skillData != null)
			    clan.addNewSkill(skillData);
		}

		// Notify target and active char
		clan.broadcastToOnlineMembers(new PledgeSkillListPacket(clan));
		foreach (Player member in clan.getOnlineMembers(0))
		{
			member.sendSkillList();
		}

		BuilderUtil.sendSysMessage(activeChar, "You gave " + skills.Count + " skills to " + player.getName() + "'s clan " + clan.getName() + ".");
		player.sendMessage("Your clan received " + skills.Count + " skills.");
	}

	/**
	 * TODO: Externalize HTML
	 * @param activeChar the active Game Master.
	 * @param pageValue
	 */
	private void removeSkillsPage(Player activeChar, int pageValue)
	{
		WorldObject? target = activeChar.getTarget();
        Player? player = target?.getActingPlayer();
		if (target == null || !target.isPlayer() || player == null)
		{
			activeChar.sendPacket(SystemMessageId.THAT_IS_AN_INCORRECT_TARGET);
			return;
		}

		Skill[] skills = player.getAllSkills().ToArray();
		int maxSkillsPerPage = 30;
		int maxPages = skills.Length / maxSkillsPerPage;
		if (skills.Length > maxSkillsPerPage * maxPages)
		{
			maxPages++;
		}

		int page = pageValue;
		if (page > maxPages)
		{
			page = maxPages;
		}

		int skillsStart = maxSkillsPerPage * page;
		int skillsEnd = skills.Length;
		if (skillsEnd - skillsStart > maxSkillsPerPage)
		{
			skillsEnd = skillsStart + maxSkillsPerPage;
		}

		StringBuilder replyMSG = new StringBuilder(500 + maxPages * 50 + (skillsEnd - skillsStart + 1) * 50);
		replyMSG.Append("<html><body><table width=260><tr><td width=40><button value=\"Main\" action=\"bypass admin_admin\" width=40 height=15 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td><td width=180><center>Character Selection Menu</center></td><td width=40><button value=\"Back\" action=\"bypass -h admin_show_skills\" width=40 height=15 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td></tr></table><br><br><center>Editing <font color=\"LEVEL\">" + player.getName() + "</font></center><br><table width=270><tr><td>Lv: " + player.getLevel() + " " + ClassListData.getInstance().getClass(player.getClassId()).getClientCode() + "</td></tr></table><br><table width=270><tr><td>Note: Dont forget that modifying players skills can</td></tr><tr><td>ruin the game...</td></tr></table><br><center>Click on the skill you wish to remove:</center><br><center><table width=270><tr>");
		for (int x = 0; x < maxPages; x++)
		{
			int pagenr = x + 1;
			replyMSG.Append("<td><a action=\"bypass -h admin_remove_skills " + x + "\">P" + pagenr + "</a></td>");
		}

		replyMSG.Append("</tr></table></center><br><table width=270><tr><td width=80>Name:</td><td width=60>Level:</td><td width=40>Id:</td></tr>");
		for (int i = skillsStart; i < skillsEnd; i++)
		{
			replyMSG.Append("<tr><td width=80><a action=\"bypass -h admin_remove_skill " + skills[i].Id + "\">" + skills[i].Name + "</a></td><td width=60>" + skills[i].Level + "</td><td width=40>" + skills[i].Id + "</td></tr>");
		}

		replyMSG.Append("</table><br><center><table>Remove skill by ID :<tr><td>Id: </td><td><edit var=\"id_to_remove\" width=110></td></tr></table></center><center><button value=\"Remove skill\" action=\"bypass -h admin_remove_skill $id_to_remove\" width=110 height=15 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></center><br><center><button value=\"Back\" action=\"bypass -h admin_current_player\" width=40 height=15 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></center></body></html>");

		HtmlContent htmlContent = HtmlContent.LoadFromText(replyMSG.ToString(), player);
		NpcHtmlMessagePacket adminReply = new NpcHtmlMessagePacket(null, 1, htmlContent);
		activeChar.sendPacket(adminReply);
	}

	/**
	 * @param activeChar the active Game Master.
	 */
	private void showMainPage(Player activeChar)
	{
		WorldObject? target = activeChar.getTarget();
        Player? player = target?.getActingPlayer();
		if (target == null || !target.isPlayer() || player == null)
		{
			activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
			return;
		}

		HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/charskills.htm", player);
		htmlContent.Replace("%name%", player.getName());
		htmlContent.Replace("%level%", player.getLevel().ToString());
		htmlContent.Replace("%class%", ClassListData.getInstance().getClass(player.getClassId()).getClientCode());
		NpcHtmlMessagePacket adminReply = new NpcHtmlMessagePacket(null, 1, htmlContent);
		activeChar.sendPacket(adminReply);
	}

	/**
	 * @param activeChar the active Game Master.
	 */
	private void adminGetSkills(Player activeChar)
	{
		WorldObject? target = activeChar.getTarget();
        Player? player = target?.getActingPlayer();
		if (target == null || !target.isPlayer() || player == null)
		{
			activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
			return;
		}

        if (player.getName().equals(activeChar.getName()))
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_USE_THIS_ON_YOURSELF);
		}
		else
		{
			Skill[] skills = player.getAllSkills().ToArray();
			_adminSkills = activeChar.getAllSkills().ToArray();
			foreach (Skill skill in _adminSkills)
			{
				activeChar.removeSkill(skill);
			}
			foreach (Skill skill in skills)
			{
				activeChar.addSkill(skill, true);
			}
			BuilderUtil.sendSysMessage(activeChar, "You now have all the skills of " + player.getName() + ".");
			activeChar.sendSkillList();
		}
		showMainPage(activeChar);
	}

	/**
	 * @param activeChar the active Game Master.
	 */
	private void adminResetSkills(Player activeChar)
	{
		WorldObject? target = activeChar.getTarget();
        Player? player = target?.getActingPlayer();
		if (target == null || !target.isPlayer() || player == null)
		{
			activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
			return;
		}

        if (_adminSkills == null)
		{
			BuilderUtil.sendSysMessage(activeChar, "You must get the skills of someone in order to do this.");
		}
		else
		{
			Skill[] skills = player.getAllSkills().ToArray();
			foreach (Skill skill in skills)
			{
				player.removeSkill(skill);
			}
			foreach (Skill skill in activeChar.getAllSkills())
			{
				player.addSkill(skill, true);
			}
			foreach (Skill skill in skills)
			{
				activeChar.removeSkill(skill);
			}
			foreach (Skill skill in _adminSkills)
			{
				activeChar.addSkill(skill, true);
			}
			player.sendMessage("[GM]" + activeChar.getName() + " updated your skills.");
			BuilderUtil.sendSysMessage(activeChar, "You now have all your skills back.");
			_adminSkills = null;
			activeChar.sendSkillList();
			player.sendSkillList();
		}
		showMainPage(activeChar);
	}

	/**
	 * @param activeChar the active Game Master.
	 * @param value
	 */
	private void adminAddSkill(Player activeChar, string value)
	{
		WorldObject? target = activeChar.getTarget();
        Player? player = target?.getActingPlayer();
		if (target == null || !target.isPlayer() || player == null)
		{
			activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
			showMainPage(activeChar);
			return;
		}

        StringTokenizer st = new StringTokenizer(value);
		if (st.countTokens() != 1 && st.countTokens() != 2)
		{
			showMainPage(activeChar);
		}
		else
		{
			Skill? skill = null;
			try
			{
				string id = st.nextToken();
				string? level = st.countTokens() == 1 ? st.nextToken() : null;
				int idval = int.Parse(id);
				int levelval = level == null ? 1 : int.Parse(level);
				skill = SkillData.Instance.GetSkill(idval, levelval);
			}
			catch (Exception e)
			{
				LOGGER.Warn(e);
			}
			if (skill != null)
			{
				string name = skill.Name;
				// Player's info.
				player.sendMessage("Admin gave you the skill " + name + ".");
				player.addSkill(skill, true);
				player.sendSkillList();
				// Admin info.
				BuilderUtil.sendSysMessage(activeChar, "You gave the skill " + name + " to " + player.getName() + ".");
				activeChar.sendSkillList();
			}
			else
			{
				BuilderUtil.sendSysMessage(activeChar, "Error: there is no such skill.");
			}
			showMainPage(activeChar); // Back to start
		}
	}

	/**
	 * @param activeChar the active Game Master.
	 * @param idval
	 */
	private void adminRemoveSkill(Player activeChar, int idval)
	{
		WorldObject? target = activeChar.getTarget();
        Player? player = target?.getActingPlayer();
		if (target == null || !target.isPlayer() || player == null)
		{
			activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
			return;
		}

        Skill? skill = SkillData.Instance.GetSkill(idval, player.getSkillLevel(idval));
		if (skill != null)
		{
			string skillname = skill.Name;
			player.sendMessage("Admin removed the skill " + skillname + " from your skills list.");
			player.removeSkill(skill);
			// Admin information
			BuilderUtil.sendSysMessage(activeChar, "You removed the skill " + skillname + " from " + player.getName() + ".");
			activeChar.sendSkillList();
		}
		else
		{
			BuilderUtil.sendSysMessage(activeChar, "Error: there is no such skill.");
		}
		removeSkillsPage(activeChar, 0); // Back to previous page
	}

	/**
	 * @param activeChar the active Game Master.
	 * @param id
	 * @param level
	 */
	private void adminAddClanSkill(Player activeChar, int id, int level)
	{
		WorldObject? target = activeChar.getTarget();
        Player? player = target?.getActingPlayer();
		if (target == null || !target.isPlayer() || player == null)
		{
			activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
			showMainPage(activeChar);
			return;
		}

		SystemMessagePacket sm;
        Clan? clan = player.getClan();
		if (!player.isClanLeader() || clan == null)
		{
			sm = new SystemMessagePacket(SystemMessageId.S1_IS_NOT_A_CLAN_LEADER);
			sm.Params.addString(player.getName());
			activeChar.sendPacket(sm);
			showMainPage(activeChar);
			return;
		}
		if (id < 370 || id > 391 || level < 1 || level > 3)
		{
			BuilderUtil.sendSysMessage(activeChar, "Usage: //add_clan_skill <skill_id> <level>");
			showMainPage(activeChar);
			return;
		}

		Skill? skill = SkillData.Instance.GetSkill(id, level);
		if (skill == null)
		{
			BuilderUtil.sendSysMessage(activeChar, "Error: there is no such skill.");
			return;
		}

		string skillname = skill.Name;
		sm = new SystemMessagePacket(SystemMessageId.THE_CLAN_SKILL_S1_HAS_BEEN_ADDED);
		sm.Params.addSkillName(skill);
		player.sendPacket(sm);
		clan.broadcastToOnlineMembers(sm);
		clan.addNewSkill(skill);
		BuilderUtil.sendSysMessage(activeChar, "You gave the Clan Skill: " + skillname + " to the clan " + clan.getName() + ".");
		clan.broadcastToOnlineMembers(new PledgeSkillListPacket(clan));
		foreach (Player member in clan.getOnlineMembers(0))
		{
			member.sendSkillList();
		}

		showMainPage(activeChar);
	}

	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}