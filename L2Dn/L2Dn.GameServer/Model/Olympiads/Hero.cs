using System.Runtime.CompilerServices;
using System.Text;
using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Creatures.Players;
using L2Dn.GameServer.Model.Events.Returns;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;
using Clan = L2Dn.GameServer.Model.Clans.Clan;

namespace L2Dn.GameServer.Model.Olympiads;

/**
 * Hero entity.
 * @author godson
 */
public class Hero
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(Hero));
	
	private const String UPDATE_ALL = "UPDATE heroes SET played = 0";
	private const String INSERT_HERO = "INSERT INTO heroes (charId, class_id, count, legend_count, played, claimed) VALUES (?,?,?,?,?,?)";
	private const String UPDATE_HERO = "UPDATE heroes SET class_id = ?, count = ?, legend_count = ?, played = ?, claimed = ? WHERE charId = ?";
	private const String GET_CLAN_ALLY = "SELECT characters.clanid AS clanid, coalesce(clan_data.ally_Id, 0) AS allyId FROM characters LEFT JOIN clan_data ON clan_data.clan_id = characters.clanid WHERE characters.charId = ?";
	
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
			using GameServerDbContext ctx = new();
			
			var query1 =
				from h in ctx.Heroes
				from c in ctx.Characters
				where h.CharacterId == c.Id && h.Played
				select new
				{
					h.CharacterId,
					CharacterName = c.Name,
					h.ClassId,
					h.Count,
					h.LegendCount,
					h.Claimed
				};
			
			foreach (var record1 in query1)
			{
				StatSet hero = new StatSet();
				int charId = record1.CharacterId;
				hero.set(Olympiad.CHAR_NAME, record1.CharacterName);
				hero.set(Olympiad.CLASS_ID, record1.ClassId);
				hero.set(COUNT, record1.Count);
				hero.set(LEGEND_COUNT, record1.LegendCount);
				hero.set(PLAYED, true);
				hero.set(CLAIMED, record1.Claimed);
				loadFights(charId);
				loadDiary(charId);
				loadMessage(charId);
				loadHeroClanAlly(ctx, charId, hero);
				HEROES.put(charId, hero);
			}
			
			var query2 =
				from h in ctx.Heroes
				from c in ctx.Characters
				where h.CharacterId == c.Id
				select new
				{
					h.CharacterId,
					CharacterName = c.Name,
					h.ClassId,
					h.Count,
					h.LegendCount,
					h.Played,
					h.Claimed
				};
			
			foreach (var record2 in query2)
			{
				StatSet hero = new StatSet();
				int charId = record2.CharacterId;
				hero.set(Olympiad.CHAR_NAME, record2.CharacterName);
				hero.set(Olympiad.CLASS_ID, record2.ClassId);
				hero.set(COUNT, record2.Count);
				hero.set(LEGEND_COUNT, record2.LegendCount);
				hero.set(PLAYED, record2.Played);
				hero.set(CLAIMED, record2.Claimed);
				loadHeroClanAlly(ctx, charId, hero);
				COMPLETE_HEROS.put(charId, hero);
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Hero System: Couldnt load Heroes: " + e);
		}
		
		LOGGER.Info("Hero System: Loaded " + HEROES.size() + " Heroes.");
		LOGGER.Info("Hero System: Loaded " + COMPLETE_HEROS.size() + " all time Heroes.");
	}
	
	private static void loadHeroClanAlly(GameServerDbContext ctx, int charId, StatSet hero)
	{
		var record =
			(from ch in ctx.Characters
				from c in ctx.Clans
				where ch.ClanId == c.Id && ch.Id == charId
				select new
				{
					ch.ClanId,
					c.AllyId,
				}).SingleOrDefault();

		if (record is not null)
		{
			int? clanId = record.ClanId;
			int? allyId = record.AllyId;
			String? clanName = null;
			String? allyName = null;
			int? clanCrest = null;
			int? allyCrest = null;
			if (clanId != null)
			{
				clanName = ClanTable.getInstance().getClan(clanId.Value).getName();
				clanCrest = ClanTable.getInstance().getClan(clanId.Value).getCrestId();
				if (allyId > 0)
				{
					allyName = ClanTable.getInstance().getClan(clanId.Value).getAllyName() ?? string.Empty;
					allyCrest = ClanTable.getInstance().getClan(clanId.Value).getAllyCrestId();
				}
			}
			
			if (clanCrest != null)
				hero.set(CLAN_CREST, clanCrest.Value);
			
			if (!string.IsNullOrEmpty(clanName))
				hero.set(CLAN_NAME, clanName);
			
			if (allyCrest != null)
				hero.set(ALLY_CREST, allyCrest);
			
			if (!string.IsNullOrEmpty(allyName))
				hero.set(ALLY_NAME, allyName);
		}
	}
	
	/**
	 * Restore hero message from Db.
	 * @param charId
	 */
	public void loadMessage(int charId)
	{
		try
		{
			using GameServerDbContext ctx = new();
			string? message = ctx.Heroes.Where(r => r.CharacterId == charId).Select(r => r.Message).SingleOrDefault();
			
			if (message != null)
				HERO_MESSAGE.put(charId, message);
		}
		catch (Exception e)
		{
			LOGGER.Error("Hero System: Couldnt load Hero Message for CharId: " + charId + ": " + e);
		}
	}
	
	public void loadDiary(int charId)
	{
		List<StatSet> diary = new();
		int diaryentries = 0;
		try
		{
			using GameServerDbContext ctx = new();
			var query = ctx.HeroesDiary.Where(r => r.CharacterId == charId).OrderBy(r => r.Time);
			foreach (var record in query)
			{
				StatSet diaryEntry = new StatSet();
				DateTime time = record.Time;
				int action = record.Action;
				int param = record.Param;
				diaryEntry.set("date", time);
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

			HERO_DIARY.put(charId, diary);

			LOGGER.Info("Hero System: Loaded " + diaryentries + " diary entries for Hero: " +
			            CharInfoTable.getInstance().getNameById(charId));
		}
		catch (Exception e)
		{
			LOGGER.Error("Hero System: Couldnt load Hero Diary for CharId: " + charId + ": " + e);
		}
	}
	
	public void loadFights(int charId)
	{
		List<StatSet> fights = new();
		StatSet heroCountData = new StatSet();
		DateTime data = DateTime.Now;
		data = new DateTime(data.Year, data.Month, 1);
		
		int numberOfFights = 0;
		int victories = 0;
		int losses = 0;
		int draws = 0;
		
		try 
		{
			using GameServerDbContext ctx = new();
			var query = ctx.OlympiadFights
				.Where(r => (r.Character1Id == charId || r.Character2Id == charId) && r.Start < data)
				.OrderBy(r => r.Start);
			
				foreach (var record in query)
				{
					int charOneId = record.Character1Id;
					CharacterClass charOneClass = record.Character1Class;
					int charTwoId = record.Character2Id;
					CharacterClass charTwoClass = record.Character2Class;
					int winner = record.Winner;
					DateTime start = record.Start;
					TimeSpan time = record.Time;
					bool classed = record.Classed;
					if (charId == charOneId)
					{
						String name = CharInfoTable.getInstance().getNameById(charTwoId);
						String cls = ClassListData.getInstance().getClass(charTwoClass).getClientCode();
						if ((name != null) && (cls != null))
						{
							StatSet fight = new StatSet();
							fight.set("oponent", name);
							fight.set("oponentclass", cls);
							fight.set("time", time.ToString("mm:ss"));
							string date = start.ToString("yyyy-MM-dd HH:mm");
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
							fight.set("time", time.ToString("mm:ss"));
							string date = start.ToString("yyyy-MM-dd HH:mm");
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
			String htmContent = HtmCache.getInstance().getHtm(player, "data/html/olympiad/herodiary.htm");
			String heroMessage = HERO_MESSAGE.get(charid);
			if ((htmContent != null) && (heroMessage != null))
			{
				HtmlPacketHelper diaryReply = new HtmlPacketHelper(htmContent); 
				diaryReply.Replace("%heroname%", CharInfoTable.getInstance().getNameById(charid));
				diaryReply.Replace("%message%", heroMessage);
				//diaryReply.disableValidation(); // TODO disableValidation
				
				if (!mainList.isEmpty())
				{
					List<StatSet> list = new(mainList);
					list.Reverse();
					
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
						diaryReply.Replace("%buttprev%", "<button value=\"Prev\" action=\"bypass _diary?class=" + heroclass + "&page=" + (page + 1) + "\" width=60 height=25 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\">");
					}
					else
					{
						diaryReply.Replace("%buttprev%", "");
					}
					
					if (page > 1)
					{
						diaryReply.Replace("%buttnext%", "<button value=\"Next\" action=\"bypass _diary?class=" + heroclass + "&page=" + (page - 1) + "\" width=60 height=25 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\">");
					}
					else
					{
						diaryReply.Replace("%buttnext%", "");
					}
					
					diaryReply.Replace("%list%", fList.ToString());
				}
				else
				{
					diaryReply.Replace("%list%", "");
					diaryReply.Replace("%buttprev%", "");
					diaryReply.Replace("%buttnext%", "");
				}
				
				NpcHtmlMessagePacket htmlMessagePacket = new NpcHtmlMessagePacket(diaryReply);
				player.sendPacket(htmlMessagePacket);
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
			String htmContent = HtmCache.getInstance().getHtm(player, "data/html/olympiad/herohistory.htm");
			if (htmContent != null)
			{
				HtmlPacketHelper fightReply = new HtmlPacketHelper(htmContent); 
				fightReply.Replace("%heroname%", CharInfoTable.getInstance().getNameById(charid));
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
						fightReply.Replace("%buttprev%", "<button value=\"Prev\" action=\"bypass _match?class=" + heroclass + "&page=" + (page + 1) + "\" width=60 height=25 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\">");
					}
					else
					{
						fightReply.Replace("%buttprev%", "");
					}
					
					if (page > 1)
					{
						fightReply.Replace("%buttnext%", "<button value=\"Next\" action=\"bypass _match?class=" + heroclass + "&page=" + (page - 1) + "\" width=60 height=25 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\">");
					}
					else
					{
						fightReply.Replace("%buttnext%", "");
					}
					
					fightReply.Replace("%list%", fList.ToString());
				}
				else
				{
					fightReply.Replace("%list%", "");
					fightReply.Replace("%buttprev%", "");
					fightReply.Replace("%buttnext%", "");
				}
				
				fightReply.Replace("%win%", win.ToString());
				fightReply.Replace("%draw%", draw.ToString());
				fightReply.Replace("%loos%", loss.ToString());

				NpcHtmlMessagePacket htmlMessagePacket = new NpcHtmlMessagePacket(fightReply);
				player.sendPacket(htmlMessagePacket);
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

			List<ItemInfo> items = new List<ItemInfo>();
			foreach (Item item in player.getInventory().getAvailableItems(false, false, false))
			{
				if ((item != null) && item.isHeroItem())
				{
					player.destroyItem("Hero", item, null, true);
					items.Add(new ItemInfo(item, ItemChangeType.REMOVED));
				}
			}
			
			if (!items.isEmpty())
			{
				InventoryUpdatePacket iu = new InventoryUpdatePacket();
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
			using GameServerDbContext ctx = new();
			if (setDefault)
			{
				ctx.Heroes.ExecuteUpdate(s => s.SetProperty(r => r.Played, false));
			}
			else
			{
				foreach (var entry in HEROES)
				{
					StatSet hero = entry.Value;
					int heroId = entry.Key;

					DbHero? record = ctx.Heroes.SingleOrDefault(r => r.CharacterId == heroId);
					if (record is null)
					{
						record = new DbHero()
						{
							CharacterId = heroId,
						};

						ctx.Heroes.Add(record);
					}

					record.ClassId = hero.getEnum<CharacterClass>(Olympiad.CLASS_ID);
					record.Count = (short)hero.getInt(COUNT, 0);
					record.LegendCount = (short)hero.getInt(LEGEND_COUNT, 0);
					record.Played = hero.getBoolean(PLAYED, false);
					record.Claimed = hero.getBoolean(CLAIMED, false);
					
					if (!COMPLETE_HEROS.containsKey(heroId))
					{
						loadHeroClanAlly(ctx, heroId, hero);

						HEROES.put(heroId, hero);
						COMPLETE_HEROS.put(heroId, hero);
					}
				}

				ctx.SaveChanges();
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
		diaryEntry.set("date", DateTime.UtcNow);
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
		diaryEntry.set("date", DateTime.UtcNow);
		diaryEntry.set("action", castle.getName() + " Castle was successfuly taken");
		
		// Add to old list
		list.add(diaryEntry);
	}
	
	public void setDiaryData(int charId, int action, int param)
	{
		try 
		{
			using GameServerDbContext ctx = new();
			ctx.HeroesDiary.Add(new DbHeroDiary()
			{
				CharacterId = charId,
				Time = DateTime.UtcNow,
				Action = (byte)action,
				Param = param
			});

			ctx.SaveChanges();
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
			using GameServerDbContext ctx = new();
			string message = HERO_MESSAGE.get(charId);
			ctx.Heroes.Where(r => r.CharacterId == charId).ExecuteUpdate(s => s.SetProperty(r => r.Message, message));
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
			using GameServerDbContext ctx = new();
			
			ctx.Items.Where(r => new List<int>()
				{
					30392, 30393, 30394, 30395, 30396, 30397, 30398, 30399, 30400, 30401, 30402, 30403, 30404, 30405,
					30372, 30373, 6842, 6611, 6612, 6613, 6614, 6615, 6616, 6617, 6618, 6619, 6620, 6621, 9388, 9389, 
					9390
				}.Contains(r.ItemId) &&
				ctx.Characters.Where(c => c.AccessLevel > 0).Select(c => c.Id).Contains(r.OwnerId))
				.ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Error("Heroes: " + e);
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
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.CLAN_MEMBER_C1_HAS_BECOME_THE_HERO_CLAN_REPUTATION_POINTS_S2);
			sm.Params.addString(CharInfoTable.getInstance().getNameById(player.getObjectId()));
			sm.Params.addInt(Config.HERO_POINTS);
			clan.broadcastToOnlineMembers(sm);
		}
		
		player.setHero(true);
		player.broadcastPacket(new SocialActionPacket(player.getObjectId(), 20016)); // Hero Animation
		player.broadcastUserInfo();
		
		// Set Gained hero and reload data
		setHeroGained(player.getObjectId());
		loadFights(player.getObjectId());
		loadDiary(player.getObjectId());
		HERO_MESSAGE.put(player.getObjectId(), "");
		
		if (EventDispatcher.getInstance().hasListener(EventType.ON_PLAYER_TAKE_HERO))
		{
			EventDispatcher.getInstance().notifyEvent<AbstractEventReturn>(new OnPlayerTakeHero(player));
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
