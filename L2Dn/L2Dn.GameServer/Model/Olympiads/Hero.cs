using System.Runtime.CompilerServices;
using System.Text;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Creatures.Players;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Model.Olympiads;

/**
 * Hero entity.
 * @author godson
 */
public class Hero
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(Hero));
	
	private const String GET_HEROES = "SELECT heroes.charId, characters.char_name, heroes.class_id, heroes.count, heroes.legend_count, heroes.played, heroes.claimed FROM heroes, characters WHERE characters.charId = heroes.charId AND heroes.played = 1";
	private const String GET_ALL_HEROES = "SELECT heroes.charId, characters.char_name, heroes.class_id, heroes.count, heroes.legend_count, heroes.played, heroes.claimed FROM heroes, characters WHERE characters.charId = heroes.charId";
	private const String UPDATE_ALL = "UPDATE heroes SET played = 0";
	private const String INSERT_HERO = "INSERT INTO heroes (charId, class_id, count, legend_count, played, claimed) VALUES (?,?,?,?,?,?)";
	private const String UPDATE_HERO = "UPDATE heroes SET class_id = ?, count = ?, legend_count = ?, played = ?, claimed = ? WHERE charId = ?";
	private const String GET_CLAN_ALLY = "SELECT characters.clanid AS clanid, coalesce(clan_data.ally_Id, 0) AS allyId FROM characters LEFT JOIN clan_data ON clan_data.clan_id = characters.clanid WHERE characters.charId = ?";
	private const String DELETE_ITEMS = "DELETE FROM items WHERE item_id IN (30392, 30393, 30394, 30395, 30396, 30397, 30398, 30399, 30400, 30401, 30402, 30403, 30404, 30405, 30372, 30373, 6842, 6611, 6612, 6613, 6614, 6615, 6616, 6617, 6618, 6619, 6620, 6621, 9388, 9389, 9390) AND owner_id NOT IN (SELECT charId FROM characters WHERE accesslevel > 0)";
	
	private static readonly Map<int, StatSet> HEROES = new();
	private static readonly Map<int, StatSet> COMPLETE_HEROS = new();
	private static readonly Map<int, StatSet> HERO_COUNTS = new();
	private static readonly Map<int, List<StatSet>> HERO_FIGHTS = new();
	private static readonly Map<int, List<StatSet>> HERO_DIARY = new();
	private static readonly Map<int, String> HERO_MESSAGE = new();
	
	public const String COUNT = "count";
	public const String LEGEND_COUNT = "legend_count";
	public const String PLAYED = "played";
	public const String CLAIMED = "claimed";
	public const String CLAN_NAME = "clan_name";
	public const String CLAN_CREST = "clan_crest";
	public const String ALLY_NAME = "ally_name";
	public const String ALLY_CREST = "ally_crest";
	
	public const int ACTION_RAID_KILLED = 1;
	public const int ACTION_HERO_GAINED = 2;
	public const int ACTION_CASTLE_TAKEN = 3;
	
	protected Hero()
	{
		if (Config.OLYMPIAD_ENABLED)
		{
			init();
		}
	}
	
	private void init()
	{
		HEROES.clear();
		COMPLETE_HEROS.clear();
		HERO_COUNTS.clear();
		HERO_FIGHTS.clear();
		HERO_DIARY.clear();
		HERO_MESSAGE.clear();
		
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			Statement s1 = con.createStatement();
			ResultSet rset = s1.executeQuery(GET_HEROES);
			PreparedStatement ps = con.prepareStatement(GET_CLAN_ALLY);
			Statement s2 = con.createStatement();
			ResultSet rset2 = s2.executeQuery(GET_ALL_HEROES);
			while (rset.next())
			{
				StatSet hero = new StatSet();
				int charId = rset.getInt(Olympiad.CHAR_ID);
				hero.set(Olympiad.CHAR_NAME, rset.getString(Olympiad.CHAR_NAME));
				hero.set(Olympiad.CLASS_ID, rset.getInt(Olympiad.CLASS_ID));
				hero.set(COUNT, rset.getInt(COUNT));
				hero.set(LEGEND_COUNT, rset.getInt(LEGEND_COUNT));
				hero.set(PLAYED, rset.getInt(PLAYED));
				hero.set(CLAIMED, Boolean.parseBoolean(rset.getString(CLAIMED)));
				loadFights(charId);
				loadDiary(charId);
				loadMessage(charId);
				processHeros(ps, charId, hero);
				HEROES.put(charId, hero);
			}
			
			while (rset2.next())
			{
				StatSet hero = new StatSet();
				int charId = rset2.getInt(Olympiad.CHAR_ID);
				hero.set(Olympiad.CHAR_NAME, rset2.getString(Olympiad.CHAR_NAME));
				hero.set(Olympiad.CLASS_ID, rset2.getInt(Olympiad.CLASS_ID));
				hero.set(COUNT, rset2.getInt(COUNT));
				hero.set(LEGEND_COUNT, rset2.getInt(LEGEND_COUNT));
				hero.set(PLAYED, rset2.getInt(PLAYED));
				hero.set(CLAIMED, Boolean.parseBoolean(rset2.getString(CLAIMED)));
				processHeros(ps, charId, hero);
				COMPLETE_HEROS.put(charId, hero);
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Hero System: Couldnt load Heroes: " + e.getMessage());
		}
		
		LOGGER.Info("Hero System: Loaded " + HEROES.size() + " Heroes.");
		LOGGER.Info("Hero System: Loaded " + COMPLETE_HEROS.size() + " all time Heroes.");
	}
	
	private void processHeros(PreparedStatement ps, int charId, StatSet hero)
	{
		ps.setInt(1, charId);
		{
			ResultSet rs = ps.executeQuery();
			if (rs.next())
			{
				int clanId = rs.getInt("clanid");
				int allyId = rs.getInt("allyId");
				String clanName = "";
				String allyName = "";
				int clanCrest = 0;
				int allyCrest = 0;
				if (clanId > 0)
				{
					clanName = ClanTable.getInstance().getClan(clanId).getName();
					clanCrest = ClanTable.getInstance().getClan(clanId).getCrestId();
					if (allyId > 0)
					{
						allyName = ClanTable.getInstance().getClan(clanId).getAllyName();
						allyCrest = ClanTable.getInstance().getClan(clanId).getAllyCrestId();
					}
				}
				hero.set(CLAN_CREST, clanCrest);
				hero.set(CLAN_NAME, clanName);
				hero.set(ALLY_CREST, allyCrest);
				hero.set(ALLY_NAME, allyName);
			}
			ps.clearParameters();
		}
	}
	
	private String calcFightTime(long fightTimeValue)
	{
		String format = String.format("%%0%dd", 2);
		long fightTime = fightTimeValue / 1000;
		return String.format(format, (fightTime % 3600) / 60) + ":" + String.format(format, fightTime % 60);
	}
	
	/**
	 * Restore hero message from Db.
	 * @param charId
	 */
	public void loadMessage(int charId)
	{
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement("SELECT message FROM heroes WHERE charId=?");
			ps.setInt(1, charId);
			{
				ResultSet rset = ps.executeQuery();
				if (rset.next())
				{
					HERO_MESSAGE.put(charId, rset.getString("message"));
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Hero System: Couldnt load Hero Message for CharId: " + charId + ": " + e);
		}
	}
	
	public void loadDiary(int charId)
	{
		List<StatSet> diary = new();
		int diaryentries = 0;
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement("SELECT * FROM  heroes_diary WHERE charId=? ORDER BY time ASC");
			ps.setInt(1, charId);
			{
				ResultSet rset = ps.executeQuery();
				while (rset.next())
				{
					StatSet diaryEntry = new StatSet();
					long time = rset.getLong("time");
					int action = rset.getInt("action");
					int param = rset.getInt("param");
					String date = (new SimpleDateFormat("yyyy-MM-dd HH")).format(new Date(time));
					diaryEntry.set("date", date);
					if (action == ACTION_RAID_KILLED)
					{
						NpcTemplate template = NpcData.getInstance().getTemplate(param);
						if (template != null)
						{
							diaryEntry.set("action", template.getName() + " was defeated");
						}
					}
					else if (action == ACTION_HERO_GAINED)
					{
						diaryEntry.set("action", "Gained Hero status");
					}
					else if (action == ACTION_CASTLE_TAKEN)
					{
						Castle castle = CastleManager.getInstance().getCastleById(param);
						if (castle != null)
						{
							diaryEntry.set("action", castle.getName() + " Castle was successfuly taken");
						}
					}
					diary.add(diaryEntry);
					diaryentries++;
				}
			}
			HERO_DIARY.put(charId, diary);
			
			LOGGER.Info("Hero System: Loaded " + diaryentries + " diary entries for Hero: " + CharInfoTable.getInstance().getNameById(charId));
		}
		catch (Exception e)
		{
			LOGGER.Warn("Hero System: Couldnt load Hero Diary for CharId: " + charId + ": " + e);
		}
	}
	
	public void loadFights(int charId)
	{
		List<StatSet> fights = new();
		StatSet heroCountData = new StatSet();
		Calendar data = Calendar.getInstance();
		data.set(Calendar.DAY_OF_MONTH, 1);
		data.set(Calendar.HOUR_OF_DAY, 0);
		data.set(Calendar.MINUTE, 0);
		data.set(Calendar.MILLISECOND, 0);
		
		long from = data.getTimeInMillis();
		int numberOfFights = 0;
		int victories = 0;
		int losses = 0;
		int draws = 0;
		
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement(
				"SELECT * FROM olympiad_fights WHERE (charOneId=? OR charTwoId=?) AND start<? ORDER BY start ASC");
			ps.setInt(1, charId);
			ps.setInt(2, charId);
			ps.setLong(3, from);
			{
				ResultSet rset = ps.executeQuery();
				int charOneId;
				int charOneClass;
				int charTwoId;
				int charTwoClass;
				int winner;
				long start;
				long time;
				int classed;
				while (rset.next())
				{
					charOneId = rset.getInt("charOneId");
					charOneClass = rset.getInt("charOneClass");
					charTwoId = rset.getInt("charTwoId");
					charTwoClass = rset.getInt("charTwoClass");
					winner = rset.getInt("winner");
					start = rset.getLong("start");
					time = rset.getLong("time");
					classed = rset.getInt("classed");
					if (charId == charOneId)
					{
						String name = CharInfoTable.getInstance().getNameById(charTwoId);
						String cls = ClassListData.getInstance().getClass(charTwoClass).getClientCode();
						if ((name != null) && (cls != null))
						{
							StatSet fight = new StatSet();
							fight.set("oponent", name);
							fight.set("oponentclass", cls);
							fight.set("time", calcFightTime(time));
							String date = (new SimpleDateFormat("yyyy-MM-dd HH:mm")).format(new Date(start));
							fight.set("start", date);
							fight.set("classed", classed);
							if (winner == 1)
							{
								fight.set("result", "<font color=\"00ff00\">victory</font>");
								victories++;
							}
							else if (winner == 2)
							{
								fight.set("result", "<font color=\"ff0000\">loss</font>");
								losses++;
							}
							else if (winner == 0)
							{
								fight.set("result", "<font color=\"ffff00\">draw</font>");
								draws++;
							}
							fights.add(fight);
							numberOfFights++;
						}
					}
					else if (charId == charTwoId)
					{
						String name = CharInfoTable.getInstance().getNameById(charOneId);
						String cls = ClassListData.getInstance().getClass(charOneClass).getClientCode();
						if ((name != null) && (cls != null))
						{
							StatSet fight = new StatSet();
							fight.set("oponent", name);
							fight.set("oponentclass", cls);
							fight.set("time", calcFightTime(time));
							String date = (new SimpleDateFormat("yyyy-MM-dd HH:mm")).format(new Date(start));
							fight.set("start", date);
							fight.set("classed", classed);
							if (winner == 1)
							{
								fight.set("result", "<font color=\"ff0000\">loss</font>");
								losses++;
							}
							else if (winner == 2)
							{
								fight.set("result", "<font color=\"00ff00\">victory</font>");
								victories++;
							}
							else if (winner == 0)
							{
								fight.set("result", "<font color=\"ffff00\">draw</font>");
								draws++;
							}
							fights.add(fight);
							numberOfFights++;
						}
					}
				}
			}
			
			heroCountData.set("victory", victories);
			heroCountData.set("draw", draws);
			heroCountData.set("loss", losses);
			HERO_COUNTS.put(charId, heroCountData);
			HERO_FIGHTS.put(charId, fights);
			
			LOGGER.Info("Hero System: Loaded " + numberOfFights + " fights for Hero: " + CharInfoTable.getInstance().getNameById(charId));
		}
		catch (Exception e)
		{
			LOGGER.Warn("Hero System: Couldnt load Hero fights history for CharId: " + charId + ": " + e);
		}
	}
	
	public Map<int, StatSet> getHeroes()
	{
		return HEROES;
	}
	
	public Map<int, StatSet> getCompleteHeroes()
	{
		return COMPLETE_HEROS;
	}
	
	public int getHeroByClass(int classid)
	{
		foreach (var e in HEROES)
		{
			if (e.Value.getInt(Olympiad.CLASS_ID) == classid)
			{
				return e.Key;
			}
		}
		return 0;
	}
	
	public void resetData()
	{
		HERO_DIARY.clear();
		HERO_FIGHTS.clear();
		HERO_COUNTS.clear();
		HERO_MESSAGE.clear();
	}
	
	public void showHeroDiary(Player player, int heroclass, int charid, int page)
	{
		int perpage = 10;
		List<StatSet> mainList = HERO_DIARY.get(charid);
		if (mainList != null)
		{
			NpcHtmlMessage diaryReply = new NpcHtmlMessage();
			String htmContent = HtmCache.getInstance().getHtm(player, "data/html/olympiad/herodiary.htm");
			String heroMessage = HERO_MESSAGE.get(charid);
			if ((htmContent != null) && (heroMessage != null))
			{
				diaryReply.setHtml(htmContent);
				diaryReply.replace("%heroname%", CharInfoTable.getInstance().getNameById(charid));
				diaryReply.replace("%message%", heroMessage);
				diaryReply.disableValidation();
				
				if (!mainList.isEmpty())
				{
					List<StatSet> list = new(mainList);
					Collections.reverse(list);
					
					bool color = true;
					StringBuilder fList = new StringBuilder(500);
					int counter = 0;
					int breakat = 0;
					for (int i = (page - 1) * perpage; i < list.size(); i++)
					{
						breakat = i;
						StatSet diaryEntry = list.get(i);
						fList.Append("<tr><td>");
						if (color)
						{
							fList.Append("<table width=270 bgcolor=\"131210\">");
						}
						else
						{
							fList.Append("<table width=270>");
						}
						fList.Append("<tr><td width=270><font color=\"LEVEL\">" + diaryEntry.getString("date") + ":xx</font></td></tr>");
						fList.Append("<tr><td width=270>" + diaryEntry.getString("action", "") + "</td></tr>");
						fList.Append("<tr><td>&nbsp;</td></tr></table>");
						fList.Append("</td></tr>");
						color = !color;
						counter++;
						if (counter >= perpage)
						{
							break;
						}
					}
					
					if (breakat < (list.size() - 1))
					{
						diaryReply.replace("%buttprev%", "<button value=\"Prev\" action=\"bypass _diary?class=" + heroclass + "&page=" + (page + 1) + "\" width=60 height=25 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\">");
					}
					else
					{
						diaryReply.replace("%buttprev%", "");
					}
					
					if (page > 1)
					{
						diaryReply.replace("%buttnext%", "<button value=\"Next\" action=\"bypass _diary?class=" + heroclass + "&page=" + (page - 1) + "\" width=60 height=25 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\">");
					}
					else
					{
						diaryReply.replace("%buttnext%", "");
					}
					
					diaryReply.replace("%list%", fList.ToString());
				}
				else
				{
					diaryReply.replace("%list%", "");
					diaryReply.replace("%buttprev%", "");
					diaryReply.replace("%buttnext%", "");
				}
				
				player.sendPacket(diaryReply);
			}
		}
	}
	
	public void showHeroFights(Player player, int heroclass, int charid, int page)
	{
		int perpage = 20;
		int win = 0;
		int loss = 0;
		int draw = 0;
		
		List<StatSet> heroFights = HERO_FIGHTS.get(charid);
		if (heroFights != null)
		{
			NpcHtmlMessage fightReply = new NpcHtmlMessage();
			String htmContent = HtmCache.getInstance().getHtm(player, "data/html/olympiad/herohistory.htm");
			if (htmContent != null)
			{
				fightReply.setHtml(htmContent);
				fightReply.replace("%heroname%", CharInfoTable.getInstance().getNameById(charid));
				if (!heroFights.isEmpty())
				{
					StatSet heroCount = HERO_COUNTS.get(charid);
					if (heroCount != null)
					{
						win = heroCount.getInt("victory");
						loss = heroCount.getInt("loss");
						draw = heroCount.getInt("draw");
					}
					
					bool color = true;
					StringBuilder fList = new StringBuilder(500);
					int counter = 0;
					int breakat = 0;
					for (int i = (page - 1) * perpage; i < heroFights.size(); i++)
					{
						breakat = i;
						StatSet fight = heroFights.get(i);
						fList.Append("<tr><td>");
						if (color)
						{
							fList.Append("<table width=270 bgcolor=\"131210\">");
						}
						else
						{
							fList.Append("<table width=270>");
						}
						fList.Append("<tr><td width=220><font color=\"LEVEL\">" + fight.getString("start") + "</font>&nbsp;&nbsp;" + fight.getString("result") + "</td><td width=50 align=right>" + (fight.getInt("classed") > 0 ? "<font color=\"FFFF99\">cls</font>" : "<font color=\"999999\">non-cls<font>") + "</td></tr>");
						fList.Append("<tr><td width=220>vs " + fight.getString("oponent") + " (" + fight.getString("oponentclass") + ")</td><td width=50 align=right>(" + fight.getString("time") + ")</td></tr>");
						fList.Append("<tr><td colspan=2>&nbsp;</td></tr></table>");
						fList.Append("</td></tr>");
						color = !color;
						counter++;
						if (counter >= perpage)
						{
							break;
						}
					}
					
					if (breakat < (heroFights.size() - 1))
					{
						fightReply.replace("%buttprev%", "<button value=\"Prev\" action=\"bypass _match?class=" + heroclass + "&page=" + (page + 1) + "\" width=60 height=25 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\">");
					}
					else
					{
						fightReply.replace("%buttprev%", "");
					}
					
					if (page > 1)
					{
						fightReply.replace("%buttnext%", "<button value=\"Next\" action=\"bypass _match?class=" + heroclass + "&page=" + (page - 1) + "\" width=60 height=25 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\">");
					}
					else
					{
						fightReply.replace("%buttnext%", "");
					}
					
					fightReply.replace("%list%", fList.ToString());
				}
				else
				{
					fightReply.replace("%list%", "");
					fightReply.replace("%buttprev%", "");
					fightReply.replace("%buttnext%", "");
				}
				
				fightReply.replace("%win%", win.ToString());
				fightReply.replace("%draw%", draw.ToString());
				fightReply.replace("%loos%", loss.ToString());
				player.sendPacket(fightReply);
			}
		}
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void computeNewHeroes(List<StatSet> newHeroes)
	{
		updateHeroes(true);
		
		foreach (int objectId in HEROES.Keys)
		{
			Player player = World.getInstance().getPlayer(objectId);
			if (player == null)
			{
				continue;
			}
			
			player.setHero(false);
			
			for (int i = 0; i < Inventory.PAPERDOLL_TOTALSLOTS; i++)
			{
				Item equippedItem = player.getInventory().getPaperdollItem(i);
				if ((equippedItem != null) && equippedItem.isHeroItem())
				{
					player.getInventory().unEquipItemInSlot(i);
				}
			}
			
			InventoryUpdate iu = new InventoryUpdate();
			foreach (Item item in player.getInventory().getAvailableItems(false, false, false))
			{
				if ((item != null) && item.isHeroItem())
				{
					player.destroyItem("Hero", item, null, true);
					iu.addRemovedItem(item);
				}
			}
			
			if (!iu.getItems().isEmpty())
			{
				player.sendInventoryUpdate(iu);
			}
			player.broadcastUserInfo();
		}
		
		deleteItemsInDb();
		HEROES.clear();
		
		if (newHeroes.isEmpty())
		{
			return;
		}
		
		foreach (StatSet hero in newHeroes)
		{
			int charId = hero.getInt(Olympiad.CHAR_ID);
			if (COMPLETE_HEROS.containsKey(charId))
			{
				StatSet oldHero = COMPLETE_HEROS.get(charId);
				if (hero.getInt(LEGEND_COUNT, 0) == 1)
				{
					int count = oldHero.getInt(LEGEND_COUNT);
					oldHero.set(LEGEND_COUNT, count + 1);
				}
				else
				{
					int count = oldHero.getInt(COUNT);
					oldHero.set(COUNT, count + 1);
				}
				oldHero.set(PLAYED, 1);
				oldHero.set(CLAIMED, false);
				HEROES.put(charId, oldHero);
			}
			else
			{
				StatSet newHero = new StatSet();
				newHero.set(Olympiad.CHAR_NAME, hero.getString(Olympiad.CHAR_NAME));
				newHero.set(Olympiad.CLASS_ID, hero.getInt(Olympiad.CLASS_ID));
				if (hero.getInt(LEGEND_COUNT, 0) == 1)
				{
					newHero.set(LEGEND_COUNT, 1);
				}
				else
				{
					newHero.set(COUNT, 1);
				}
				newHero.set(PLAYED, 1);
				newHero.set(CLAIMED, false);
				HEROES.put(charId, newHero);
			}
		}
		
		updateHeroes(false);
	}
	
	public void updateHeroes(bool setDefault)
	{
		try
		{
			Connection con = DatabaseFactory.getConnection();
			if (setDefault)
			{
				{
					Statement s = con.createStatement();
					s.executeUpdate(UPDATE_ALL);
				}
			}
			else
			{
				StatSet hero;
				int heroId;
				foreach (var entry in HEROES)
				{
					hero = entry.Value;
					heroId = entry.Key;
					if (!COMPLETE_HEROS.containsKey(heroId))
					{
						try
						{
							PreparedStatement insert = con.prepareStatement(INSERT_HERO);
							insert.setInt(1, heroId);
							insert.setInt(2, hero.getInt(Olympiad.CLASS_ID));
							insert.setInt(3, hero.getInt(COUNT, 0));
							insert.setInt(4, hero.getInt(LEGEND_COUNT, 0));
							insert.setInt(5, hero.getInt(PLAYED, 0));
							insert.setString(6, String.valueOf(hero.getBoolean(CLAIMED, false)));
							insert.execute();
							insert.close();
						}
						
						try
						{
							PreparedStatement statement = con.prepareStatement(GET_CLAN_ALLY);
							statement.setInt(1, heroId);
							{
								ResultSet rset = statement.executeQuery();
								if (rset.next())
								{
									int clanId = rset.getInt("clanid");
									int allyId = rset.getInt("allyId");
									String clanName = "";
									String allyName = "";
									int clanCrest = 0;
									int allyCrest = 0;
									if (clanId > 0)
									{
										clanName = ClanTable.getInstance().getClan(clanId).getName();
										clanCrest = ClanTable.getInstance().getClan(clanId).getCrestId();
										if (allyId > 0)
										{
											allyName = ClanTable.getInstance().getClan(clanId).getAllyName();
											allyCrest = ClanTable.getInstance().getClan(clanId).getAllyCrestId();
										}
									}
									hero.set(CLAN_CREST, clanCrest);
									hero.set(CLAN_NAME, clanName);
									hero.set(ALLY_CREST, allyCrest);
									hero.set(ALLY_NAME, allyName);
								}
							}
						}
						
						HEROES.put(heroId, hero);
						COMPLETE_HEROS.put(heroId, hero);
					}
					else
					{
						{
							PreparedStatement statement = con.prepareStatement(UPDATE_HERO);
							statement.setInt(1, hero.getInt(Olympiad.CLASS_ID));
							statement.setInt(2, hero.getInt(COUNT, 0));
							statement.setInt(3, hero.getInt(LEGEND_COUNT, 0));
							statement.setInt(4, hero.getInt(PLAYED, 0));
							statement.setString(5, String.valueOf(hero.getBoolean(CLAIMED, false)));
							statement.setInt(6, heroId);
							statement.execute();
						}
					}
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Hero System: Couldnt update Heroes: " + e);
		}
	}
	
	public void setHeroGained(int charId)
	{
		setDiaryData(charId, ACTION_HERO_GAINED, 0);
	}
	
	public void setRBkilled(int charId, int npcId)
	{
		setDiaryData(charId, ACTION_RAID_KILLED, npcId);
		
		NpcTemplate template = NpcData.getInstance().getTemplate(npcId);
		List<StatSet> list = HERO_DIARY.get(charId);
		if ((list == null) || (template == null))
		{
			return;
		}
		
		// Prepare new data
		StatSet diaryEntry = new StatSet();
		String date = (new SimpleDateFormat("yyyy-MM-dd HH")).format(new Date(System.currentTimeMillis()));
		diaryEntry.set("date", date);
		diaryEntry.set("action", template.getName() + " was defeated");
		
		// Add to old list
		list.add(diaryEntry);
	}
	
	public void setCastleTaken(int charId, int castleId)
	{
		setDiaryData(charId, ACTION_CASTLE_TAKEN, castleId);
		
		Castle castle = CastleManager.getInstance().getCastleById(castleId);
		List<StatSet> list = HERO_DIARY.get(charId);
		if ((list == null) || (castle == null))
		{
			return;
		}
		
		// Prepare new data
		StatSet diaryEntry = new StatSet();
		String date = (new SimpleDateFormat("yyyy-MM-dd HH")).format(new Date(System.currentTimeMillis()));
		diaryEntry.set("date", date);
		diaryEntry.set("action", castle.getName() + " Castle was successfuly taken");
		
		// Add to old list
		list.add(diaryEntry);
	}
	
	public void setDiaryData(int charId, int action, int param)
	{
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps =
				con.prepareStatement("INSERT INTO heroes_diary (charId, time, action, param) values(?,?,?,?)");
			ps.setInt(1, charId);
			ps.setLong(2, System.currentTimeMillis());
			ps.setInt(3, action);
			ps.setInt(4, param);
			ps.execute();
		}
		catch (Exception e)
		{
			LOGGER.Error("SQL exception while saving DiaryData: " + e);
		}
	}
	
	/**
	 * Set new hero message for hero
	 * @param player the player instance
	 * @param message String to set
	 */
	public void setHeroMessage(Player player, String message)
	{
		HERO_MESSAGE.put(player.getObjectId(), message);
	}
	
	/**
	 * Update hero message in database
	 * @param charId character objid
	 */
	public void saveHeroMessage(int charId)
	{
		if (!HERO_MESSAGE.containsKey(charId))
		{
			return;
		}
		
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps = con.prepareStatement("UPDATE heroes SET message=? WHERE charId=?;");
			ps.setString(1, HERO_MESSAGE.get(charId));
			ps.setInt(2, charId);
			ps.execute();
		}
		catch (Exception e)
		{
			LOGGER.Error("SQL exception while saving HeroMessage:" + e);
		}
	}
	
	private void deleteItemsInDb()
	{
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			Statement s = con.createStatement();
			s.executeUpdate(DELETE_ITEMS);
		}
		catch (Exception e)
		{
			LOGGER.Warn("Heroes: " + e);
		}
	}
	
	/**
	 * Saving task for {@link Hero}<br>
	 * Save all hero messages to DB.
	 */
	public void shutdown()
	{
		HERO_MESSAGE.Keys.forEach(saveHeroMessage);
	}
	
	/**
	 * Verifies if the given object ID belongs to a claimed hero.
	 * @param objectId the player's object ID to verify
	 * @return {@code true} if there are heros and the player is in the list, {@code false} otherwise
	 */
	public bool isHero(int objectId)
	{
		return HEROES.containsKey(objectId) && HEROES.get(objectId).getBoolean(CLAIMED);
	}
	
	/**
	 * Verifies if the given object ID belongs to an unclaimed hero.
	 * @param objectId the player's object ID to verify
	 * @return {@code true} if player is unclaimed hero
	 */
	public bool isUnclaimedHero(int objectId)
	{
		return HEROES.containsKey(objectId) && !HEROES.get(objectId).getBoolean(CLAIMED);
	}
	
	/**
	 * Claims the hero status for the given player.
	 * @param player the player to become hero
	 */
	public void claimHero(Player player)
	{
		StatSet hero = HEROES.get(player.getObjectId());
		if (hero == null)
		{
			hero = new StatSet();
			HEROES.put(player.getObjectId(), hero);
		}
		
		hero.set(CLAIMED, true);
		
		Clan clan = player.getClan();
		if ((clan != null) && (clan.getLevel() >= 3))
		{
			clan.addReputationScore(Config.HERO_POINTS);
			SystemMessage sm = new SystemMessage(SystemMessageId.CLAN_MEMBER_C1_HAS_BECOME_THE_HERO_CLAN_REPUTATION_POINTS_S2);
			sm.addString(CharInfoTable.getInstance().getNameById(player.getObjectId()));
			sm.addInt(Config.HERO_POINTS);
			clan.broadcastToOnlineMembers(sm);
		}
		
		player.setHero(true);
		player.broadcastPacket(new SocialAction(player.getObjectId(), 20016)); // Hero Animation
		player.broadcastUserInfo();
		
		// Set Gained hero and reload data
		setHeroGained(player.getObjectId());
		loadFights(player.getObjectId());
		loadDiary(player.getObjectId());
		HERO_MESSAGE.put(player.getObjectId(), "");
		
		if (EventDispatcher.getInstance().hasListener(EventType.ON_PLAYER_TAKE_HERO))
		{
			EventDispatcher.getInstance().notifyEvent(new OnPlayerTakeHero(player));
		}
		
		updateHeroes(false);
	}
	
	public static Hero getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly Hero INSTANCE = new Hero();
	}
}
