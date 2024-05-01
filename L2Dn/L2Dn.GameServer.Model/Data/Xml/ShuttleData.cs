using System.Runtime.CompilerServices;
using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Shuttles;
using L2Dn.GameServer.Utilities;
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
		if (!_shuttleInstances.isEmpty())
		{
			foreach (Shuttle shuttle in _shuttleInstances.values())
			{
				shuttle.deleteMe();
			}
			_shuttleInstances.clear();
		}
		
		XDocument document = LoadXmlDocument(DataFileLocation.Data, "ShuttleData.xml");
		document.Elements("list").Elements("shuttle").ForEach(parseElement);
		
		init();
		LOGGER.Info(GetType().Name + ": Loaded " + _shuttles.size() + " shuttles.");
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
				stop.addDimension(new Location(stopX, stopY, stopZ));
			});

			data.addStop(stop);
		});

		element.Elements("routes").Elements("route").ForEach(el =>
		{
			List<Location> locs = new();
			el.Elements("loc").ForEach(e =>
			{
				int locX = e.GetAttributeValueAsInt32("x");
				int locY = e.GetAttributeValueAsInt32("y");
				int locZ = e.GetAttributeValueAsInt32("z");
				int locHeading = e.GetAttributeValueAsInt32("heading");
				locs.add(new Location(locX, locY, locZ, locHeading));
			});
										
			VehiclePathPoint[] route = new VehiclePathPoint[locs.size()];
			int i = 0;
			foreach (Location loc in locs)
				route[i++] = new VehiclePathPoint(loc);
			
			data.addRoute(route);
		});

		_shuttles.put(id, data);
	}

	private void init()
	{
		foreach (ShuttleDataHolder data in _shuttles.values())
		{
			Shuttle shuttle = new Shuttle(new CreatureTemplate(new StatSet()));
			shuttle.setData(data);
			shuttle.setHeading(data.getLocation().getHeading());
			shuttle.setLocationInvisible(data.getLocation().ToLocation3D());
			shuttle.spawnMe();
			shuttle.getStat().setMoveSpeed(300);
			shuttle.getStat().setRotationSpeed(0);
			shuttle.registerEngine(new ShuttleEngine(data, shuttle));
			shuttle.runEngine(1000);
			_shuttleInstances.put(shuttle.getObjectId(), shuttle);
		}
	}
	
	public Shuttle getShuttle(int id)
	{
		foreach (Shuttle shuttle in _shuttleInstances.values())
		{
			if ((shuttle.getObjectId() == id) || (shuttle.getId() == id))
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