using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Shuttles;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author UnAfraid
 */
public class ShuttleData
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
		parseDatapackFile("data/ShuttleData.xml");
		init();
		LOGGER.Info(GetType().Name + ": Loaded " + _shuttles.size() + " shuttles.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		NamedNodeMap attrs;
		StatSet set;
		Node att;
		ShuttleDataHolder data;
		for (Node n = doc.getFirstChild(); n != null; n = n.getNextSibling())
		{
			if ("list".equalsIgnoreCase(n.getNodeName()))
			{
				for (Node d = n.getFirstChild(); d != null; d = d.getNextSibling())
				{
					if ("shuttle".equalsIgnoreCase(d.getNodeName()))
					{
						attrs = d.getAttributes();
						set = new StatSet();
						for (int i = 0; i < attrs.getLength(); i++)
						{
							att = attrs.item(i);
							set.set(att.getNodeName(), att.getNodeValue());
						}
						data = new ShuttleDataHolder(set);
						for (Node b = d.getFirstChild(); b != null; b = b.getNextSibling())
						{
							if ("doors".equalsIgnoreCase(b.getNodeName()))
							{
								for (Node a = b.getFirstChild(); a != null; a = a.getNextSibling())
								{
									if ("door".equalsIgnoreCase(a.getNodeName()))
									{
										attrs = a.getAttributes();
										data.addDoor(parseInteger(attrs, "id"));
									}
								}
							}
							else if ("stops".equalsIgnoreCase(b.getNodeName()))
							{
								for (Node a = b.getFirstChild(); a != null; a = a.getNextSibling())
								{
									if ("stop".equalsIgnoreCase(a.getNodeName()))
									{
										attrs = a.getAttributes();
										ShuttleStop stop = new ShuttleStop(parseInteger(attrs, "id"));
										for (Node z = a.getFirstChild(); z != null; z = z.getNextSibling())
										{
											if ("dimension".equalsIgnoreCase(z.getNodeName()))
											{
												attrs = z.getAttributes();
												stop.addDimension(new Location(parseInteger(attrs, "x"), parseInteger(attrs, "y"), parseInteger(attrs, "z")));
											}
										}
										data.addStop(stop);
									}
								}
							}
							else if ("routes".equalsIgnoreCase(b.getNodeName()))
							{
								for (Node a = b.getFirstChild(); a != null; a = a.getNextSibling())
								{
									if ("route".equalsIgnoreCase(a.getNodeName()))
									{
										attrs = a.getAttributes();
										List<Location> locs = new();
										for (Node z = a.getFirstChild(); z != null; z = z.getNextSibling())
										{
											if ("loc".equalsIgnoreCase(z.getNodeName()))
											{
												attrs = z.getAttributes();
												locs.add(new Location(parseInteger(attrs, "x"), parseInteger(attrs, "y"), parseInteger(attrs, "z")));
											}
										}
										
										VehiclePathPoint[] route = new VehiclePathPoint[locs.size()];
										int i = 0;
										foreach (Location loc in locs)
										{
											route[i++] = new VehiclePathPoint(loc);
										}
										data.addRoute(route);
									}
								}
							}
						}
						_shuttles.put(data.getId(), data);
					}
				}
			}
		}
	}
	
	private void init()
	{
		foreach (ShuttleDataHolder data in _shuttles.values())
		{
			Shuttle shuttle = new Shuttle(new CreatureTemplate(new StatSet()));
			shuttle.setData(data);
			shuttle.setHeading(data.getLocation().getHeading());
			shuttle.setLocationInvisible(data.getLocation());
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