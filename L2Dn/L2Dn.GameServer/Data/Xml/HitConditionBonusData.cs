using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.TaskManagers;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

/**
 * This class load, holds and calculates the hit condition bonuses.
 * @author Nik
 */
public class HitConditionBonusData
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
		parseDatapackFile("data/stats/hitConditionBonus.xml");
		LOGGER.Info(GetType().Name + ": Loaded hit condition bonuses.");
	}
	
	public void parseDocument(Document doc, File f)
	{
		for (Node d = doc.getFirstChild().getFirstChild(); d != null; d = d.getNextSibling())
		{
			NamedNodeMap attrs = d.getAttributes();
			switch (d.getNodeName())
			{
				case "front":
				{
					frontBonus = parseInteger(attrs, "val");
					break;
				}
				case "side":
				{
					sideBonus = parseInteger(attrs, "val");
					break;
				}
				case "back":
				{
					backBonus = parseInteger(attrs, "val");
					break;
				}
				case "high":
				{
					highBonus = parseInteger(attrs, "val");
					break;
				}
				case "low":
				{
					lowBonus = parseInteger(attrs, "val");
					break;
				}
				case "dark":
				{
					darkBonus = parseInteger(attrs, "val");
					break;
				}
				case "rain":
				{
					rainBonus = parseInteger(attrs, "val");
					break;
				}
			}
		}
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
		switch (Position.getPosition(attacker, target))
		{
			case SIDE:
			{
				mod += sideBonus;
				break;
			}
			case BACK:
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