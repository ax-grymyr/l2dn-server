using System.Runtime.CompilerServices;
using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author UnAfraid
 */
public class SiegeScheduleData: DataReaderBase
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
		XDocument document = LoadXmlDocument(DataFileLocation.Config, "SiegeSchedule.xml");
		document.Elements("list").Elements("schedule").ForEach(parseElement);

		LOGGER.Info(GetType().Name + ": Loaded " + _scheduleData.size() + " siege schedulers.");
	}

	private void parseElement(XElement element)
	{
		int castleId = element.GetAttributeValueAsInt32("castleId");
		DayOfWeek day = element.Attribute("day").GetEnum<DayOfWeek>();
		int hour = element.Attribute("hour").GetInt32(16);
		int maxConcurrent = element.Attribute("maxConcurrent").GetInt32(5);
		bool siegeEnabled = element.Attribute("siegeEnabled").GetBoolean(false);
		_scheduleData.put(castleId, new SiegeScheduleDate(day, hour, maxConcurrent, siegeEnabled));
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