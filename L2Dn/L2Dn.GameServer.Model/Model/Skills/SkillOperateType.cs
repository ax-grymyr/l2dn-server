namespace L2Dn.GameServer.Model.Skills;

/**
 * This enum class holds the skill operative types:
 * <ul>
 * <li>A1</li>
 * <li>A2</li>
 * <li>A3</li>
 * <li>A4</li>
 * <li>CA1</li>
 * <li>CA5</li>
 * <li>DA1</li>
 * <li>DA2</li>
 * <li>P</li>
 * <li>T</li>
 * </ul>
 * @author Zoey76
 */
public enum SkillOperateType
{
	/**
	 * Active Skill with "Instant Effect" (for example damage skills heal/pdam/mdam/cpdam skills).
	 */
	A1,

	/**
	 * Active Skill with "Continuous effect + Instant effect" (for example buff/debuff or damage/heal over time skills).
	 */
	A2,

	/**
	 * Active Skill with "Instant effect for target + Continuous effect + Continuous effect for self"
	 */
	A3,

	/**
	 * Active Skill with "Instant effect + ?" used for special event herb.
	 */
	A4,

	/**
	 * Aura Active Skill
	 */
	A5,

	/**
	 * Synergy Active Skill
	 */
	A6,

	/**
	 * Continuous Active Skill with "instant effect" (instant effect casted by ticks).
	 */
	CA1,

	/**
	 * ?
	 */
	CA2,

	/**
	 * Continuous Active Skill with "continuous effect" (continuous effect casted by ticks).
	 */
	CA5,

	/**
	 * Directional Active Skill with "Charge/Rush instant effect".
	 */
	DA1,

	/**
	 * Directional Active Skill with "Charge/Rush Continuous effect".
	 */
	DA2,

	/**
	 * Directional Active Skill with Blink effect
	 */
	DA3,

	/**
	 * Directional Active Skill with "Left Continuous effect".
	 */
	DA4,

	/**
	 * Directional Active Skill with "Right Continuous effect".
	 */
	DA5,

	/**
	 * Passive Skill.
	 */
	P,

	/**
	 * Toggle Skill.
	 */
	T,

	/**
	 * Toggle Skill with Group.
	 */
	TG,

	/**
	 * Aura Skill.
	 */
	AU
}

public static class SkillOperateTypeUtil
{
/**
 * Verifies if the operative type correspond to an active skill.
 * @return {@code true} if the operative skill type is active, {@code false} otherwise
 */
	public static bool isActive(this SkillOperateType skillOperateType)
	{
		switch (skillOperateType)
		{
			case SkillOperateType.A1:
			case SkillOperateType.A2:
			case SkillOperateType.A3:
			case SkillOperateType.A4:
			case SkillOperateType.A5:
			case SkillOperateType.A6:
			case SkillOperateType.CA1:
			case SkillOperateType.CA5:
			case SkillOperateType.DA1:
			case SkillOperateType.DA2:
			case SkillOperateType.DA4:
			case SkillOperateType.DA5:
			{
				return true;
			}
			default:
			{
				return false;
			}
		}
	}
	
	/**
	 * Verifies if the operative type correspond to a continuous skill.
	 * @return {@code true} if the operative skill type is continuous, {@code false} otherwise
	 */
	public static bool isContinuous(this SkillOperateType skillOperateType)
	{
		switch (skillOperateType)
		{
			case SkillOperateType.A2:
			case SkillOperateType.A3:
			case SkillOperateType.A4:
			case SkillOperateType.A5:
			case SkillOperateType.A6:
			case SkillOperateType.DA2:
			case SkillOperateType.DA4:
			case SkillOperateType.DA5:
			{
				return true;
			}
			default:
			{
				return false;
			}
		}
	}
	
	/**
	 * Verifies if the operative type correspond to a continuous skill.
	 * @return {@code true} if the operative skill type is continuous, {@code false} otherwise
	 */
	public static bool isSelfContinuous(this SkillOperateType skillOperateType)
	{
		return skillOperateType == SkillOperateType.A3;
	}
	
	/**
	 * Verifies if the operative type correspond to a passive skill.
	 * @return {@code true} if the operative skill type is passive, {@code false} otherwise
	 */
	public static bool isPassive(this SkillOperateType skillOperateType)
	{
		return skillOperateType == SkillOperateType.P;
	}
	
	/**
	 * Verifies if the operative type correspond to a toggle skill.
	 * @return {@code true} if the operative skill type is toggle, {@code false} otherwise
	 */
	public static bool isToggle(this SkillOperateType skillOperateType)
	{
		return skillOperateType == SkillOperateType.T || skillOperateType == SkillOperateType.TG || skillOperateType == SkillOperateType.AU;
	}
	
	/**
	 * Verifies if the operative type correspond to a active aura skill.
	 * @return {@code true} if the operative skill type is active aura, {@code false} otherwise
	 */
	public static bool isAura(this SkillOperateType skillOperateType)
	{
		return skillOperateType == SkillOperateType.A5 || skillOperateType == SkillOperateType.A6 || skillOperateType == SkillOperateType.AU;
	}
	
	/**
	 * @return {@code true} if the operate type skill type should not send messages for start/finish, {@code false} otherwise
	 */
	public static bool isHidingMessages(this SkillOperateType skillOperateType)
	{
		return skillOperateType == SkillOperateType.P || skillOperateType == SkillOperateType.A5 || skillOperateType == SkillOperateType.A6 || skillOperateType == SkillOperateType.TG;
	}
	
	/**
	 * @return {@code true} if the operate type skill type should not be broadcasted as MagicSkillUse, MagicSkillLaunched, {@code false} otherwise
	 */
	public static bool isNotBroadcastable(this SkillOperateType skillOperateType)
	{
		return skillOperateType == SkillOperateType.AU || skillOperateType == SkillOperateType.A5 || 
		       skillOperateType == SkillOperateType.A6 || skillOperateType == SkillOperateType.TG || skillOperateType == SkillOperateType.T;
	}
	
	/**
	 * Verifies if the operative type correspond to a channeling skill.
	 * @return {@code true} if the operative skill type is channeling, {@code false} otherwise
	 */
	public static bool isChanneling(this SkillOperateType skillOperateType)
	{
		switch (skillOperateType)
		{
			case SkillOperateType.CA1:
			case SkillOperateType.CA2:
			case SkillOperateType.CA5:
			{
				return true;
			}
			default:
			{
				return false;
			}
		}
	}
	
	/**
	 * Verifies if the operative type correspond to a synergy skill.
	 * @return {@code true} if the operative skill type is synergy, {@code false} otherwise
	 */
	public static bool isSynergy(this SkillOperateType skillOperateType)
	{
		return skillOperateType == SkillOperateType.A6;
	}
	
	public static bool isFlyType(this SkillOperateType skillOperateType)
	{
		switch (skillOperateType)
		{
			case SkillOperateType.DA1:
			case SkillOperateType.DA2:
			case SkillOperateType.DA3:
			case SkillOperateType.DA4:
			case SkillOperateType.DA5:
			{
				return true;
			}
			default:
			{
				return false;
			}
		}
	}
}
