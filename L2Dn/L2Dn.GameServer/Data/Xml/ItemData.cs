using System.Text;
using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Conditions;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Items;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Model.Stats.Functions;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Data.Xml;

/**
 * This class serves as a container for all item templates in the game.
 */
public class ItemData: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ItemData));
	
	private ItemTemplate[] _allTemplates;
	private readonly Map<int, EtcItem> _etcItems = new();
	private readonly Map<int, Armor> _armors = new();
	private readonly Map<int, Weapon> _weapons = new();
	
	public static readonly Map<string, long> SLOTS = new();
	
	static ItemData()
	{
		SLOTS.put("shirt", ItemTemplate.SLOT_UNDERWEAR);
		SLOTS.put("lbracelet", ItemTemplate.SLOT_L_BRACELET);
		SLOTS.put("rbracelet", ItemTemplate.SLOT_R_BRACELET);
		SLOTS.put("talisman", ItemTemplate.SLOT_DECO);
		SLOTS.put("chest", ItemTemplate.SLOT_CHEST);
		SLOTS.put("fullarmor", ItemTemplate.SLOT_FULL_ARMOR);
		SLOTS.put("head", ItemTemplate.SLOT_HEAD);
		SLOTS.put("hair", ItemTemplate.SLOT_HAIR);
		SLOTS.put("hairall", ItemTemplate.SLOT_HAIRALL);
		SLOTS.put("underwear", ItemTemplate.SLOT_UNDERWEAR);
		SLOTS.put("back", ItemTemplate.SLOT_BACK);
		SLOTS.put("neck", ItemTemplate.SLOT_NECK);
		SLOTS.put("legs", ItemTemplate.SLOT_LEGS);
		SLOTS.put("feet", ItemTemplate.SLOT_FEET);
		SLOTS.put("gloves", ItemTemplate.SLOT_GLOVES);
		SLOTS.put("chest,legs", ItemTemplate.SLOT_CHEST | ItemTemplate.SLOT_LEGS);
		SLOTS.put("belt", ItemTemplate.SLOT_BELT);
		SLOTS.put("rhand", ItemTemplate.SLOT_R_HAND);
		SLOTS.put("lhand", ItemTemplate.SLOT_L_HAND);
		SLOTS.put("lrhand", ItemTemplate.SLOT_LR_HAND);
		SLOTS.put("rear;lear", ItemTemplate.SLOT_R_EAR | ItemTemplate.SLOT_L_EAR);
		SLOTS.put("rfinger;lfinger", ItemTemplate.SLOT_R_FINGER | ItemTemplate.SLOT_L_FINGER);
		SLOTS.put("wolf", ItemTemplate.SLOT_WOLF);
		SLOTS.put("greatwolf", ItemTemplate.SLOT_GREATWOLF);
		SLOTS.put("hatchling", ItemTemplate.SLOT_HATCHLING);
		SLOTS.put("strider", ItemTemplate.SLOT_STRIDER);
		SLOTS.put("babypet", ItemTemplate.SLOT_BABYPET);
		SLOTS.put("brooch", ItemTemplate.SLOT_BROOCH);
		SLOTS.put("brooch_jewel", ItemTemplate.SLOT_BROOCH_JEWEL);
		SLOTS.put("agathion", ItemTemplate.SLOT_AGATHION);
		SLOTS.put("artifactbook", ItemTemplate.SLOT_ARTIFACT_BOOK);
		SLOTS.put("artifact", ItemTemplate.SLOT_ARTIFACT);
		SLOTS.put("none", ItemTemplate.SLOT_NONE);
		
		// retail compatibility
		SLOTS.put("onepiece", ItemTemplate.SLOT_FULL_ARMOR);
		SLOTS.put("hair2", ItemTemplate.SLOT_HAIR2);
		SLOTS.put("dhair", ItemTemplate.SLOT_HAIRALL);
		SLOTS.put("alldress", ItemTemplate.SLOT_ALLDRESS);
		SLOTS.put("deco1", ItemTemplate.SLOT_DECO);
		SLOTS.put("waist", ItemTemplate.SLOT_BELT);
	}
	
	protected ItemData()
	{
		load();
	}
	
	private void load()
	{
		_armors.clear();
		_etcItems.clear();
		_weapons.clear();

		LoadXmlDocuments(DataFileLocation.Data, "stats/items", true).ForEach(t =>
		{
			t.Document.Elements("list").Elements("item").ForEach(x => loadElement(t.FilePath, x));
		});

		if (Config.CUSTOM_ITEMS_LOAD)
		{
			LoadXmlDocuments(DataFileLocation.Data, "stats/items/custom", true).ForEach(t =>
			{
				t.Document.Elements("list").Elements("item").ForEach(x => loadElement(t.FilePath, x));
			});
		}

		buildFastLookupTable();
		LOGGER.Info(GetType().Name + ": Loaded " + _etcItems.size() + " Etc Items");
		LOGGER.Info(GetType().Name + ": Loaded " + _armors.size() + " Armor Items");
		LOGGER.Info(GetType().Name + ": Loaded " + _weapons.size() + " Weapon Items");
		LOGGER.Info(GetType().Name + ": Loaded " + (_etcItems.size() + _armors.size() + _weapons.size()) +
		            " Items in total.");
	}

	private void loadElement(string fileName, XElement element)
	{
		int id = element.Attribute("id").GetInt32();
		string name = element.Attribute("name").GetString();
		string className = element.Attribute("type").GetString();
		string additionalName = element.Attribute("additionalName").GetString(null);

		StatSet set = new();
		set.set("item_id", id);
		set.set("name", name);
		set.set("additionalName", additionalName);

		ItemTemplate? item = null;
		element.Elements().ForEach(el =>
		{
			switch (el.Name.LocalName)
			{
				case "table":
				{
					if (item is not null)
						throw new InvalidOperationException("Item created but table node found! Item " + id);
					throw new NotImplementedException();
				}

				case "set":
				{
					if (item is not null)
						throw new InvalidOperationException("Item created but set node found! Item " + id);

					string setName = el.Attribute("name").GetString().Trim();
					string value = el.Attribute("val").GetString().Trim();
					char ch = string.IsNullOrEmpty(value) ? ' ' : value[0];
					if ((ch == '#') || (ch == '-') || char.IsDigit(ch))
						throw new NotImplementedException(); // set.set(setName, getValue(value, 1)));
					else
						set.set(setName, value);
					break;
				}

				case "stats":
				{
					ItemTemplate currentItem = MakeItem(className, set, ref item);
					el.Elements("stat").ForEach(e =>
					{
						Stat type = e.Attribute("type").GetEnum<Stat>();
						double val = (double)e;
						currentItem.addFunctionTemplate(new FuncTemplate(null, null, "add", 0x00, type, val));
					});
					break;
				}

				case "skills":
				{
					ItemTemplate currentItem = MakeItem(className, set, ref item);
					el.Elements("skill").ForEach(e =>
					{
						int skillId = e.Attribute("id").GetInt32();
						int level = e.Attribute("level").GetInt32();
						ItemSkillType type = e.Attribute("type").GetEnum(ItemSkillType.NORMAL);
						int chance = e.Attribute("type_chance").GetInt32(100);
						int value = e.Attribute("type_value").GetInt32(0);
						currentItem.addSkill(new ItemSkillHolder(skillId, level, type, chance, value));
					});
					break;
				}

				case "capsuled_items":
				{
					ItemTemplate currentItem = MakeItem(className, set, ref item);
					el.Elements("item").ForEach(e =>
					{
						int itemId = e.Attribute("id").GetInt32();
						long min = e.Attribute("min").GetInt64();
						long max = e.Attribute("max").GetInt64();
						double chance = e.Attribute("chance").GetDouble();
						int minEnchant = e.Attribute("minEnchant").GetInt32(0);
						int maxEnchant = e.Attribute("maxEnchant").GetInt32(0);
						currentItem.addCapsuledItem(new ExtractableProduct(itemId, min, max, chance, minEnchant, maxEnchant));
					});
					break;
				}

				case "cond":
				{
					ItemTemplate currentItem = MakeItem(className, set, ref item);
					XElement conditionEl = el.Elements().Single();
					Condition condition = parseCondition(conditionEl, currentItem);
					string? msg = conditionEl.Attribute("msg")?.Value;
					string? msgId = conditionEl.Attribute("msgId")?.Value;
					if (condition != null && msg != null)
						condition.setMessage(msg);
					else if (condition != null && msgId != null)
					{
						condition.setMessageId((SystemMessageId)int.Parse(msgId));
						string? addName = conditionEl.Attribute("addName").GetString();
						if ((addName != null) && (int.Parse(msgId) > 0))
							condition.addName();
					}
					break;
				}

				default:
					throw new InvalidOperationException($"Unknown tag {el.Name.LocalName} in file {fileName}");
			}
		});
		
		MakeItem(className, set, ref item);
		
		switch (item)
		{
			case EtcItem etcItem:
			{
				_etcItems.put(etcItem.getId(), etcItem);

				if ((item.getItemType() == EtcItemType.ARROW) || (item.getItemType() == EtcItemType.BOLT) ||
				    (item.getItemType() == EtcItemType.ELEMENTAL_ORB))
				{
					List<ItemSkillHolder> skills = item.getAllSkills();
					if (skills != null)
					{
						AmmunitionSkillList.add(skills);
					}
				}

				break;
			}
			case Armor armor:
				_armors.put(armor.getId(), armor);
				break;
			case Weapon weapon:
				_weapons.put(item.getId(), weapon);
				break;
			default:
				throw new InvalidOperationException("Invalid item type");
		}
	}

	private static Condition parseCondition(XElement element, ItemTemplate template)
	{
		string elementName = element.Name.LocalName;
		return elementName switch
		{
			"and" => parseLogicAnd(element, template),
			"or" => parseLogicOr(element, template),
			"not" => parseLogicNot(element, template),
			"player" => parsePlayerCondition(element, template),
			"target" => parseTargetCondition(element, template),
			"using" => parseUsingCondition(element),
			"game" => parseGameCondition(element),
			_ => throw new InvalidOperationException($"Invalid condition node: {elementName}")
		};
	}
	private static Condition parseLogicAnd(XElement element, ItemTemplate template)
	{
		ConditionLogicAnd cond = new ConditionLogicAnd();
		element.Elements().ForEach(e => cond.add(parseCondition(e, template)));
		if ((cond.conditions == null) || (cond.conditions.Length == 0))
			LOGGER.Error($"Empty <and> condition in item {template.getId()}");
		
		return cond;
	}
	
	private static Condition parseLogicOr(XElement element, ItemTemplate template)
	{
		ConditionLogicOr cond = new ConditionLogicOr();
		element.Elements().ForEach(e => cond.add(parseCondition(e, template)));
		if ((cond.conditions == null) || (cond.conditions.Length == 0))
			LOGGER.Error($"Empty <or> condition in item {template.getId()}");
		
		return cond;
	}

	private static Condition parseLogicNot(XElement element, ItemTemplate template)
	{
		XElement inner = element.Elements().Single();
		return new ConditionLogicNot(parseCondition(inner, template));
	}

	private static Condition parsePlayerCondition(XElement element, ItemTemplate template)
	{
		Condition cond = null;
		foreach (XAttribute attribute in element.Attributes())
		{
			switch (attribute.Name.LocalName)
			{
				case "races":
				{
					string[] racesVal = ((string)attribute).Split(",");
					Set<Race> races = new();
					for (int r = 0; r < racesVal.Length; r++)
					{
						if (!string.IsNullOrEmpty(racesVal[r]))
							races.add(Enum.Parse<Race>(racesVal[r]));
					}
					cond = joinAnd(cond, new ConditionPlayerRace(races));
					break;
				}
				case "level":
				{
					int lvl = (int)attribute;
					cond = joinAnd(cond, new ConditionPlayerLevel(lvl));
					break;
				}
				case "levelrange":
				{
					string[] range = ((string)attribute).Split(";");
					if (range.Length == 2)
					{
						int[] lvlRange = new int[2];
						lvlRange[0] = int.Parse(range[0]);
						lvlRange[1] = int.Parse(range[1]);
						cond = joinAnd(cond, new ConditionPlayerLevelRange(lvlRange));
					}
					break;
				}
				case "resting":
				{
					bool val = (bool)attribute;
					cond = joinAnd(cond, new ConditionPlayerState(PlayerState.RESTING, val));
					break;
				}
				case "flying":
				{
					bool val = (bool)attribute;
					cond = joinAnd(cond, new ConditionPlayerState(PlayerState.FLYING, val));
					break;
				}
				case "moving":
				{
					bool val = (bool)attribute;
					cond = joinAnd(cond, new ConditionPlayerState(PlayerState.MOVING, val));
					break;
				}
				case "running":
				{
					bool val = (bool)attribute;
					cond = joinAnd(cond, new ConditionPlayerState(PlayerState.RUNNING, val));
					break;
				}
				case "standing":
				{
					bool val = (bool)attribute;
					cond = joinAnd(cond, new ConditionPlayerState(PlayerState.STANDING, val));
					break;
				}
				case "behind":
				{
					bool val = (bool)attribute;
					cond = joinAnd(cond, new ConditionPlayerState(PlayerState.BEHIND, val));
					break;
				}
				case "front":
				{
					bool val = (bool)attribute;
					cond = joinAnd(cond, new ConditionPlayerState(PlayerState.FRONT, val));
					break;
				}
				case "chaotic":
				{
					bool val = (bool)attribute;
					cond = joinAnd(cond, new ConditionPlayerState(PlayerState.CHAOTIC, val));
					break;
				}
				case "olympiad":
				{
					bool val = (bool)attribute;
					cond = joinAnd(cond, new ConditionPlayerState(PlayerState.OLYMPIAD, val));
					break;
				}
				case "ishero":
				{
					bool val = (bool)attribute;
					cond = joinAnd(cond, new ConditionPlayerIsHero(val));
					break;
				}
				case "ispvpflagged":
				{
					bool val = (bool)attribute;
					cond = joinAnd(cond, new ConditionPlayerIsPvpFlagged(val));
					break;
				}
				case "transformationid":
				{
					int id = (int)attribute;
					cond = joinAnd(cond, new ConditionPlayerTransformationId(id));
					break;
				}
				case "hp":
				{
					int hp = (int)attribute;
					cond = joinAnd(cond, new ConditionPlayerHp(hp));
					break;
				}
				case "mp":
				{
					int mp = (int)attribute;
					cond = joinAnd(cond, new ConditionPlayerMp(mp));
					break;
				}
				case "cp":
				{
					int cp = (int)attribute;
					cond = joinAnd(cond, new ConditionPlayerCp(cp));
					break;
				}
				case "pkcount":
				{
					int expIndex = (int)attribute;
					cond = joinAnd(cond, new ConditionPlayerPkCount(expIndex));
					break;
				}
				case "siegezone":
				{
					int value = (int)attribute;
					cond = joinAnd(cond, new ConditionSiegeZone(value, true));
					break;
				}
				case "siegeside":
				{
					int value = (int)attribute;
					cond = joinAnd(cond, new ConditionPlayerSiegeSide(value));
					break;
				}
				case "charges":
				{
					int value = (int)attribute;
					cond = joinAnd(cond, new ConditionPlayerCharges(value));
					break;
				}
				case "souls":
				{
					// TODO: something wrong here 
					int value = (int)attribute;
					SoulType type = attribute.GetEnum<SoulType>();
					cond = joinAnd(cond, new ConditionPlayerSouls(value, type));
					break;
				}
				case "weight":
				{
					int weight = (int)attribute;
					cond = joinAnd(cond, new ConditionPlayerWeight(weight));
					break;
				}
				case "invsize":
				{
					int size = (int)attribute;
					cond = joinAnd(cond, new ConditionPlayerInvSize(size));
					break;
				}
				case "isclanleader":
				{
					bool val = (bool)attribute;
					cond = joinAnd(cond, new ConditionPlayerIsClanLeader(val));
					break;
				}
				case "pledgeclass":
				{
					int pledgeClass = (int)attribute;
					cond = joinAnd(cond, new ConditionPlayerPledgeClass(pledgeClass));
					break;
				}
				case "clanhall":
				{
					StringTokenizer st = new StringTokenizer((string)attribute, ",");
					List<int> array = new();
					while (st.hasMoreTokens())
					{
						string item = st.nextToken().Trim();
						array.add(int.Parse(item));
					}
					cond = joinAnd(cond, new ConditionPlayerHasClanHall(array));
					break;
				}
				case "fort":
				{
					int fort = (int)attribute;
					cond = joinAnd(cond, new ConditionPlayerHasFort(fort));
					break;
				}
				case "castle":
				{
					int castle = (int)attribute;
					cond = joinAnd(cond, new ConditionPlayerHasCastle(castle));
					break;
				}
				case "sex":
				{
					int sex = (int)attribute;
					cond = joinAnd(cond, new ConditionPlayerSex(sex));
					break;
				}
				case "flymounted":
				{
					bool val = (bool)attribute;
					cond = joinAnd(cond, new ConditionPlayerFlyMounted(val));
					break;
				}
				case "vehiclemounted":
				{
					bool val = (bool)attribute;
					cond = joinAnd(cond, new ConditionPlayerVehicleMounted(val));
					break;
				}
				case "landingzone":
				{
					bool val = (bool)attribute;
					cond = joinAnd(cond, new ConditionPlayerLandingZone(val));
					break;
				}
				case "active_effect_id":
				{
					int effect_id = (int)attribute;
					cond = joinAnd(cond, new ConditionPlayerActiveEffectId(effect_id));
					break;
				}
				case "active_effect_id_lvl":
				{
					string[] val = ((string)attribute).Split(",");
					int effect_id = int.Parse(val[0]);
					int effect_lvl = int.Parse(val[1]);
					cond = joinAnd(cond, new ConditionPlayerActiveEffectId(effect_id, effect_lvl));
					break;
				}
				case "active_skill_id":
				{
					int skill_id = (int)attribute;
					cond = joinAnd(cond, new ConditionPlayerActiveSkillId(skill_id));
					break;
				}
				case "active_skill_id_lvl":
				{
					string[] val = ((string)attribute).Split(",");
					int skill_id = int.Parse(val[0]);
					int skill_lvl = int.Parse(val[1]);
					cond = joinAnd(cond, new ConditionPlayerActiveSkillId(skill_id, skill_lvl));
					break;
				}
				case "class_id_restriction":
				{
					StringTokenizer st = new StringTokenizer((string)attribute, ",");
					Set<int> array = new();
					while (st.hasMoreTokens())
					{
						string item = st.nextToken().Trim();
						array.add(int.Parse(item));
					}
					cond = joinAnd(cond, new ConditionPlayerClassIdRestriction(array));
					break;
				}
				case "subclass":
				{
					bool val = (bool)attribute;
					cond = joinAnd(cond, new ConditionPlayerSubclass(val));
					break;
				}
				case "dualclass":
				{
					bool val = (bool)attribute;
					cond = joinAnd(cond, new ConditionPlayerDualclass(val));
					break;
				}
				case "canswitchsubclass":
				{
					int val = (int)attribute;
					cond = joinAnd(cond, new ConditionPlayerCanSwitchSubclass(val));
					break;
				}
				case "instanceid":
				{
					StringTokenizer st = new StringTokenizer((string)attribute, ",");
					Set<int> set = new();
					while (st.hasMoreTokens())
					{
						string item = st.nextToken().Trim();
						set.add(int.Parse(item));
					}
					cond = joinAnd(cond, new ConditionPlayerInstanceId(set));
					break;
				}
				case "agathionid":
				{
					int agathionId = (int)attribute;
					cond = joinAnd(cond, new ConditionPlayerAgathionId(agathionId));
					break;
				}
				case "cloakstatus":
				{
					bool val = (bool)attribute;
					cond = joinAnd(cond, new ConditionPlayerCloakStatus(val));
					break;
				}
				case "hassummon":
				{
					bool val = (bool)attribute;
					cond = joinAnd(cond, new ConditionPlayerHasSummon(val));
					break;
				}
				case "haspet":
				{
					StringTokenizer st = new StringTokenizer((string)attribute, ",");
					List<int> array = new();
					while (st.hasMoreTokens())
					{
						string item = st.nextToken().Trim();
						array.add(int.Parse(item));
					}
					cond = joinAnd(cond, new ConditionPlayerHasPet(array));
					break;
				}
				case "servitornpcid":
				{
					StringTokenizer st = new StringTokenizer((string)attribute, ",");
					List<int> array = new();
					while (st.hasMoreTokens())
					{
						string item = st.nextToken().Trim();
						array.add(int.Parse(item));
					}
					cond = joinAnd(cond, new ConditionPlayerServitorNpcId(array));
					break;
				}
				case "npcidradius":
				{
					StringTokenizer st = new StringTokenizer((string)attribute, ",");
					if (st.countTokens() == 3)
					{
						string[] ids = st.nextToken().Split(";");
						Set<int> npcIds = new();
						for (int index = 0; index < ids.Length; index++)
							npcIds.add(int.Parse(ids[index]));
						
						int radius = int.Parse(st.nextToken());
						bool val = bool.Parse(st.nextToken());
						cond = joinAnd(cond, new ConditionPlayerRangeFromNpc(npcIds, radius, val));
					}
					break;
				}
				case "summonednpcidradius":
				{
					StringTokenizer st = new StringTokenizer((string)attribute, ",");
					if (st.countTokens() == 3)
					{
						string[] ids = st.nextToken().Split(";");
						Set<int> npcIds = new();
						for (int index = 0; index < ids.Length; index++)
							npcIds.add(int.Parse(ids[index]));
						
						int radius = int.Parse(st.nextToken());
						bool val = bool.Parse(st.nextToken());
						cond = joinAnd(cond, new ConditionPlayerRangeFromSummonedNpc(npcIds, radius, val));
					}
					break;
				}
				case "callpc":
				{
					bool val = (bool)attribute;
					cond = joinAnd(cond, new ConditionPlayerCallPc(val));
					break;
				}
				case "cancreatebase":
				{
					bool val = (bool)attribute;
					cond = joinAnd(cond, new ConditionPlayerCanCreateBase(val));
					break;
				}
				case "canescape":
				{
					bool val = (bool)attribute;
					cond = joinAnd(cond, new ConditionPlayerCanEscape(val));
					break;
				}
				case "canrefuelairship":
				{
					int val = (int)attribute;
					cond = joinAnd(cond, new ConditionPlayerCanRefuelAirship(val));
					break;
				}
				case "canresurrect":
				{
					bool val = (bool)attribute;
					cond = joinAnd(cond, new ConditionPlayerCanResurrect(val));
					break;
				}
				case "cansummonpet":
				{
					bool val = (bool)attribute;
					cond = joinAnd(cond, new ConditionPlayerCanSummonPet(val));
					break;
				}
				case "cansummonservitor":
				{
					bool val = (bool)attribute;
					cond = joinAnd(cond, new ConditionPlayerCanSummonServitor(val));
					break;
				}
				case "hasfreesummonpoints":
				{
					int val = (int)attribute;
					cond = joinAnd(cond, new ConditionPlayerHasFreeSummonPoints(val));
					break;
				}
				case "hasfreeteleportbookmarkslots":
				{
					int val = (int)attribute;
					cond = joinAnd(cond, new ConditionPlayerHasFreeTeleportBookmarkSlots(val));
					break;
				}
				case "cansummonsiegegolem":
				{
					bool val = (bool)attribute;
					cond = joinAnd(cond, new ConditionPlayerCanSummonSiegeGolem(val));
					break;
				}
				case "cansweep":
				{
					bool val = (bool)attribute;
					cond = joinAnd(cond, new ConditionPlayerCanSweep(val));
					break;
				}
				case "cantakecastle":
				{
					bool val = (bool)attribute;
					cond = joinAnd(cond, new ConditionPlayerCanTakeCastle(val));
					break;
				}
				case "cantakefort":
				{
					bool val = (bool)attribute;
					cond = joinAnd(cond, new ConditionPlayerCanTakeFort(val));
					break;
				}
				case "cantransform":
				{
					bool val = (bool)attribute;
					cond = joinAnd(cond, new ConditionPlayerCanTransform(val));
					break;
				}
				case "canuntransform":
				{
					bool val = (bool)attribute;
					cond = joinAnd(cond, new ConditionPlayerCanUntransform(val));
					break;
				}
				case "insidezoneid":
				{
					StringTokenizer st = new StringTokenizer((string)attribute, ",");
					Set<int> set = new();
					while (st.hasMoreTokens())
					{
						string item = st.nextToken().Trim();
						set.add(int.Parse(item));
					}
					cond = joinAnd(cond, new ConditionPlayerInsideZoneId(set));
					break;
				}
				case "checkabnormal":
				{
					string value = (string)attribute;
					if (value.contains(","))
					{
						string[] values = value.Split(",");
						AbnormalType type = Enum.Parse<AbnormalType>(values[0]);
						int val = int.Parse(values[1]);
						cond = joinAnd(cond, new ConditionPlayerCheckAbnormal(type, val));
					}
					else
					{
						AbnormalType type = Enum.Parse<AbnormalType>(value);
						cond = joinAnd(cond, new ConditionPlayerCheckAbnormal(type));
					}
					break;
				}
				case "categorytype":
				{
					string[] values = ((string)attribute).Split(",");
					Set<CategoryType> array = new();
					foreach (string value in values)
						array.add(Enum.Parse<CategoryType>(value));

					cond = joinAnd(cond, new ConditionCategoryType(array));
					break;
				}
				case "immobile":
				{
					bool val = (bool)attribute;
					cond = joinAnd(cond, new ConditionPlayerImmobile(val));
					break;
				}
				case "incombat":
				{
					bool val = (bool)attribute;
					cond = joinAnd(cond, new ConditionPlayerIsInCombat(val));
					break;
				}
				case "isonside":
				{
					CastleSide side = attribute.GetEnum<CastleSide>();
					cond = joinAnd(cond, new ConditionPlayerIsOnSide(side));
					break;
				}
				case "ininstance":
				{
					bool val = (bool)attribute;
					cond = joinAnd(cond, new ConditionPlayerInInstance(val));
					break;
				}
				case "minimumvitalitypoints":
				{
					int count = (int)attribute;
					cond = joinAnd(cond, new ConditionMinimumVitalityPoints(count));
					break;
				}
			}
		}
		
		if (cond == null)
			throw new InvalidOperationException("Unrecognized <player> condition");

		return cond;
	}
	
	private static Condition parseTargetCondition(XElement element, ItemTemplate template)
	{
		Condition cond = null;
		foreach (XAttribute attribute in element.Attributes())
		{
			switch (attribute.Name.LocalName)
			{
				case "aggro":
				{
					bool val = (bool)attribute;
					cond = joinAnd(cond, new ConditionTargetAggro(val));
					break;
				}
				case "siegezone":
				{
					int value = (int)attribute;
					cond = joinAnd(cond, new ConditionSiegeZone(value, false));
					break;
				}
				case "level":
				{
					int lvl = (int)attribute;
					cond = joinAnd(cond, new ConditionTargetLevel(lvl));
					break;
				}
				case "levelrange":
				{
					string[] range = ((string)attribute).Split(";");
					if (range.Length == 2)
					{
						int[] lvlRange = new int[2];
						lvlRange[0] = int.Parse(range[0]);
						lvlRange[1] = int.Parse(range[1]);
						cond = joinAnd(cond, new ConditionTargetLevelRange(lvlRange));
					}
					break;
				}
				case "mypartyexceptme":
				{
					bool val = (bool)attribute;
					cond = joinAnd(cond, new ConditionTargetMyPartyExceptMe(val));
					break;
				}
				case "playable":
				{
					cond = joinAnd(cond, new ConditionTargetPlayable());
					break;
				}
				case "player":
				{
					cond = joinAnd(cond, new ConditionTargetPlayer());
					break;
				}
				case "class_id_restriction":
				{
					StringTokenizer st = new StringTokenizer((string)attribute, ",");
					Set<int> set = new();
					while (st.hasMoreTokens())
					{
						string item = st.nextToken().Trim();
						set.add(int.Parse(item));
					}
					cond = joinAnd(cond, new ConditionTargetClassIdRestriction(set));
					break;
				}
				case "active_effect_id":
				{
					int effect_id = (int)attribute;
					cond = joinAnd(cond, new ConditionTargetActiveEffectId(effect_id));
					break;
				}
				case "active_effect_id_lvl":
				{
					string[] val = ((string)attribute).Split(",");
					int effect_id = int.Parse(val[0]);
					int effect_lvl = int.Parse(val[1]);
					cond = joinAnd(cond, new ConditionTargetActiveEffectId(effect_id, effect_lvl));
					break;
				}
				case "active_skill_id":
				{
					int skill_id = (int)attribute;
					cond = joinAnd(cond, new ConditionTargetActiveSkillId(skill_id));
					break;
				}
				case "active_skill_id_lvl":
				{
					string[] val = ((string)attribute).Split(",");
					int skill_id = int.Parse(val[0]);
					int skill_lvl = int.Parse(val[1]);
					cond = joinAnd(cond, new ConditionTargetActiveSkillId(skill_id, skill_lvl));
					break;
				}
				case "abnormaltype":
				{
					AbnormalType abnormalType = attribute.GetEnum<AbnormalType>();
					cond = joinAnd(cond, new ConditionTargetAbnormalType(abnormalType));
					break;
				}
				case "mindistance":
				{
					int distance = (int)attribute;
					cond = joinAnd(cond, new ConditionMinDistance(distance));
					break;
				}
				case "race":
				{
					Race race = attribute.GetEnum<Race>();
					cond = joinAnd(cond, new ConditionTargetRace(race));
					break;
				}
				case "using":
				{
					int mask = 0;
					StringTokenizer st = new StringTokenizer((string)attribute, ",");
					while (st.hasMoreTokens())
					{
						string item = st.nextToken().Trim();
						foreach (WeaponType wt in Enum.GetValues<WeaponType>())
						{
							if (wt.ToString().equals(item))
							{
								mask |= wt.GetMask();
								break;
							}
						}
						foreach (ArmorType at in Enum.GetValues<ArmorType>())
						{
							if (at.ToString().equals(item))
							{
								mask |= at.GetMask();
								break;
							}
						}
					}
					cond = joinAnd(cond, new ConditionTargetUsesWeaponKind(mask));
					break;
				}
				case "npcid":
				{
					StringTokenizer st = new StringTokenizer((string)attribute, ",");
					Set<int> set = new();
					while (st.hasMoreTokens())
					{
						string item = st.nextToken().Trim();
						set.add(int.Parse(item));
					}
					cond = joinAnd(cond, new ConditionTargetNpcId(set));
					break;
				}
				case "npctype":
				{
					string values = ((string)attribute).Trim();
					string[] valuesSplit = values.Split(",");
					InstanceType[] types = new InstanceType[valuesSplit.Length];
					for (int j = 0; j < valuesSplit.Length; j++)
						types[j] = Enum.Parse<InstanceType>(valuesSplit[j]);

					cond = joinAnd(cond, new ConditionTargetNpcType(types));
					break;
				}
				case "weight":
				{
					int weight = (int)attribute;
					cond = joinAnd(cond, new ConditionTargetWeight(weight));
					break;
				}
				case "invsize":
				{
					int size = (int)attribute;
					cond = joinAnd(cond, new ConditionTargetInvSize(size));
					break;
				}
				case "checkcrteffect":
				{
					bool val = (bool)attribute;
					cond = joinAnd(cond, new ConditionTargetCheckCrtEffect(val));
					break;
				}
			}
		}

		if (cond == null)
			throw new InvalidOperationException("Unrecognized <target> condition");

		return cond;
	}
	
	private static Condition parseUsingCondition(XElement element)
	{
		Condition cond = null;
		foreach (XAttribute attribute in element.Attributes())
		{
			switch (attribute.Name.LocalName)
			{
				case "kind":
				{
					int mask = 0;
					StringTokenizer st = new StringTokenizer((string)attribute, ",");
					while (st.hasMoreTokens())
					{
						int old = mask;
						String item = st.nextToken().Trim();
						foreach (WeaponType wt in Enum.GetValues<WeaponType>())
						{
							if (wt.ToString().equals(item))
							{
								mask |= wt.GetMask();
							}
						}
						
						foreach (ArmorType at in Enum.GetValues<ArmorType>())
						{
							if (at.ToString().equals(item))
							{
								mask |= at.GetMask();
							}
						}
						
						if (old == mask)
							LOGGER.Error("[parseUsingCondition=\"kind\"] Unknown item type name: " + item);
					}
					cond = joinAnd(cond, new ConditionUsingItemType(mask));
					break;
				}
				case "slot":
				{
					long mask = 0;
					StringTokenizer st = new StringTokenizer((string)attribute, ",");
					while (st.hasMoreTokens())
					{
						long old = mask;
						string item = st.nextToken().Trim();
						if (SLOTS.containsKey(item))
						{
							mask |= SLOTS.get(item);
						}
						
						if (old == mask)
							LOGGER.Error("[parseUsingCondition=\"slot\"] Unknown item slot name: " + item);
					}
					cond = joinAnd(cond, new ConditionUsingSlotType(mask));
					break;
				}
				case "skill":
				{
					int id = (int)attribute;
					cond = joinAnd(cond, new ConditionUsingSkill(id));
					break;
				}
				case "slotitem":
				{
					StringTokenizer st = new StringTokenizer((string)attribute, ";");
					int id = int.Parse(st.nextToken().Trim());
					int slot = int.Parse(st.nextToken().Trim());
					int enchant = 0;
					if (st.hasMoreTokens())
						enchant = int.Parse(st.nextToken().Trim());

					cond = joinAnd(cond, new ConditionSlotItemId(slot, id, enchant));
					break;
				}
				case "weaponchange":
				{
					bool val = (bool)attribute;
					cond = joinAnd(cond, new ConditionChangeWeapon(val));
					break;
				}
			}
		}

		if (cond == null)
			throw new InvalidOperationException("Unrecognized <using> condition");

		return cond;
	}
	
	private static Condition parseGameCondition(XElement element)
	{
		Condition cond = null;
		foreach (XAttribute attribute in element.Attributes())
		{
			switch (attribute.Name.LocalName)
			{
				case "skill":
				{
					bool val = (bool)attribute;
					cond = joinAnd(cond, new ConditionWithSkill(val));
					break;
				}
				case "night":
				{
					bool val = (bool)attribute;
					cond = joinAnd(cond, new ConditionGameTime(val));
					break;
				}
				case "chance":
				{
					int val = (int)attribute;
					cond = joinAnd(cond, new ConditionGameChance(val));
					break;
				}
			}
		}

		if (cond == null)
			throw new InvalidOperationException("Unrecognized <game> condition");
		
		return cond;
	}

	private static Condition joinAnd(Condition? cond, Condition c)
	{
		if (cond == null)
			return c;

		if (cond is ConditionLogicAnd logicAnd)
		{
			logicAnd.add(c);
			return logicAnd;
		}
		
		logicAnd = new ConditionLogicAnd();
		logicAnd.add(cond);
		logicAnd.add(c);
		return logicAnd;
	}
	
	private static ItemTemplate MakeItem(string className, StatSet set, ref ItemTemplate? template)
	{
		if (template is not null)
			template.set(set);
		else
		{
			template = className switch
			{
				"Weapon" => new Weapon(set),
				"Armor" => new Armor(set),
				"EtcItem" => new EtcItem(set),
				_ => throw new InvalidOperationException($"Invalid item class: {className}")
			};
		}

		return template;
	}
	
	/**
	 * Builds a variable in which all items are putting in in function of their ID.
	 * @param size
	 */
	private void buildFastLookupTable()
	{
		IEnumerable<int> ids = _armors.Keys.Concat(_weapons.Keys).Concat(_etcItems.Keys);
		int maxId = ids.Any() ? ids.Max() : 0;
			
		// Create a FastLookUp Table called _allTemplates of size : value of the highest item ID
		LOGGER.Info(GetType().Name + ": Highest item id used: " + maxId);
		_allTemplates = new ItemTemplate[maxId + 1];
		
		// Insert armor item in Fast Look Up Table
		foreach (Armor item in _armors.values())
		{
			_allTemplates[item.getId()] = item;
		}
		
		// Insert weapon item in Fast Look Up Table
		foreach (Weapon item in _weapons.values())
		{
			_allTemplates[item.getId()] = item;
		}
		
		// Insert etcItem item in Fast Look Up Table
		foreach (EtcItem item in _etcItems.values())
		{
			_allTemplates[item.getId()] = item;
		}
	}
	
	/**
	 * Returns the item corresponding to the item ID
	 * @param id : int designating the item
	 * @return Item
	 */
	public ItemTemplate getTemplate(int id)
	{
		if ((id >= _allTemplates.Length) || (id < 0))
		{
			return null;
		}
		return _allTemplates[id];
	}

	/**
	 * Create the Item corresponding to the Item Identifier and quantitiy add logs the activity. <b><u>Actions</u>:</b>
	 * <li>Create and Init the Item corresponding to the Item Identifier and quantity</li>
	 * <li>Add the Item object to _allObjects of L2world</li>
	 * <li>Logs Item creation according to log settings</li><br>
	 * @param process : String Identifier of process triggering this action
	 * @param itemId : int Item Identifier of the item to be created
	 * @param count : int Quantity of items to be created for stackable items
	 * @param actor : Creature requesting the item creation
	 * @param reference : Object Object referencing current action like NPC selling item or previous item in transformation
	 * @return Item corresponding to the new item
	 */
	public Item createItem(String process, int itemId, long count, Creature actor, Object reference)
	{
		// Create and Init the Item corresponding to the Item Identifier
		Item item = new Item(IdManager.getInstance().getNextId(), itemId);
		if (process.equalsIgnoreCase("loot") && !Config.AUTO_LOOT_ITEM_IDS.Contains(itemId))
		{
			ScheduledFuture itemLootShedule;
			if ((reference is Attackable) && ((Attackable)reference).isRaid()) // loot privilege for raids
			{
				Attackable raid = (Attackable)reference;
				// if in CommandChannel and was killing a World/RaidBoss
				if ((raid.getFirstCommandChannelAttacked() != null) && !Config.AUTO_LOOT_RAIDS)
				{
					item.setOwnerId(raid.getFirstCommandChannelAttacked().getLeaderObjectId());
					itemLootShedule = ThreadPool.schedule(new ResetOwner(item), Config.LOOT_RAIDS_PRIVILEGE_INTERVAL);
					item.setItemLootShedule(itemLootShedule);
				}
			}
			else if (!Config.AUTO_LOOT ||
			         ((reference is EventMonster) && ((EventMonster)reference).eventDropOnGround()))
			{
				item.setOwnerId(actor.getObjectId());
				itemLootShedule = ThreadPool.schedule(new ResetOwner(item), 15000);
				item.setItemLootShedule(itemLootShedule);
			}
		}

		// Add the Item object to _allObjects of L2world
		World.getInstance().addObject(item);

		// Set Item parameters
		if (item.isStackable() && (count > 1))
		{
			item.setCount(count);
		}

		if ((Config.LOG_ITEMS && !process.equals("Reset") &&
		     ((!Config.LOG_ITEMS_SMALL_LOG) && (!Config.LOG_ITEMS_IDS_ONLY))) ||
		    (Config.LOG_ITEMS_SMALL_LOG && (item.isEquipable() || (item.getId() == Inventory.ADENA_ID))) ||
		    (Config.LOG_ITEMS_IDS_ONLY && Config.LOG_ITEMS_IDS_LIST.Contains(item.getId())))
		{
			if (item.getEnchantLevel() > 0)
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("CREATE:");
				sb.Append(process);
				sb.Append(", item ");
				sb.Append(item.getObjectId());
				sb.Append(":+");
				sb.Append(item.getEnchantLevel());
				sb.Append(" ");
				sb.Append(item.getTemplate().getName());
				sb.Append("(");
				sb.Append(item.getCount());
				sb.Append("), ");
				sb.Append(actor);
				sb.Append(", ");
				sb.Append(reference);
				LOGGER.Info(sb.ToString());
			}
			else
			{
				StringBuilder sb = new StringBuilder();
				sb.Append("CREATE:");
				sb.Append(process);
				sb.Append(", item ");
				sb.Append(item.getObjectId());
				sb.Append(":");
				sb.Append(item.getTemplate().getName());
				sb.Append("(");
				sb.Append(item.getCount());
				sb.Append("), ");
				sb.Append(actor);
				sb.Append(", ");
				sb.Append(reference);
				LOGGER.Info(sb.ToString());
			}
		}

		if ((actor != null) && actor.isGM() && Config.GMAUDIT)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(process);
			sb.Append("(id: ");
			sb.Append(itemId);
			sb.Append(" count: ");
			sb.Append(count);
			sb.Append(" name: ");
			sb.Append(item.getItemName());
			sb.Append(" objId: ");
			sb.Append(item.getObjectId());
			sb.Append(")");

			String targetName = (actor.getTarget() != null ? actor.getTarget().getName() : "no-target");

			String referenceName = "no-reference";
			if (reference is WorldObject)
			{
				referenceName = (((WorldObject)reference).getName() != null
					? ((WorldObject)reference).getName()
					: "no-name");
			}
			else if (reference is String)
			{
				referenceName = (String)reference;
			}

			// TODO: GMAudit 
			// GMAudit.auditGMAction(actor.ToString(), sb.ToString(), targetName,
			// 	StringUtil.concat("Object referencing this action is: ", referenceName));
		}

		// Notify to scripts
		if (EventDispatcher.getInstance().hasListener(EventType.ON_ITEM_CREATE, item.getTemplate()))
		{
			EventDispatcher.getInstance()
				.notifyEventAsync(new OnItemCreate(process, item, actor, reference), item.getTemplate());
		}

		return item;
	}

	public Item createItem(String process, int itemId, long count, Player actor)
	{
		return createItem(process, itemId, count, actor, null);
	}
	
	/**
	 * Destroys the Item.<br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>Sets Item parameters to be unusable</li>
	 * <li>Removes the Item object to _allObjects of L2world</li>
	 * <li>Logs Item deletion according to log settings</li>
	 * </ul>
	 * @param process a string identifier of process triggering this action.
	 * @param item the item instance to be destroyed.
	 * @param actor the player requesting the item destroy.
	 * @param reference the object referencing current action like NPC selling item or previous item in transformation.
	 */
	public void destroyItem(String process, Item item, Player actor, Object reference)
	{
		lock (item)
		{
			long old = item.getCount();
			item.setCount(0);
			item.setOwnerId(0);
			item.setItemLocation(ItemLocation.VOID);
			item.setLastChange(Item.REMOVED);
			
			World.getInstance().removeObject(item);
			IdManager.getInstance().releaseId(item.getObjectId());
			
			if ((Config.LOG_ITEMS && ((!Config.LOG_ITEMS_SMALL_LOG) && (!Config.LOG_ITEMS_IDS_ONLY))) || (Config.LOG_ITEMS_SMALL_LOG && (item.isEquipable() || (item.getId() == Inventory.ADENA_ID))) || (Config.LOG_ITEMS_IDS_ONLY && Config.LOG_ITEMS_IDS_LIST.Contains(item.getId())))
			{
				if (item.getEnchantLevel() > 0)
				{
					StringBuilder sb = new StringBuilder();
					sb.Append("DELETE:");
					sb.Append(process);
					sb.Append(", item ");
					sb.Append(item.getObjectId());
					sb.Append(":+");
					sb.Append(item.getEnchantLevel());
					sb.Append(" ");
					sb.Append(item.getTemplate().getName());
					sb.Append("(");
					sb.Append(item.getCount());
					sb.Append("), PrevCount(");
					sb.Append(old);
					sb.Append("), ");
					sb.Append(actor);
					sb.Append(", ");
					sb.Append(reference);
					LOGGER.Info(sb.ToString());
				}
				else
				{
					StringBuilder sb = new StringBuilder();
					sb.Append("DELETE:");
					sb.Append(process);
					sb.Append(", item ");
					sb.Append(item.getObjectId());
					sb.Append(":");
					sb.Append(item.getTemplate().getName());
					sb.Append("(");
					sb.Append(item.getCount());
					sb.Append("), PrevCount(");
					sb.Append(old);
					sb.Append("), ");
					sb.Append(actor);
					sb.Append(", ");
					sb.Append(reference);
					LOGGER.Info(sb.ToString());
				}
			}

			if ((actor != null) && actor.isGM() && Config.GMAUDIT)
			{
				StringBuilder sb = new StringBuilder();
				sb.Append(process);
				sb.Append("(id: ");
				sb.Append(item.getId());
				sb.Append(" count: ");
				sb.Append(item.getCount());
				sb.Append(" itemObjId: ");
				sb.Append(item.getObjectId());
				sb.Append(")");

				String targetName = (actor.getTarget() != null ? actor.getTarget().getName() : "no-target");

				String referenceName = "no-reference";
				if (reference is WorldObject)
				{
					referenceName = (((WorldObject)reference).getName() != null
						? ((WorldObject)reference).getName()
						: "no-name");
				}
				else if (reference is String)
				{
					referenceName = (String)reference;
				}

				// TODO: GMAudit 
				//GMAudit.auditGMAction(actor.ToString(), sb.ToString(), targetName,
				//	StringUtil.concat("Object referencing this action is: ", referenceName));
			}

			// if it's a pet control item, delete the pet as well
			if (item.getTemplate().isPetItem())
			{
				try
				{
					using GameServerDbContext ctx = new();
					int itemId = item.getObjectId();
					// Delete the pet in db
					ctx.Pets.Where(pet => pet.ItemObjectId == itemId).ExecuteDelete();
				}
				catch (Exception e)
				{
					LOGGER.Error(GetType().Name + ": Could not delete pet objectid:", e);
				}
			}
		}
	}
	
	public void reload()
	{
		load();
		EnchantItemHPBonusData.getInstance().load();
	}
	
	public ICollection<int> getAllArmorsId()
	{
		return _armors.Keys;
	}
	
	public ICollection<Armor> getAllArmors()
	{
		return _armors.values();
	}
	
	public ICollection<int> getAllWeaponsId()
	{
		return _weapons.Keys;
	}
	
	public ICollection<Weapon> getAllWeapons()
	{
		return _weapons.values();
	}
	
	public ICollection<int> getAllEtcItemsId()
	{
		return _etcItems.Keys;
	}
	
	public ICollection<EtcItem> getAllEtcItems()
	{
		return _etcItems.values();
	}
	
	public ItemTemplate[] getAllItems()
	{
		return _allTemplates;
	}
	
	public int getArraySize()
	{
		return _allTemplates.Length;
	}
	
	protected class ResetOwner : Runnable
	{
		Item _item;
		
		public ResetOwner(Item item)
		{
			_item = item;
		}
		
		public void run()
		{
			// Set owner id to 0 only when location is VOID.
			if (_item.getItemLocation() == ItemLocation.VOID)
			{
				_item.setOwnerId(0);
			}
			_item.setItemLootShedule(null);
		}
	}
	
	public static ItemData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly ItemData INSTANCE = new();
	}
}