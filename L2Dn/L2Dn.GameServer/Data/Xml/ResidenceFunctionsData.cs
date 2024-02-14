using System.Runtime.CompilerServices;
using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Residences;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * The residence functions data
 * @author UnAfraid
 */
public class ResidenceFunctionsData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ResidenceFunctionsData));
	private readonly Map<int, List<ResidenceFunctionTemplate>> _functions = new();
	
	protected ResidenceFunctionsData()
	{
		load();
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)] 
	public void load()
	{
		_functions.clear();
		
		string filePath = Path.Combine(Config.DATAPACK_ROOT_PATH, "data/ResidenceFunctions.xml");
		using FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
		XDocument document = XDocument.Load(stream);
		document.Elements("list").Elements("function").ForEach(parseElement);
		
		LOGGER.Info(GetType().Name + ": Loaded " + _functions.size() + " functions.");
	}

	private void parseElement(XElement element)
	{
		int id = element.Attribute("id").GetInt32();
		ResidenceFunctionType type = element.Attribute("type").GetEnum<ResidenceFunctionType>();

		element.Elements("function").ForEach(el =>
		{
			int level = el.Attribute("level").GetInt32();
			int costId = el.Attribute("costId").GetInt32();
			long costCount = el.Attribute("costCount").GetInt64();
			TimeSpan duration = el.Attribute("duration").GetTimeSpan();
			double value = el.Attribute("value").GetDouble(0);

			ItemHolder cost = new ItemHolder(costId, costCount);
			ResidenceFunctionTemplate template = new ResidenceFunctionTemplate(id, level, type, cost, duration, value);
			_functions.computeIfAbsent(template.getId(), key => new()).add(template);
		});
	}

	/**
	 * @param id
	 * @param level
	 * @return function template by id and level, null if not available
	 */
	public ResidenceFunctionTemplate getFunction(int id, int level)
	{
		if (_functions.containsKey(id))
		{
			foreach (ResidenceFunctionTemplate template in _functions.get(id))
			{
				if (template.getLevel() == level)
				{
					return template;
				}
			}
		}
		return null;
	}
	
	/**
	 * @param id
	 * @return function template by id, null if not available
	 */
	public List<ResidenceFunctionTemplate> getFunctions(int id)
	{
		return _functions.get(id);
	}
	
	public static ResidenceFunctionsData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly ResidenceFunctionsData INSTANCE = new();
	}
}