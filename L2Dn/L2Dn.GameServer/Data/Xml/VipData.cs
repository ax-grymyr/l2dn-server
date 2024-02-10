using L2Dn.GameServer.Model.Vips;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Gabriel Costa Souza
 */
public class VipData
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(VipData));
	
	private readonly Map<Byte, VipInfo> _vipTiers = new();
	
	protected VipData()
	{
		load();
	}
	
	public void load()
	{
		if (!Config.VIP_SYSTEM_ENABLED)
		{
			return;
		}
		_vipTiers.clear();
		parseDatapackFile("data/Vip.xml");
		LOGGER.Info(GetType().Name + ": Loaded " + _vipTiers.size() + " vips.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		for (Node n = doc.getFirstChild(); n != null; n = n.getNextSibling())
		{
			if ("list".equalsIgnoreCase(n.getNodeName()))
			{
				VIP_FILE: for (Node d = n.getFirstChild(); d != null; d = d.getNextSibling())
				{
					if ("vip".equalsIgnoreCase(d.getNodeName()))
					{
						NamedNodeMap attrs = d.getAttributes();
						Node att;
						byte tier = -1;
						int required = -1;
						int lose = -1;
						
						att = attrs.getNamedItem("tier");
						if (att == null)
						{
							LOGGER.Error(GetType().Name + ": Missing tier for vip, skipping");
							continue;
						}
						tier = Byte.parseByte(att.getNodeValue());
						
						att = attrs.getNamedItem("points-required");
						if (att == null)
						{
							LOGGER.Error(GetType().Name + ": Missing points-required for vip: " + tier + ", skipping");
							continue;
						}
						required = int.Parse(att.getNodeValue());
						
						att = attrs.getNamedItem("points-lose");
						if (att == null)
						{
							LOGGER.Error(GetType().Name + ": Missing points-lose for vip: " + tier + ", skipping");
							continue;
						}
						lose = int.Parse(att.getNodeValue());
						
						VipInfo vipInfo = new VipInfo(tier, required, lose);
						for (Node c = d.getFirstChild(); c != null; c = c.getNextSibling())
						{
							if ("bonus".equalsIgnoreCase(c.getNodeName()))
							{
								int skill = int.Parse(c.getAttributes().getNamedItem("skill").getNodeValue());
								try
								{
									vipInfo.setSkill(skill);
								}
								catch (Exception e)
								{
									LOGGER.Error(GetType().Name + ": Error in bonus parameter for vip: " + tier + ", skipping");
									continue VIP_FILE;
								}
							}
						}
						_vipTiers.put(tier, vipInfo);
					}
				}
			}
		}
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
	
	public int getSkillId(byte tier)
	{
		return _vipTiers.get(tier).getSkill();
	}
	
	public Map<Byte, VipInfo> getVipTiers()
	{
		return _vipTiers;
	}
}