using System.Runtime.CompilerServices;
using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Model.Enums;
using L2Dn.Utilities;
using NLog;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Data.Xml;

/**
 * NPC data parser.
 * @author NosBit
 */
public class NpcData: DataReaderBase
{
	protected static readonly Logger LOGGER = LogManager.GetLogger(nameof(NpcData));

	private readonly Map<int, NpcTemplate> _npcs = new();
	private readonly Map<string, int> _clans = new();
	private static readonly Set<int> _masterMonsterIDs = new();
	private static int? _genericClanId;

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

		LOGGER.Info(GetType().Name + ": Loaded " + _npcs.Count + " NPCs.");

		if (Config.General.CUSTOM_NPC_DATA)
		{
			int npcCount = _npcs.Count;
			LoadXmlDocuments(DataFileLocation.Data, "stats/npcs/custom").ForEach(t =>
			{
				t.Document.Elements("list").Elements("npc").ForEach(x => loadElement(t.FilePath, x));
			});

			LOGGER.Info(GetType().Name + ": Loaded " + (_npcs.Count - npcCount) + " custom NPCs.");
		}
	}

	private void loadElement(string filePath, XElement element)
	{
		StatSet set = new StatSet();
		int npcId = element.GetAttributeValueAsInt32("id");
		int level = element.Attribute("level").GetInt32(85);
		string type = element.Attribute("type").GetString("Folk");
		Map<string, object>? parameters = null;
		Map<int, Skill> skills = new();
		Set<int>? clans = null;
		Set<int>? ignoreClanNpcIds = null;
		List<DropHolder>? dropLists = null;
		List<DropGroupHolder>? dropGroups = null;
		set.set("id", npcId);

		if (element.Attribute("displayId") != null)
			set.set("displayId", element.GetAttributeValueAsInt32("displayId"));

		set.set("level", level);
		set.set("type", type);
		set.set("name", element.GetAttributeValueAsString("name"));
		set.set("usingServerSideName", element.Attribute("usingServerSideName").GetBoolean(false));
		set.set("title", element.GetAttributeValueAsString("title", string.Empty));
		set.set("usingServerSideTitle", element.Attribute("usingServerSideTitle").GetBoolean(false));
		set.set("elementalType", element.Attribute("element").GetEnum(ElementalType.NONE));

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
			set.set("mpRewardValue", el.GetAttributeValueAsInt32("value"));
			set.set("mpRewardType", el.Attribute("type").GetEnum<MpRewardType>());
			set.set("mpRewardTicks", el.GetAttributeValueAsInt32("ticks"));
			set.set("mpRewardAffectType", el.Attribute("affects").GetEnum<MpRewardAffectType>());
		});

		element.Elements("stats").ForEach(el =>
		{
			set.set("baseSTR", el.GetAttributeValueAsInt32OrNull("str"));
			set.set("baseINT", el.GetAttributeValueAsInt32OrNull("int"));
			set.set("baseDEX", el.GetAttributeValueAsInt32OrNull("dex"));
			set.set("baseWIT", el.GetAttributeValueAsInt32OrNull("wit"));
			set.set("baseCON", el.GetAttributeValueAsInt32OrNull("con"));
			set.set("baseMEN", el.GetAttributeValueAsInt32OrNull("men"));

			el.Elements("vitals").ForEach(e =>
			{
				set.set("baseHpMax", e.GetAttributeValueAsDouble("hp"));
				set.set("baseHpReg", e.GetAttributeValueAsDoubleOrNull("hpRegen"));
				set.set("baseMpMax", e.GetAttributeValueAsDouble("mp"));
				set.set("baseMpReg", e.GetAttributeValueAsDoubleOrNull("mpRegen"));
			});

			el.Elements("attack").ForEach(e =>
			{
				set.set("basePAtk", e.GetAttributeValueAsDouble("physical"));
				set.set("baseMAtk", e.GetAttributeValueAsDouble("magical"));
				set.set("baseRndDam", e.GetAttributeValueAsInt32OrNull("random"));
				set.set("baseCritRate", e.GetAttributeValueAsDoubleOrNull("critical"));
				set.set("accuracy", e.GetAttributeValueAsFloatOrNull("accuracy")); // TODO: Implement me
				set.set("basePAtkSpd", e.GetAttributeValueAsFloat("attackSpeed"));
				set.set("reuseDelay", e.GetAttributeValueAsInt32OrNull("reuseDelay")); // TODO: Implement me
				set.set("baseAtkType", e.GetAttributeValueAsString("type", string.Empty));
				set.set("baseAtkRange", e.GetAttributeValueAsInt32("range"));
				set.set("distance", e.GetAttributeValueAsInt32OrNull("distance")); // TODO: Implement me
				set.set("width", e.GetAttributeValueAsInt32OrNull("width")); // TODO: Implement me
			});

			el.Elements("defence").ForEach(e =>
			{
				set.set("basePDef", e.GetAttributeValueAsDouble("physical"));
				set.set("baseMDef", e.GetAttributeValueAsDouble("magical"));
				set.set("evasion", e.GetAttributeValueAsInt32OrNull("evasion")); // TODO: Implement me
				set.set("baseShldDef", e.GetAttributeValueAsInt32OrNull("shield"));
				set.set("baseShldRate", e.GetAttributeValueAsInt32OrNull("shieldRate"));
			});

			el.Elements("abnormalresist").ForEach(e =>
			{
				set.set("physicalAbnormalResist", e.GetAttributeValueAsDouble("physical"));
				set.set("magicAbnormalResist", e.GetAttributeValueAsDouble("magic"));
			});

			el.Elements("attribute").Elements("attack").ForEach(e =>
			{
				string attackAttributeType = e.GetAttributeValueAsString("type");
				int value = e.GetAttributeValueAsInt32("value");
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

			el.Elements("attribute").Elements("defence").ForEach(e =>
			{
				set.set("baseFireRes", e.GetAttributeValueAsInt32("fire"));
				set.set("baseWaterRes", e.GetAttributeValueAsInt32("water"));
				set.set("baseWindRes", e.GetAttributeValueAsInt32("wind"));
				set.set("baseEarthRes", e.GetAttributeValueAsInt32("earth"));
				set.set("baseHolyRes", e.GetAttributeValueAsInt32("holy"));
				set.set("baseDarkRes", e.GetAttributeValueAsInt32("dark"));
				set.set("baseElementRes", e.GetAttributeValueAsInt32OrNull("default"));
			});

			el.Elements("speed").Elements("walk").ForEach(e =>
			{
				double groundWalk = e.GetAttributeValueAsDouble("ground");
				set.set("baseWalkSpd", groundWalk <= 0d ? 0.1 : groundWalk);
				set.set("baseSwimWalkSpd", e.GetAttributeValueAsDoubleOrNull("swim"));
				set.set("baseFlyWalkSpd", e.GetAttributeValueAsDoubleOrNull("fly"));
			});

			el.Elements("speed").Elements("run").ForEach(e =>
			{
				double runSpeed = e.GetAttributeValueAsDouble("ground");
				set.set("baseRunSpd", runSpeed <= 0d ? 0.1 : runSpeed);
				set.set("baseSwimRunSpd", e.GetAttributeValueAsDoubleOrNull("swim"));
				set.set("baseFlyRunSpd", e.GetAttributeValueAsDoubleOrNull("fly"));
			});

			el.Elements("hittime").ForEach(e =>
			{
				set.set("hitTime", e.Value); // TODO: Implement me default 600 (value in ms)
			});
		});

		element.Elements("status").ForEach(el =>
		{
			set.set("unique", el.GetAttributeValueAsBooleanOrNull("unique"));
			set.set("attackable", el.GetAttributeValueAsBooleanOrNull("attackable"));
			set.set("targetable", el.GetAttributeValueAsBooleanOrNull("targetable"));
			set.set("talkable", el.GetAttributeValueAsBooleanOrNull("talkable"));
			set.set("undying", el.GetAttributeValueAsBooleanOrNull("undying"));
			set.set("showName", el.GetAttributeValueAsBooleanOrNull("showName"));
			set.set("randomWalk", el.GetAttributeValueAsBooleanOrNull("randomWalk"));
			set.set("randomAnimation", el.GetAttributeValueAsBooleanOrNull("randomAnimation"));
			set.set("flying", el.GetAttributeValueAsBooleanOrNull("flying"));
			set.set("canMove", el.GetAttributeValueAsBooleanOrNull("canMove"));
			set.set("noSleepMode", el.GetAttributeValueAsBooleanOrNull("noSleepMode"));
			set.set("passableDoor", el.GetAttributeValueAsBooleanOrNull("passableDoor"));
			set.set("hasSummoner", el.GetAttributeValueAsBooleanOrNull("hasSummoner"));
			set.set("canBeSown", el.GetAttributeValueAsBooleanOrNull("canBeSown"));
			set.set("isDeathPenalty", el.GetAttributeValueAsBooleanOrNull("isDeathPenalty"));
			set.set("fakePlayer", el.GetAttributeValueAsBooleanOrNull("fakePlayer"));
			set.set("fakePlayerTalkable", el.GetAttributeValueAsBooleanOrNull("fakePlayerTalkable"));
		});

		element.Elements("skilllist").Elements("skill").ForEach(el =>
		{
			int skillId = el.GetAttributeValueAsInt32("id");
			int skillLevel = el.GetAttributeValueAsInt32("level");
			Skill? skill = SkillData.getInstance().getSkill(skillId, skillLevel);
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
			set.set("soulShot", el.GetAttributeValueAsInt32OrNull("soul"));
			set.set("spiritShot", el.GetAttributeValueAsInt32OrNull("spirit"));
			set.set("shotShotChance", el.GetAttributeValueAsInt32OrNull("shotChance"));
			set.set("spiritShotChance", el.GetAttributeValueAsInt32OrNull("spiritChance"));
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
			set.set("aiType", el.GetAttributeValueAsString("type", string.Empty));
			set.set("aggroRange", el.GetAttributeValueAsInt32OrNull("aggroRange"));
			set.set("clanHelpRange", el.GetAttributeValueAsInt32OrNull("clanHelpRange"));
			set.set("isChaos", el.GetAttributeValueAsBooleanOrNull("isChaos"));
			set.set("isAggressive", el.GetAttributeValueAsBooleanOrNull("isAggressive"));

			el.Elements("skill").ForEach(e =>
			{
				set.set("minSkillChance", e.GetAttributeValueAsInt32OrNull("minChance"));
				set.set("maxSkillChance", e.GetAttributeValueAsInt32OrNull("maxChance"));
				set.set("primarySkillId", e.GetAttributeValueAsInt32OrNull("primaryId"));
				set.set("shortRangeSkillId", e.GetAttributeValueAsInt32OrNull("shortRangeId"));
				set.set("shortRangeSkillChance", e.GetAttributeValueAsInt32OrNull("shortRangeChance"));
				set.set("longRangeSkillId", e.GetAttributeValueAsInt32OrNull("longRangeId"));
				set.set("longRangeSkillChance", e.GetAttributeValueAsInt32OrNull("longRangeChance"));
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

		element.Elements("dropLists").Elements().ForEach(el =>
		{
			DropType dropType = Enum.Parse<DropType>(el.Name.LocalName, true);
			el.Elements("group").ForEach(e =>
			{
				if (dropGroups == null)
				{
					dropGroups = new();
				}

				double chance = e.GetAttributeValueAsDouble("chance");
				DropGroupHolder group = new DropGroupHolder(chance);

				e.Elements("item").ForEach(itemEl =>
				{
					int itemId = itemEl.GetAttributeValueAsInt32("id");

					// Drop materials for random craft configuration.
					if (!Config.RandomCraft.DROP_RANDOM_CRAFT_MATERIALS && itemId >= 92908 && itemId <= 92919)
						return;

					if (ItemData.getInstance().getTemplate(itemId) == null)
					{
						LOGGER.Error("DropListItem: Could not find item with id " + itemId + ".");
					}
					else
					{
						long min = itemEl.GetAttributeValueAsInt64("min");
						long max = itemEl.GetAttributeValueAsInt64("max");
						double chance1 = itemEl.GetAttributeValueAsDouble("chance");
						group.addDrop(new DropHolder(dropType, itemId, min, max, chance1));
					}
				});

				dropGroups.Add(group);
			});

			el.Elements("item").ForEach(e =>
			{
				if (dropLists == null)
				{
					dropLists = new();
				}

				int itemId = e.GetAttributeValueAsInt32("id");

				// Drop materials for random craft configuration.
				if (!Config.RandomCraft.DROP_RANDOM_CRAFT_MATERIALS && itemId >= 92908 && itemId <= 92919)
					return;

				if (ItemData.getInstance().getTemplate(itemId) == null)
				{
					LOGGER.Error("DropListItem: Could not find item with id " + itemId + ".");
				}
				else
				{
					long min = e.GetAttributeValueAsInt64("min");
					long max = e.GetAttributeValueAsInt64("max");
					double chance1 = e.GetAttributeValueAsDouble("chance");
					dropLists.Add(new DropHolder(dropType, itemId, min, max, chance1));
				}
			});
		});

		element.Elements("collision").Elements("radius").ForEach(el =>
		{
			set.set("collision_radius", el.GetAttributeValueAsDouble("normal"));
			set.set("collisionRadiusGrown", el.GetAttributeValueAsDoubleOrNull("grown"));
		});

		element.Elements("collision").Elements("height").ForEach(el =>
		{
			set.set("collision_height", el.GetAttributeValueAsDouble("normal"));
			set.set("collisionHeightGrown", el.GetAttributeValueAsDoubleOrNull("grown"));
		});

		NpcTemplate? template = _npcs.get(npcId);
		if (template == null)
		{
			template = new NpcTemplate(set);
			_npcs.put(template.getId(), template);
		}
		else
		{
			throw new InvalidOperationException("Npc template already exist with id: " + template.getId());
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
			Map<AISkillScope, List<Skill>>? aiSkillLists = null;
			foreach (Skill skill in skills.Values)
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
						aiSkillScopes.Add(AISkillScope.SUICIDE);
					}
					else
					{
						aiSkillScopes.Add(AISkillScope.GENERAL);

						if (skill.isContinuous())
						{
							if (!skill.isDebuff())
							{
								aiSkillScopes.Add(AISkillScope.BUFF);
							}
							else
							{
								aiSkillScopes.Add(AISkillScope.DEBUFF);
								aiSkillScopes.Add(AISkillScope.COT);
								aiSkillScopes.Add(shortOrLongRangeScope);
							}
						}
						else if (skill.hasEffectType(EffectType.DISPEL, EffectType.DISPEL_BY_SLOT))
						{
							aiSkillScopes.Add(AISkillScope.NEGATIVE);
							aiSkillScopes.Add(shortOrLongRangeScope);
						}
						else if (skill.hasEffectType(EffectType.HEAL))
						{
							aiSkillScopes.Add(AISkillScope.HEAL);
						}
						else if (skill.hasEffectType(EffectType.PHYSICAL_ATTACK, EffectType.PHYSICAL_ATTACK_HP_LINK,
							         EffectType.MAGICAL_ATTACK, EffectType.DEATH_LINK, EffectType.HP_DRAIN))
						{
							aiSkillScopes.Add(AISkillScope.ATTACK);
							aiSkillScopes.Add(AISkillScope.UNIVERSAL);
							aiSkillScopes.Add(shortOrLongRangeScope);
						}
						else if (skill.hasEffectType(EffectType.SLEEP))
						{
							aiSkillScopes.Add(AISkillScope.IMMOBILIZE);
						}
						else if (skill.hasEffectType(EffectType.BLOCK_ACTIONS, EffectType.ROOT))
						{
							aiSkillScopes.Add(AISkillScope.IMMOBILIZE);
							aiSkillScopes.Add(shortOrLongRangeScope);
						}
						else if (skill.hasEffectType(EffectType.MUTE, EffectType.BLOCK_CONTROL))
						{
							aiSkillScopes.Add(AISkillScope.COT);
							aiSkillScopes.Add(shortOrLongRangeScope);
						}
						else if (skill.hasEffectType(EffectType.DMG_OVER_TIME, EffectType.DMG_OVER_TIME_PERCENT))
						{
							aiSkillScopes.Add(shortOrLongRangeScope);
						}
						else if (skill.hasEffectType(EffectType.RESURRECTION))
						{
							aiSkillScopes.Add(AISkillScope.RES);
						}
						else
						{
							aiSkillScopes.Add(AISkillScope.UNIVERSAL);
						}
					}

					foreach (AISkillScope aiSkillScope in aiSkillScopes)
					{
						List<Skill>? aiSkills = aiSkillLists.get(aiSkillScope);
						if (aiSkills == null)
						{
							aiSkills = new();
							aiSkillLists.put(aiSkillScope, aiSkills);
						}

						aiSkills.Add(skill);
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
		if (Config.Rates.BOSS_DROP_ENABLED && type.contains("RaidBoss") && level >= Config.Rates.BOSS_DROP_MIN_LEVEL &&
            level <= Config.Rates.BOSS_DROP_MAX_LEVEL)
		{
			if (dropLists == null)
			{
				dropLists = new();
			}

			dropLists.AddRange(Config.Rates.BOSS_DROP_LIST);
		}

		// Add configurable LCoin drop for monsters.
		if (Config.Rates.LCOIN_DROP_ENABLED && type.contains("Monster") && !type.contains("boss") &&
            level >= Config.Rates.LCOIN_MIN_MOB_LEVEL)
		{
			if (dropLists == null)
			{
				dropLists = new();
			}

			dropLists.Add(new DropHolder(DropType.DROP, Inventory.LCOIN_ID, Config.Rates.LCOIN_MIN_QUANTITY,
				Config.Rates.LCOIN_MAX_QUANTITY, Config.Rates.LCOIN_DROP_CHANCE));
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

		if (template.getParameters().getMinionList("Privates").Count != 0 &&
		    template.getParameters().getSet().get("SummonPrivateRate") == null)
		{
			_masterMonsterIDs.add(template.getId());
		}
	}

	/**
	 * Gets or creates a clan id if it does not exist.
	 * @param clanName the clan name to get or create its id
	 * @return the clan id for the given clan name
	 */
	private int getOrCreateClanId(string clanName)
    {
        if (!_clans.TryGetValue(clanName, out int id))
        {
            id = _clans.Count;
            _clans[clanName] = id;
        }

		return id;
	}

	/**
	 * Gets the clan id
	 * @param clanName the clan name to get its id
	 * @return the clan id for the given clan name if it exists, -1 otherwise
	 */
	public int getClanId(string clanName)
    {
        return _clans.GetValueOrDefault(clanName, -1);
    }

	public Set<string> getClansByIds(Set<int> clanIds)
	{
		Set<string> result = new();
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
	public NpcTemplate? getTemplate(int id)
	{
		NpcTemplate? template = _npcs.GetValueOrDefault(id);
        if(template is null)
            LOGGER.Warn($"NPC template id={id} requested but not found.");

        return template;
    }

	/**
	 * Gets the template by name.
	 * @param name of the template to get.
	 * @return the template for the given name.
	 */
	public NpcTemplate? getTemplateByName(string name)
	{
		foreach (NpcTemplate npcTemplate in _npcs.Values)
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
		foreach (NpcTemplate npcTemplate in _npcs.Values)
		{
			if (filter(npcTemplate))
			{
				result.Add(npcTemplate);
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
	public List<NpcTemplate> getAllNpcStartingWith(string text)
	{
		return getTemplates(template => template.isType("Folk") && template.getName().startsWith(text));
	}

	/**
	 * Gets the all npc of class type.
	 * @param classTypes of all the templates to get.
	 * @return the template list for the given class type.
	 */
	public List<NpcTemplate> getAllNpcOfClassType(params string[] classTypes)
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

	private new static Map<string, object> parseParameters(XElement element)
	{
		Map<string, object> parameters = new();

		element.Elements("param").ForEach(el =>
		{
			string name = el.GetAttributeValueAsString("name");
			string value = el.GetAttributeValueAsString("value");
			parameters.put(name, value);
		});

		element.Elements("skill").ForEach(el =>
		{
			string name = el.GetAttributeValueAsString("name");
			int id = el.GetAttributeValueAsInt32("id");
			int level = el.GetAttributeValueAsInt32("level");
			parameters.put(name, new SkillHolder(id, level));
		});

		element.Elements("location").ForEach(el =>
		{
			string name = el.GetAttributeValueAsString("name");
			int x = el.GetAttributeValueAsInt32("x");
			int y = el.GetAttributeValueAsInt32("y");
			int z = el.GetAttributeValueAsInt32("z");
			int heading = el.Attribute("heading").GetInt32(0);
			parameters.put(name, new Location(x, y, z, heading));
		});

		element.Elements("minions").ForEach(el =>
		{
			List<MinionHolder> minions = new();
			el.Elements("npc").ForEach(e =>
			{
				int id = e.GetAttributeValueAsInt32("id");
				int count = e.GetAttributeValueAsInt32("count");
				int max = e.Attribute("max").GetInt32(0);
				int respawnTime = e.GetAttributeValueAsInt32("respawnTime");
				int weightPoint = e.Attribute("weightPoint").GetInt32(0);
				minions.Add(new MinionHolder(id, count, max, TimeSpan.FromMilliseconds(respawnTime), weightPoint));
			});

			if (minions.Count != 0)
				parameters.put(el.GetAttributeValueAsString("name"), minions);
		});

		return parameters;
	}

	public int getGenericClanId()
	{
		if (_genericClanId != null)
		{
			return _genericClanId.Value;
		}

		lock (this)
		{
			_genericClanId = _clans.get("ALL");

			if (_genericClanId == null)
			{
				_genericClanId = -1;
			}
		}

		return _genericClanId.Value;
	}
}