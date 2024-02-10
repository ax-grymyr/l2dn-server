using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Henna;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * This class holds the henna related information.<br>
 * Cost and required amount to add the henna to the player.<br>
 * Cost and retrieved amount for removing the henna from the player.<br>
 * Allowed classes to wear each henna.
 * @author Zoey76, Mobius
 */
public class HennaData
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
		_hennaItemIdList.clear();
		_hennaDyeIdList.clear();
		parseDatapackFile("data/stats/hennaList.xml");
		LOGGER.Info(GetType().Name + ": Loaded " + _hennaDyeIdList.size() + " henna data.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		for (Node n = doc.getFirstChild(); n != null; n = n.getNextSibling())
		{
			if ("list".equals(n.getNodeName()))
			{
				for (Node d = n.getFirstChild(); d != null; d = d.getNextSibling())
				{
					if ("henna".equals(d.getNodeName()))
					{
						parseHenna(d);
					}
				}
			}
		}
	}
	
	/**
	 * Parses the henna.
	 * @param d the node
	 */
	private void parseHenna(Node d)
	{
		StatSet set = new StatSet();
		List<int> wearClassIds = new();
		List<Skill> skills = new();
		NamedNodeMap attrs = d.getAttributes();
		Node attr;
		for (int i = 0; i < attrs.getLength(); i++)
		{
			attr = attrs.item(i);
			set.set(attr.getNodeName(), attr.getNodeValue());
		}
		
		for (Node c = d.getFirstChild(); c != null; c = c.getNextSibling())
		{
			String name = c.getNodeName();
			attrs = c.getAttributes();
			switch (name)
			{
				case "stats":
				{
					for (int i = 0; i < attrs.getLength(); i++)
					{
						attr = attrs.item(i);
						set.set(attr.getNodeName(), attr.getNodeValue());
					}
					break;
				}
				case "wear":
				{
					attr = attrs.getNamedItem("count");
					set.set("wear_count", attr.getNodeValue());
					attr = attrs.getNamedItem("fee");
					set.set("wear_fee", attr.getNodeValue());
					
					attr = attrs.getNamedItem("l2coinfee");
					if (attr != null)
					{
						set.set("l2coin_fee", attr.getNodeValue());
					}
					
					break;
				}
				case "cancel":
				{
					attr = attrs.getNamedItem("count");
					set.set("cancel_count", attr.getNodeValue());
					attr = attrs.getNamedItem("fee");
					set.set("cancel_fee", attr.getNodeValue());
					
					attr = attrs.getNamedItem("l2coinfee_cancel");
					if (attr != null)
					{
						set.set("cancel_l2coin_fee", attr.getNodeValue());
					}
					
					break;
				}
				case "duration":
				{
					attr = attrs.getNamedItem("time"); // in minutes
					set.set("duration", attr.getNodeValue());
					break;
				}
				case "skill":
				{
					skills.add(SkillData.getInstance().getSkill(parseInteger(attrs, "id"), parseInteger(attrs, "level")));
					break;
				}
				case "classId":
				{
					foreach (String s in c.getTextContent().split(","))
					{
						wearClassIds.add(int.Parse(s));
					}
					break;
				}
			}
		}
		
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
	public Henna getHenna(int id)
	{
		return _hennaDyeIdList.get(id);
	}
	
	public Henna getHennaByDyeId(int id)
	{
		return _hennaDyeIdList.get(id);
	}
	
	public Henna getHennaByItemId(int id)
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
		foreach (Henna henna in _hennaDyeIdList.values())
		{
			if (henna.isAllowedClass(player))
			{
				list.add(henna);
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