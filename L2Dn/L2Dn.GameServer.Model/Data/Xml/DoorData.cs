using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.DataPack;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * This class loads and hold info about doors.
 * @author JIV, GodKratos, UnAfraid
 */
public class DoorData: DataReaderBase
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(DoorData));
	
	// Info holders
	private readonly Map<string, Set<int>> _groups = new();
	private readonly Map<int, Door> _doors = new();
	private readonly Map<int, DoorTemplate> _templates = new();
	
	private DoorData()
	{
		load();
	}
	
	public void load()
	{
		_doors.clear();
		_groups.clear();
		
		LoadXmlDocument<XmlDoorList>(DataFileLocation.Data, "DoorData.xml")
			.Doors.ForEach(xmlDoor =>
			{
				DoorTemplate template = new(xmlDoor);
				_templates.TryAdd(template.getId(), template);
				spawnDoor(template);
			});
		
		_logger.Info(GetType().Name + ": Loaded " + _doors.Count + " doors.");
	}
	
	/**
	 * Spawns the door, adds the group name and registers it to templates, regions and doors also inserts collisions data
	 * @param set
	 * @return
	 */
	public Door spawnDoor(DoorTemplate template)
	{
		Door door = spawnDoor(template, null);
		
		// Register the door
		_doors.put(door.getId(), door);
		
		return door;
	}
	
	/**
	 * Spawns the door, adds the group name and registers it to templates
	 * @param template
	 * @param instance
	 * @return a new door instance based on provided template
	 */
	public Door spawnDoor(DoorTemplate template, Instance instance, bool? isOpened = null)
	{
		Door door = new Door(template, isOpened);
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
	
	public DoorTemplate? getDoorTemplate(int doorId)
	{
		return _templates.GetValueOrDefault(doorId);
	}
	
	public Door? getDoor(int doorId)
	{
		return _doors.GetValueOrDefault(doorId);
	}
	
	public Set<int> getDoorsByGroup(string groupName)
	{
		return _groups.getOrDefault(groupName, new());
	}
	
	public ICollection<Door> getDoors()
	{
		return _doors.values();
	}
	
	public bool checkIfDoorsBetween(Location start, Location end, Instance? instance = null)
	{
		return checkIfDoorsBetween(start.getX(), start.getY(), start.getZ(), end.getX(), end.getY(), end.getZ(), instance);
	}
	
	public bool checkIfDoorsBetween(int x, int y, int z, int tx, int ty, int tz, Instance? instance = null,
		bool doubleFaceCheck = false)
	{
		ICollection<Door>? doors = instance?.getDoors() ?? World.getInstance().getRegion(x, y)?.getDoors();
		if (doors == null || doors.Count == 0)
			return false;

		foreach (Door doorInst in doors)
		{
			// check dead and open
			if (instance != doorInst.getInstanceWorld() || doorInst.isDead() || doorInst.isOpen() ||
			    !doorInst.checkCollision() || doorInst.getX(0) == 0)
			{
				continue;
			}

			bool intersectFace = false;
			for (int i = 0; i < 4; i++)
			{
				int j = i + 1 < 4 ? i + 1 : 0;
				// lower part of the multiplier fraction, if it is 0 we avoid an error and also know that the lines are parallel
				int denominator = (ty - y) * (doorInst.getX(i) - doorInst.getX(j)) -
				                  (tx - x) * (doorInst.getY(i) - doorInst.getY(j));
				if (denominator == 0)
				{
					continue;
				}

				// multipliers to the equations of the lines. If they are lower than 0 or bigger than 1, we know that segments don't intersect
				float multiplier1 = (float)((doorInst.getX(j) - doorInst.getX(i)) * (y - doorInst.getY(i)) -
				                            (doorInst.getY(j) - doorInst.getY(i)) * (x - doorInst.getX(i))) /
				                    denominator;
				
				float multiplier2 = (float)((tx - x) * (y - doorInst.getY(i)) - (ty - y) * (x - doorInst.getX(i))) /
				                    denominator;
				
				if (multiplier1 >= 0 && multiplier1 <= 1 && multiplier2 >= 0 && multiplier2 <= 1)
				{
					int intersectZ = (int)Math.Round(z + multiplier1 * (tz - z));
					
					// now checking if the resulting point is between door's min and max z
					if (intersectZ > doorInst.getZMin() && intersectZ < doorInst.getZMax())
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