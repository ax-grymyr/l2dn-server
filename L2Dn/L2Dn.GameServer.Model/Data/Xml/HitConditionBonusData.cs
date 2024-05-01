using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.TaskManagers;
using L2Dn.Geometry;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * This class load, holds and calculates the hit condition bonuses.
 * @author Nik
 */
public class HitConditionBonusData: DataReaderBase
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(HitConditionBonusData));
	
	private int frontBonus = 0;
	private int sideBonus = 0;
	private int backBonus = 0;
	private int highBonus = 0;
	private int lowBonus = 0;
	private int darkBonus = 0;
	private int rainBonus = 0;
	
	/**
	 * Instantiates a new hit condition bonus.
	 */
	protected HitConditionBonusData()
	{
		load();
	}
	
	public void load()
	{
		XDocument document = LoadXmlDocument(DataFileLocation.Data, "stats/hitConditionBonus.xml");
		document.Elements("hitConditionBonus").ForEach(element =>
		{
			frontBonus = (element.Element("front")?.Attribute("val")).GetInt32(0);
			sideBonus = (element.Element("side")?.Attribute("val")).GetInt32(0);
			backBonus = (element.Element("back")?.Attribute("val")).GetInt32(0);
			highBonus = (element.Element("high")?.Attribute("val")).GetInt32(0);
			lowBonus = (element.Element("low")?.Attribute("val")).GetInt32(0);
			darkBonus = (element.Element("dark")?.Attribute("val")).GetInt32(0);
			rainBonus = (element.Element("rain")?.Attribute("val")).GetInt32(0);
		});
		
		LOGGER.Info(GetType().Name + ": Loaded hit condition bonuses.");
	}
	
	/**
	 * Gets the condition bonus.
	 * @param attacker the attacking character.
	 * @param target the attacked character.
	 * @return the bonus of the attacker against the target.
	 */
	public double getConditionBonus(Creature attacker, Creature target)
	{
		double mod = 100;
		// Get high or low bonus
		if ((attacker.getZ() - target.getZ()) > 50)
		{
			mod += highBonus;
		}
		else if ((attacker.getZ() - target.getZ()) < -50)
		{
			mod += lowBonus;
		}
		
		// Get weather bonus
		if (GameTimeTaskManager.getInstance().isNight())
		{
			mod += darkBonus;
			// else if () No rain support yet.
			// chance += hitConditionBonus.rainBonus;
		}
		
		// Get side bonus
		switch (PositionUtil.getPosition(attacker, target))
		{
			case Position.Side:
			{
				mod += sideBonus;
				break;
			}
			case Position.Back:
			{
				mod += backBonus;
				break;
			}
			default:
			{
				mod += frontBonus;
				break;
			}
		}
		
		// If (mod / 100) is less than 0, return 0, because we can't lower more than 100%.
		return Math.Max(mod / 100, 0);
	}
	
	/**
	 * Gets the single instance of HitConditionBonus.
	 * @return single instance of HitConditionBonus
	 */
	public static HitConditionBonusData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly HitConditionBonusData INSTANCE = new();
	}
}