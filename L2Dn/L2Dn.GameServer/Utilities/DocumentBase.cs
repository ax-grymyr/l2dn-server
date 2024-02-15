using System.Xml.Linq;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Conditions;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Model.Stats.Functions;
using NLog;

namespace L2Dn.GameServer.Utilities;

public abstract class DocumentBase
{
	protected readonly Logger LOGGER = LogManager.GetLogger(nameof(DocumentBase));
	
	private readonly string _filePath;
	protected readonly Map<string, string[]> _tables = new();
	
	protected DocumentBase(string pFile)
	{
		_filePath = pFile;
	}
	
	public Document parse()
	{
		Document doc = null;
		try
		{
			DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();
			factory.setValidating(false);
			factory.setIgnoringComments(true);
			doc = factory.newDocumentBuilder().parse(_file);
			parseDocument(doc);
		}
		catch (Exception e)
		{
			LOGGER.Error("Error loading file " + _filePath, e);
		}
		return doc;
	}
	
	protected abstract void parseDocument(XDocument doc);
	
	protected abstract StatSet getStatSet();
	
	protected abstract String getTableValue(String name);
	
	protected abstract String getTableValue(String name, int idx);
	
	protected void resetTable()
	{
		_tables.clear();
	}
	
	protected void setTable(String name, String[] table)
	{
		_tables.put(name, table);
	}
	
	protected void parseTemplate(XElement node, Object template)
	{
		Condition condition = null;
		Node n = node.getFirstChild();
		if (n == null)
		{
			return;
		}
		
		if ("cond".equalsIgnoreCase(n.getNodeName()))
		{
			condition = parseCondition(n.getFirstChild(), template);
			Node msg = n.getAttributes().getNamedItem("msg");
			Node msgId = n.getAttributes().getNamedItem("msgId");
			if ((condition != null) && (msg != null))
			{
				condition.setMessage(msg.getNodeValue());
			}
			else if ((condition != null) && (msgId != null))
			{
				condition.setMessageId(Integer.decode(getValue(msgId.getNodeValue(), null)));
				Node addName = n.getAttributes().getNamedItem("addName");
				if ((addName != null) && (Integer.decode(getValue(msgId.getNodeValue(), null)) > 0))
				{
					condition.addName();
				}
			}
			n = n.getNextSibling();
		}
		for (; n != null; n = n.getNextSibling())
		{
			String name = n.getNodeName().toLowerCase();
			
			switch (name)
			{
				case "add":
				case "sub":
				case "mul":
				case "div":
				case "set":
				case "enchant":
				case "enchanthp":
				{
					attachFunc(n, template, name, condition);
				}
			}
		}
	}
	
	protected void attachFunc(Node n, Object template, String functionName, Condition attachCond)
	{
		Stat stat = Stat.valueOfXml(n.getAttributes().getNamedItem("stat").getNodeValue());
		int order = -1;
		Node orderNode = n.getAttributes().getNamedItem("order");
		if (orderNode != null)
		{
			order = Integer.parseInt(orderNode.getNodeValue());
		}
		
		String valueString = n.getAttributes().getNamedItem("val").getNodeValue();
		double value;
		if (valueString.charAt(0) == '#')
		{
			value = Double.parseDouble(getTableValue(valueString));
		}
		else
		{
			value = Double.parseDouble(valueString);
		}
		
		Condition applayCond = parseCondition(n.getFirstChild(), template);
		FuncTemplate ft = new FuncTemplate(attachCond, applayCond, functionName, order, stat, value);
		if (template is ItemTemplate)
		{
			((ItemTemplate) template).addFunctionTemplate(ft);
		}
		else
		{
			throw new InvalidOperationException("Attaching stat to a non-effect template [" + template + "]!!!");
		}
	}
	
	protected Condition parseCondition(XElement node, Object template)
	{
		Node n = node;
		while ((n != null) && (n.getNodeType() != Node.ELEMENT_NODE))
		{
			n = n.getNextSibling();
		}
		
		Condition condition = null;
		if (n != null)
		{
			switch (n.getNodeName().toLowerCase())
			{
				case "and":
				{
					condition = parseLogicAnd(n, template);
					break;
				}
				case "or":
				{
					condition = parseLogicOr(n, template);
					break;
				}
				case "not":
				{
					condition = parseLogicNot(n, template);
					break;
				}
				case "player":
				{
					condition = parsePlayerCondition(n, template);
					break;
				}
				case "target":
				{
					condition = parseTargetCondition(n, template);
					break;
				}
				case "using":
				{
					condition = parseUsingCondition(n);
					break;
				}
				case "game":
				{
					condition = parseGameCondition(n);
					break;
				}
			}
		}
		
		return condition;
	}
	
	protected Condition parseLogicAnd(Node node, Object template)
	{
		ConditionLogicAnd cond = new ConditionLogicAnd();
		Node n = node;
		for (n = n.getFirstChild(); n != null; n = n.getNextSibling())
		{
			if (n.getNodeType() == Node.ELEMENT_NODE)
			{
				cond.add(parseCondition(n, template));
			}
		}
		if ((cond.conditions == null) || (cond.conditions.length == 0))
		{
			LOGGER.severe("Empty <and> condition in " + _file);
		}
		return cond;
	}
	
	protected Condition parseLogicOr(Node node, Object template)
	{
		ConditionLogicOr cond = new ConditionLogicOr();
		Node n = node;
		for (n = n.getFirstChild(); n != null; n = n.getNextSibling())
		{
			if (n.getNodeType() == Node.ELEMENT_NODE)
			{
				cond.add(parseCondition(n, template));
			}
		}
		if ((cond.conditions == null) || (cond.conditions.length == 0))
		{
			LOGGER.severe("Empty <or> condition in " + _file);
		}
		return cond;
	}
	
	protected Condition parseLogicNot(Node node, Object template)
	{
		Node n = node;
		for (n = n.getFirstChild(); n != null; n = n.getNextSibling())
		{
			if (n.getNodeType() == Node.ELEMENT_NODE)
			{
				return new ConditionLogicNot(parseCondition(n, template));
			}
		}
		LOGGER.severe("Empty <not> condition in " + _file);
		return null;
	}
	
	protected Condition parsePlayerCondition(Node n, Object template)
	{
		Condition cond = null;
		NamedNodeMap attrs = n.getAttributes();
		for (int i = 0; i < attrs.getLength(); i++)
		{
			Node a = attrs.item(i);
			switch (a.getNodeName().toLowerCase())
			{
				case "races":
				{
					String[] racesVal = a.getNodeValue().split(",");
					Set<Race> races = EnumSet.noneOf(Race.class);
					for (int r = 0; r < racesVal.length; r++)
					{
						if (racesVal[r] != null)
						{
							races.add(Race.valueOf(racesVal[r]));
						}
					}
					cond = joinAnd(cond, new ConditionPlayerRace(races));
					break;
				}
				case "level":
				{
					int lvl = Integer.decode(getValue(a.getNodeValue(), template));
					cond = joinAnd(cond, new ConditionPlayerLevel(lvl));
					break;
				}
				case "levelrange":
				{
					String[] range = getValue(a.getNodeValue(), template).split(";");
					if (range.length == 2)
					{
						int[] lvlRange = new int[2];
						lvlRange[0] = Integer.decode(getValue(a.getNodeValue(), template).split(";")[0]);
						lvlRange[1] = Integer.decode(getValue(a.getNodeValue(), template).split(";")[1]);
						cond = joinAnd(cond, new ConditionPlayerLevelRange(lvlRange));
					}
					break;
				}
				case "resting":
				{
					boolean val = Boolean.parseBoolean(a.getNodeValue());
					cond = joinAnd(cond, new ConditionPlayerState(PlayerState.RESTING, val));
					break;
				}
				case "flying":
				{
					boolean val = Boolean.parseBoolean(a.getNodeValue());
					cond = joinAnd(cond, new ConditionPlayerState(PlayerState.FLYING, val));
					break;
				}
				case "moving":
				{
					boolean val = Boolean.parseBoolean(a.getNodeValue());
					cond = joinAnd(cond, new ConditionPlayerState(PlayerState.MOVING, val));
					break;
				}
				case "running":
				{
					boolean val = Boolean.parseBoolean(a.getNodeValue());
					cond = joinAnd(cond, new ConditionPlayerState(PlayerState.RUNNING, val));
					break;
				}
				case "standing":
				{
					boolean val = Boolean.parseBoolean(a.getNodeValue());
					cond = joinAnd(cond, new ConditionPlayerState(PlayerState.STANDING, val));
					break;
				}
				case "behind":
				{
					boolean val = Boolean.parseBoolean(a.getNodeValue());
					cond = joinAnd(cond, new ConditionPlayerState(PlayerState.BEHIND, val));
					break;
				}
				case "front":
				{
					boolean val = Boolean.parseBoolean(a.getNodeValue());
					cond = joinAnd(cond, new ConditionPlayerState(PlayerState.FRONT, val));
					break;
				}
				case "chaotic":
				{
					boolean val = Boolean.parseBoolean(a.getNodeValue());
					cond = joinAnd(cond, new ConditionPlayerState(PlayerState.CHAOTIC, val));
					break;
				}
				case "olympiad":
				{
					boolean val = Boolean.parseBoolean(a.getNodeValue());
					cond = joinAnd(cond, new ConditionPlayerState(PlayerState.OLYMPIAD, val));
					break;
				}
				case "ishero":
				{
					boolean val = Boolean.parseBoolean(a.getNodeValue());
					cond = joinAnd(cond, new ConditionPlayerIsHero(val));
					break;
				}
				case "ispvpflagged":
				{
					boolean val = Boolean.parseBoolean(a.getNodeValue());
					cond = joinAnd(cond, new ConditionPlayerIsPvpFlagged(val));
					break;
				}
				case "transformationid":
				{
					int id = Integer.parseInt(a.getNodeValue());
					cond = joinAnd(cond, new ConditionPlayerTransformationId(id));
					break;
				}
				case "hp":
				{
					int hp = Integer.decode(getValue(a.getNodeValue(), template));
					cond = joinAnd(cond, new ConditionPlayerHp(hp));
					break;
				}
				case "mp":
				{
					int mp = Integer.decode(getValue(a.getNodeValue(), template));
					cond = joinAnd(cond, new ConditionPlayerMp(mp));
					break;
				}
				case "cp":
				{
					int cp = Integer.decode(getValue(a.getNodeValue(), template));
					cond = joinAnd(cond, new ConditionPlayerCp(cp));
					break;
				}
				case "pkcount":
				{
					int expIndex = Integer.decode(getValue(a.getNodeValue(), template));
					cond = joinAnd(cond, new ConditionPlayerPkCount(expIndex));
					break;
				}
				case "siegezone":
				{
					int value = Integer.decode(getValue(a.getNodeValue(), null));
					cond = joinAnd(cond, new ConditionSiegeZone(value, true));
					break;
				}
				case "siegeside":
				{
					int value = Integer.decode(getValue(a.getNodeValue(), null));
					cond = joinAnd(cond, new ConditionPlayerSiegeSide(value));
					break;
				}
				case "charges":
				{
					int value = Integer.decode(getValue(a.getNodeValue(), template));
					cond = joinAnd(cond, new ConditionPlayerCharges(value));
					break;
				}
				case "souls":
				{
					int value = Integer.decode(getValue(a.getNodeValue(), template));
					SoulType type = Enum.valueOf(SoulType.class, a.getNodeValue());
					cond = joinAnd(cond, new ConditionPlayerSouls(value, type));
					break;
				}
				case "weight":
				{
					int weight = Integer.decode(getValue(a.getNodeValue(), null));
					cond = joinAnd(cond, new ConditionPlayerWeight(weight));
					break;
				}
				case "invsize":
				{
					int size = Integer.decode(getValue(a.getNodeValue(), null));
					cond = joinAnd(cond, new ConditionPlayerInvSize(size));
					break;
				}
				case "isclanleader":
				{
					boolean val = Boolean.parseBoolean(a.getNodeValue());
					cond = joinAnd(cond, new ConditionPlayerIsClanLeader(val));
					break;
				}
				case "pledgeclass":
				{
					int pledgeClass = Integer.decode(getValue(a.getNodeValue(), null));
					cond = joinAnd(cond, new ConditionPlayerPledgeClass(pledgeClass));
					break;
				}
				case "clanhall":
				{
					StringTokenizer st = new StringTokenizer(a.getNodeValue(), ",");
					ArrayList<Integer> array = new ArrayList<>(st.countTokens());
					while (st.hasMoreTokens())
					{
						String item = st.nextToken().trim();
						array.add(Integer.decode(getValue(item, template)));
					}
					cond = joinAnd(cond, new ConditionPlayerHasClanHall(array));
					break;
				}
				case "fort":
				{
					int fort = Integer.decode(getValue(a.getNodeValue(), null));
					cond = joinAnd(cond, new ConditionPlayerHasFort(fort));
					break;
				}
				case "castle":
				{
					int castle = Integer.decode(getValue(a.getNodeValue(), null));
					cond = joinAnd(cond, new ConditionPlayerHasCastle(castle));
					break;
				}
				case "sex":
				{
					int sex = Integer.decode(getValue(a.getNodeValue(), null));
					cond = joinAnd(cond, new ConditionPlayerSex(sex));
					break;
				}
				case "flymounted":
				{
					boolean val = Boolean.parseBoolean(a.getNodeValue());
					cond = joinAnd(cond, new ConditionPlayerFlyMounted(val));
					break;
				}
				case "vehiclemounted":
				{
					boolean val = Boolean.parseBoolean(a.getNodeValue());
					cond = joinAnd(cond, new ConditionPlayerVehicleMounted(val));
					break;
				}
				case "landingzone":
				{
					boolean val = Boolean.parseBoolean(a.getNodeValue());
					cond = joinAnd(cond, new ConditionPlayerLandingZone(val));
					break;
				}
				case "active_effect_id":
				{
					int effect_id = Integer.decode(getValue(a.getNodeValue(), template));
					cond = joinAnd(cond, new ConditionPlayerActiveEffectId(effect_id));
					break;
				}
				case "active_effect_id_lvl":
				{
					String val = getValue(a.getNodeValue(), template);
					int effect_id = Integer.decode(getValue(val.split(",")[0], template));
					int effect_lvl = Integer.decode(getValue(val.split(",")[1], template));
					cond = joinAnd(cond, new ConditionPlayerActiveEffectId(effect_id, effect_lvl));
					break;
				}
				case "active_skill_id":
				{
					int skill_id = Integer.decode(getValue(a.getNodeValue(), template));
					cond = joinAnd(cond, new ConditionPlayerActiveSkillId(skill_id));
					break;
				}
				case "active_skill_id_lvl":
				{
					String val = getValue(a.getNodeValue(), template);
					int skill_id = Integer.decode(getValue(val.split(",")[0], template));
					int skill_lvl = Integer.decode(getValue(val.split(",")[1], template));
					cond = joinAnd(cond, new ConditionPlayerActiveSkillId(skill_id, skill_lvl));
					break;
				}
				case "class_id_restriction":
				{
					StringTokenizer st = new StringTokenizer(a.getNodeValue(), ",");
					Set<Integer> array = new HashSet<>(st.countTokens());
					while (st.hasMoreTokens())
					{
						String item = st.nextToken().trim();
						array.add(Integer.decode(getValue(item, template)));
					}
					cond = joinAnd(cond, new ConditionPlayerClassIdRestriction(array));
					break;
				}
				case "subclass":
				{
					boolean val = Boolean.parseBoolean(a.getNodeValue());
					cond = joinAnd(cond, new ConditionPlayerSubclass(val));
					break;
				}
				case "dualclass":
				{
					boolean val = Boolean.parseBoolean(a.getNodeValue());
					cond = joinAnd(cond, new ConditionPlayerDualclass(val));
					break;
				}
				case "canswitchsubclass":
				{
					cond = joinAnd(cond, new ConditionPlayerCanSwitchSubclass(Integer.decode(a.getNodeValue())));
					break;
				}
				case "instanceid":
				{
					StringTokenizer st = new StringTokenizer(a.getNodeValue(), ",");
					Set<Integer> set = new HashSet<>(st.countTokens());
					while (st.hasMoreTokens())
					{
						String item = st.nextToken().trim();
						set.add(Integer.decode(getValue(item, template)));
					}
					cond = joinAnd(cond, new ConditionPlayerInstanceId(set));
					break;
				}
				case "agathionid":
				{
					int agathionId = Integer.decode(a.getNodeValue());
					cond = joinAnd(cond, new ConditionPlayerAgathionId(agathionId));
					break;
				}
				case "cloakstatus":
				{
					boolean val = Boolean.parseBoolean(a.getNodeValue());
					cond = joinAnd(cond, new ConditionPlayerCloakStatus(val));
					break;
				}
				case "hassummon":
				{
					boolean val = Boolean.parseBoolean(a.getNodeValue());
					cond = joinAnd(cond, new ConditionPlayerHasSummon(val));
					break;
				}
				case "haspet":
				{
					StringTokenizer st = new StringTokenizer(a.getNodeValue(), ",");
					ArrayList<Integer> array = new ArrayList<>(st.countTokens());
					while (st.hasMoreTokens())
					{
						String item = st.nextToken().trim();
						array.add(Integer.decode(getValue(item, template)));
					}
					cond = joinAnd(cond, new ConditionPlayerHasPet(array));
					break;
				}
				case "servitornpcid":
				{
					StringTokenizer st = new StringTokenizer(a.getNodeValue(), ",");
					ArrayList<Integer> array = new ArrayList<>(st.countTokens());
					while (st.hasMoreTokens())
					{
						String item = st.nextToken().trim();
						array.add(Integer.decode(getValue(item, null)));
					}
					cond = joinAnd(cond, new ConditionPlayerServitorNpcId(array));
					break;
				}
				case "npcidradius":
				{
					StringTokenizer st = new StringTokenizer(a.getNodeValue(), ",");
					if (st.countTokens() == 3)
					{
						String[] ids = st.nextToken().split(";");
						Set<Integer> npcIds = new HashSet<>(ids.length);
						for (int index = 0; index < ids.length; index++)
						{
							npcIds.add(Integer.parseInt(getValue(ids[index], template)));
						}
						int radius = Integer.parseInt(st.nextToken());
						boolean val = Boolean.parseBoolean(st.nextToken());
						cond = joinAnd(cond, new ConditionPlayerRangeFromNpc(npcIds, radius, val));
					}
					break;
				}
				case "summonednpcidradius":
				{
					StringTokenizer st = new StringTokenizer(a.getNodeValue(), ",");
					if (st.countTokens() == 3)
					{
						String[] ids = st.nextToken().split(";");
						Set<Integer> npcIds = new HashSet<>(ids.length);
						for (int index = 0; index < ids.length; index++)
						{
							npcIds.add(Integer.parseInt(getValue(ids[index], template)));
						}
						int radius = Integer.parseInt(st.nextToken());
						boolean val = Boolean.parseBoolean(st.nextToken());
						cond = joinAnd(cond, new ConditionPlayerRangeFromSummonedNpc(npcIds, radius, val));
					}
					break;
				}
				case "callpc":
				{
					cond = joinAnd(cond, new ConditionPlayerCallPc(Boolean.parseBoolean(a.getNodeValue())));
					break;
				}
				case "cancreatebase":
				{
					cond = joinAnd(cond, new ConditionPlayerCanCreateBase(Boolean.parseBoolean(a.getNodeValue())));
					break;
				}
				case "canescape":
				{
					cond = joinAnd(cond, new ConditionPlayerCanEscape(Boolean.parseBoolean(a.getNodeValue())));
					break;
				}
				case "canrefuelairship":
				{
					cond = joinAnd(cond, new ConditionPlayerCanRefuelAirship(Integer.parseInt(a.getNodeValue())));
					break;
				}
				case "canresurrect":
				{
					cond = joinAnd(cond, new ConditionPlayerCanResurrect(Boolean.parseBoolean(a.getNodeValue())));
					break;
				}
				case "cansummonpet":
				{
					cond = joinAnd(cond, new ConditionPlayerCanSummonPet(Boolean.parseBoolean(a.getNodeValue())));
					break;
				}
				case "cansummonservitor":
				{
					cond = joinAnd(cond, new ConditionPlayerCanSummonServitor(Boolean.parseBoolean(a.getNodeValue())));
					break;
				}
				case "hasfreesummonpoints":
				{
					cond = joinAnd(cond, new ConditionPlayerHasFreeSummonPoints(Integer.parseInt(a.getNodeValue())));
					break;
				}
				case "hasfreeteleportbookmarkslots":
				{
					cond = joinAnd(cond, new ConditionPlayerHasFreeTeleportBookmarkSlots(Integer.parseInt(a.getNodeValue())));
					break;
				}
				case "cansummonsiegegolem":
				{
					cond = joinAnd(cond, new ConditionPlayerCanSummonSiegeGolem(Boolean.parseBoolean(a.getNodeValue())));
					break;
				}
				case "cansweep":
				{
					cond = joinAnd(cond, new ConditionPlayerCanSweep(Boolean.parseBoolean(a.getNodeValue())));
					break;
				}
				case "cantakecastle":
				{
					cond = joinAnd(cond, new ConditionPlayerCanTakeCastle(Boolean.parseBoolean(a.getNodeValue())));
					break;
				}
				case "cantakefort":
				{
					cond = joinAnd(cond, new ConditionPlayerCanTakeFort(Boolean.parseBoolean(a.getNodeValue())));
					break;
				}
				case "cantransform":
				{
					cond = joinAnd(cond, new ConditionPlayerCanTransform(Boolean.parseBoolean(a.getNodeValue())));
					break;
				}
				case "canuntransform":
				{
					cond = joinAnd(cond, new ConditionPlayerCanUntransform(Boolean.parseBoolean(a.getNodeValue())));
					break;
				}
				case "insidezoneid":
				{
					StringTokenizer st = new StringTokenizer(a.getNodeValue(), ",");
					Set<Integer> set = new HashSet<>(st.countTokens());
					while (st.hasMoreTokens())
					{
						String item = st.nextToken().trim();
						set.add(Integer.decode(getValue(item, template)));
					}
					cond = joinAnd(cond, new ConditionPlayerInsideZoneId(set));
					break;
				}
				case "checkabnormal":
				{
					String value = a.getNodeValue();
					if (value.contains(","))
					{
						String[] values = value.split(",");
						cond = joinAnd(cond, new ConditionPlayerCheckAbnormal(AbnormalType.valueOf(values[0]), Integer.decode(getValue(values[1], template))));
					}
					else
					{
						cond = joinAnd(cond, new ConditionPlayerCheckAbnormal(AbnormalType.valueOf(value)));
					}
					break;
				}
				case "categorytype":
				{
					String[] values = a.getNodeValue().split(",");
					Set<CategoryType> array = new HashSet<>(values.length);
					for (String value : values)
					{
						array.add(CategoryType.valueOf(getValue(value, template)));
					}
					cond = joinAnd(cond, new ConditionCategoryType(array));
					break;
				}
				case "immobile":
				{
					cond = joinAnd(cond, new ConditionPlayerImmobile(Boolean.parseBoolean(a.getNodeValue())));
					break;
				}
				case "incombat":
				{
					cond = joinAnd(cond, new ConditionPlayerIsInCombat(Boolean.parseBoolean(a.getNodeValue())));
					break;
				}
				case "isonside":
				{
					cond = joinAnd(cond, new ConditionPlayerIsOnSide(Enum.valueOf(CastleSide.class, a.getNodeValue())));
					break;
				}
				case "ininstance":
				{
					cond = joinAnd(cond, new ConditionPlayerInInstance(Boolean.parseBoolean(a.getNodeValue())));
					break;
				}
				case "minimumvitalitypoints":
				{
					int count = Integer.decode(getValue(a.getNodeValue(), null));
					cond = joinAnd(cond, new ConditionMinimumVitalityPoints(count));
					break;
				}
			}
		}
		
		if (cond == null)
		{
			LOGGER.severe("Unrecognized <player> condition in " + _file);
		}
		return cond;
	}
	
	protected Condition parseTargetCondition(Node n, Object template)
	{
		Condition cond = null;
		NamedNodeMap attrs = n.getAttributes();
		for (int i = 0; i < attrs.getLength(); i++)
		{
			Node a = attrs.item(i);
			switch (a.getNodeName().toLowerCase())
			{
				case "aggro":
				{
					boolean val = Boolean.parseBoolean(a.getNodeValue());
					cond = joinAnd(cond, new ConditionTargetAggro(val));
					break;
				}
				case "siegezone":
				{
					int value = Integer.decode(getValue(a.getNodeValue(), null));
					cond = joinAnd(cond, new ConditionSiegeZone(value, false));
					break;
				}
				case "level":
				{
					int lvl = Integer.decode(getValue(a.getNodeValue(), template));
					cond = joinAnd(cond, new ConditionTargetLevel(lvl));
					break;
				}
				case "levelrange":
				{
					String[] range = getValue(a.getNodeValue(), template).split(";");
					if (range.length == 2)
					{
						int[] lvlRange = new int[2];
						lvlRange[0] = Integer.decode(getValue(a.getNodeValue(), template).split(";")[0]);
						lvlRange[1] = Integer.decode(getValue(a.getNodeValue(), template).split(";")[1]);
						cond = joinAnd(cond, new ConditionTargetLevelRange(lvlRange));
					}
					break;
				}
				case "mypartyexceptme":
				{
					cond = joinAnd(cond, new ConditionTargetMyPartyExceptMe(Boolean.parseBoolean(a.getNodeValue())));
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
					StringTokenizer st = new StringTokenizer(a.getNodeValue(), ",");
					Set<Integer> set = new HashSet<>(st.countTokens());
					while (st.hasMoreTokens())
					{
						String item = st.nextToken().trim();
						set.add(Integer.decode(getValue(item, null)));
					}
					cond = joinAnd(cond, new ConditionTargetClassIdRestriction(set));
					break;
				}
				case "active_effect_id":
				{
					int effect_id = Integer.decode(getValue(a.getNodeValue(), template));
					cond = joinAnd(cond, new ConditionTargetActiveEffectId(effect_id));
					break;
				}
				case "active_effect_id_lvl":
				{
					String val = getValue(a.getNodeValue(), template);
					int effect_id = Integer.decode(getValue(val.split(",")[0], template));
					int effect_lvl = Integer.decode(getValue(val.split(",")[1], template));
					cond = joinAnd(cond, new ConditionTargetActiveEffectId(effect_id, effect_lvl));
					break;
				}
				case "active_skill_id":
				{
					int skill_id = Integer.decode(getValue(a.getNodeValue(), template));
					cond = joinAnd(cond, new ConditionTargetActiveSkillId(skill_id));
					break;
				}
				case "active_skill_id_lvl":
				{
					String val = getValue(a.getNodeValue(), template);
					int skill_id = Integer.decode(getValue(val.split(",")[0], template));
					int skill_lvl = Integer.decode(getValue(val.split(",")[1], template));
					cond = joinAnd(cond, new ConditionTargetActiveSkillId(skill_id, skill_lvl));
					break;
				}
				case "abnormaltype":
				{
					AbnormalType abnormalType = AbnormalType.getAbnormalType(getValue(a.getNodeValue(), template));
					cond = joinAnd(cond, new ConditionTargetAbnormalType(abnormalType));
					break;
				}
				case "mindistance":
				{
					int distance = Integer.decode(getValue(a.getNodeValue(), null));
					cond = joinAnd(cond, new ConditionMinDistance(distance));
					break;
				}
				case "race":
				{
					cond = joinAnd(cond, new ConditionTargetRace(Race.valueOf(a.getNodeValue())));
					break;
				}
				case "using":
				{
					int mask = 0;
					StringTokenizer st = new StringTokenizer(a.getNodeValue(), ",");
					while (st.hasMoreTokens())
					{
						String item = st.nextToken().trim();
						for (WeaponType wt : WeaponType.values())
						{
							if (wt.name().equals(item))
							{
								mask |= wt.mask();
								break;
							}
						}
						for (ArmorType at : ArmorType.values())
						{
							if (at.name().equals(item))
							{
								mask |= at.mask();
								break;
							}
						}
					}
					cond = joinAnd(cond, new ConditionTargetUsesWeaponKind(mask));
					break;
				}
				case "npcid":
				{
					StringTokenizer st = new StringTokenizer(a.getNodeValue(), ",");
					Set<Integer> set = new HashSet<>(st.countTokens());
					while (st.hasMoreTokens())
					{
						String item = st.nextToken().trim();
						set.add(Integer.decode(getValue(item, null)));
					}
					cond = joinAnd(cond, new ConditionTargetNpcId(set));
					break;
				}
				case "npctype":
				{
					String values = getValue(a.getNodeValue(), template).trim();
					String[] valuesSplit = values.split(",");
					InstanceType[] types = new InstanceType[valuesSplit.length];
					InstanceType type;
					for (int j = 0; j < valuesSplit.length; j++)
					{
						type = Enum.valueOf(InstanceType.class, valuesSplit[j]);
						if (type == null)
						{
							throw new IllegalArgumentException("Instance type not recognized: " + valuesSplit[j]);
						}
						types[j] = type;
					}
					cond = joinAnd(cond, new ConditionTargetNpcType(types));
					break;
				}
				case "weight":
				{
					int weight = Integer.decode(getValue(a.getNodeValue(), null));
					cond = joinAnd(cond, new ConditionTargetWeight(weight));
					break;
				}
				case "invsize":
				{
					int size = Integer.decode(getValue(a.getNodeValue(), null));
					cond = joinAnd(cond, new ConditionTargetInvSize(size));
					break;
				}
				case "checkcrteffect":
				{
					cond = joinAnd(cond, new ConditionTargetCheckCrtEffect(Boolean.parseBoolean(a.getNodeValue())));
					break;
				}
			}
		}
		
		if (cond == null)
		{
			LOGGER.severe("Unrecognized <target> condition in " + _file);
		}
		return cond;
	}
	
	protected Condition parseUsingCondition(Node n)
	{
		Condition cond = null;
		NamedNodeMap attrs = n.getAttributes();
		for (int i = 0; i < attrs.getLength(); i++)
		{
			Node a = attrs.item(i);
			switch (a.getNodeName().toLowerCase())
			{
				case "kind":
				{
					int mask = 0;
					StringTokenizer st = new StringTokenizer(a.getNodeValue(), ",");
					while (st.hasMoreTokens())
					{
						int old = mask;
						String item = st.nextToken().trim();
						for (WeaponType wt : WeaponType.values())
						{
							if (wt.name().equals(item))
							{
								mask |= wt.mask();
							}
						}
						
						for (ArmorType at : ArmorType.values())
						{
							if (at.name().equals(item))
							{
								mask |= at.mask();
							}
						}
						
						if (old == mask)
						{
							LOGGER.info("[parseUsingCondition=\"kind\"] Unknown item type name: " + item);
						}
					}
					cond = joinAnd(cond, new ConditionUsingItemType(mask));
					break;
				}
				case "slot":
				{
					int mask = 0;
					StringTokenizer st = new StringTokenizer(a.getNodeValue(), ",");
					while (st.hasMoreTokens())
					{
						int old = mask;
						String item = st.nextToken().trim();
						if (ItemData.SLOTS.containsKey(item))
						{
							mask |= ItemData.SLOTS.get(item);
						}
						
						if (old == mask)
						{
							LOGGER.info("[parseUsingCondition=\"slot\"] Unknown item slot name: " + item);
						}
					}
					cond = joinAnd(cond, new ConditionUsingSlotType(mask));
					break;
				}
				case "skill":
				{
					int id = Integer.parseInt(a.getNodeValue());
					cond = joinAnd(cond, new ConditionUsingSkill(id));
					break;
				}
				case "slotitem":
				{
					StringTokenizer st = new StringTokenizer(a.getNodeValue(), ";");
					int id = Integer.parseInt(st.nextToken().trim());
					int slot = Integer.parseInt(st.nextToken().trim());
					int enchant = 0;
					if (st.hasMoreTokens())
					{
						enchant = Integer.parseInt(st.nextToken().trim());
					}
					cond = joinAnd(cond, new ConditionSlotItemId(slot, id, enchant));
					break;
				}
				case "weaponchange":
				{
					boolean val = Boolean.parseBoolean(a.getNodeValue());
					cond = joinAnd(cond, new ConditionChangeWeapon(val));
					break;
				}
			}
		}
		
		if (cond == null)
		{
			LOGGER.severe("Unrecognized <using> condition in " + _file);
		}
		return cond;
	}
	
	protected Condition parseGameCondition(Node n)
	{
		Condition cond = null;
		NamedNodeMap attrs = n.getAttributes();
		for (int i = 0; i < attrs.getLength(); i++)
		{
			Node a = attrs.item(i);
			if ("skill".equalsIgnoreCase(a.getNodeName()))
			{
				boolean val = Boolean.parseBoolean(a.getNodeValue());
				cond = joinAnd(cond, new ConditionWithSkill(val));
			}
			if ("night".equalsIgnoreCase(a.getNodeName()))
			{
				boolean val = Boolean.parseBoolean(a.getNodeValue());
				cond = joinAnd(cond, new ConditionGameTime(val));
			}
			if ("chance".equalsIgnoreCase(a.getNodeName()))
			{
				int val = Integer.decode(getValue(a.getNodeValue(), null));
				cond = joinAnd(cond, new ConditionGameChance(val));
			}
		}
		if (cond == null)
		{
			LOGGER.severe("Unrecognized <game> condition in " + _file);
		}
		return cond;
	}
	
	protected void parseTable(Node n)
	{
		NamedNodeMap attrs = n.getAttributes();
		String name = attrs.getNamedItem("name").getNodeValue();
		if (name.charAt(0) != '#')
		{
			throw new IllegalArgumentException("Table name must start with #");
		}
		StringTokenizer data = new StringTokenizer(n.getFirstChild().getNodeValue());
		List<String> array = new ArrayList<>(data.countTokens());
		while (data.hasMoreTokens())
		{
			array.add(data.nextToken());
		}
		setTable(name, array.toArray(new String[array.size()]));
	}
	
	protected void parseBeanSet(Node n, StatSet set, Integer level)
	{
		String name = n.getAttributes().getNamedItem("name").getNodeValue().trim();
		String value = n.getAttributes().getNamedItem("val").getNodeValue().trim();
		char ch = value.isEmpty() ? ' ' : value.charAt(0);
		if ((ch == '#') || (ch == '-') || Character.isDigit(ch))
		{
			set.set(name, getValue(value, level));
		}
		else
		{
			set.set(name, value);
		}
	}
	
	protected void setExtractableSkillData(StatSet set, String value)
	{
		set.set("capsuled_items_skill", value);
	}
	
	protected String getValue(String value, Object template)
	{
		// is it a table?
		if (value.charAt(0) == '#')
		{
			if (template instanceof Skill)
			{
				return getTableValue(value);
			}
			else if (template instanceof Integer)
			{
				return getTableValue(value, ((Integer) template).intValue());
			}
			else
			{
				throw new IllegalStateException();
			}
		}
		return value;
	}
	
	protected Condition joinAnd(Condition cond, Condition c)
	{
		if (cond == null)
		{
			return c;
		}
		if (cond is ConditionLogicAnd)
		{
			((ConditionLogicAnd) cond).add(c);
			return cond;
		}
		
		ConditionLogicAnd and = new ConditionLogicAnd();
		and.add(cond);
		and.add(c);
		return and;
	}
}