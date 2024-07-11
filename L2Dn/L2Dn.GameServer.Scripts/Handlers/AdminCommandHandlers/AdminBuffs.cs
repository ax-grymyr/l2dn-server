using System.Text;
using L2Dn.GameServer.Data;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Model.Html.Styles;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

public class AdminBuffs: IAdminCommandHandler
{
	private static readonly string[] ADMIN_COMMANDS =
	{
		"admin_buff",
		"admin_getbuffs",
		"admin_getbuffs_ps",
		"admin_stopbuff",
		"admin_stopallbuffs",
		"admin_viewblockedeffects",
		"admin_areacancel",
		"admin_removereuse",
		"admin_switch_gm_buffs"
	};
	// Misc
	private const string FONT_RED1 = "<font color=\"FF0000\">";
	private const string FONT_RED2 = "</font>";
	
	public bool useAdminCommand(string commandValue, Player activeChar)
	{
		string command = commandValue;
		if (command.startsWith("admin_buff"))
		{
			if ((activeChar.getTarget() == null) || !activeChar.getTarget().isCreature())
			{
				activeChar.sendPacket(SystemMessageId.THAT_IS_AN_INCORRECT_TARGET);
				return false;
			}
			
			StringTokenizer st = new StringTokenizer(command, " ");
			st.nextToken();
			if (!st.hasMoreTokens())
			{
				BuilderUtil.sendSysMessage(activeChar, "Skill Id and level are not specified.");
				BuilderUtil.sendSysMessage(activeChar, "Usage: //buff <skillId> <skillLevel>");
				return false;
			}
			
			try
			{
				int skillId = int.Parse(st.nextToken());
				int skillLevel = st.hasMoreTokens() ? int.Parse(st.nextToken()) : SkillData.getInstance().getMaxLevel(skillId);
				Creature target = (Creature) activeChar.getTarget();
				Skill skill = SkillData.getInstance().getSkill(skillId, skillLevel);
				if (skill == null)
				{
					BuilderUtil.sendSysMessage(activeChar, "Skill with id: " + skillId + ", level: " + skillLevel + " not found.");
					return false;
				}
				
				BuilderUtil.sendSysMessage(activeChar, "Admin buffing " + skill.getName() + " (" + skillId + "," + skillLevel + ")");
				skill.applyEffects(activeChar, target);
				return true;
			}
			catch (Exception e)
			{
				BuilderUtil.sendSysMessage(activeChar, "Failed buffing: " + e);
				BuilderUtil.sendSysMessage(activeChar, "Usage: //buff <skillId> <skillLevel>");
				return false;
			}
		}
		else if (command.startsWith("admin_getbuffs"))
		{
			StringTokenizer st = new StringTokenizer(command, " ");
			command = st.nextToken();
			if (st.hasMoreTokens())
			{
				string playername = st.nextToken();
				Player player = World.getInstance().getPlayer(playername);
				if (player != null)
				{
					int page = 0;
					if (st.hasMoreTokens())
					{
						page = int.Parse(st.nextToken());
					}
					showBuffs(activeChar, player, page, command.endsWith("_ps"));
					return true;
				}
				BuilderUtil.sendSysMessage(activeChar, "The player " + playername + " is not online.");
				return false;
			}
			else if ((activeChar.getTarget() != null) && activeChar.getTarget().isCreature())
			{
				showBuffs(activeChar, (Creature) activeChar.getTarget(), 0, command.endsWith("_ps"));
				return true;
			}
			else
			{
				activeChar.sendPacket(SystemMessageId.THAT_IS_AN_INCORRECT_TARGET);
				return false;
			}
		}
		else if (command.startsWith("admin_stopbuff"))
		{
			try
			{
				StringTokenizer st = new StringTokenizer(command, " ");
				st.nextToken();
				int objectId = int.Parse(st.nextToken());
				int skillId = int.Parse(st.nextToken());
				removeBuff(activeChar, objectId, skillId);
				return true;
			}
			catch (Exception e)
			{
				BuilderUtil.sendSysMessage(activeChar, "Failed removing effect: " + e);
				BuilderUtil.sendSysMessage(activeChar, "Usage: //stopbuff <objectId> <skillId>");
				return false;
			}
		}
		else if (command.startsWith("admin_stopallbuffs"))
		{
			try
			{
				StringTokenizer st = new StringTokenizer(command, " ");
				st.nextToken();
				int objectId = int.Parse(st.nextToken());
				removeAllBuffs(activeChar, objectId);
				return true;
			}
			catch (Exception e)
			{
				BuilderUtil.sendSysMessage(activeChar, "Failed removing all effects: " + e);
				BuilderUtil.sendSysMessage(activeChar, "Usage: //stopallbuffs <objectId>");
				return false;
			}
		}
		else if (command.startsWith("admin_viewblockedeffects"))
		{
			try
			{
				StringTokenizer st = new StringTokenizer(command, " ");
				st.nextToken();
				int objectId = int.Parse(st.nextToken());
				viewBlockedEffects(activeChar, objectId);
				return true;
			}
			catch (Exception e)
			{
				BuilderUtil.sendSysMessage(activeChar, "Failed viewing blocked effects: " + e);
				BuilderUtil.sendSysMessage(activeChar, "Usage: //viewblockedeffects <objectId>");
				return false;
			}
		}
		else if (command.startsWith("admin_areacancel"))
		{
			StringTokenizer st = new StringTokenizer(command, " ");
			st.nextToken();
			string val = st.nextToken();
			try
			{
				int radius = int.Parse(val);
				World.getInstance().forEachVisibleObjectInRange<Player>(activeChar, radius, x => x.stopAllEffects());
				BuilderUtil.sendSysMessage(activeChar, "All effects canceled within radius " + radius);
				return true;
			}
			catch (Exception e)
			{
				BuilderUtil.sendSysMessage(activeChar, "Usage: //areacancel <radius>");
				return false;
			}
		}
		else if (command.startsWith("admin_removereuse"))
		{
			StringTokenizer st = new StringTokenizer(command, " ");
			st.nextToken();
			
			Player player = null;
			if (st.hasMoreTokens())
			{
				string playername = st.nextToken();
				
				try
				{
					player = World.getInstance().getPlayer(playername);
				}
				catch (Exception e)
				{
				}
				
				if (player == null)
				{
					BuilderUtil.sendSysMessage(activeChar, "The player " + playername + " is not online.");
					return false;
				}
			}
			else if ((activeChar.getTarget() != null) && activeChar.getTarget().isPlayer())
			{
				player = activeChar.getTarget().getActingPlayer();
			}
			else
			{
				activeChar.sendPacket(SystemMessageId.THAT_IS_AN_INCORRECT_TARGET);
				return false;
			}
			
			try
			{
				player.resetTimeStamps();
				player.resetDisabledSkills();
				player.sendPacket(new SkillCoolTimePacket(player));
				BuilderUtil.sendSysMessage(activeChar, "Skill reuse was removed from " + player.getName() + ".");
				return true;
			}
			catch (Exception e)
			{
				return false;
			}
		}
		else if (command.startsWith("admin_switch_gm_buffs"))
		{
			if (Config.GM_GIVE_SPECIAL_SKILLS != Config.GM_GIVE_SPECIAL_AURA_SKILLS)
			{
				bool toAuraSkills = activeChar.getKnownSkill(7041) != null;
				switchSkills(activeChar, toAuraSkills);
				activeChar.sendSkillList();
				BuilderUtil.sendSysMessage(activeChar, "You have successfully changed to target " + (toAuraSkills ? "aura" : "one") + " special skills.");
				return true;
			}
			BuilderUtil.sendSysMessage(activeChar, "There is nothing to switch.");
			return false;
		}
		return true;
	}
	
	/**
	 * @param gmchar the player to switch the Game Master skills.
	 * @param toAuraSkills if {@code true} it will remove "GM Aura" skills and add "GM regular" skills, vice versa if {@code false}.
	 */
	private void switchSkills(Player gmchar, bool toAuraSkills)
	{
		ICollection<Skill> skills = toAuraSkills ? SkillTreeData.getInstance().getGMSkillTree() : SkillTreeData.getInstance().getGMAuraSkillTree();
		foreach (Skill skill in skills)
		{
			gmchar.removeSkill(skill, false); // Don't Save GM skills to database
		}
		SkillTreeData.getInstance().addSkills(gmchar, toAuraSkills);
	}
	
	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
	
	private void showBuffs(Player activeChar, Creature target, int page, bool passive)
	{
		List<BuffInfo> effects = new();
		if (!passive)
		{
			effects.AddRange(target.getEffectList().getEffects());
		}
		else
		{
			effects.AddRange(target.getEffectList().getPassives());
		}
		
		string pageLink = "bypass -h admin_getbuffs" + (passive ? "_ps " : " ") + target.getName();
		PageResult result = PageBuilder.newBuilder(effects, 3, pageLink).currentPage(page).
			style(ButtonsStyle.INSTANCE).bodyHandler((pages, info, sb) =>
		{
			foreach (AbstractEffect effect in info.getEffects())
			{
				sb.Append("<tr><td>");
				sb.Append(!info.isInUse() ? FONT_RED1 : "");
				sb.Append(info.getSkill().getName());
				sb.Append(" Lv ");
				sb.Append(info.getSkill().getLevel());
				sb.Append(" (");
				sb.Append(effect.GetType().Name);
				sb.Append(")");
				sb.Append(!info.isInUse() ? FONT_RED2 : "");
				sb.Append("</td><td>");
				sb.Append(info.getSkill().isToggle() ? "T" : info.getSkill().isPassive() ? "P" : info.getTime() + "s");
				sb.Append("</td><td><button value=\"X\" action=\"bypass -h admin_stopbuff ");
				sb.Append(target.getObjectId());
				sb.Append(" ");
				sb.Append(info.getSkill().getId());
				sb.Append("\" width=30 height=21 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td></tr>");
			}
		}).build();

		HtmlContent htmlContent = HtmlContent.LoadFromFile("html/admin/getbuffs.htm", activeChar);
		NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(null, 1, htmlContent);
		if (result.getPages() > 0)
		{
			htmlContent.Replace("%pages%", "<table width=280 cellspacing=0><tr>" + result.getPagerTemplate() + "</tr></table>");
		}
		else
		{
			htmlContent.Replace("%pages%", "");
		}
		
		htmlContent.Replace("%buffsText%", passive ? "Hide Passives" : "Show Passives");
		htmlContent.Replace("%passives%", passive ? "" : "_ps");
		htmlContent.Replace("%targetName%", target.getName());
		htmlContent.Replace("%targetObjId%", target.getObjectId().ToString());
		htmlContent.Replace("%buffs%", result.getBodyTemplate().ToString());
		htmlContent.Replace("%effectSize%", effects.Count.ToString());
		activeChar.sendPacket(html);
		
		if (Config.GMAUDIT)
		{
			// TODO: GMAudit
			//GMAudit.auditGMAction(activeChar.getName() + " [" + activeChar.getObjectId() + "]", "getbuffs", target.getName() + " (" + target.getObjectId() + ")", "");
		}
	}
	
	private void removeBuff(Player activeChar, int objId, int skillId)
	{
		Creature target = null;
		try
		{
			target = (Creature) World.getInstance().findObject(objId);
		}
		catch (Exception e)
		{
			// Checked bellow.
		}
		
		if ((target != null) && (skillId > 0))
		{
			if (target.isAffectedBySkill(skillId))
			{
				target.stopSkillEffects(SkillFinishType.REMOVED, skillId);
				BuilderUtil.sendSysMessage(activeChar, "Removed skill ID: " + skillId + " effects from " + target.getName() + " (" + objId + ").");
			}
			
			showBuffs(activeChar, target, 0, false);
			if (Config.GMAUDIT)
			{
				// TODO: GMAudit
				//GMAudit.auditGMAction(activeChar.getName() + " [" + activeChar.getObjectId() + "]", "stopbuff", target.getName() + " (" + objId + ")", Integer.toString(skillId));
			}
		}
	}
	
	private void removeAllBuffs(Player activeChar, int objId)
	{
		Creature target = null;
		try
		{
			target = (Creature) World.getInstance().findObject(objId);
		}
		catch (Exception e)
		{
			// Checked bellow.
		}
		
		if (target != null)
		{
			target.stopAllEffects();
			BuilderUtil.sendSysMessage(activeChar, "Removed all effects from " + target.getName() + " (" + objId + ")");
			showBuffs(activeChar, target, 0, false);
			if (Config.GMAUDIT)
			{
				// TODO: GMAudit
				//GMAudit.auditGMAction(activeChar.getName() + " [" + activeChar.getObjectId() + "]", "stopallbuffs", target.getName() + " (" + objId + ")", "");
			}
		}
	}
	
	private void viewBlockedEffects(Player activeChar, int objId)
	{
		Creature target = null;
		try
		{
			target = (Creature) World.getInstance().findObject(objId);
		}
		catch (Exception e)
		{
			BuilderUtil.sendSysMessage(activeChar, "Target with object id " + objId + " not found.");
			return;
		}
		
		if (target != null)
		{
			Set<AbnormalType> blockedAbnormals = target.getEffectList().getBlockedAbnormalTypes();
			int blockedAbnormalsSize = blockedAbnormals != null ? blockedAbnormals.size() : 0;
			StringBuilder html = new StringBuilder(500 + (blockedAbnormalsSize * 50));
			html.Append("<html><table width=\"100%\"><tr><td width=45><button value=\"Main\" action=\"bypass admin_admin\" width=45 height=21 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td><td width=180><center><font color=\"LEVEL\">Blocked effects of ");
			html.Append(target.getName());
			html.Append("</font></td><td width=45><button value=\"Back\" action=\"bypass -h admin_getbuffs" + (target.isPlayer() ? (" " + target.getName()) : "") + "\" width=45 height=21 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td></tr></table><br>");
			if ((blockedAbnormals != null) && !blockedAbnormals.isEmpty())
			{
				html.Append("<br>Blocked buff slots: ");
				foreach (AbnormalType slot in blockedAbnormals)
				{
					html.Append("<br>").Append(slot.ToString());
				}
			}
			
			html.Append("</html>");
			
			// Send the packet
			HtmlContent htmlContent = HtmlContent.LoadFromText(html.ToString(), activeChar);
			activeChar.sendPacket(new NpcHtmlMessagePacket(null, 1, htmlContent));
			if (Config.GMAUDIT)
			{
				// TODO: GMAudit
				//GMAudit.auditGMAction(activeChar.getName() + " [" + activeChar.getObjectId() + "]", "viewblockedeffects", target.getName() + " (" + Integer.toString(target.getObjectId()) + ")", "");
			}
		}
	}
}
