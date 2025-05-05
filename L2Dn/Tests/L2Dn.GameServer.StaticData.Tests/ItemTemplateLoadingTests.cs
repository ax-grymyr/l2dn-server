using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Conditions;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Enchant.Attributes;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Model.Stats.Functions;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Templates;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;
using L2Dn.Utilities;
using ConditionFactory = L2Dn.GameServer.Handlers.ConditionFactory;

namespace L2Dn.GameServer.StaticData.Tests;

public sealed class ItemTemplateLoadingTests
{
    [Fact]
    public void CompareItemTemplates()
    {
        // Register item handlers
        ConditionFactory.Instance.Register(new Model.Conditions.ConditionFactory());

        // Loading skill templates the old way
        OldLoader oldLoader = new();
        oldLoader.Load();

        // Load skill templates
        ItemData itemData = ItemData.getInstance();
        itemData.Load();

        // Compare skills
        ItemComparer.CompareItems(oldLoader.getAllItems(), itemData.getAllItems());
    }

    private sealed class OldLoader: DataReaderBase
    {
        private OldItemTemplate?[] _allTemplates = [];
        private readonly Map<int, OldEtcItem> _etcItems = new();
        private readonly Map<int, OldArmor> _armors = new();
        private readonly Map<int, OldWeapon> _weapons = new();
        private readonly Map<string, ImmutableArray<string>> _tables = new();

        private static readonly Map<string, long> _slotNameMap = new();

        static OldLoader()
        {
            _slotNameMap.put("shirt", OldItemTemplate.SLOT_UNDERWEAR);
            _slotNameMap.put("lbracelet", OldItemTemplate.SLOT_L_BRACELET);
            _slotNameMap.put("rbracelet", OldItemTemplate.SLOT_R_BRACELET);
            _slotNameMap.put("talisman", OldItemTemplate.SLOT_DECO);
            _slotNameMap.put("chest", OldItemTemplate.SLOT_CHEST);
            _slotNameMap.put("fullarmor", OldItemTemplate.SLOT_FULL_ARMOR);
            _slotNameMap.put("head", OldItemTemplate.SLOT_HEAD);
            _slotNameMap.put("hair", OldItemTemplate.SLOT_HAIR);
            _slotNameMap.put("hairall", OldItemTemplate.SLOT_HAIRALL);
            _slotNameMap.put("underwear", OldItemTemplate.SLOT_UNDERWEAR);
            _slotNameMap.put("back", OldItemTemplate.SLOT_BACK);
            _slotNameMap.put("neck", OldItemTemplate.SLOT_NECK);
            _slotNameMap.put("legs", OldItemTemplate.SLOT_LEGS);
            _slotNameMap.put("feet", OldItemTemplate.SLOT_FEET);
            _slotNameMap.put("gloves", OldItemTemplate.SLOT_GLOVES);
            _slotNameMap.put("chest,legs", OldItemTemplate.SLOT_CHEST | OldItemTemplate.SLOT_LEGS);
            _slotNameMap.put("belt", OldItemTemplate.SLOT_BELT);
            _slotNameMap.put("rhand", OldItemTemplate.SLOT_R_HAND);
            _slotNameMap.put("lhand", OldItemTemplate.SLOT_L_HAND);
            _slotNameMap.put("lrhand", OldItemTemplate.SLOT_LR_HAND);
            _slotNameMap.put("rear;lear", OldItemTemplate.SLOT_R_EAR | OldItemTemplate.SLOT_L_EAR);
            _slotNameMap.put("rfinger;lfinger", OldItemTemplate.SLOT_R_FINGER | OldItemTemplate.SLOT_L_FINGER);
            _slotNameMap.put("wolf", OldItemTemplate.SLOT_WOLF);
            _slotNameMap.put("greatwolf", OldItemTemplate.SLOT_GREATWOLF);
            _slotNameMap.put("hatchling", OldItemTemplate.SLOT_HATCHLING);
            _slotNameMap.put("strider", OldItemTemplate.SLOT_STRIDER);
            _slotNameMap.put("babypet", OldItemTemplate.SLOT_BABYPET);
            _slotNameMap.put("brooch", OldItemTemplate.SLOT_BROOCH);
            _slotNameMap.put("brooch_jewel", OldItemTemplate.SLOT_BROOCH_JEWEL);
            _slotNameMap.put("agathion", OldItemTemplate.SLOT_AGATHION);
            _slotNameMap.put("artifactbook", OldItemTemplate.SLOT_ARTIFACT_BOOK);
            _slotNameMap.put("artifact", OldItemTemplate.SLOT_ARTIFACT);
            _slotNameMap.put("none", OldItemTemplate.SLOT_NONE);

            // retail compatibility
            _slotNameMap.put("onepiece", OldItemTemplate.SLOT_FULL_ARMOR);
            _slotNameMap.put("hair2", OldItemTemplate.SLOT_HAIR2);
            _slotNameMap.put("dhair", OldItemTemplate.SLOT_HAIRALL);
            _slotNameMap.put("alldress", OldItemTemplate.SLOT_ALLDRESS);
            _slotNameMap.put("deco1", OldItemTemplate.SLOT_DECO);
            _slotNameMap.put("waist", OldItemTemplate.SLOT_BELT);
        }

        public void Load()
        {
            _armors.Clear();
            _etcItems.Clear();
            _weapons.Clear();

            LoadXmlDocuments(DataFileLocation.Data, "stats/items").ForEach(t =>
            {
                t.Document.Elements("list").Elements("item").ForEach(x => loadElement(t.FilePath, x));
            });

            if (Config.General.CUSTOM_ITEMS_LOAD)
            {
                LoadXmlDocuments(DataFileLocation.Data, "stats/items/custom").ForEach(t =>
                {
                    t.Document.Elements("list").Elements("item").ForEach(x => loadElement(t.FilePath, x));
                });
            }

            buildFastLookupTable();
        }

        private void loadElement(string fileName, XElement element)
        {
            int id = element.GetAttributeValueAsInt32("id");
            string name = element.GetAttributeValueAsString("name");
            string className = element.GetAttributeValueAsString("type");
            string additionalName = element.Attribute("additionalName").GetString(string.Empty);

            StatSet set = new();
            set.set("item_id", id);
            set.set("name", name);
            set.set("additionalName", additionalName);

            OldItemTemplate? item = null;
            element.Elements().ForEach(el =>
            {
                switch (el.Name.LocalName)
                {
                    case "table":
                    {
                        if (item is not null)
                            throw new InvalidOperationException("Item created but table node found! Item " + id);

                        parseTable(el);
                        break;
                    }

                    case "set":
                    {
                        if (item is not null)
                            throw new InvalidOperationException("Item created but set node found! Item " + id);

                        string setName = el.GetAttributeValueAsString("name").Trim();
                        string value = el.GetAttributeValueAsString("val").Trim();
                        char ch = string.IsNullOrEmpty(value) ? ' ' : value[0];
                        if (ch == '#' || ch == '-' || char.IsDigit(ch))
                            set.set(setName, getValue(value, 1));
                        else
                            set.set(setName, value);

                        break;
                    }

                    case "stats":
                    {
                        item ??= MakeItem(className, set);
                        el.Elements("stat").ForEach(e =>
                        {
                            Stat type = StatUtil.SearchByXmlName(e.GetAttributeValueAsString("type"));
                            double val = (double)e;
                            item.addFunctionTemplate(new StatFuncParameters(StatFuncType.ADD, 0, type, val));
                        });

                        break;
                    }

                    case "skills":
                    {
                        item ??= MakeItem(className, set);
                        el.Elements("skill").ForEach(e =>
                        {
                            int skillId = e.GetAttributeValueAsInt32("id");
                            int level = e.GetAttributeValueAsInt32("level");
                            ItemSkillType type = e.Attribute("type").GetEnum(ItemSkillType.NORMAL);
                            int chance = e.Attribute("type_chance").GetInt32(100);
                            int value = e.Attribute("type_value").GetInt32(0);
                            item.addSkill(new ItemSkillHolder(skillId, level, type, chance, value));
                        });

                        break;
                    }

                    case "capsuled_items":
                    {
                        item ??= MakeItem(className, set);
                        el.Elements("item").ForEach(e =>
                        {
                            int itemId = e.GetAttributeValueAsInt32("id");
                            long min = e.GetAttributeValueAsInt64("min");
                            long max = e.GetAttributeValueAsInt64("max");
                            double chance = e.GetAttributeValueAsDouble("chance");
                            int minEnchant = e.Attribute("minEnchant").GetInt32(0);
                            int maxEnchant = e.Attribute("maxEnchant").GetInt32(0);
                            item.addCapsuledItem(new ExtractableProduct(itemId, min, max, chance, minEnchant,
                                maxEnchant));
                        });

                        break;
                    }

                    case "cond":
                    {
                        item ??= MakeItem(className, set);
                        XElement conditionEl = el.Elements().Single();
                        Condition condition = parseCondition(conditionEl, item);
                        string? msg = conditionEl.Attribute("msg")?.Value;
                        string? msgId = conditionEl.Attribute("msgId")?.Value;
                        if (msg is not null)
                            condition.setMessage(msg);
                        else if (msgId is not null)
                        {
                            condition.setMessageId((SystemMessageId)int.Parse(msgId));
                            string? addName = conditionEl.GetAttributeValueAsString("addName");
                            if (addName != null && int.Parse(msgId) > 0)
                                condition.addName();
                        }

                        item.attachCondition(condition);

                        break;
                    }

                    default:
                        throw new InvalidOperationException($"Unknown tag {el.Name.LocalName} in file {fileName}");
                }
            });

            item ??= MakeItem(className, set);

            switch (item)
            {
                case OldEtcItem etcItem:
                    _etcItems.put(etcItem.Id, etcItem);
                    break;
                case OldArmor armor:
                    _armors.put(armor.Id, armor);
                    break;
                case OldWeapon weapon:
                    _weapons.put(item.Id, weapon);
                    break;

                default:
                    throw new InvalidOperationException("Invalid item type");
            }
        }

        private string getTableValue(string name)
        {
            throw new NotImplementedException();
            //return _tables.get(name)[_currentItem.currentLevel];
        }

        private string getTableValue(string name, int idx)
        {
            return _tables.get(name)[idx - 1];
        }

        private string getValue(string value, object template)
        {
            // is it a table?
            if (value[0] == '#')
            {
                if (template is Skill)
                    return getTableValue(value);

                if (template is int)
                    return getTableValue(value, (int)template);

                throw new InvalidOperationException();
            }

            return value;
        }

        private void parseTable(XElement element)
        {
            string name = element.GetAttributeValueAsString("name");
            if (string.IsNullOrEmpty(name) || name[0] != '#')
                throw new InvalidOperationException("Table name must start with #");

            StringTokenizer data = new StringTokenizer(element.Value, " ");
            List<string> array = new(data.countTokens());
            while (data.hasMoreTokens())
            {
                array.Add(data.nextToken());
            }

            _tables[name] = array.ToImmutableArray();
        }

        private static Condition parseCondition(XElement element, OldItemTemplate template)
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
                _ => throw new InvalidOperationException($"Invalid condition node: {elementName}"),
            };
        }

        private static Condition parseLogicAnd(XElement element, OldItemTemplate template)
        {
            ConditionLogicAnd cond = new ConditionLogicAnd();
            element.Elements().ForEach(e => cond.add(parseCondition(e, template)));
            if (cond.conditions == null || cond.conditions.Count == 0)
                Assert.Fail($"Empty <and> condition in item {template.Id}");

            return cond;
        }

        private static Condition parseLogicOr(XElement element, OldItemTemplate template)
        {
            ConditionLogicOr cond = new ConditionLogicOr();
            element.Elements().ForEach(e => cond.add(parseCondition(e, template)));
            if (cond.conditions == null || cond.conditions.Count == 0)
                Assert.Fail($"Empty <or> condition in item {template.Id}");

            return cond;
        }

        private static Condition parseLogicNot(XElement element, OldItemTemplate template)
        {
            XElement inner = element.Elements().Single();
            return new ConditionLogicNot(parseCondition(inner, template));
        }

        private static Condition parsePlayerCondition(XElement element, OldItemTemplate template)
        {
            Condition? cond = null;
            foreach (XAttribute attribute in element.Attributes())
            {
                switch (attribute.Name.LocalName.ToLowerInvariant())
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

                        cond = joinAnd(cond, new ConditionPlayerRace(races.ToFrozenSet()));
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
                            int minLevel = int.Parse(range[0]);
                            int maxLevel = int.Parse(range[1]);
                            cond = joinAnd(cond, new ConditionPlayerLevelRange(minLevel, maxLevel));
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
                        cond = joinAnd(cond, new ConditionPlayerPledgeClass((SocialClass)pledgeClass));
                        break;
                    }
                    case "clanhall":
                    {
                        StringTokenizer st = new StringTokenizer((string)attribute, ",");
                        List<int> array = new();
                        while (st.hasMoreTokens())
                        {
                            string item = st.nextToken().Trim();
                            array.Add(int.Parse(item));
                        }

                        cond = joinAnd(cond, new ConditionPlayerHasClanHall(array.ToFrozenSet()));
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
                        Sex sex = (int)attribute == 1 ? Sex.Female : Sex.Male;
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
                        Set<CharacterClass> array = new();
                        while (st.hasMoreTokens())
                        {
                            string item = st.nextToken().Trim();
                            array.add((CharacterClass)int.Parse(item));
                        }

                        cond = joinAnd(cond, new ConditionPlayerClassIdRestriction(array.ToFrozenSet()));
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

                        cond = joinAnd(cond, new ConditionPlayerInstanceId(set.ToFrozenSet()));
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
                            array.Add(int.Parse(item));
                        }

                        cond = joinAnd(cond, new ConditionPlayerHasPet(array.ToFrozenSet()));
                        break;
                    }
                    case "servitornpcid":
                    {
                        StringTokenizer st = new StringTokenizer((string)attribute, ",");
                        List<int> array = new();
                        while (st.hasMoreTokens())
                        {
                            string item = st.nextToken().Trim();
                            array.Add(int.Parse(item));
                        }

                        cond = joinAnd(cond, new ConditionPlayerServitorNpcId(array.ToFrozenSet()));
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
                            cond = joinAnd(cond, new ConditionPlayerRangeFromNpc(npcIds.ToFrozenSet(), radius, val));
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
                            cond = joinAnd(cond, new ConditionPlayerRangeFromSummonedNpc(npcIds.ToFrozenSet(), radius, val));
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

                        cond = joinAnd(cond, new ConditionPlayerInsideZoneId(set.ToFrozenSet()));
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
                        Set<CategoryType> array = [];
                        foreach (string value in values)
                            array.add(Enum.Parse<CategoryType>(value));

                        cond = joinAnd(cond, new ConditionCategoryType(array.ToFrozenSet()));
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

        private static Condition parseTargetCondition(XElement element, OldItemTemplate template)
        {
            Condition? cond = null;
            foreach (XAttribute attribute in element.Attributes())
            {
                switch (attribute.Name.LocalName.ToLowerInvariant())
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
                            int minLevel = int.Parse(range[0]);
                            int maxLevel = int.Parse(range[1]);
                            cond = joinAnd(cond, new ConditionTargetLevelRange(minLevel, maxLevel));
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
                        Set<CharacterClass> set = new();
                        while (st.hasMoreTokens())
                        {
                            string item = st.nextToken().Trim();
                            set.add((CharacterClass)int.Parse(item));
                        }

                        cond = joinAnd(cond, new ConditionTargetClassIdRestriction(set.ToFrozenSet()));
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
                        ItemTypeMask mask = ItemTypeMask.Zero;
                        StringTokenizer st = new StringTokenizer((string)attribute, ",");
                        while (st.hasMoreTokens())
                        {
                            string item = st.nextToken().Trim();
                            foreach (WeaponType wt in EnumUtil.GetValues<WeaponType>())
                            {
                                if (wt.ToString().equals(item))
                                {
                                    mask |= wt;
                                    break;
                                }
                            }

                            foreach (ArmorType at in EnumUtil.GetValues<ArmorType>())
                            {
                                if (at.ToString().equals(item))
                                {
                                    mask |= at;
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

                        cond = joinAnd(cond, new ConditionTargetNpcId(set.ToFrozenSet()));
                        break;
                    }
                    case "npctype":
                    {
                        string values = ((string)attribute).Trim();
                        string[] valuesSplit = values.Split(",");
                        InstanceType[] types = new InstanceType[valuesSplit.Length];
                        for (int j = 0; j < valuesSplit.Length; j++)
                            types[j] = Enum.Parse<InstanceType>(valuesSplit[j]);

                        cond = joinAnd(cond, new ConditionTargetNpcType(types.ToFrozenSet()));
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
            Condition? cond = null;
            foreach (XAttribute attribute in element.Attributes())
            {
                switch (attribute.Name.LocalName)
                {
                    case "kind":
                    {
                        ItemTypeMask mask = ItemTypeMask.Zero;
                        StringTokenizer st = new StringTokenizer((string)attribute, ",");
                        while (st.hasMoreTokens())
                        {
                            ItemTypeMask old = mask;
                            string item = st.nextToken().Trim();
                            foreach (WeaponType wt in EnumUtil.GetValues<WeaponType>())
                            {
                                if (wt.ToString().equals(item))
                                {
                                    mask |= wt;
                                }
                            }

                            foreach (ArmorType at in EnumUtil.GetValues<ArmorType>())
                            {
                                if (at.ToString().equals(item))
                                {
                                    mask |= at;
                                }
                            }

                            if (old == mask)
                                Assert.Fail("[parseUsingCondition=\"kind\"] Unknown item type name: " + item);
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
                            if (_slotNameMap.TryGetValue(item, out long value))
                            {
                                mask |= value;
                            }

                            if (old == mask)
                                Assert.Fail("[parseUsingCondition=\"slot\"] Unknown item slot name: " + item);
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
            Condition? cond = null;
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

        private static OldItemTemplate MakeItem(string className, StatSet set)
        {
            return className switch
            {
                "Weapon" => new OldWeapon(set),
                "Armor" => new OldArmor(set),
                "EtcItem" => new OldEtcItem(set),
                _ => throw new InvalidOperationException($"Invalid item class: {className}"),
            };
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
            _allTemplates = new OldItemTemplate[maxId + 1];

            // Insert armor item in Fast Look Up Table
            foreach (OldArmor item in _armors.Values)
            {
                _allTemplates[item.Id] = item;
            }

            // Insert weapon item in Fast Look Up Table
            foreach (OldWeapon item in _weapons.Values)
            {
                _allTemplates[item.Id] = item;
            }

            // Insert etcItem item in Fast Look Up Table
            foreach (OldEtcItem item in _etcItems.Values)
            {
                _allTemplates[item.Id] = item;
            }
        }

        public ICollection<int> getAllArmorsId()
        {
            return _armors.Keys;
        }

        public ICollection<OldArmor> getAllArmors()
        {
            return _armors.Values;
        }

        public ICollection<int> getAllWeaponsId()
        {
            return _weapons.Keys;
        }

        public ICollection<OldWeapon> getAllWeapons()
        {
            return _weapons.Values;
        }

        public ICollection<int> getAllEtcItemsId()
        {
            return _etcItems.Keys;
        }

        public ICollection<OldEtcItem> getAllEtcItems()
        {
            return _etcItems.Values;
        }

        public OldItemTemplate?[] getAllItems()
        {
            return _allTemplates;
        }
    }

    private static class ItemComparer
    {
        public static void CompareItems(OldItemTemplate?[] oldItems, ImmutableArray<ItemTemplate> newItems)
        {
            List<ItemPair> itemPairs = oldItems.Where(x => x != null).Select(x => new ItemPair(x!.Id, [x], [])).
                Concat(newItems.Select(x => new ItemPair(x.Id, [], [x]))).GroupBy(x => x.Id).
                Select(x => new ItemPair(x.Key, x.SelectMany(y => y.Old).ToList(), x.SelectMany(y => y.New).ToList())).
                ToList();

            foreach (ItemPair itemPair in itemPairs)
            {
                if (itemPair.Old.Count != 1 || itemPair.Old.Count != itemPair.New.Count)
                {
                    Assert.Fail($"Item id={itemPair.Id}: old count={itemPair.Old.Count}, " +
                        $"new count={itemPair.New.Count}");

                    continue;
                }

                CompareItem(itemPair.Old[0], itemPair.New[0]);
            }
        }

        private static void CompareItem(OldItemTemplate oldItem, ItemTemplate newItem)
        {
            string owner = $"Item template id={oldItem.Id}";
            ValueComparer.CompareValue(owner, "Id", oldItem.Id, newItem.Id);
            ValueComparer.CompareValue(owner, "Name", oldItem.getName(), newItem.getName());
            ValueComparer.CompareValue(owner, "Duration", oldItem.getDuration(), newItem.getDuration());
            ValueComparer.CompareValue(owner, "Icon", oldItem.getIcon(), newItem.getIcon());
            ValueComparer.CompareValue(owner, "Time", oldItem.getTime(), newItem.getTime());
            ValueComparer.CompareValue(owner, "Type1", oldItem.getType1(), newItem.getType1());
            ValueComparer.CompareValue(owner, "Type2", oldItem.getType2(), newItem.getType2());
            ValueComparer.CompareValue(owner, "Weight", oldItem.getWeight(), newItem.getWeight());
            ValueComparer.CompareValue(owner, "hasSkills", oldItem.hasSkills(), newItem.hasSkills());
            ValueComparer.CompareValue(owner, "isAppearanceable", oldItem.isAppearanceable(), newItem.isAppearanceable());
            ValueComparer.CompareValue(owner, "isArmor", oldItem.isArmor(), newItem.isArmor());
            ValueComparer.CompareValue(owner, "isBlessed", oldItem.isBlessed(), newItem.isBlessed());
            ValueComparer.CompareValue(owner, "isCommon", oldItem.isCommon(), newItem.isCommon());
            ValueComparer.CompareValue(owner, "isCrystallizable", oldItem.isCrystallizable(), newItem.isCrystallizable());
            ValueComparer.CompareValue(owner, "isDepositable", oldItem.isDepositable(), newItem.isDepositable());
            ValueComparer.CompareValue(owner, "isDestroyable", oldItem.isDestroyable(), newItem.isDestroyable());
            ValueComparer.CompareValue(owner, "isDropable", oldItem.isDropable(), newItem.isDropable());
            ValueComparer.CompareValue(owner, "isElementable", oldItem.isElementable(), newItem.isElementable());
            ValueComparer.CompareValue(owner, "isElixir", oldItem.isElixir(), newItem.isElixir());
            ValueComparer.CompareValue(owner, "isEnchantable", oldItem.isEnchantable(), newItem.isEnchantable());
            ValueComparer.CompareValue(owner, "isEquipable", oldItem.isEquipable(), newItem.isEquipable());
            ValueComparer.CompareValue(owner, "isFreightable", oldItem.isFreightable(), newItem.isFreightable());
            ValueComparer.CompareValue(owner, "isPotion", oldItem.isPotion(), newItem.isPotion());
            ValueComparer.CompareValue(owner, "isScroll", oldItem.isScroll(), newItem.isScroll());
            ValueComparer.CompareValue(owner, "isSellable", oldItem.isSellable(), newItem.isSellable());
            ValueComparer.CompareValue(owner, "isStackable", oldItem.isStackable(), newItem.isStackable());
            ValueComparer.CompareValue(owner, "isTradeable", oldItem.isTradeable(), newItem.isTradeable());
            ValueComparer.CompareValue(owner, "isWeapon", oldItem.isWeapon(), newItem.isWeapon());
            ValueComparer.CompareValue(owner, "getAdditionalName", oldItem.getAdditionalName(), newItem.getAdditionalName());
            ValueComparer.CompareValue(owner, "getArtifactSlot", oldItem.getArtifactSlot(), newItem.getArtifactSlot());
            ValueComparer.CompareValue(owner, "getBodyPart", oldItem.getBodyPart(), newItem.getBodyPart());
            ValueComparer.CompareValue(owner, "getCrystalCount", oldItem.getCrystalCount(), newItem.getCrystalCount());
            ValueComparer.CompareValue(owner, "getCrystalType", oldItem.getCrystalType(), newItem.getCrystalType());
            ValueComparer.CompareValue(owner, "getDefaultAction", oldItem.getDefaultAction(), newItem.getDefaultAction());
            ValueComparer.CompareValue(owner, "getDisplayId", oldItem.getDisplayId(), newItem.getDisplayId());
            ValueComparer.CompareValue(owner, "getEnchantLimit", oldItem.getEnchantLimit(), newItem.getEnchantLimit());
            ValueComparer.CompareValue(owner, "getEnsoulSlots", oldItem.getEnsoulSlots(), newItem.getEnsoulSlots());
            ValueComparer.CompareValue(owner, "getItemGrade", oldItem.getItemGrade(), newItem.getItemGrade());
            ValueComparer.CompareValue(owner, "getItemMask", oldItem.getItemMask(), newItem.getItemMask());
            ValueComparer.CompareValue(owner, "getItemType", oldItem.getItemType(), newItem.getItemType());
            ValueComparer.CompareValue(owner, "getMaterialType", oldItem.getMaterialType(), newItem.getMaterialType());
            ValueComparer.CompareValue(owner, "getReferencePrice", oldItem.getReferencePrice(), newItem.getReferencePrice());
            ValueComparer.CompareValue(owner, "getReuseDelay", oldItem.getReuseDelay(), newItem.getReuseDelay());
            ValueComparer.CompareValue(owner, "hasImmediateEffect", oldItem.hasImmediateEffect(), newItem.hasImmediateEffect());
            ValueComparer.CompareValue(owner, "isConditionAttached", oldItem.isConditionAttached(), newItem.isConditionAttached());
            ValueComparer.CompareValue(owner, "isEtcItem", oldItem.isEtcItem(), newItem.isEtcItem());
            ValueComparer.CompareValue(owner, "isForNpc", oldItem.isForNpc(), newItem.isForNpc());
            ValueComparer.CompareValue(owner, "isHeroItem", oldItem.isHeroItem(), newItem.isHeroItem());
            ValueComparer.CompareValue(owner, "isMagicWeapon", oldItem.isMagicWeapon(), newItem.isMagicWeapon());
            ValueComparer.CompareValue(owner, "isPetItem", oldItem.isPetItem(), newItem.isPetItem());
            ValueComparer.CompareValue(owner, "isPvpItem", oldItem.isPvpItem(), newItem.isPvpItem());
            ValueComparer.CompareValue(owner, "isQuestItem", oldItem.isQuestItem(), newItem.isQuestItem());
            ValueComparer.CompareValue(owner, "getAutoDestroyTime", oldItem.getAutoDestroyTime(), newItem.getAutoDestroyTime());
            ValueComparer.CompareValue(owner, "getCommissionItemType", oldItem.getCommissionItemType(), newItem.getCommissionItemType());
            ValueComparer.CompareValue(owner, "getCrystalItemId", oldItem.getCrystalItemId(), newItem.getCrystalItemId());
            ValueComparer.CompareValue(owner, "getCrystalTypePlus", oldItem.getCrystalTypePlus(), newItem.getCrystalTypePlus());
            ValueComparer.CompareValue(owner, "getDefaultEnchantLevel", oldItem.getDefaultEnchantLevel(), newItem.getDefaultEnchantLevel());
            ValueComparer.CompareValue(owner, "getEquipReuseDelay", oldItem.getEquipReuseDelay(), newItem.getEquipReuseDelay());
            ValueComparer.CompareValue(owner, "getSharedReuseGroup", oldItem.getSharedReuseGroup(), newItem.getSharedReuseGroup());
            ValueComparer.CompareValue(owner, "getSpecialEnsoulSlots", oldItem.getSpecialEnsoulSlots(), newItem.getSpecialEnsoulSlots());
            ValueComparer.CompareValue(owner, "hasExImmediateEffect", oldItem.hasExImmediateEffect(), newItem.hasExImmediateEffect());
            ValueComparer.CompareValue(owner, "isAllowSelfResurrection", oldItem.isAllowSelfResurrection(), newItem.isAllowSelfResurrection());
            ValueComparer.CompareValue(owner, "isEventRestrictedItem", oldItem.isEventRestrictedItem(), newItem.isEventRestrictedItem());
            ValueComparer.CompareValue(owner, "isOlyRestrictedItem", oldItem.isOlyRestrictedItem(), newItem.isOlyRestrictedItem());
            ValueComparer.CompareValue(owner, "useSkillDisTime", oldItem.useSkillDisTime(), newItem.useSkillDisTime());

            ValueComparer.CompareValue(owner, "skills.Length", oldItem.getAllSkills().Count, newItem.getAllSkills().Length);
            for (int i = 0; i < oldItem.getAllSkills().Count; i++)
                ValueComparer.CompareValue(owner, $"skills[{i}]", oldItem.getAllSkills()[i], newItem.getAllSkills()[i]);

            ValueComparer.CompareValue(owner, "conditions.Length", oldItem.getConditions()?.Count ?? 0, newItem.getConditions().Length);
            if (oldItem.getConditions() is { } conditions)
            {
                for (int i = 0; i < conditions.Count; i++)
                    ValueComparer.CompareValue(owner, $"conditions[{i}]", conditions[i],
                        newItem.getConditions()[i]);
            }

            ValueComparer.CompareValue(owner, "unequipSkills.Length", 0, newItem.UnequipSkills.Length);
            for (int i = 0; i < newItem.UnequipSkills.Length; i++)
                ValueComparer.CompareValue(owner, $"unequipSkills[{i}]", null, newItem.UnequipSkills[i]);

            ValueComparer.CompareValue(owner, "attributes.Length", oldItem.getAttributes()?.Count ?? 0, newItem.getAttributes().Length);
            List<AttributeHolder>? attributes = oldItem.getAttributes()?.ToList();
            if (attributes is not null)
            {
                for (int i = 0; i < attributes.Count; i++)
                    ValueComparer.CompareValue(owner, $"attributes[{i}]", attributes[i],
                        newItem.getAttributes()[i]);
            }

            Assert.Equal(oldItem is OldArmor, newItem is Armor);
            Assert.Equal(oldItem is OldWeapon, newItem is Weapon);
            Assert.Equal(oldItem is OldEtcItem, newItem is EtcItem);

            if (oldItem is OldArmor oldArmor && newItem is Armor newArmor)
            {
                ValueComparer.CompareValue(owner, "getArmorType", oldArmor.getArmorType(), newArmor.getArmorType());
            }

            if (oldItem is OldWeapon oldWeapon && newItem is Weapon newWeapon)
            {
                ValueComparer.CompareValue(owner, "getWeaponType", oldWeapon.getWeaponType(), newWeapon.getWeaponType());
                ValueComparer.CompareValue(owner, "isMagicWeapon", oldWeapon.isMagicWeapon(), newWeapon.isMagicWeapon());
                ValueComparer.CompareValue(owner, "getSoulShotCount", oldWeapon.getSoulShotCount(), newWeapon.getSoulShotCount());
                ValueComparer.CompareValue(owner, "getSpiritShotCount", oldWeapon.getSpiritShotCount(), newWeapon.getSpiritShotCount());
                ValueComparer.CompareValue(owner, "getReducedSoulShot", oldWeapon.getReducedSoulShot(), newWeapon.getReducedSoulShot());
                ValueComparer.CompareValue(owner, "getReducedSoulShotChance", oldWeapon.getReducedSoulShotChance(), newWeapon.getReducedSoulShotChance());
                ValueComparer.CompareValue(owner, "getMpConsume", oldWeapon.getMpConsume(), newWeapon.getMpConsume());
                ValueComparer.CompareValue(owner, "getBaseAttackRange", oldWeapon.getBaseAttackRange(), newWeapon.getBaseAttackRange());
                ValueComparer.CompareValue(owner, "getBaseAttackRadius", oldWeapon.getBaseAttackRadius(), newWeapon.getBaseAttackRadius());
                ValueComparer.CompareValue(owner, "getBaseAttackAngle", oldWeapon.getBaseAttackAngle(), newWeapon.getBaseAttackAngle());
                ValueComparer.CompareValue(owner, "getReducedMpConsume", oldWeapon.getReducedMpConsume(), newWeapon.getReducedMpConsume());
                ValueComparer.CompareValue(owner, "getReducedMpConsumeChance", oldWeapon.getReducedMpConsumeChance(), newWeapon.getReducedMpConsumeChance());
                ValueComparer.CompareValue(owner, "getChangeWeaponId", oldWeapon.getChangeWeaponId(), newWeapon.getChangeWeaponId());
                ValueComparer.CompareValue(owner, "isForceEquip", oldWeapon.isForceEquip(), newWeapon.isForceEquip());
                ValueComparer.CompareValue(owner, "isAttackWeapon", oldWeapon.isAttackWeapon(), newWeapon.isAttackWeapon());
                ValueComparer.CompareValue(owner, "useWeaponSkillsOnly", oldWeapon.useWeaponSkillsOnly(), newWeapon.useWeaponSkillsOnly());
            }

            if (oldItem is OldEtcItem oldEtcItem && newItem is EtcItem newEtcItem)
            {
                ValueComparer.CompareValue(owner, "getHandlerName", oldEtcItem.getHandlerName(), newEtcItem.getHandlerName());
                ValueComparer.CompareValue(owner, "getExtractableCountMin", oldEtcItem.getExtractableCountMin(), newEtcItem.getExtractableCountMin());
                ValueComparer.CompareValue(owner, "getExtractableCountMax", oldEtcItem.getExtractableCountMax(), newEtcItem.getExtractableCountMax());
                ValueComparer.CompareValue(owner, "isInfinite", oldEtcItem.isInfinite(), newEtcItem.isInfinite());
                ValueComparer.CompareValue(owner, "isMineral", oldEtcItem.isMineral(), newEtcItem.isMineral());
                ValueComparer.CompareValue(owner, "isEnsoulStone", oldEtcItem.isEnsoulStone(), newEtcItem.isEnsoulStone());
            }
        }

        private readonly record struct ItemPair(int Id, List<OldItemTemplate> Old, List<ItemTemplate> New);
    }
}