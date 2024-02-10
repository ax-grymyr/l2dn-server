using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Residences;
using L2Dn.GameServer.Utilities;
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
		parseDatapackFile("data/ResidenceFunctions.xml");
		LOGGER.Info(GetType().Name + ": Loaded " + _functions.size() + " functions.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		forEach(doc, "list", list => forEach(list, "function", func =>
		{
			NamedNodeMap attrs = func.getAttributes();
			StatSet set = new StatSet(HashMap::new);
			for (int i = 0; i < attrs.getLength(); i++)
			{
				Node node = attrs.item(i);
				set.set(node.getNodeName(), node.getNodeValue());
			}
			forEach(func, "function", levelNode =>
			{
				NamedNodeMap levelAttrs = levelNode.getAttributes();
				StatSet levelSet = new StatSet(HashMap::new);
				levelSet.merge(set);
				for (int i = 0; i < levelAttrs.getLength(); i++)
				{
					Node node = levelAttrs.item(i);
					levelSet.set(node.getNodeName(), node.getNodeValue());
				}
				ResidenceFunctionTemplate template = new ResidenceFunctionTemplate(levelSet);
				_functions.computeIfAbsent(template.getId(), key => new()).add(template);
			});
		}));
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