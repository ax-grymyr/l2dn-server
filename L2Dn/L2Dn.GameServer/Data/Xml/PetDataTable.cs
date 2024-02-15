using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * This class parse and hold all pet parameters.<br>
 * @author Zoey76 (rework)
 */
public class PetDataTable: DataReaderBase
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
		
		LoadXmlDocuments(DataFileLocation.Data, "stats/pets").ForEach(t =>
		{
			t.Document.Elements("pets").Elements("pet").ForEach(x => loadElement(t.FilePath, x));
		});
		
		try
		{
			using GameServerDbContext ctx = new();
			var pets = ctx.Pets.Select(pet => new { pet.ItemObjectId, pet.Name });
			foreach (var pet in pets)
			{
				string? name = pet.Name;
				if (string.IsNullOrEmpty(name))
					name = "No name";

				_petNames.put(pet.ItemObjectId, name);
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Problem loading pet names! " + e);
		}
		
		LOGGER.Info(GetType().Name + ": Loaded " + _pets.size() + " pets.");
	}

	private void loadElement(string filePath, XElement element)
	{
		int npcId = element.Attribute("id").GetInt32();
		int itemId = element.Attribute("itemId").GetInt32();
		int index = element.Attribute("index").GetInt32(0);
		int defaultPetType = element.Attribute("defaultPetType").GetInt32(0);
		EvolveLevel evolveLevel = element.Attribute("evolveLevel").GetEnum<EvolveLevel>();
		int petType = element.Attribute("type").GetInt32(0);

		// index ignored for now
		PetData data = new PetData(npcId, itemId, defaultPetType, evolveLevel, index, petType);

		element.Elements("set").ForEach(el =>
		{
			el.Elements("food").ForEach(e =>
			{
				string[] val = e.Attribute("val").GetString().Split(";");
				foreach (string foodId in val)
					data.addFood(int.Parse(foodId));
			});

			el.Elements("load").ForEach(e =>
			{
				int val = e.Attribute("val").GetInt32();
				data.setLoad(val);
			});

			el.Elements("hungry_limit").ForEach(e =>
			{
				int val = e.Attribute("val").GetInt32();
				data.setHungryLimit(val);
			});

			el.Elements("sync_level").ForEach(e =>
			{
				int val = e.Attribute("val").GetInt32();
				data.setSyncLevel(val == 1);
			});

			// evolve ignored
		});

		element.Elements("skills").Elements("skill").ForEach(el =>
		{
			int skillId = el.Attribute("skillId").GetInt32();
			int skillLevel = el.Attribute("skillLevel").GetInt32();
			int minLevel = el.Attribute("minLevel").GetInt32();
			data.addNewSkill(skillId, skillLevel, minLevel);
		});

		element.Elements("stats").Elements("stat").ForEach(el =>
		{
			int level = el.Attribute("level").GetInt32();
			StatSet set = new StatSet();
			el.Elements("set").ForEach(e =>
			{
				string name = e.Attribute("name").GetString();
				string val = e.Attribute("val").GetString();
				if (name == "speed_on_ride")
				{
					set.set("walkSpeedOnRide", e.Attribute("walk").GetString());
					set.set("runSpeedOnRide", e.Attribute("run").GetString());
					set.set("slowSwimSpeedOnRide", e.Attribute("slowSwim").GetString());
					set.set("fastSwimSpeedOnRide", e.Attribute("fastSwim").GetString());
					if (e.Attribute("slowFly") != null)
					{
						set.set("slowFlySpeedOnRide", e.Attribute("slowFly").GetString());
					}

					if (e.Attribute("fastFly") != null)
					{
						set.set("fastFlySpeedOnRide", e.Attribute("fastFly").GetString());
					}
				}
				else
				{
					set.set(name, val);
				}
			});

			data.addNewStat(level, new PetLevelData(set));
		});

		_pets.put(npcId, data);
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
		return MountTypeUtil.findByNpcId(npcId) != MountType.NONE;
	}
	
	public int getTypeByIndex(int index)
	{
		var first = _pets.FirstOrDefault(it => it.Value.getIndex() == index);
		return first.Value == null ? 0 : first.Value.getType();
	}

	public PetData getPetDataByEvolve(int itemId, EvolveLevel evolveLevel, int index)
	{
		var firstByItem = _pets.FirstOrDefault(it =>
			(it.Value.getItemId() == itemId) && (it.Value.getIndex() == index) &&
			(it.Value.getEvolveLevel() == evolveLevel)).Value;
		return firstByItem;
	}

	public PetData getPetDataByEvolve(int itemId, EvolveLevel evolveLevel)
	{
		var firstByItem = _pets
			.FirstOrDefault(it => (it.Value.getItemId() == itemId) && (it.Value.getEvolveLevel() == evolveLevel)).Value;
		return firstByItem;
	}

	public List<PetData> getPetDatasByEvolve(int itemId, EvolveLevel evolveLevel)
	{
		return _pets.values().Where(petData => (petData.getItemId() == itemId) && (petData.getEvolveLevel() == evolveLevel)).ToList();
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