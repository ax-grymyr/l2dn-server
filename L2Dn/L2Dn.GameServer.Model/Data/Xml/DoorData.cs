using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Model.Xml;
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
		_doors.Clear();
		_groups.Clear();

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
	public Door spawnDoor(DoorTemplate template, Instance? instance, bool? isOpened = null)
	{
		Door door = new Door(template, isOpened);
		door.setCurrentHp(door.getMaxHp());

		// Set instance world if provided
		if (instance != null)
		{
			door.setInstance(instance);
		}

		// Spawn the door on the world
		door.spawnMe(template.Location);

		// Register door's group
		if (template.getGroupName() != null)
		{
			_groups.GetOrAdd(door.getGroupName(), _ => []).add(door.getId());
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
		return _groups.GetValueOrDefault(groupName, []);
	}

	public ICollection<Door> getDoors()
	{
		return _doors.Values;
	}

	public bool checkIfDoorsBetween(Location3D location, Location3D targetLocation, Instance? instance = null,
		bool doubleFaceCheck = false)
    {
        IEnumerable<Door> doors;
        if (instance != null)
        {
            ICollection<Door> instanceDoors = instance.getDoors();
            if (instanceDoors.Count == 0)
                return false;

            doors = instanceDoors;
        }
        else
        {
            IReadOnlyCollection<Door> regionDoors = World.getInstance().getRegion(location.X, location.Y).Doors;
            if (regionDoors.Count == 0)
                return false;

            doors = regionDoors;
        }

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
				int denominator = (targetLocation.Y - location.Y) * (doorInst.getX(i) - doorInst.getX(j)) -
				                  (targetLocation.X - location.X) * (doorInst.getY(i) - doorInst.getY(j));
				if (denominator == 0)
				{
					continue;
				}

				// multipliers to the equations of the lines. If they are lower than 0 or bigger than 1, we know that segments don't intersect
				float multiplier1 = (float)((doorInst.getX(j) - doorInst.getX(i)) * (location.Y - doorInst.getY(i)) -
					(doorInst.getY(j) - doorInst.getY(i)) * (location.X - doorInst.getX(i))) / denominator;

				float multiplier2 = (float)((targetLocation.X - location.X) * (location.Y - doorInst.getY(i)) -
					(targetLocation.Y - location.Y) * (location.X - doorInst.getX(i))) / denominator;

				if (multiplier1 >= 0 && multiplier1 <= 1 && multiplier2 >= 0 && multiplier2 <= 1)
				{
					int intersectZ = (int)Math.Round(location.Z + multiplier1 * (targetLocation.Z - location.Z));

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