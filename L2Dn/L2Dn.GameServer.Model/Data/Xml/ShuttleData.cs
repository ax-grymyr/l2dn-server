using System.Runtime.CompilerServices;
using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Shuttles;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author UnAfraid
 */
public class ShuttleData: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ShuttleData));

	private readonly Map<int, ShuttleDataHolder> _shuttles = new();
	private readonly Map<int, Shuttle> _shuttleInstances = new();

	protected ShuttleData()
	{
		load();
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public void load()
	{
		if (_shuttleInstances.Count != 0)
		{
			foreach (Shuttle shuttle in _shuttleInstances.Values)
			{
				shuttle.deleteMe();
			}
			_shuttleInstances.Clear();
		}

		XDocument document = LoadXmlDocument(DataFileLocation.Data, "ShuttleData.xml");
		document.Elements("list").Elements("shuttle").ForEach(parseElement);

		init();
		LOGGER.Info(GetType().Name + ": Loaded " + _shuttles.Count + " shuttles.");
	}

	private void parseElement(XElement element)
	{
		int id = element.GetAttributeValueAsInt32("id");
		int x = element.Attribute("x").GetInt32(0);
		int y = element.Attribute("y").GetInt32(0);
		int z = element.Attribute("z").GetInt32(0);
		int heading = element.Attribute("heading").GetInt32(0);
		ShuttleDataHolder data = new ShuttleDataHolder(id, new Location(x, y, z, heading));

		//string name = element.GetAttributeValueAsString("name");

		element.Elements("doors").Elements("door").ForEach(el =>
		{
			int doorId = el.GetAttributeValueAsInt32("id");
			data.addDoor(doorId);
		});

		element.Elements("stops").Elements("stop").ForEach(el =>
		{
			int stopId = el.GetAttributeValueAsInt32("id");
			ShuttleStop stop = new ShuttleStop(stopId);
			el.Elements("dimension").ForEach(e =>
			{
				int stopX = e.GetAttributeValueAsInt32("x");
				int stopY = e.GetAttributeValueAsInt32("y");
				int stopZ = e.GetAttributeValueAsInt32("z");
				stop.addDimension(new Location3D(stopX, stopY, stopZ));
			});

			data.addStop(stop);
		});

		element.Elements("routes").Elements("route").ForEach(el =>
		{
			List<Location3D> locs = new();
			el.Elements("loc").ForEach(e =>
			{
				int locX = e.GetAttributeValueAsInt32("x");
				int locY = e.GetAttributeValueAsInt32("y");
				int locZ = e.GetAttributeValueAsInt32("z");
				//int locHeading = e.GetAttributeValueAsInt32("heading"); // Heading not used
				locs.Add(new Location3D(locX, locY, locZ));
			});

			VehiclePathPoint[] route = new VehiclePathPoint[locs.Count];
			int i = 0;
			foreach (Location3D loc in locs)
				route[i++] = new VehiclePathPoint(loc);

			data.addRoute(route);
		});

		_shuttles.put(id, data);
	}

	private void init()
	{
		foreach (ShuttleDataHolder data in _shuttles.Values)
		{
			Shuttle shuttle = new Shuttle(new CreatureTemplate(new StatSet()), data);
			shuttle.setHeading(data.Location.Heading);
			shuttle.setLocationInvisible(data.Location.Location3D);
			shuttle.spawnMe();
			shuttle.getStat().setMoveSpeed(300);
			shuttle.getStat().setRotationSpeed(0);
			shuttle.registerEngine(new ShuttleEngine(data, shuttle));
			shuttle.runEngine(1000);
			_shuttleInstances.put(shuttle.ObjectId, shuttle);
		}
	}

	public Shuttle? getShuttle(int id)
	{
		foreach (Shuttle shuttle in _shuttleInstances.Values)
		{
			if (shuttle.ObjectId == id || shuttle.getId() == id)
			{
				return shuttle;
			}
		}
		return null;
	}

	public static ShuttleData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly ShuttleData INSTANCE = new();
	}
}