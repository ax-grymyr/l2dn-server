using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Henna;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * This class holds the henna related information.<br>
 * Cost and required amount to add the henna to the player.<br>
 * Cost and retrieved amount for removing the henna from the player.<br>
 * Allowed classes to wear each henna.
 * @author Zoey76, Mobius
 */
public class HennaData: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(HennaData));

	private readonly Map<int, Henna> _hennaDyeIdList = new();
	private readonly Map<int, Henna> _hennaItemIdList = new();

	/**
	 * Instantiates a new henna data.
	 */
	protected HennaData()
	{
		load();
	}

	public void load()
	{
		_hennaItemIdList.Clear();
		_hennaDyeIdList.Clear();

		XDocument document = LoadXmlDocument(DataFileLocation.Data, "stats/hennaList.xml");
		document.Elements("list").Elements("henna").ForEach(parseElement);

		LOGGER.Info(GetType().Name + ": Loaded " + _hennaDyeIdList.Count + " henna data.");
	}

	/**
	 * Parses the henna.
	 * @param d the node
	 */
	private void parseElement(XElement element)
	{
		List<int> wearClassIds = new();
		List<Skill> skills = new();
		StatSet set = new();

		foreach (XAttribute attribute in element.Attributes())
			set.set(attribute.Name.LocalName, attribute.Value);

		element.Elements("stats").Attributes().ForEach(a => set.set(a.Name.LocalName, a.Value));

		element.Elements("wear").ForEach(e =>
		{
			int count = e.GetAttributeValueAsInt32("count");
			set.set("wear_count", count);
			int fee = e.GetAttributeValueAsInt32("fee");
			set.set("wear_fee", fee);

			fee = e.Attribute("l2coinfee").GetInt32(0);
			set.set("l2coin_fee", fee);
		});

		element.Elements("cancel").ForEach(e =>
		{
			int count = e.GetAttributeValueAsInt32("count");
			set.set("cancel_count", count);
			int fee = e.GetAttributeValueAsInt32("fee");
			set.set("cancel_fee", fee);

			fee = e.Attribute("l2coinfee_cancel").GetInt32(0);
			set.set("cancel_l2coin_fee", fee);
		});

		element.Elements("duration").ForEach(e =>
		{
			int duration = e.Attribute("time").GetInt32(-1); // in minutes
			set.set("duration", duration);
		});

		element.Elements("skill").ForEach(e =>
		{
			int id = e.GetAttributeValueAsInt32("id");
			int level = e.GetAttributeValueAsInt32("level");
			skills.Add(SkillData.getInstance().getSkill(id, level));
		});

		element.Elements("classId").ForEach(e =>
		{
			string[] ids = ((string)e).Split(",");
			foreach (string s in ids)
				wearClassIds.Add(int.Parse(s));
		});

		Henna henna = new Henna(set);
		henna.setSkills(skills);
		henna.setWearClassIds(wearClassIds);
		_hennaDyeIdList.put(henna.getDyeId(), henna);
		_hennaItemIdList.put(henna.getDyeItemId(), henna);
	}

	/**
	 * Gets the henna.
	 * @param id of the dye.
	 * @return the dye with that id.
	 */
	public Henna? getHenna(int id)
	{
		return _hennaDyeIdList.get(id);
	}

	public Henna? getHennaByDyeId(int id)
	{
		return _hennaDyeIdList.get(id);
	}

	public Henna? getHennaByItemId(int id)
	{
		return _hennaItemIdList.get(id);
	}

	/**
	 * Gets the henna list.
	 * @param player the player's class Id.
	 * @return the list with all the allowed dyes.
	 */
	public List<Henna> getHennaList(Player player)
	{
		List<Henna> list = new();
		foreach (Henna henna in _hennaDyeIdList.Values)
		{
			if (henna.isAllowedClass(player))
			{
				list.Add(henna);
			}
		}
		return list;
	}

	/**
	 * Gets the single instance of HennaData.
	 * @return single instance of HennaData
	 */
	public static HennaData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly HennaData INSTANCE = new();
	}
}