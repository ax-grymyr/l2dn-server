using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * This class loads and hold info about doors.
 * @author JIV, GodKratos, UnAfraid
 */
public class DoorData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(DoorData));
	
	// Info holders
	private readonly Map<String, Set<int>> _groups = new();
	private readonly Map<int, Door> _doors = new();
	private readonly Map<int, StatSet> _templates = new();
	
	protected DoorData()
	{
		load();
	}
	
	public void load()
	{
		_doors.clear();
		_groups.clear();
		
		string filePath = Path.Combine(Config.DATAPACK_ROOT_PATH, "data/DoorData.xml");
		using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
		XDocument document = XDocument.Load(stream);
		document.Elements("list").Elements("door").ForEach(element => spawnDoor(parseDoor(element)));
		LOGGER.Info(GetType().Name + ": Loaded " + _doors.size() + " doors.");
	}
	
	private StatSet parseDoor(XElement doorNode)
	{
		StatSet stat = new StatSet();
		foreach (XAttribute attribute in doorNode.Attributes())
			stat.set(attribute.Name.LocalName, attribute.Value);
		
		stat.set("baseHpMax", 1); // Avoid doors without HP value created dead due to default value 0 in CreatureTemplate

		doorNode.Elements("nodes").ForEach(nodesEl =>
		{
			stat.set("nodeZ", nodesEl.Attribute("nodeZ").GetInt32());
			
			int count = 0;
			nodesEl.Elements("node").ForEach(nodeEl =>
			{
				stat.set("nodeX_" + count, nodeEl.Attribute("x").GetInt32());
				stat.set("nodeY_" + count, nodeEl.Attribute("y").GetInt32());
			});
		});

		doorNode.Elements().Where(el => el.Name.LocalName != "nodes").Attributes()
			.ForEach(a => stat.set(a.Name.LocalName, a.Value));
		
		applyCollisions(stat);
		return stat;
	}
	
	/**
	 * @param set
	 */
	private void applyCollisions(StatSet set)
	{
		// Insert Collision data
		if (set.Contains("nodeX_0") && set.Contains("nodeY_0") && set.Contains("nodeX_1") && set.Contains("nodeY_1"))
		{
			int height = set.getInt("height", 150);
			int nodeX = set.getInt("nodeX_0");
			int nodeY = set.getInt("nodeY_0");
			int posX = set.getInt("nodeX_1");
			int posY = set.getInt("nodeY_1");
			int collisionRadius; // (max) radius for movement checks
			collisionRadius = Math.Min(Math.Abs(nodeX - posX), Math.Abs(nodeY - posY));
			if (collisionRadius < 20)
			{
				collisionRadius = 20;
			}
			
			set.set("collision_radius", collisionRadius);
			set.set("collision_height", height);
		}
	}
	
	/**
	 * Spawns the door, adds the group name and registers it to templates, regions and doors also inserts collisions data
	 * @param set
	 * @return
	 */
	public Door spawnDoor(StatSet set)
	{
		// Create door template + door instance
		DoorTemplate template = new DoorTemplate(set);
		Door door = spawnDoor(template, null);
		
		// Register the door
		_templates.put(door.getId(), set);
		_doors.put(door.getId(), door);
		
		return door;
	}
	
	/**
	 * Spawns the door, adds the group name and registers it to templates
	 * @param template
	 * @param instance
	 * @return a new door instance based on provided template
	 */
	public Door spawnDoor(DoorTemplate template, Instance instance)
	{
		Door door = new Door(template);
		door.setCurrentHp(door.getMaxHp());
		
		// Set instance world if provided
		if (instance != null)
		{
			door.setInstance(instance);
		}
		
		// Spawn the door on the world
		door.spawnMe(template.getX(), template.getY(), template.getZ());
		
		// Register door's group
		if (template.getGroupName() != null)
		{
			_groups.computeIfAbsent(door.getGroupName(), key => new()).add(door.getId());
		}
		return door;
	}
	
	public StatSet getDoorTemplate(int doorId)
	{
		return _templates.get(doorId);
	}
	
	public Door getDoor(int doorId)
	{
		return _doors.get(doorId);
	}
	
	public Set<int> getDoorsByGroup(String groupName)
	{
		return _groups.getOrDefault(groupName, new());
	}
	
	public ICollection<Door> getDoors()
	{
		return _doors.values();
	}
	
	public bool checkIfDoorsBetween(Location start, Location end, Instance instance)
	{
		return checkIfDoorsBetween(start.getX(), start.getY(), start.getZ(), end.getX(), end.getY(), end.getZ(), instance);
	}
	
	public bool checkIfDoorsBetween(int x, int y, int z, int tx, int ty, int tz, Instance instance)
	{
		return checkIfDoorsBetween(x, y, z, tx, ty, tz, instance, false);
	}
	
	public bool checkIfDoorsBetween(int x, int y, int z, int tx, int ty, int tz, Instance instance, bool doubleFaceCheck)
	{
		ICollection<Door> doors;
		if (instance == null)
		{
			WorldRegion region = World.getInstance().getRegion(x, y);
			if (region != null)
			{
				doors = region.getDoors();
			}
			else
			{
				doors = null;
			}
		}
		else
		{
			doors = instance.getDoors();
		}
		if ((doors == null) || doors.isEmpty())
		{
			return false;
		}
		
		foreach (Door doorInst in doors)
		{
			// check dead and open
			if ((instance != doorInst.getInstanceWorld()) || doorInst.isDead() || doorInst.isOpen() || !doorInst.checkCollision() || (doorInst.getX(0) == 0))
			{
				continue;
			}
			
			bool intersectFace = false;
			for (int i = 0; i < 4; i++)
			{
				int j = (i + 1) < 4 ? i + 1 : 0;
				// lower part of the multiplier fraction, if it is 0 we avoid an error and also know that the lines are parallel
				int denominator = ((ty - y) * (doorInst.getX(i) - doorInst.getX(j))) - ((tx - x) * (doorInst.getY(i) - doorInst.getY(j)));
				if (denominator == 0)
				{
					continue;
				}
				
				// multipliers to the equations of the lines. If they are lower than 0 or bigger than 1, we know that segments don't intersect
				float multiplier1 = (float) (((doorInst.getX(j) - doorInst.getX(i)) * (y - doorInst.getY(i))) - ((doorInst.getY(j) - doorInst.getY(i)) * (x - doorInst.getX(i)))) / denominator;
				float multiplier2 = (float) (((tx - x) * (y - doorInst.getY(i))) - ((ty - y) * (x - doorInst.getX(i)))) / denominator;
				if ((multiplier1 >= 0) && (multiplier1 <= 1) && (multiplier2 >= 0) && (multiplier2 <= 1))
				{
					int intersectZ = (int)Math.Round(z + (multiplier1 * (tz - z)));
					// now checking if the resulting point is between door's min and max z
					if ((intersectZ > doorInst.getZMin()) && (intersectZ < doorInst.getZMax()))
					{
						if (!doubleFaceCheck || intersectFace)
						{
							return true;
						}
						intersectFace = true;
					}
				}
			}
		}
		return false;
	}
	
	public static DoorData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly DoorData INSTANCE = new();
	}
}