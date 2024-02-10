using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * This class parse and hold all pet parameters.<br>
 * @author Zoey76 (rework)
 */
public class PetDataTable
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(PetDataTable));
	
	private readonly Map<int, PetData> _pets = new();
	private readonly Map<int, String> _petNames = new();
	
	/**
	 * Instantiates a new pet data table.
	 */
	protected PetDataTable()
	{
		load();
	}
	
	public void load()
	{
		_pets.clear();
		parseDatapackDirectory("data/stats/pets", false);
		
		try 
		{
			Connection conn = DatabaseFactory.getConnection();
			PreparedStatement ps = conn.prepareStatement("SELECT * FROM pets");
			ResultSet rs = ps.executeQuery();
			while (rs.next())
			{
				String name = rs.getString("name");
				if (name == null)
				{
					name = "No name";
				}
				_petNames.put(rs.getInt("item_obj_id"), name);
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Problem loading pet names! " + e);
		}
		
		LOGGER.Info(GetType().Name + ": Loaded " + _pets.size() + " pets.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		NamedNodeMap attrs;
		Node n = doc.getFirstChild();
		for (Node d = n.getFirstChild(); d != null; d = d.getNextSibling())
		{
			if (d.getNodeName().equals("pet"))
			{
				int npcId = parseInteger(d.getAttributes(), "id");
				int itemId = parseInteger(d.getAttributes(), "itemId");
				int index = parseInteger(d.getAttributes(), "index");
				int defaultPetType = parseInteger(d.getAttributes(), "defaultPetType");
				EvolveLevel evolveLevel = parseEnum(d.getAttributes(), EvolveLevel.class, "evolveLevel");
				int petType = parseInteger(d.getAttributes(), "type");
				if (defaultPetType == null)
				{
					defaultPetType = 0;
				}
				if (index == null)
				{
					index = 0;
				}
				if (petType == null)
				{
					petType = 0;
				}
				// index ignored for now
				PetData data = new PetData(npcId, itemId, defaultPetType, evolveLevel, index, petType);
				for (Node p = d.getFirstChild(); p != null; p = p.getNextSibling())
				{
					if (p.getNodeName().equals("set"))
					{
						attrs = p.getAttributes();
						String type = attrs.getNamedItem("name").getNodeValue();
						if ("food".equals(type))
						{
							foreach (String foodId in attrs.getNamedItem("val").getNodeValue().split(";"))
							{
								data.addFood(int.Parse(foodId));
							}
						}
						else if ("load".equals(type))
						{
							data.setLoad(parseInteger(attrs, "val"));
						}
						else if ("hungry_limit".equals(type))
						{
							data.setHungryLimit(parseInteger(attrs, "val"));
						}
						else if ("sync_level".equals(type))
						{
							data.setSyncLevel(parseInteger(attrs, "val") == 1);
						}
						// evolve ignored
					}
					else if (p.getNodeName().equals("skills"))
					{
						for (Node s = p.getFirstChild(); s != null; s = s.getNextSibling())
						{
							if (s.getNodeName().equals("skill"))
							{
								attrs = s.getAttributes();
								data.addNewSkill(parseInteger(attrs, "skillId"), parseInteger(attrs, "skillLevel"), parseInteger(attrs, "minLevel"));
							}
						}
					}
					else if (p.getNodeName().equals("stats"))
					{
						for (Node s = p.getFirstChild(); s != null; s = s.getNextSibling())
						{
							if (s.getNodeName().equals("stat"))
							{
								int level = int.Parse(s.getAttributes().getNamedItem("level").getNodeValue());
								StatSet set = new StatSet();
								for (Node bean = s.getFirstChild(); bean != null; bean = bean.getNextSibling())
								{
									if (bean.getNodeName().equals("set"))
									{
										attrs = bean.getAttributes();
										if (attrs.getNamedItem("name").getNodeValue().equals("speed_on_ride"))
										{
											set.set("walkSpeedOnRide", attrs.getNamedItem("walk").getNodeValue());
											set.set("runSpeedOnRide", attrs.getNamedItem("run").getNodeValue());
											set.set("slowSwimSpeedOnRide", attrs.getNamedItem("slowSwim").getNodeValue());
											set.set("fastSwimSpeedOnRide", attrs.getNamedItem("fastSwim").getNodeValue());
											if (attrs.getNamedItem("slowFly") != null)
											{
												set.set("slowFlySpeedOnRide", attrs.getNamedItem("slowFly").getNodeValue());
											}
											if (attrs.getNamedItem("fastFly") != null)
											{
												set.set("fastFlySpeedOnRide", attrs.getNamedItem("fastFly").getNodeValue());
											}
										}
										else
										{
											set.set(attrs.getNamedItem("name").getNodeValue(), attrs.getNamedItem("val").getNodeValue());
										}
									}
								}
								data.addNewStat(level, new PetLevelData(set));
							}
						}
					}
				}
				_pets.put(npcId, data);
			}
		}
	}
	
	/**
	 * @param itemId
	 * @return
	 */
	public PetData getPetDataByItemId(int itemId)
	{
		foreach (PetData data in _pets.values())
		{
			if (data.getItemId() == itemId)
			{
				return data;
			}
		}
		return null;
	}
	
	/**
	 * Gets the pet level data.
	 * @param petId the pet Id.
	 * @param petLevel the pet level.
	 * @return the pet's parameters for the given Id and level.
	 */
	public PetLevelData getPetLevelData(int petId, int petLevel)
	{
		PetData pd = getPetData(petId);
		if (pd != null)
		{
			if (petLevel > pd.getMaxLevel())
			{
				return pd.getPetLevelData(pd.getMaxLevel());
			}
			return pd.getPetLevelData(petLevel);
		}
		return null;
	}
	
	/**
	 * Gets the pet data.
	 * @param petId the pet Id.
	 * @return the pet data
	 */
	public PetData getPetData(int petId)
	{
		if (!_pets.containsKey(petId))
		{
			LOGGER.Info(GetType().Name + ": Missing pet data for npcid: " + petId);
		}
		return _pets.get(petId);
	}
	
	/**
	 * Gets the pet min level.
	 * @param petId the pet Id.
	 * @return the pet min level
	 */
	public int getPetMinLevel(int petId)
	{
		return _pets.get(petId).getMinLevel();
	}
	
	/**
	 * Gets the pet items by npc.
	 * @param npcId the NPC ID to get its summoning item
	 * @return summoning item for the given NPC ID
	 */
	public int getPetItemsByNpc(int npcId)
	{
		return _pets.get(npcId).getItemId();
	}
	
	/**
	 * Checks if is mountable.
	 * @param npcId the NPC Id to verify.
	 * @return {@code true} if the given Id is from a mountable pet, {@code false} otherwise.
	 */
	public static bool isMountable(int npcId)
	{
		return MountType.findByNpcId(npcId) != MountType.NONE;
	}
	
	public int getTypeByIndex(int index)
	{
		Entry<int, PetData> first = _pets.entrySet().stream().filter(it => it.getValue().getIndex() == index).findFirst().orElse(null);
		return first == null ? 0 : first.getValue().getType();
	}
	
	public PetData getPetDataByEvolve(int itemId, EvolveLevel evolveLevel, int index)
	{
		Optional<Entry<int, PetData>> firstByItem = _pets.entrySet().stream().filter(it => (it.getValue().getItemId() == itemId) && (it.getValue().getIndex() == index) && (it.getValue().getEvolveLevel() == evolveLevel)).findFirst();
		return firstByItem.map(Entry::getValue).orElse(null);
	}
	
	public PetData getPetDataByEvolve(int itemId, EvolveLevel evolveLevel)
	{
		Optional<Entry<int, PetData>> firstByItem = _pets.entrySet().stream().filter(it => (it.getValue().getItemId() == itemId) && (it.getValue().getEvolveLevel() == evolveLevel)).findFirst();
		return firstByItem.map(Entry::getValue).orElse(null);
	}
	
	public List<PetData> getPetDatasByEvolve(int itemId, EvolveLevel evolveLevel)
	{
		return _pets.values().stream().filter(petData => (petData.getItemId() == itemId) && (petData.getEvolveLevel() == evolveLevel)).collect(Collectors.toList());
	}
	
	public void setPetName(int objectId, String name)
	{
		_petNames.put(objectId, name);
	}
	
	public String getPetName(int objectId)
	{
		return _petNames.getOrDefault(objectId, "No name");
	}
	
	public String getNameByItemObjectId(int objectId)
	{
		String name = getPetName(objectId);
		SkillHolder type = PetTypeData.getInstance().getSkillByName(name);
		if (type == null)
		{
			return "";
		}
		return type.getSkillId() + ";" + type.getSkillLevel() + ";" + PetTypeData.getInstance().getIdByName(name);
	}
	
	/**
	 * Gets the single instance of PetDataTable.
	 * @return this class unique instance.
	 */
	public static PetDataTable getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly PetDataTable INSTANCE = new();
	}
}