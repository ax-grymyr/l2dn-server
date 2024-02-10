using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author UnAfraid
 */
public class SiegeScheduleData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(SiegeScheduleData));
	
	private readonly Map<int, SiegeScheduleDate> _scheduleData = new();
	
	protected SiegeScheduleData()
	{
		load();
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)] 
	public void load()
	{
		parseDatapackFile("config/SiegeSchedule.xml");
		LOGGER.Info(GetType().Name + ": Loaded " + _scheduleData.size() + " siege schedulers.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		for (Node n = doc.getFirstChild(); n != null; n = n.getNextSibling())
		{
			if ("list".equalsIgnoreCase(n.getNodeName()))
			{
				for (Node cd = n.getFirstChild(); cd != null; cd = cd.getNextSibling())
				{
					switch (cd.getNodeName())
					{
						case "schedule":
						{
							StatSet set = new StatSet();
							NamedNodeMap attrs = cd.getAttributes();
							for (int i = 0; i < attrs.getLength(); i++)
							{
								Node node = attrs.item(i);
								String key = node.getNodeName();
								String val = node.getNodeValue();
								if ("day".equals(key) && !Util.isDigit(val))
								{
									val = int.toString(getValueForField(val));
								}
								set.set(key, val);
							}
							_scheduleData.put(set.getInt("castleId"), new SiegeScheduleDate(set));
							break;
						}
					}
				}
			}
		}
	}
	
	private int getValueForField(String field)
	{
		try
		{
			return Calendar.class.getField(field).getInt(Calendar.class.getName());
		}
		catch (Exception e)
		{
			LOGGER.Warn("", e);
			return -1;
		}
	}
	
	public SiegeScheduleDate getScheduleDateForCastleId(int castleId)
	{
		return _scheduleData.get(castleId);
	}
	
	public static SiegeScheduleData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly SiegeScheduleData INSTANCE = new();
	}
}