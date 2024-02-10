using System.Text;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Olympiads;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * Sell Buffs Manager
 * @author St3eT
 */
public class SellBuffsManager: IXmlReader
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(SellBuffsManager));
	private static readonly Set<int> ALLOWED_BUFFS = new();
	private const string HTML_FOLDER = "data/html/mods/SellBuffs/";
	
	protected SellBuffsManager()
	{
		load();
	}
	
	public void load()
	{
		if (Config.SELLBUFF_ENABLED)
		{
			ALLOWED_BUFFS.clear();
			parseDatapackFile("data/SellBuffData.xml");
			LOGGER.Info(GetType().Name +": Loaded " + ALLOWED_BUFFS.Count + " allowed buffs.");
		}
	}
	
	public void parseDocument(Document doc, File f)
	{
		NodeList node = doc.getDocumentElement().getElementsByTagName("skill");
		for (int i = 0; i < node.getLength(); ++i)
		{
			Element elem = (Element) node.item(i);
			int skillId = int.Parse(elem.getAttribute("id"));
			ALLOWED_BUFFS.add(skillId);
		}
	}
	
	public void sendSellMenu(Player player)
	{
		String html = HtmCache.getInstance().getHtm(player, HTML_FOLDER + (player.isSellingBuffs() ? "BuffMenu_already.html" : "BuffMenu.html"));
		CommunityBoardHandler.separateAndSend(html, player);
	}
	
	public void sendBuffChoiceMenu(Player player, int index)
	{
		String html = HtmCache.getInstance().getHtm(player, HTML_FOLDER + "BuffChoice.html");
		html = html.replace("%list%", buildSkillMenu(player, index));
		CommunityBoardHandler.separateAndSend(html, player);
	}
	
	public void sendBuffEditMenu(Player player)
	{
		String html = HtmCache.getInstance().getHtm(player, HTML_FOLDER + "BuffChoice.html");
		html = html.replace("%list%", buildEditMenu(player));
		CommunityBoardHandler.separateAndSend(html, player);
	}
	
	public void sendBuffMenu(Player player, Player seller, int index)
	{
		if (!seller.isSellingBuffs() || seller.getSellingBuffs().isEmpty())
		{
			return;
		}
		
		String html = HtmCache.getInstance().getHtm(player, HTML_FOLDER + "BuffBuyMenu.html");
		html = html.replace("%list%", buildBuffMenu(seller, index));
		CommunityBoardHandler.separateAndSend(html, player);
	}
	
	public void startSellBuffs(Player player, String title)
	{
		player.sitDown();
		player.setSellingBuffs(true);
		player.setPrivateStoreType(PrivateStoreType.PACKAGE_SELL);
		player.getSellList().setTitle(title);
		player.getSellList().setPackaged(true);
		player.broadcastUserInfo();
		player.broadcastPacket(new ExPrivateStoreSetWholeMsg(player));
		sendSellMenu(player);
	}
	
	public void stopSellBuffs(Player player)
	{
		player.setSellingBuffs(false);
		player.setPrivateStoreType(PrivateStoreType.NONE);
		player.standUp();
		player.broadcastUserInfo();
		sendSellMenu(player);
	}
	
	private String buildBuffMenu(Player seller, int index)
	{
		int ceiling = 10;
		int nextIndex = -1;
		int previousIndex = -1;
		int emptyFields = 0;
		StringBuilder sb = new StringBuilder();
		List<SellBuffHolder> sellList = new();
		
		int count = 0;
		foreach (SellBuffHolder holder in seller.getSellingBuffs())
		{
			count++;
			if ((count > index) && (count <= (ceiling + index)))
			{
				sellList.add(holder);
			}
		}
		
		if ((count > 10) && (count > (index + 10)))
		{
			nextIndex = index + 10;
		}
		
		if (index >= 10)
		{
			previousIndex = index - 10;
		}
		
		emptyFields = ceiling - sellList.size();
		
		sb.Append("<br>");
		sb.Append(HtmlUtil.getMpGauge(250, (long) seller.getCurrentMp(), seller.getMaxMp(), false));
		sb.Append("<br>");
		
		sb.Append("<table border=0 cellpadding=0 cellspacing=0 background=\"L2UI_CH3.refinewnd_back_Pattern\">");
		sb.Append("<tr><td><br><br><br></td></tr>");
		sb.Append("<tr>");
		sb.Append("<td fixwidth=\"10\"></td>");
		sb.Append("<td> <button action=\"\" value=\"Icon\" width=75 height=23 back=\"L2UI_CT1.OlympiadWnd_DF_Watch_Down\" fore=\"L2UI_CT1.OlympiadWnd_DF_Watch\"> </td>"); // Icon
		sb.Append("<td> <button action=\"\" value=\"Name\" width=175 height=23 back=\"L2UI_CT1.OlympiadWnd_DF_Watch_Down\" fore=\"L2UI_CT1.OlympiadWnd_DF_Watch\"> </td>"); // Name
		sb.Append("<td> <button action=\"\" value=\"Level\" width=85 height=23 back=\"L2UI_CT1.OlympiadWnd_DF_Watch_Down\" fore=\"L2UI_CT1.OlympiadWnd_DF_Watch\"> </td>"); // Leve
		sb.Append("<td> <button action=\"\" value=\"MP Cost\" width=100 height=23 back=\"L2UI_CT1.OlympiadWnd_DF_Watch_Down\" fore=\"L2UI_CT1.OlympiadWnd_DF_Watch\"> </td>"); // Price
		sb.Append("<td> <button action=\"\" value=\"Price\" width=200 height=23 back=\"L2UI_CT1.OlympiadWnd_DF_Watch_Down\" fore=\"L2UI_CT1.OlympiadWnd_DF_Watch\"> </td>"); // Price
		sb.Append("<td> <button action=\"\" value=\"Action\" width=100 height=23 back=\"L2UI_CT1.OlympiadWnd_DF_Watch_Down\" fore=\"L2UI_CT1.OlympiadWnd_DF_Watch\"> </td>"); // Action
		sb.Append("<td fixwidth=\"20\"></td>");
		sb.Append("</tr>");
		
		foreach (SellBuffHolder holder in sellList)
		{
			Skill skill = seller.getKnownSkill(holder.getSkillId());
			if (skill == null)
			{
				emptyFields++;
				continue;
			}
			
			ItemTemplate item = ItemData.getInstance().getTemplate(Config.SELLBUFF_PAYMENT_ID);
			
			sb.Append("<tr>");
			sb.Append("<td fixwidth=\"20\"></td>");
			sb.Append("<td align=center><img src=\"" + skill.getIcon() + "\" width=\"32\" height=\"32\"></td>");
			sb.Append("<td align=left>" + skill.getName() + (skill.getLevel() > 100 ? "<font color=\"LEVEL\"> + " + (skill.getLevel() % 100) + "</font></td>" : "</td>"));
			sb.Append("<td align=center>" + ((skill.getLevel() > 100) ? SkillData.getInstance().getMaxLevel(skill.getId()) : skill.getLevel()) + "</td>");
			sb.Append("<td align=center> <font color=\"1E90FF\">" + (skill.getMpConsume() * Config.SELLBUFF_MP_MULTIPLER) + "</font></td>");
			sb.Append("<td align=center> " + Util.formatAdena(holder.getPrice()) + " <font color=\"LEVEL\"> " + (item != null ? item.getName() : "") + "</font> </td>");
			sb.Append("<td align=center fixwidth=\"50\"><button value=\"Buy Buff\" action=\"bypass -h sellbuffbuyskill " + seller.getObjectId() + " " + skill.getId() + " " + index + "\" width=\"85\" height=\"26\" back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td>");
			sb.Append("</tr>");
			sb.Append("<tr><td><br><br></td></tr>");
		}
		
		for (int i = 0; i < emptyFields; i++)
		{
			sb.Append("<tr>");
			sb.Append("<td fixwidth=\"20\" height=\"32\"></td>");
			sb.Append("<td align=center></td>");
			sb.Append("<td align=left></td>");
			sb.Append("<td align=center></td>");
			sb.Append("<td align=center></font></td>");
			sb.Append("<td align=center></td>");
			sb.Append("<td align=center fixwidth=\"50\"></td>");
			sb.Append("</tr>");
			sb.Append("<tr><td><br><br></td></tr>");
		}
		
		sb.Append("</table>");
		
		sb.Append("<table width=\"250\" border=\"0\">");
		sb.Append("<tr>");
		
		if (previousIndex > -1)
		{
			sb.Append("<td align=left><button value=\"Previous Page\" action=\"bypass -h sellbuffbuymenu " + seller.getObjectId() + " " + previousIndex + "\" width=\"100\" height=\"30\" back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td>");
		}
		
		if (nextIndex > -1)
		{
			sb.Append("<td align=right><button value=\"Next Page\" action=\"bypass -h sellbuffbuymenu " + seller.getObjectId() + " " + nextIndex + "\" width=\"100\" height=\"30\" back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td>");
		}
		sb.Append("</tr>");
		sb.Append("</table>");
		return sb.ToString();
	}
	
	private String buildEditMenu(Player player)
	{
		StringBuilder sb = new StringBuilder();
		
		sb.Append("<table border=0 cellpadding=0 cellspacing=0 background=\"L2UI_CH3.refinewnd_back_Pattern\">");
		sb.Append("<tr><td><br><br><br></td></tr>");
		sb.Append("<tr>");
		sb.Append("<td fixwidth=\"10\"></td>");
		sb.Append("<td> <button action=\"\" value=\"Icon\" width=75 height=23 back=\"L2UI_CT1.OlympiadWnd_DF_Watch_Down\" fore=\"L2UI_CT1.OlympiadWnd_DF_Watch\"> </td>"); // Icon
		sb.Append("<td> <button action=\"\" value=\"Name\" width=150 height=23 back=\"L2UI_CT1.OlympiadWnd_DF_Watch_Down\" fore=\"L2UI_CT1.OlympiadWnd_DF_Watch\"> </td>"); // Name
		sb.Append("<td> <button action=\"\" value=\"Level\" width=75 height=23 back=\"L2UI_CT1.OlympiadWnd_DF_Watch_Down\" fore=\"L2UI_CT1.OlympiadWnd_DF_Watch\"> </td>"); // Level
		sb.Append("<td> <button action=\"\" value=\"Old Price\" width=100 height=23 back=\"L2UI_CT1.OlympiadWnd_DF_Watch_Down\" fore=\"L2UI_CT1.OlympiadWnd_DF_Watch\"> </td>"); // Old price
		sb.Append("<td> <button action=\"\" value=\"New Price\" width=125 height=23 back=\"L2UI_CT1.OlympiadWnd_DF_Watch_Down\" fore=\"L2UI_CT1.OlympiadWnd_DF_Watch\"> </td>"); // New price
		sb.Append("<td> <button action=\"\" value=\"Action\" width=125 height=23 back=\"L2UI_CT1.OlympiadWnd_DF_Watch_Down\" fore=\"L2UI_CT1.OlympiadWnd_DF_Watch\"> </td>"); // Change Price
		sb.Append("<td> <button action=\"\" value=\"Remove\" width=85 height=23 back=\"L2UI_CT1.OlympiadWnd_DF_Watch_Down\" fore=\"L2UI_CT1.OlympiadWnd_DF_Watch\"> </td>"); // Remove Buff
		sb.Append("<td fixwidth=\"20\"></td>");
		sb.Append("</tr>");
		
		if (player.getSellingBuffs().isEmpty())
		{
			sb.Append("</table>");
			sb.Append("<br><br><br>");
			sb.Append("You don't have added any buffs yet!");
		}
		else
		{
			foreach (SellBuffHolder holder in player.getSellingBuffs())
			{
				Skill skill = player.getKnownSkill(holder.getSkillId());
				if (skill == null)
				{
					continue;
				}
				
				sb.Append("<tr>");
				sb.Append("<td fixwidth=\"20\"></td>");
				sb.Append("<td align=center><img src=\"" + skill.getIcon() + "\" width=\"32\" height=\"32\"></td>"); // Icon
				sb.Append("<td align=left>" + skill.getName() + (skill.getLevel() > 100 ? "<font color=\"LEVEL\"> + " + (skill.getLevel() % 100) + "</font></td>" : "</td>")); // Name + enchant
				sb.Append("<td align=center>" + ((skill.getLevel() > 100) ? SkillData.getInstance().getMaxLevel(skill.getId()) : skill.getLevel()) + "</td>"); // Level
				sb.Append("<td align=center> " + Util.formatAdena(holder.getPrice()) + " </td>"); // Price show
				sb.Append("<td align=center><edit var=\"price_" + skill.getId() + "\" width=120 type=\"number\"></td>"); // Price edit
				sb.Append("<td align=center><button value=\"Edit\" action=\"bypass -h sellbuffchangeprice " + skill.getId() + " $price_" + skill.getId() + "\" width=\"85\" height=\"26\" back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td>");
				sb.Append("<td align=center><button value=\" X \" action=\"bypass -h sellbuffremove " + skill.getId() + "\" width=\"26\" height=\"26\" back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td>");
				sb.Append("</tr>");
				sb.Append("<tr><td><br><br></td></tr>");
			}
			sb.Append("</table>");
		}
		
		return sb.ToString();
	}
	
	private String buildSkillMenu(Player player, int index)
	{
		int ceiling = index + 10;
		int nextIndex = -1;
		int previousIndex = -1;
		StringBuilder sb = new StringBuilder();
		List<Skill> skillList = new();
		
		int count = 0;
		foreach (Skill skill in player.getAllSkills())
		{
			if (ALLOWED_BUFFS.Contains(skill.getId()) && !isInSellList(player, skill))
			{
				count++;
				
				if ((count > index) && (count <= ceiling))
				{
					skillList.add(skill);
				}
			}
		}
		
		if ((count > 10) && (count > (index + 10)))
		{
			nextIndex = index + 10;
		}
		
		if (index >= 10)
		{
			previousIndex = index - 10;
		}
		
		sb.Append("<table border=0 cellpadding=0 cellspacing=0 background=\"L2UI_CH3.refinewnd_back_Pattern\">");
		sb.Append("<tr><td><br><br><br></td></tr>");
		sb.Append("<tr>");
		sb.Append("<td fixwidth=\"10\"></td>");
		sb.Append("<td> <button action=\"\" value=\"Icon\" width=100 height=23 back=\"L2UI_CT1.OlympiadWnd_DF_Watch_Down\" fore=\"L2UI_CT1.OlympiadWnd_DF_Watch\"> </td>"); // Icon
		sb.Append("<td> <button action=\"\" value=\"Name\" width=175 height=23 back=\"L2UI_CT1.OlympiadWnd_DF_Watch_Down\" fore=\"L2UI_CT1.OlympiadWnd_DF_Watch\"> </td>"); // Name
		sb.Append("<td> <button action=\"\" value=\"Level\" width=150 height=23 back=\"L2UI_CT1.OlympiadWnd_DF_Watch_Down\" fore=\"L2UI_CT1.OlympiadWnd_DF_Watch\"> </td>"); // Leve
		sb.Append("<td> <button action=\"\" value=\"Price\" width=150 height=23 back=\"L2UI_CT1.OlympiadWnd_DF_Watch_Down\" fore=\"L2UI_CT1.OlympiadWnd_DF_Watch\"> </td>"); // Price
		sb.Append("<td> <button action=\"\" value=\"Action\" width=125 height=23 back=\"L2UI_CT1.OlympiadWnd_DF_Watch_Down\" fore=\"L2UI_CT1.OlympiadWnd_DF_Watch\"> </td>"); // Action
		sb.Append("<td fixwidth=\"20\"></td>");
		sb.Append("</tr>");
		
		if (skillList.isEmpty())
		{
			sb.Append("</table>");
			sb.Append("<br><br><br>");
			sb.Append("At this moment you cant add any buffs!");
		}
		else
		{
			foreach (Skill skill in skillList)
			{
				sb.Append("<tr>");
				sb.Append("<td fixwidth=\"20\"></td>");
				sb.Append("<td align=center><img src=\"" + skill.getIcon() + "\" width=\"32\" height=\"32\"></td>");
				sb.Append("<td align=left>" + skill.getName() + (skill.getLevel() > 100 ? "<font color=\"LEVEL\"> + " + (skill.getLevel() % 100) + "</font></td>" : "</td>"));
				sb.Append("<td align=center>" + ((skill.getLevel() > 100) ? SkillData.getInstance().getMaxLevel(skill.getId()) : skill.getLevel()) + "</td>");
				sb.Append("<td align=center><edit var=\"price_" + skill.getId() + "\" width=120 type=\"number\"></td>");
				sb.Append("<td align=center fixwidth=\"50\"><button value=\"Add Buff\" action=\"bypass -h sellbuffaddskill " + skill.getId() + " $price_" + skill.getId() + "\" width=\"85\" height=\"26\" back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td>");
				sb.Append("</tr>");
				sb.Append("<tr><td><br><br></td></tr>");
			}
			sb.Append("</table>");
		}
		
		sb.Append("<table width=\"250\" border=\"0\">");
		sb.Append("<tr>");
		
		if (previousIndex > -1)
		{
			sb.Append("<td align=left><button value=\"Previous Page\" action=\"bypass -h sellbuffadd " + previousIndex + "\" width=\"100\" height=\"30\" back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td>");
		}
		
		if (nextIndex > -1)
		{
			sb.Append("<td align=right><button value=\"Next Page\" action=\"bypass -h sellbuffadd " + nextIndex + "\" width=\"100\" height=\"30\" back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td>");
		}
		sb.Append("</tr>");
		sb.Append("</table>");
		return sb.ToString();
	}
	
	public bool isInSellList(Player player, Skill skill)
	{
		foreach (SellBuffHolder holder in player.getSellingBuffs())
		{
			if (holder.getSkillId() == skill.getId())
			{
				return true;
			}
		}
		return false;
	}
	
	public bool canStartSellBuffs(Player player)
	{
		if (player.isAlikeDead())
		{
			player.sendMessage("You can't sell buffs in fake death!");
			return false;
		}
		else if (player.isInOlympiadMode() || OlympiadManager.getInstance().isRegistered(player))
		{
			player.sendMessage("You can't sell buffs with Olympiad status!");
			return false;
		}
		else if (player.isRegisteredOnEvent())
		{
			player.sendMessage("You can't sell buffs while registered in an event!");
			return false;
		}
		else if (player.isCursedWeaponEquipped() || (player.getReputation() < 0))
		{
			player.sendMessage("You can't sell buffs in Chaotic state!");
			return false;
		}
		else if (player.isInDuel())
		{
			player.sendMessage("You can't sell buffs in Duel state!");
			return false;
		}
		else if (player.isFishing())
		{
			player.sendMessage("You can't sell buffs while fishing.");
			return false;
		}
		else if (player.isMounted() || player.isFlyingMounted() || player.isFlying())
		{
			player.sendMessage("You can't sell buffs in Mount state!");
			return false;
		}
		else if (player.isTransformed())
		{
			player.sendMessage("You can't sell buffs in Transform state!");
			return false;
		}
		else if (player.isInsideZone(ZoneId.NO_STORE) || !player.isInsideZone(ZoneId.PEACE) || player.isJailed())
		{
			player.sendMessage("You can't sell buffs here!");
			return false;
		}
		return true;
	}
	
	/**
	 * Gets the single instance of {@code SellBuffsManager}.
	 * @return single instance of {@code SellBuffsManager}
	 */
	public static SellBuffsManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly SellBuffsManager INSTANCE = new SellBuffsManager();
	}
}