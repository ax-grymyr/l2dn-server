using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model.Vips;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Gabriel Costa Souza
 */
public class VipData: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(VipData));

	private readonly Map<int, VipInfo> _vipTiers = new();

	protected VipData()
	{
		load();
	}

	public void load()
	{
		if (!Config.VipSystem.VIP_SYSTEM_ENABLED)
		{
			return;
		}

		_vipTiers.Clear();

		XDocument document = LoadXmlDocument(DataFileLocation.Data, "Vip.xml");
		document.Elements("list").Elements("vip").ForEach(parseElement);

		LOGGER.Info(GetType().Name + ": Loaded " + _vipTiers.Count + " vips.");
	}

	private void parseElement(XElement element)
	{
		int tier = element.GetAttributeValueAsInt32("tier");
		int pointsRequired = element.Attribute("points-required").GetInt32();
		int pointsLose = element.Attribute("points-lose").GetInt32();
		VipInfo vipInfo = new VipInfo(tier, pointsRequired, pointsLose);
		element.Elements("bonus").ForEach(el =>
		{
			int skill = el.GetAttributeValueAsInt32("skill");
			vipInfo.setSkill(skill);
		});

		_vipTiers.put(tier, vipInfo);
	}

	/**
	 * Gets the single instance of VipData.
	 * @return single instance of VipData
	 */
	public static VipData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	/**
	 * The Class SingletonHolder.
	 */
	private static class SingletonHolder
	{
		public static readonly VipData INSTANCE = new();
	}

	public int getSkillId(int tier)
	{
		return _vipTiers.get(tier)?.getSkill() ?? 0;
	}

	public Map<int, VipInfo> getVipTiers()
	{
		return _vipTiers;
	}
}