using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model.Items.Henna;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Index
 */
public class HennaCombinationData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(HennaCombinationData));
	
	private readonly List<CombinationHenna> _henna = new();
	
	protected HennaCombinationData()
	{
		load();
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)] 
	public void load()
	{
		_henna.Clear();
		parseDatapackFile("data/stats/hennaCombinations.xml");
		LOGGER.Info(GetType().Name + ": Loaded " + _henna.size() + " henna combinations.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		forEach(doc, "list", listNode =>
		{
			forEach(listNode, "henna", hennaNode =>
			{
				CombinationHenna henna = new CombinationHenna(new StatSet(parseAttributes(hennaNode)));
				forEach(hennaNode, "reward", rewardNode =>
				{
					int hennaId = parseInteger(rewardNode.getAttributes(), "dyeId");
					int id = parseInteger(rewardNode.getAttributes(), "id", -1);
					int count = parseInteger(rewardNode.getAttributes(), "count", 0);
					CombinationItemType type = parseEnum(rewardNode.getAttributes(), CombinationItemType.class, "type");
					henna.addReward(new CombinationHennaReward(hennaId, id, count, type));
					if ((id != -1) && (ItemData.getInstance().getTemplate(id) == null))
					{
						LOGGER.Info(GetType().Name + ": Could not find item with id " + id);
					}
					if ((hennaId != 0) && (HennaData.getInstance().getHenna(hennaId) == null))
					{
						LOGGER.Info(GetType().Name + ": Could not find henna with id " + id);
					}
				});
				_henna.add(henna);
			});
		});
	}
	
	public List<CombinationHenna> getHenna()
	{
		return _henna;
	}
	
	public CombinationHenna getByHenna(int hennaId)
	{
		foreach (CombinationHenna henna in _henna)
		{
			if (henna.getHenna() == hennaId)
			{
				return henna;
			}
		}
		return null;
	}
	
	public static HennaCombinationData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly HennaCombinationData INSTANCE = new();
	}
}