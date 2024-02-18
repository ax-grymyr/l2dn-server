using System.Globalization;
using System.Text;
using L2Dn.GameServer.Data;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Actor.Instances;

public class SchemeBuffer : Npc
{
	private const int PAGE_LIMIT = 6;
	
	public SchemeBuffer(NpcTemplate template): base(template)
	{
	}
	
	public override void onBypassFeedback(Player player, String commandValue)
	{
		// Simple hack to use createscheme bypass with a space.
		String command = commandValue.Replace("createscheme ", "createscheme;");
		
		StringTokenizer st = new StringTokenizer(command, ";");
		String currentCommand = st.nextToken();
		if (currentCommand.startsWith("menu"))
		{
			HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data, getHtmlPath(getId(), 0, player));
			helper.Replace("%objectId%", getObjectId().ToString());
			NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(getObjectId(), helper);
			player.sendPacket(html);
		}
		else if (currentCommand.startsWith("cleanup"))
		{
			player.stopAllEffects();
			
			Summon summon = player.getPet();
			if (summon != null)
			{
				summon.stopAllEffects();
			}
			player.getServitors().values().forEach(servitor => servitor.stopAllEffects());
			
			HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data, getHtmlPath(getId(), 0, player));
			helper.Replace("%objectId%", getObjectId().ToString());
			NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(getObjectId(), helper);
			player.sendPacket(html);
		}
		else if (currentCommand.startsWith("heal"))
		{
			player.setCurrentHpMp(player.getMaxHp(), player.getMaxMp());
			player.setCurrentCp(player.getMaxCp());
			
			Summon summon = player.getPet();
			if (summon != null)
			{
				summon.setCurrentHpMp(summon.getMaxHp(), summon.getMaxMp());
			}
			player.getServitors().values().forEach(servitor => servitor.setCurrentHpMp(servitor.getMaxHp(), servitor.getMaxMp()));
			
			HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data, getHtmlPath(getId(), 0, player));
			helper.Replace("%objectId%", getObjectId().ToString());
			NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(getObjectId(), helper);
			player.sendPacket(html);
		}
		else if (currentCommand.startsWith("support"))
		{
			showGiveBuffsWindow(player);
		}
		else if (currentCommand.startsWith("givebuffs"))
		{
			String schemeName = st.nextToken();
			int cost = int.Parse(st.nextToken());
			bool buffSummons = st.hasMoreTokens() && st.nextToken().equalsIgnoreCase("pet");
			if (buffSummons && (player.getPet() == null) && !player.hasServitors())
			{
				player.sendMessage("You don't have a pet.");
			}
			else if ((cost == 0) || ((Config.BUFFER_ITEM_ID == 57) && player.reduceAdena("NPC Buffer", cost, this, true)) || ((Config.BUFFER_ITEM_ID != 57) && player.destroyItemByItemId("NPC Buffer", Config.BUFFER_ITEM_ID, cost, player, true)))
			{
				foreach (int skillId in SchemeBufferTable.getInstance().getScheme(player.getObjectId(), schemeName))
				{
					Skill skill = SkillData.getInstance().getSkill(skillId, SchemeBufferTable.getInstance().getAvailableBuff(skillId).getLevel());
					if (buffSummons)
					{
						if (player.getPet() != null)
						{
							skill.applyEffects(this, player.getPet());
						}
						player.getServitors().values().forEach(servitor => skill.applyEffects(this, servitor));
					}
					else
					{
						skill.applyEffects(this, player);
					}
				}
			}
		}
		else if (currentCommand.startsWith("editschemes"))
		{
			showEditSchemeWindow(player, st.nextToken(), st.nextToken(), int.Parse(st.nextToken()));
		}
		else if (currentCommand.startsWith("skill"))
		{
			String groupType = st.nextToken();
			String schemeName = st.nextToken();
			int skillId = int.Parse(st.nextToken());
			int page = int.Parse(st.nextToken());
			List<int> skills = SchemeBufferTable.getInstance().getScheme(player.getObjectId(), schemeName);
			if (currentCommand.startsWith("skillselect") && !schemeName.equalsIgnoreCase("none"))
			{
				Skill skill = SkillData.getInstance().getSkill(skillId, SkillData.getInstance().getMaxLevel(skillId));
				if (skill.isDance())
				{
					if (getCountOf(skills, true) < Config.DANCES_MAX_AMOUNT)
					{
						skills.Add(skillId);
					}
					else
					{
						player.sendMessage("This scheme has reached the maximum amount of dances/songs.");
					}
				}
				else
				{
					if (getCountOf(skills, false) < player.getStat().getMaxBuffCount())
					{
						skills.Add(skillId);
					}
					else
					{
						player.sendMessage("This scheme has reached the maximum amount of buffs.");
					}
				}
			}
			else if (currentCommand.startsWith("skillunselect"))
			{
				skills.Remove(skillId);
			}
			
			showEditSchemeWindow(player, groupType, schemeName, page);
		}
		else if (currentCommand.startsWith("createscheme"))
		{
			try
			{
				String schemeName = st.nextToken().Trim();
				if (schemeName.Length > 14)
				{
					player.sendMessage("Scheme's name must contain up to 14 chars.");
					return;
				}
				// Simple hack to use spaces, dots, commas, minus, plus, exclamations or question marks.
				if (!Util.isAlphaNumeric(schemeName.Replace(" ", "").Replace(".", "").Replace(",", "").Replace("-", "")
					    .Replace("+", "").Replace("!", "").Replace("?", "")))
				{
					player.sendMessage("Please use plain alphanumeric characters.");
					return;
				}

				Map<String, List<int>> schemes = SchemeBufferTable.getInstance().getPlayerSchemes(player.getObjectId());
				if (schemes != null)
				{
					if (schemes.Count == Config.BUFFER_MAX_SCHEMES)
					{
						player.sendMessage("Maximum schemes amount is already reached.");
						return;
					}
					
					if (schemes.containsKey(schemeName))
					{
						player.sendMessage("The scheme name already exists.");
						return;
					}
				}
				
				SchemeBufferTable.getInstance().setScheme(player.getObjectId(), schemeName.Trim(), new List<int>());
				showGiveBuffsWindow(player);
			}
			catch (Exception e)
			{
				player.sendMessage("Scheme's name must contain up to 14 chars.");
			}
		}
		else if (currentCommand.startsWith("deletescheme"))
		{
			try
			{
				String schemeName = st.nextToken();
				Map<String, List<int>> schemes = SchemeBufferTable.getInstance().getPlayerSchemes(player.getObjectId());
				if ((schemes != null) && schemes.containsKey(schemeName))
				{
					schemes.remove(schemeName);
				}
			}
			catch (Exception e)
			{
				player.sendMessage("This scheme name is invalid.");
			}
			showGiveBuffsWindow(player);
		}
	}
	
	public override String getHtmlPath(int npcId, int value, Player player)
	{
		String filename = "";
		if (value == 0)
		{
			filename = npcId.ToString(CultureInfo.InvariantCulture);
		}
		else
		{
			filename = npcId + "-" + value;
		}
		return "data/html/mods/SchemeBuffer/" + filename + ".htm";
	}
	
	/**
	 * Sends an html packet to player with Give Buffs menu info for player and pet, depending on targetType parameter {player, pet}
	 * @param player : The player to make checks on.
	 */
	private void showGiveBuffsWindow(Player player)
	{
		StringBuilder sb = new StringBuilder(200);
		Map<String, List<int>> schemes = SchemeBufferTable.getInstance().getPlayerSchemes(player.getObjectId());
		if ((schemes == null) || schemes.isEmpty())
		{
			sb.Append("<font color=\"LEVEL\">You haven't defined any scheme.</font>");
		}
		else
		{
			foreach (var scheme in schemes)
			{
				int cost = getFee(scheme.Value);
				sb.Append("<font color=\"LEVEL\">" + scheme.Key + " [" + scheme.Value.size() + " skill(s)]" +
				          ((cost > 0) ? " - cost: " + cost : "") + "</font><br1>");
				sb.Append("<a action=\"bypass -h npc_%objectId%_givebuffs;" + scheme.Key + ";" + cost +
				          "\">Use on Me</a>&nbsp;|&nbsp;");
				sb.Append("<a action=\"bypass -h npc_%objectId%_givebuffs;" + scheme.Key + ";" + cost +
				          ";pet\">Use on Pet</a>&nbsp;|&nbsp;");
				sb.Append("<a action=\"bypass -h npc_%objectId%_editschemes;Buffs;" + scheme.Key +
				          ";1\">Edit</a>&nbsp;|&nbsp;");
				sb.Append("<a action=\"bypass -h npc_%objectId%_deletescheme;" + scheme.Key + "\">Delete</a><br>");
			}
		}
		
		HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data, getHtmlPath(getId(), 1, player));
		helper.Replace("%schemes%", sb.ToString());
		helper.Replace("%max_schemes%", Config.BUFFER_MAX_SCHEMES.ToString());
		helper.Replace("%objectId%", getObjectId().ToString());
		NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(getObjectId(), helper);
		player.sendPacket(html);
	}
	
	/**
	 * This sends an html packet to player with Edit Scheme Menu info. This allows player to edit each created scheme (add/delete skills)
	 * @param player : The player to make checks on.
	 * @param groupType : The group of skills to select.
	 * @param schemeName : The scheme to make check.
	 * @param page The page.
	 */
	private void showEditSchemeWindow(Player player, String groupType, String schemeName, int page)
	{
		List<int> schemeSkills = SchemeBufferTable.getInstance().getScheme(player.getObjectId(), schemeName);

		HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data, getHtmlPath(getId(), 2, player));
		helper.Replace("%schemename%", schemeName);
		helper.Replace("%count%",
			getCountOf(schemeSkills, false) + " / " + player.getStat().getMaxBuffCount() + " buffs, " +
			getCountOf(schemeSkills, true) + " / " + Config.DANCES_MAX_AMOUNT + " dances/songs");
		
		helper.Replace("%typesframe%", getTypesFrame(groupType, schemeName));
		helper.Replace("%skilllistframe%", getGroupSkillList(player, groupType, schemeName, page));
		helper.Replace("%objectId%", getObjectId().ToString());
		NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(getObjectId(), helper);
		player.sendPacket(html);
	}

	/**
	 * @param player : The player to make checks on.
	 * @param groupType : The group of skills to select.
	 * @param schemeName : The scheme to make check.
	 * @param pageValue The page.
	 * @return a String representing skills available to selection for a given groupType.
	 */
	private String getGroupSkillList(Player player, String groupType, String schemeName, int pageValue)
	{
		// Retrieve the entire skills list based on group type.
		List<int> skills = SchemeBufferTable.getInstance().getSkillsIdsByType(groupType);
		if (skills.Count == 0)
		{
			return "That group doesn't contain any skills.";
		}

		// Calculate page number.
		int max = MathUtil.countPagesNumber(skills.Count, PAGE_LIMIT);
		int page = pageValue;
		if (page > max)
		{
			page = max;
		}

		// Cut skills list up to page number.
		skills = skills.GetRange((page - 1) * PAGE_LIMIT, Math.Min(page * PAGE_LIMIT, skills.Count));

		List<int> schemeSkills = SchemeBufferTable.getInstance().getScheme(player.getObjectId(), schemeName);
		StringBuilder sb = new StringBuilder(skills.Count * 150);
		int row = 0;
		foreach (int skillId in skills)
		{
			sb.Append(((row % 2) == 0 ? "<table width=\"280\" bgcolor=\"000000\"><tr>" : "<table width=\"280\"><tr>"));

			Skill skill = SkillData.getInstance().getSkill(skillId, 1);
			if (schemeSkills.Contains(skillId))
			{
				sb.Append("<td height=40 width=40><img src=\"" + skill.getIcon() +
				          "\" width=32 height=32></td><td width=190>" + skill.getName() +
				          "<br1><font color=\"B09878\">" +
				          SchemeBufferTable.getInstance().getAvailableBuff(skillId).getDescription() +
				          "</font></td><td><button value=\" \" action=\"bypass -h npc_%objectId%_skillunselect;" +
				          groupType + ";" + schemeName + ";" + skillId + ";" + page +
				          "\" width=32 height=32 back=\"L2UI_CH3.mapbutton_zoomout2\" fore=\"L2UI_CH3.mapbutton_zoomout1\"></td>");
			}
			else
			{
				sb.Append("<td height=40 width=40><img src=\"" + skill.getIcon() +
				          "\" width=32 height=32></td><td width=190>" + skill.getName() +
				          "<br1><font color=\"B09878\">" +
				          SchemeBufferTable.getInstance().getAvailableBuff(skillId).getDescription() +
				          "</font></td><td><button value=\" \" action=\"bypass -h npc_%objectId%_skillselect;" +
				          groupType + ";" + schemeName + ";" + skillId + ";" + page +
				          "\" width=32 height=32 back=\"L2UI_CH3.mapbutton_zoomin2\" fore=\"L2UI_CH3.mapbutton_zoomin1\"></td>");
			}

			sb.Append("</tr></table><img src=\"L2UI.SquareGray\" width=277 height=1>");
			row++;
		}

		// Build page footer.
		sb.Append("<br><img src=\"L2UI.SquareGray\" width=277 height=1><table width=\"100%\" bgcolor=000000><tr>");
		if (page > 1)
		{
			sb.Append("<td align=left width=70><a action=\"bypass -h npc_" + getObjectId() + "_editschemes;" +
			          groupType + ";" + schemeName + ";" + (page - 1) + "\">Previous</a></td>");
		}
		else
		{
			sb.Append("<td align=left width=70>Previous</td>");
		}

		sb.Append("<td align=center width=100>Page " + page + "</td>");
		if (page < max)
		{
			sb.Append("<td align=right width=70><a action=\"bypass -h npc_" + getObjectId() + "_editschemes;" +
			          groupType + ";" + schemeName + ";" + (page + 1) + "\">Next</a></td>");
		}
		else
		{
			sb.Append("<td align=right width=70>Next</td>");
		}

		sb.Append("</tr></table><img src=\"L2UI.SquareGray\" width=277 height=1>");
		return sb.ToString();
	}

	/**
	 * @param groupType : The group of skills to select.
	 * @param schemeName : The scheme to make check.
	 * @return a string representing all groupTypes available. The group currently on selection isn't linkable.
	 */
	private static String getTypesFrame(String groupType, String schemeName)
	{
		StringBuilder sb = new StringBuilder(500);
		sb.Append("<table>");
		
		int count = 0;
		foreach (String type in SchemeBufferTable.getInstance().getSkillTypes())
		{
			if (count == 0)
			{
				sb.Append("<tr>");
			}
			
			if (groupType.equalsIgnoreCase(type))
			{
				sb.Append("<td width=65>" + type + "</td>");
			}
			else
			{
				sb.Append("<td width=65><a action=\"bypass -h npc_%objectId%_editschemes;" + type + ";" + schemeName + ";1\">" + type + "</a></td>");
			}
			
			count++;
			if (count == 4)
			{
				sb.Append("</tr>");
				count = 0;
			}
		}
		
		if (!sb.ToString().EndsWith("</tr>"))
		{
			sb.Append("</tr>");
		}
		
		sb.Append("</table>");
		
		return sb.ToString();
	}
	
	/**
	 * @param list : A list of skill ids.
	 * @return a global fee for all skills contained in list.
	 */
	private static int getFee(List<int> list)
	{
		if (Config.BUFFER_STATIC_BUFF_COST > 0)
		{
			return list.Count * Config.BUFFER_STATIC_BUFF_COST;
		}
		
		int fee = 0;
		foreach (int sk in list)
		{
			fee += SchemeBufferTable.getInstance().getAvailableBuff(sk).getPrice();
		}
		
		return fee;
	}
	
	private static int getCountOf(List<int> skills, bool dances)
	{
		int count = 0;
		foreach (int skillId in skills)
		{
			if (SkillData.getInstance().getSkill(skillId, 1).isDance() == dances)
			{
				count++;
			}
		}
		return count;
	}
}