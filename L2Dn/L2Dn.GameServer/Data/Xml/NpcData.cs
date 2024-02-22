using System.Runtime.CompilerServices;
using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * NPC data parser.
 * @author NosBit
 */
public class NpcData: DataReaderBase
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(NpcData));
	
	private readonly Map<int, NpcTemplate> _npcs = new();
	private readonly Map<String, int> _clans = new();
	private static readonly Set<int> _masterMonsterIDs = new();
	
	protected NpcData()
	{
		load();
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)] 
	public void load()
	{
		_masterMonsterIDs.clear();
		
		LoadXmlDocuments(DataFileLocation.Data, "stats/npcs").ForEach(t =>
		{
			t.Document.Elements("list").Elements("npc").ForEach(x => loadElement(t.FilePath, x));
		});
		
		LOGGER.Info(GetType().Name + ": Loaded " + _npcs.size() + " NPCs.");

		if (Config.CUSTOM_NPC_DATA)
		{
			int npcCount = _npcs.size();
			LoadXmlDocuments(DataFileLocation.Data, "stats/npcs/custom").ForEach(t =>
			{
				t.Document.Elements("list").Elements("npc").ForEach(x => loadElement(t.FilePath, x));
			});
		
			LOGGER.Info(GetType().Name + ": Loaded " + (_npcs.size() - npcCount) + " custom NPCs.");
		}
	}

	private void loadElement(string filePath, XElement element)
	{
		StatSet set = new StatSet();
		int npcId = element.Attribute("id").GetInt32();
		int level = element.Attribute("level").GetInt32(85);
		String type = element.Attribute("type").GetString("Folk");
		Map<String, Object> parameters = null;
		Map<int, Skill> skills = new();
		Set<int> clans = null;
		Set<int> ignoreClanNpcIds = null;
		List<DropHolder> dropLists = null;
		List<DropGroupHolder> dropGroups = null;
		set.set("id", npcId);
		set.set("displayId", element.Attribute("displayId").GetInt32());
		set.set("level", level);
		set.set("type", type);
		set.set("name", element.Attribute("name").GetString());
		set.set("usingServerSideName", element.Attribute("usingServerSideName").GetBoolean());
		set.set("title", element.Attribute("title").GetString());
		set.set("usingServerSideTitle", element.Attribute("usingServerSideTitle").GetBoolean());
		set.set("elementalType", element.Attribute("element").GetEnum<ElementalType>());

		element.Elements("parameters").ForEach(el =>
		{
			if (parameters == null)
				parameters = new();

			parameters.putAll(parseParameters(el));
		});

		element.Elements("race").ForEach(el => { set.set("race", el.Value); });

		element.Elements("sex").ForEach(el => { set.set("sex", el.Value); });

		element.Elements("equipment").ForEach(el =>
		{
			set.set("chestId", el.Attribute("chest").GetInt32(0));
			set.set("rhandId", el.Attribute("rhand").GetInt32(0));
			set.set("lhandId", el.Attribute("lhand").GetInt32(0));
			set.set("weaponEnchant", el.Attribute("weaponEnchant").GetInt32(0));
		});

		element.Elements("acquire").ForEach(el =>
		{
			set.set("exp", el.Attribute("exp").GetDouble(0)); // TODO: why double?
			set.set("sp", el.Attribute("sp").GetDouble(0));
			set.set("raidPoints", el.Attribute("raidPoints").GetDouble(0));
			set.set("attributeExp", el.Attribute("attributeExp").GetInt64(0));
		});

		element.Elements("mpreward").ForEach(el =>
		{
			set.set("mpRewardValue", el.Attribute("value").GetInt32());
			set.set("mpRewardType", el.Attribute("type").GetEnum<MpRewardType>());
			set.set("mpRewardTicks", el.Attribute("ticks").GetInt32());
			set.set("mpRewardAffectType", el.Attribute("affects").GetEnum<MpRewardAffectType>());
		});

		element.Elements("stats").ForEach(el =>
		{
			set.set("baseSTR", el.Attribute("str").GetInt32());
			set.set("baseINT", el.Attribute("int").GetInt32());
			set.set("baseDEX", el.Attribute("dex").GetInt32());
			set.set("baseWIT", el.Attribute("wit").GetInt32());
			set.set("baseCON", el.Attribute("con").GetInt32());
			set.set("baseMEN", el.Attribute("men").GetInt32());

			el.Elements("vitals").forEach(e =>
			{
				set.set("baseHpMax", e.Attribute("hp").GetDouble());
				set.set("baseHpReg", e.Attribute("hpRegen").GetDouble());
				set.set("baseMpMax", e.Attribute("mp").GetDouble());
				set.set("baseMpReg", e.Attribute("mpRegen").GetDouble());
			});

			el.Elements("attack").forEach(e =>
			{
				set.set("basePAtk", e.Attribute("physical").GetDouble());
				set.set("baseMAtk", e.Attribute("magical").GetDouble());
				set.set("baseRndDam", e.Attribute("random").GetInt32());
				set.set("baseCritRate", e.Attribute("critical").GetDouble());
				set.set("accuracy", e.Attribute("accuracy").GetFloat()); // TODO: Implement me
				set.set("basePAtkSpd", e.Attribute("attackSpeed").GetFloat());
				set.set("reuseDelay", e.Attribute("reuseDelay").GetInt32()); // TODO: Implement me
				set.set("baseAtkType", e.Attribute("type").GetString());
				set.set("baseAtkRange", e.Attribute("range").GetInt32());
				set.set("distance", e.Attribute("distance").GetInt32()); // TODO: Implement me
				set.set("width", e.Attribute("width").GetInt32()); // TODO: Implement me
			});

			el.Elements("defence").forEach(e =>
			{
				set.set("basePDef", e.Attribute("physical").GetDouble());
				set.set("baseMDef", e.Attribute("magical").GetDouble());
				set.set("evasion", e.Attribute("evasion").GetInt32()); // TODO: Implement me
				set.set("baseShldDef", e.Attribute("shield").GetInt32());
				set.set("baseShldRate", e.Attribute("shieldRate").GetInt32());
			});

			el.Elements("abnormalresist").forEach(e =>
			{
				set.set("physicalAbnormalResist", e.Attribute("physical").GetDouble());
				set.set("magicAbnormalResist", e.Attribute("magic").GetDouble());
			});

			el.Elements("attribute").Elements("attack").forEach(e =>
			{
				string attackAttributeType = e.Attribute("type").GetString();
				int value = e.Attribute("value").GetInt32();
				switch (attackAttributeType.toUpperCase())
				{
					case "FIRE":
					{
						set.set("baseFire", value);
						break;
					}
					case "WATER":
					{
						set.set("baseWater", value);
						break;
					}
					case "WIND":
					{
						set.set("baseWind", value);
						break;
					}
					case "EARTH":
					{
						set.set("baseEarth", value);
						break;
					}
					case "DARK":
					{
						set.set("baseDark", value);
						break;
					}
					case "HOLY":
					{
						set.set("baseHoly", value);
						break;
					}
					default:
						throw new InvalidOperationException("Invalid elemental attribute");
				}
			});

			el.Elements("attribute").Elements("defence").forEach(e =>
			{
				set.set("baseFireRes", e.Attribute("fire").GetInt32());
				set.set("baseWaterRes", e.Attribute("water").GetInt32());
				set.set("baseWindRes", e.Attribute("wind").GetInt32());
				set.set("baseEarthRes", e.Attribute("earth").GetInt32());
				set.set("baseHolyRes", e.Attribute("holy").GetInt32());
				set.set("baseDarkRes", e.Attribute("dark").GetInt32());
				set.set("baseElementRes", e.Attribute("default").GetInt32());
			});

			el.Elements("speed").Elements("walk").forEach(e =>
			{
				double groundWalk = e.Attribute("ground").GetDouble();
				set.set("baseWalkSpd", groundWalk <= 0d ? 0.1 : groundWalk);
				set.set("baseSwimWalkSpd", e.Attribute("swim").GetDouble());
				set.set("baseFlyWalkSpd", e.Attribute("fly").GetDouble());
			});

			el.Elements("speed").Elements("run").forEach(e =>
			{
				double runSpeed = e.Attribute("ground").GetDouble();
				set.set("baseRunSpd", runSpeed <= 0d ? 0.1 : runSpeed);
				set.set("baseSwimRunSpd", e.Attribute("swim").GetDouble());
				set.set("baseFlyRunSpd", e.Attribute("fly").GetDouble());
			});

			el.Elements("hittime").forEach(e =>
			{
				set.set("hitTime", e.Value); // TODO: Implement me default 600 (value in ms)
			});
		});

		element.Elements("status").ForEach(el =>
		{
			set.set("unique", el.Attribute("unique").GetBoolean());
			set.set("attackable", el.Attribute("attackable").GetBoolean());
			set.set("targetable", el.Attribute("targetable").GetBoolean());
			set.set("talkable", el.Attribute("talkable").GetBoolean());
			set.set("undying", el.Attribute("undying").GetBoolean());
			set.set("showName", el.Attribute("showName").GetBoolean());
			set.set("randomWalk", el.Attribute("randomWalk").GetBoolean());
			set.set("randomAnimation", el.Attribute("randomAnimation").GetBoolean());
			set.set("flying", el.Attribute("flying").GetBoolean());
			set.set("canMove", el.Attribute("canMove").GetBoolean());
			set.set("noSleepMode", el.Attribute("noSleepMode").GetBoolean());
			set.set("passableDoor", el.Attribute("passableDoor").GetBoolean());
			set.set("hasSummoner", el.Attribute("hasSummoner").GetBoolean());
			set.set("canBeSown", el.Attribute("canBeSown").GetBoolean());
			set.set("isDeathPenalty", el.Attribute("isDeathPenalty").GetBoolean());
			set.set("fakePlayer", el.Attribute("fakePlayer").GetBoolean());
			set.set("fakePlayerTalkable", el.Attribute("fakePlayerTalkable").GetBoolean());
		});

		element.Elements("skilllist").Elements("skill").ForEach(el =>
		{
			int skillId = el.Attribute("id").GetInt32();
			int skillLevel = el.Attribute("level").GetInt32();
			Skill skill = SkillData.getInstance().getSkill(skillId, skillLevel);
			if (skill != null)
			{
				skills.put(skill.getId(), skill);
			}
			else
			{
				LOGGER.Warn("[" + filePath + "] skill not found. NPC ID: " + npcId + " Skill ID: " +
				            skillId + " Skill Level: " + skillLevel);
			}
		});


		element.Elements("shots").ForEach(el =>
		{
			set.set("soulShot", el.Attribute("soul").GetInt32());
			set.set("spiritShot", el.Attribute("spirit").GetInt32());
			set.set("shotShotChance", el.Attribute("shotChance").GetInt32());
			set.set("spiritShotChance", el.Attribute("spiritChance").GetInt32());
		});

		element.Elements("corpsetime").ForEach(el => { set.set("corpseTime", el.Value); });

		element.Elements("excrteffect").ForEach(el =>
		{
			set.set("exCrtEffect", el.Value); // TODO: Implement me default ? type bool
		});

		element.Elements("snpcprophprate").ForEach(el =>
		{
			set.set("sNpcPropHpRate", el.Value); // TODO: Implement me default 1 type double
		});

		element.Elements("ai").ForEach(el =>
		{
			set.set("aiType", el.Attribute("type").GetString());
			set.set("aggroRange", el.Attribute("aggroRange").GetInt32());
			set.set("clanHelpRange", el.Attribute("clanHelpRange").GetInt32());
			set.set("isChaos", el.Attribute("isChaos").GetBoolean());
			set.set("isAggressive", el.Attribute("isAggressive").GetBoolean());

			el.Elements("skill").ForEach(e =>
			{
				set.set("minSkillChance", e.Attribute("minChance").GetInt32());
				set.set("maxSkillChance", e.Attribute("maxChance").GetInt32());
				set.set("primarySkillId", e.Attribute("primaryId").GetInt32());
				set.set("shortRangeSkillId", e.Attribute("shortRangeId").GetInt32());
				set.set("shortRangeSkillChance", e.Attribute("shortRangeChance").GetInt32());
				set.set("longRangeSkillId", e.Attribute("longRangeId").GetInt32());
				set.set("longRangeSkillChance", e.Attribute("longRangeChance").GetInt32());
			});

			el.Elements("clanlist").Elements("clan").ForEach(e =>
			{
				if (clans == null)
					clans = new();

				clans.add(getOrCreateClanId(e.Value));
			});

			el.Elements("clanlist").Elements("ignorenpcid").ForEach(e =>
			{
				if (ignoreClanNpcIds == null)
					ignoreClanNpcIds = new();

				ignoreClanNpcIds.add((int)e);
			});
		});

		element.Elements("droplists").Elements().ForEach(el =>
		{
			DropType dropType = Enum.Parse<DropType>(el.Name.LocalName);
			el.Elements("group").ForEach(e =>
			{
				if (dropGroups == null)
				{
					dropGroups = new();
				}

				double chance = e.Attribute("chance").GetDouble();
				DropGroupHolder group = new DropGroupHolder(chance);

				e.Elements("item").ForEach(itemEl =>
				{
					int itemId = itemEl.Attribute("id").GetInt32();

					// Drop materials for random craft configuration.
					if (!Config.DROP_RANDOM_CRAFT_MATERIALS && (itemId >= 92908) && (itemId <= 92919))
						return;

					if (ItemData.getInstance().getTemplate(itemId) == null)
					{
						LOGGER.Error("DropListItem: Could not find item with id " + itemId + ".");
					}
					else
					{
						long min = itemEl.Attribute("min").GetInt64();
						long max = itemEl.Attribute("max").GetInt64();
						double chance1 = itemEl.Attribute("chance").GetDouble();
						group.addDrop(new DropHolder(dropType, itemId, min, max, chance1));
					}
				});

				dropGroups.add(group);
			});

			el.Elements("item").ForEach(e =>
			{
				if (dropLists == null)
				{
					dropLists = new();
				}

				int itemId = e.Attribute("id").GetInt32();

				// Drop materials for random craft configuration.
				if (!Config.DROP_RANDOM_CRAFT_MATERIALS && (itemId >= 92908) && (itemId <= 92919))
					return;

				if (ItemData.getInstance().getTemplate(itemId) == null)
				{
					LOGGER.Error("DropListItem: Could not find item with id " + itemId + ".");
				}
				else
				{
					long min = e.Attribute("min").GetInt64();
					long max = e.Attribute("max").GetInt64();
					double chance1 = e.Attribute("chance").GetDouble();
					dropLists.add(new DropHolder(dropType, itemId, min, max, chance1));
				}
			});
		});

		element.Elements("collision").Elements("radius").ForEach(el =>
		{
			set.set("collision_radius", el.Attribute("normal").GetDouble());
			set.set("collisionRadiusGrown", el.Attribute("grown").GetDouble());
		});

		element.Elements("collision").Elements("height").ForEach(el =>
		{
			set.set("collision_height", el.Attribute("normal").GetDouble());
			set.set("collisionHeightGrown", el.Attribute("grown").GetDouble());
		});

		NpcTemplate template = _npcs.get(npcId);
		if (template == null)
		{
			template = new NpcTemplate(set);
			_npcs.put(template.getId(), template);
		}
		else
		{
			template.set(set);
		}

		if (parameters != null)
		{
			// Using unmodifiable map parameters of template are not meant to be changed at runtime.
			template.setParameters(new StatSet(parameters));
		}
		else
		{
			template.setParameters(StatSet.EMPTY_STATSET);
		}

		if (skills != null)
		{
			Map<AISkillScope, List<Skill>> aiSkillLists = null;
			foreach (Skill skill in skills.values())
			{
				if (!skill.isPassive())
				{
					if (aiSkillLists == null)
					{
						aiSkillLists = new();
					}

					List<AISkillScope> aiSkillScopes = new();
					AISkillScope shortOrLongRangeScope =
						skill.getCastRange() <= 150 ? AISkillScope.SHORT_RANGE : AISkillScope.LONG_RANGE;
					if (skill.isSuicideAttack())
					{
						aiSkillScopes.add(AISkillScope.SUICIDE);
					}
					else
					{
						aiSkillScopes.add(AISkillScope.GENERAL);

						if (skill.isContinuous())
						{
							if (!skill.isDebuff())
							{
								aiSkillScopes.add(AISkillScope.BUFF);
							}
							else
							{
								aiSkillScopes.add(AISkillScope.DEBUFF);
								aiSkillScopes.add(AISkillScope.COT);
								aiSkillScopes.add(shortOrLongRangeScope);
							}
						}
						else if (skill.hasEffectType(EffectType.DISPEL, EffectType.DISPEL_BY_SLOT))
						{
							aiSkillScopes.add(AISkillScope.NEGATIVE);
							aiSkillScopes.add(shortOrLongRangeScope);
						}
						else if (skill.hasEffectType(EffectType.HEAL))
						{
							aiSkillScopes.add(AISkillScope.HEAL);
						}
						else if (skill.hasEffectType(EffectType.PHYSICAL_ATTACK, EffectType.PHYSICAL_ATTACK_HP_LINK,
							         EffectType.MAGICAL_ATTACK, EffectType.DEATH_LINK, EffectType.HP_DRAIN))
						{
							aiSkillScopes.add(AISkillScope.ATTACK);
							aiSkillScopes.add(AISkillScope.UNIVERSAL);
							aiSkillScopes.add(shortOrLongRangeScope);
						}
						else if (skill.hasEffectType(EffectType.SLEEP))
						{
							aiSkillScopes.add(AISkillScope.IMMOBILIZE);
						}
						else if (skill.hasEffectType(EffectType.BLOCK_ACTIONS, EffectType.ROOT))
						{
							aiSkillScopes.add(AISkillScope.IMMOBILIZE);
							aiSkillScopes.add(shortOrLongRangeScope);
						}
						else if (skill.hasEffectType(EffectType.MUTE, EffectType.BLOCK_CONTROL))
						{
							aiSkillScopes.add(AISkillScope.COT);
							aiSkillScopes.add(shortOrLongRangeScope);
						}
						else if (skill.hasEffectType(EffectType.DMG_OVER_TIME, EffectType.DMG_OVER_TIME_PERCENT))
						{
							aiSkillScopes.add(shortOrLongRangeScope);
						}
						else if (skill.hasEffectType(EffectType.RESURRECTION))
						{
							aiSkillScopes.add(AISkillScope.RES);
						}
						else
						{
							aiSkillScopes.add(AISkillScope.UNIVERSAL);
						}
					}

					foreach (AISkillScope aiSkillScope in aiSkillScopes)
					{
						List<Skill> aiSkills = aiSkillLists.get(aiSkillScope);
						if (aiSkills == null)
						{
							aiSkills = new();
							aiSkillLists.put(aiSkillScope, aiSkills);
						}

						aiSkills.add(skill);
					}
				}
			}

			template.setSkills(skills);
			template.setAISkillLists(aiSkillLists);
		}
		else
		{
			template.setSkills(null);
			template.setAISkillLists(null);
		}

		template.setClans(clans);
		template.setIgnoreClanNpcIds(ignoreClanNpcIds);

		// Clean old drop groups.
		template.removeDropGroups();

		// Set new drop groups.
		if (dropGroups != null)
		{
			template.setDropGroups(dropGroups);
		}

		// Clean old drop lists.
		template.removeDrops();

		// Add configurable item drop for bosses.
		if ((Config.BOSS_DROP_ENABLED) && (type.contains("RaidBoss") && (level >= Config.BOSS_DROP_MIN_LEVEL) &&
		                                   (level <= Config.BOSS_DROP_MAX_LEVEL)))
		{
			if (dropLists == null)
			{
				dropLists = new();
			}

			dropLists.AddRange(Config.BOSS_DROP_LIST);
		}

		// Add configurable LCoin drop for monsters.
		if ((Config.LCOIN_DROP_ENABLED) && (type.contains("Monster") && !type.contains("boss")) &&
		    (level >= Config.LCOIN_MIN_MOB_LEVEL))
		{
			if (dropLists == null)
			{
				dropLists = new();
			}

			dropLists.add(new DropHolder(DropType.DROP, Inventory.LCOIN_ID, Config.LCOIN_MIN_QUANTITY,
				Config.LCOIN_MAX_QUANTITY, Config.LCOIN_DROP_CHANCE));
		}

		// Set new drop lists.
		if (dropLists != null)
		{
			// Drops are sorted by chance (high to low).
			dropLists.Sort((d1, d2) => d2.getChance().CompareTo(d1.getChance()));
			foreach (DropHolder dropHolder in dropLists)
			{
				switch (dropHolder.getDropType())
				{
					case DropType.DROP:
					case DropType.LUCKY: // Lucky drops are added to normal drops and calculated later.
					{
						template.addDrop(dropHolder);
						break;
					}
					case DropType.SPOIL:
					{
						template.addSpoil(dropHolder);
						break;
					}
					case DropType.FORTUNE:
					{
						template.addFortune(dropHolder);
						break;
					}
				}
			}
		}

		if (!template.getParameters().getMinionList("Privates").isEmpty() &&
		    (template.getParameters().getSet().get("SummonPrivateRate") == null))
		{
			_masterMonsterIDs.add(template.getId());
		}
	}

	/**
	 * Gets or creates a clan id if it does not exist.
	 * @param clanName the clan name to get or create its id
	 * @return the clan id for the given clan name
	 */
	private int getOrCreateClanId(String clanName)
	{
		int id = _clans.get(clanName);
		if (id == null)
		{
			id = _clans.size();
			_clans.put(clanName, id);
		}
		return id;
	}
	
	/**
	 * Gets the clan id
	 * @param clanName the clan name to get its id
	 * @return the clan id for the given clan name if it exists, -1 otherwise
	 */
	public int getClanId(String clanName)
	{
		int id = _clans.get(clanName);
		return id != null ? id : -1;
	}
	
	public Set<String> getClansByIds(Set<int> clanIds)
	{
		Set<String> result = new();
		if (clanIds == null)
		{
			return result;
		}
		foreach (var record in _clans)
		{
			foreach (int id in clanIds)
			{
				if (record.Value == id)
				{
					result.add(record.Key);
				}
			}
		}
		return result;
	}
	
	/**
	 * Gets the template.
	 * @param id the template Id to get.
	 * @return the template for the given id.
	 */
	public NpcTemplate getTemplate(int id)
	{
		return _npcs.get(id);
	}
	
	/**
	 * Gets the template by name.
	 * @param name of the template to get.
	 * @return the template for the given name.
	 */
	public NpcTemplate getTemplateByName(String name)
	{
		foreach (NpcTemplate npcTemplate in _npcs.values())
		{
			if (npcTemplate.getName().equalsIgnoreCase(name))
			{
				return npcTemplate;
			}
		}
		return null;
	}
	
	/**
	 * Gets all templates matching the filter.
	 * @param filter
	 * @return the template list for the given filter
	 */
	public List<NpcTemplate> getTemplates(Predicate<NpcTemplate> filter)
	{
		List<NpcTemplate> result = new();
		foreach (NpcTemplate npcTemplate in _npcs.values())
		{
			if (filter(npcTemplate))
			{
				result.add(npcTemplate);
			}
		}
		return result;
	}
	
	/**
	 * Gets the all of level.
	 * @param levels of all the templates to get.
	 * @return the template list for the given level.
	 */
	public List<NpcTemplate> getAllOfLevel(params int[] levels)
	{
		return getTemplates(template => levels.Contains(template.getLevel()));
	}
	
	/**
	 * Gets the all monsters of level.
	 * @param levels of all the monster templates to get.
	 * @return the template list for the given level.
	 */
	public List<NpcTemplate> getAllMonstersOfLevel(params int[] levels)
	{
		return getTemplates(template => levels.Contains(template.getLevel()) && template.isType("Monster"));
	}
	
	/**
	 * Gets the all npc starting with.
	 * @param text of all the NPC templates which its name start with.
	 * @return the template list for the given letter.
	 */
	public List<NpcTemplate> getAllNpcStartingWith(String text)
	{
		return getTemplates(template => template.isType("Folk") && template.getName().startsWith(text));
	}
	
	/**
	 * Gets the all npc of class type.
	 * @param classTypes of all the templates to get.
	 * @return the template list for the given class type.
	 */
	public List<NpcTemplate> getAllNpcOfClassType(params String[] classTypes)
	{
		return getTemplates(
			template => classTypes.Contains(template.getType(), StringComparer.CurrentCultureIgnoreCase));
	}
	
	/**
	 * @return the IDs of monsters that have minions.
	 */
	public static ICollection<int> getMasterMonsterIDs()
	{
		return _masterMonsterIDs;
	}
	
	/**
	 * Gets the single instance of NpcData.
	 * @return single instance of NpcData
	 */
	public static NpcData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly NpcData INSTANCE = new();
	}
	
	private static Map<String, Object> parseParameters(XElement element)
	{
		Map<String, Object> parameters = new();
		
		element.Elements("param").ForEach(el =>
		{
			string name = el.Attribute("name").GetString();
			string value = el.Attribute("value").GetString();
			parameters.put(name, value);
		});
		
		element.Elements("skill").ForEach(el =>
		{
			string name = el.Attribute("name").GetString();
			int id = el.Attribute("id").GetInt32();
			int level = el.Attribute("level").GetInt32();
			parameters.put(name, new SkillHolder(id, level));
		});
		
		element.Elements("location").ForEach(el =>
		{
			string name = el.Attribute("name").GetString();
			int x = el.Attribute("x").GetInt32();
			int y = el.Attribute("y").GetInt32();
			int z = el.Attribute("z").GetInt32();
			int heading = el.Attribute("heading").GetInt32(0);
			parameters.put(name, new Location(x, y, z, heading));
		});
		
		element.Elements("minions").ForEach(el =>
		{
			List<MinionHolder> minions = new();
			el.Elements("npc").ForEach(e =>
			{
				int id = el.Attribute("id").GetInt32();
				int count = el.Attribute("count").GetInt32();
				int max = el.Attribute("max").GetInt32(0);
				int respawnTime = el.Attribute("respawnTime").GetInt32();
				int weightPoint = el.Attribute("weightPoint").GetInt32(0);
				minions.add(new MinionHolder(id, count, max, TimeSpan.FromMilliseconds(respawnTime), weightPoint));
			});
					
			if (!minions.isEmpty())
				parameters.put(el.Attribute("name").GetString(), minions);
		});

		return parameters;
	}
}