using System.Runtime.CompilerServices;
using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model.Items.Combination;
using L2Dn.GameServer.Model.Items.Henna;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Index
 */
public class HennaCombinationData: DataReaderBase
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
		
		XDocument document = LoadXmlDocument(DataFileLocation.Data, "stats/hennaCombinations.xml");
		document.Elements("list").Elements("henna").ForEach(parseElement);
		
		LOGGER.Info(GetType().Name + ": Loaded " + _henna.size() + " henna combinations.");
	}

	private void parseElement(XElement element)
	{
		CombinationHenna henna = new CombinationHenna(element);
		element.Elements("reward").ForEach(el =>
		{
			int hennaId = el.GetAttributeValueAsInt32("dyeId");
			int id = el.Attribute("id").GetInt32(-1);
			int count = el.Attribute("count").GetInt32(0);
			CombinationItemType type = el.Attribute("type").GetEnum<CombinationItemType>();
			henna.addReward(new CombinationHennaReward(hennaId, id, count, type));
			if (id != -1 && ItemData.getInstance().getTemplate(id) == null)
			{
				LOGGER.Error(GetType().Name + ": Could not find item with id " + id);
			}

			if (hennaId != 0 && HennaData.getInstance().getHenna(hennaId) == null)
			{
				LOGGER.Error(GetType().Name + ": Could not find henna with id " + hennaId);
			}
		});

		_henna.add(henna);
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