using System.Collections.Immutable;
using L2Dn.GameServer.Model.Holders;

namespace L2Dn.GameServer.Enums;

public enum BroochJewel
{
	None = 0,
	
	// Working effect
	RUBY_LV1,
	RUBY_LV2,
	RUBY_LV3,
	RUBY_LV4,
	RUBY_LV5,
	GREATER_RUBY_LV1, // + 1% p atk
	GREATER_RUBY_LV2, // + 2% p atk
	GREATER_RUBY_LV3, // + 3% p atk
	GREATER_RUBY_LV4, // + 5% p atk
	GREATER_RUBY_LV5, // + 7% p atk + crit. p. rate +10%

	// Not show recharge effect - not used in classics maybe
	RUBY_LV1_2,
	RUBY_LV2_2,
	RUBY_LV3_2,
	RUBY_LV4_2,
	RUBY_LV5_2,
	GREATER_RUBY_LV1_2, // + 1% p atk
	GREATER_RUBY_LV2_2, // + 2% p atk
	GREATER_RUBY_LV3_2, // + 3% p atk
	GREATER_RUBY_LV4_2, // + 5% p atk
	GREATER_RUBY_LV5_2, // + 7% p atk + crit. p. rate +10%

	// Onyx effect
	ONYX_LV1, // Soulshot / Spiritshot damage +2%
	ONYX_LV2, // Soulshot / Spiritshot damage +3%
	ONYX_LV3, // Soulshot / Spiritshot damage +5%
	ONYX_LV4, // Soulshot / Spiritshot damage +8%
	ONYX_LV5, // Soulshot / Spiritshot damage +12%
	ONYX_LV6, // Soulshot / Spiritshot damage +16%, P. Atk. +50, M. Atk. +50
	ONYX_LV7, // Soulshot / Spiritshot damage +20%, P. Atk. +150, M. Atk. +150
	ONYX_LV8, // Soulshot / Spiritshot damage +25%, P. Atk. +300, M. Atk. +300

	// Working effect
	SAPPHIRE_LV1,
	SAPPHIRE_LV2,
	SAPPHIRE_LV3,
	SAPPHIRE_LV4,
	SAPPHIRE_LV5,
	GREATER_SAPPHIRE_LV1, // + 2 % m attack
	GREATER_SAPPHIRE_LV2, // + 4 % m attack
	GREATER_SAPPHIRE_LV3, // + 6 % m attack
	GREATER_SAPPHIRE_LV4, // + 10 % m attack
	GREATER_SAPPHIRE_LV5, // + 14 % m attack + crit. m. rate +10%

	// Not show recharge effect - not used in classics maybe
	SAPPHIRE_LV1_2,
	SAPPHIRE_LV2_2,
	SAPPHIRE_LV3_2,
	SAPPHIRE_LV4_2,
	SAPPHIRE_LV5_2,
	GREATER_SAPPHIRE_LV1_2, // + 2 % m attack
	GREATER_SAPPHIRE_LV2_2, // + 4 % m attack
	GREATER_SAPPHIRE_LV3_2, // + 6 % m attack
	GREATER_SAPPHIRE_LV4_2, // + 10 % m attack
	GREATER_SAPPHIRE_LV5_2 // + 14 % m attack + crit. m. ata +10%
};

public static class BroochJewelUtil
{
	public static int GetItemId(this BroochJewel broochJewel)
	{
		return BroochJewelInfo.All[broochJewel].getItemId();
	}

	public static SkillHolder GetSkill(this BroochJewel broochJewel)
	{
		BroochJewelInfo info = BroochJewelInfo.All[broochJewel];
		return new SkillHolder(info.getSkillId(), info.getSkillLevel());
	}

	public static bool isRuby(this BroochJewel broochJewel)
	{
		return BroochJewelInfo.All[broochJewel].isRuby();
	}

	public static bool isSapphire(this BroochJewel broochJewel)
	{
		return BroochJewelInfo.All[broochJewel].isSapphire();
	}

	public static double getBonus(this BroochJewel broochJewel)
	{
		return BroochJewelInfo.All[broochJewel].getBonus();
	}
} 

public class BroochJewelInfo
{
	// Working effect
	public static readonly BroochJewelInfo RUBY_LV1 = new(BroochJewel.RUBY_LV1, 70451, 17817, 1, 0.02, true, false);
	public static readonly BroochJewelInfo RUBY_LV2 = new(BroochJewel.RUBY_LV2, 70452, 17817, 1, 0.03, true, false);
	public static readonly BroochJewelInfo RUBY_LV3 = new(BroochJewel.RUBY_LV3, 70453, 17817, 1, 0.05, true, false);
	public static readonly BroochJewelInfo RUBY_LV4 = new(BroochJewel.RUBY_LV4, 70454, 17817, 1, 0.08, true, false);
	public static readonly BroochJewelInfo RUBY_LV5 = new(BroochJewel.RUBY_LV5, 70455, 17817, 1, 0.16, true, false);
	public static readonly BroochJewelInfo GREATER_RUBY_LV1 = new(BroochJewel.GREATER_RUBY_LV1, 71368, 17817, 1, 0.17, true, false); // + 1% p atk
	public static readonly BroochJewelInfo GREATER_RUBY_LV2 = new(BroochJewel.GREATER_RUBY_LV2, 71369, 17817, 1, 0.18, true, false); // + 2% p atk
	public static readonly BroochJewelInfo GREATER_RUBY_LV3 = new(BroochJewel.GREATER_RUBY_LV3, 71370, 17817, 1, 0.19, true, false); // + 3% p atk
	public static readonly BroochJewelInfo GREATER_RUBY_LV4 = new(BroochJewel.GREATER_RUBY_LV4, 71371, 17817, 1, 0.20, true, false); // + 5% p atk
	public static readonly BroochJewelInfo GREATER_RUBY_LV5 = new(BroochJewel.GREATER_RUBY_LV5, 71372, 17817, 1, 0.20, true, false); // + 7% p atk + crit. p. rate +10%
	
	// Not show recharge effect - not used in classics maybe
	public static readonly BroochJewelInfo RUBY_LV1_2 = new(BroochJewel.RUBY_LV1_2, 90328, 59150, 1, 0.02, true, false);
	public static readonly BroochJewelInfo RUBY_LV2_2 = new(BroochJewel.RUBY_LV2_2, 90329, 59150, 1, 0.03, true, false);
	public static readonly BroochJewelInfo RUBY_LV3_2 = new(BroochJewel.RUBY_LV3_2, 90330, 59150, 1, 0.05, true, false);
	public static readonly BroochJewelInfo RUBY_LV4_2 = new(BroochJewel.RUBY_LV4_2, 90331, 59150, 1, 0.08, true, false);
	public static readonly BroochJewelInfo RUBY_LV5_2 = new(BroochJewel.RUBY_LV5_2, 90332, 59150, 1, 0.16, true, false);
	public static readonly BroochJewelInfo GREATER_RUBY_LV1_2 = new(BroochJewel.GREATER_RUBY_LV1_2, 91320, 59150, 1, 0.17, true, false); // + 1% p atk
	public static readonly BroochJewelInfo GREATER_RUBY_LV2_2 = new(BroochJewel.GREATER_RUBY_LV2_2, 91321, 59150, 1, 0.18, true, false); // + 2% p atk
	public static readonly BroochJewelInfo GREATER_RUBY_LV3_2 = new(BroochJewel.GREATER_RUBY_LV3_2, 91322, 59150, 1, 0.19, true, false); // + 3% p atk
	public static readonly BroochJewelInfo GREATER_RUBY_LV4_2 = new(BroochJewel.GREATER_RUBY_LV4_2, 91323, 59150, 1, 0.20, true, false); // + 5% p atk
	public static readonly BroochJewelInfo GREATER_RUBY_LV5_2 = new(BroochJewel.GREATER_RUBY_LV5_2, 91324, 59150, 1, 0.20, true, false); // + 7% p atk + crit. p. rate +10%
	
	// Onyx effect
	public static readonly BroochJewelInfo ONYX_LV1 = new(BroochJewel.ONYX_LV1, 92066, 50198, 1, 0.02, true, true); // Soulshot / Spiritshot damage +2%
	public static readonly BroochJewelInfo ONYX_LV2 = new(BroochJewel.ONYX_LV2, 92067, 50198, 2, 0.03, true, true); // Soulshot / Spiritshot damage +3%
	public static readonly BroochJewelInfo ONYX_LV3 = new(BroochJewel.ONYX_LV3, 92068, 50198, 3, 0.05, true, true); // Soulshot / Spiritshot damage +5%
	public static readonly BroochJewelInfo ONYX_LV4 = new(BroochJewel.ONYX_LV4, 92069, 50198, 4, 0.08, true, true); // Soulshot / Spiritshot damage +8%
	public static readonly BroochJewelInfo ONYX_LV5 = new(BroochJewel.ONYX_LV5, 94521, 50198, 5, 0.12, true, true); // Soulshot / Spiritshot damage +12%
	public static readonly BroochJewelInfo ONYX_LV6 = new(BroochJewel.ONYX_LV6, 92070, 50198, 6, 0.16, true, true); // Soulshot / Spiritshot damage +16%, P. Atk. +50, M. Atk. +50
	public static readonly BroochJewelInfo ONYX_LV7 = new(BroochJewel.ONYX_LV7, 92071, 50198, 7, 0.20, true, true); // Soulshot / Spiritshot damage +20%, P. Atk. +150, M. Atk. +150
	public static readonly BroochJewelInfo ONYX_LV8 = new(BroochJewel.ONYX_LV8, 92072, 50198, 8, 0.25, true, true); // Soulshot / Spiritshot damage +25%, P. Atk. +300, M. Atk. +300
	
	// Working effect
	public static readonly BroochJewelInfo SAPPHIRE_LV1 = new(BroochJewel.SAPPHIRE_LV1, 70456, 17821, 1, 0.02, false, true);
	public static readonly BroochJewelInfo SAPPHIRE_LV2 = new(BroochJewel.SAPPHIRE_LV2, 70457, 17821, 1, 0.03, false, true);
	public static readonly BroochJewelInfo SAPPHIRE_LV3 = new(BroochJewel.SAPPHIRE_LV3, 70458, 17821, 1, 0.05, false, true);
	public static readonly BroochJewelInfo SAPPHIRE_LV4 = new(BroochJewel.SAPPHIRE_LV4, 70459, 17821, 1, 0.08, false, true);
	public static readonly BroochJewelInfo SAPPHIRE_LV5 = new(BroochJewel.SAPPHIRE_LV5, 70460, 17821, 1, 0.16, false, true);
	public static readonly BroochJewelInfo GREATER_SAPPHIRE_LV1 = new(BroochJewel.GREATER_SAPPHIRE_LV1, 71373, 17821, 1, 00.17, false, true); // + 2 % m attack
	public static readonly BroochJewelInfo GREATER_SAPPHIRE_LV2 = new(BroochJewel.GREATER_SAPPHIRE_LV2, 71374, 17821, 1, 00.18, false, true); // + 4 % m attack
	public static readonly BroochJewelInfo GREATER_SAPPHIRE_LV3 = new(BroochJewel.GREATER_SAPPHIRE_LV3, 71375, 17821, 1, 00.19, false, true); // + 6 % m attack
	public static readonly BroochJewelInfo GREATER_SAPPHIRE_LV4 = new(BroochJewel.GREATER_SAPPHIRE_LV4, 71376, 17821, 1, 00.20, false, true); // + 10 % m attack
	public static readonly BroochJewelInfo GREATER_SAPPHIRE_LV5 = new(BroochJewel.GREATER_SAPPHIRE_LV5, 71377, 17821, 1, 00.20, false, true); // + 14 % m attack + crit. m. rate +10%
	
	// Not show recharge effect - not used in classics maybe
	public static readonly BroochJewelInfo SAPPHIRE_LV1_2 = new(BroochJewel.SAPPHIRE_LV1_2, 90333, 59151, 1, 0.02, false, true);
	public static readonly BroochJewelInfo SAPPHIRE_LV2_2 = new(BroochJewel.SAPPHIRE_LV2_2, 90334, 59151, 1, 0.03, false, true);
	public static readonly BroochJewelInfo SAPPHIRE_LV3_2 = new(BroochJewel.SAPPHIRE_LV3_2, 90335, 59151, 1, 0.05, false, true);
	public static readonly BroochJewelInfo SAPPHIRE_LV4_2 = new(BroochJewel.SAPPHIRE_LV4_2, 90336, 59151, 1, 0.08, false, true);
	public static readonly BroochJewelInfo SAPPHIRE_LV5_2 = new(BroochJewel.SAPPHIRE_LV5_2, 90337, 59151, 1, 0.16, false, true);
	public static readonly BroochJewelInfo GREATER_SAPPHIRE_LV1_2 = new(BroochJewel.GREATER_SAPPHIRE_LV1_2, 91325, 59151, 1, 0.17, false, true); // + 2 % m attack
	public static readonly BroochJewelInfo GREATER_SAPPHIRE_LV2_2 = new(BroochJewel.GREATER_SAPPHIRE_LV2_2, 91326, 59151, 1, 0.18, false, true); // + 4 % m attack
	public static readonly BroochJewelInfo GREATER_SAPPHIRE_LV3_2 = new(BroochJewel.GREATER_SAPPHIRE_LV3_2, 91327, 59151, 1, 0.19, false, true); // + 6 % m attack
	public static readonly BroochJewelInfo GREATER_SAPPHIRE_LV4_2 = new(BroochJewel.GREATER_SAPPHIRE_LV4_2, 91328, 59151, 1, 0.20, false, true); // + 10 % m attack
	public static readonly BroochJewelInfo GREATER_SAPPHIRE_LV5_2 = new(BroochJewel.GREATER_SAPPHIRE_LV5_2, 91329, 59151, 1, 0.20, false, true); // + 14 % m attack + crit. m. ata +10%

	public static readonly ImmutableDictionary<BroochJewel, BroochJewelInfo> All = new[]
	{
		// Working effect
		RUBY_LV1,
		RUBY_LV2,
		RUBY_LV3,
		RUBY_LV4,
		RUBY_LV5,
		GREATER_RUBY_LV1, // + 1% p atk
		GREATER_RUBY_LV2, // + 2% p atk
		GREATER_RUBY_LV3, // + 3% p atk
		GREATER_RUBY_LV4, // + 5% p atk
		GREATER_RUBY_LV5, // + 7% p atk + crit. p. rate +10%

		// Not show recharge effect - not used in classics maybe
		RUBY_LV1_2,
		RUBY_LV2_2,
		RUBY_LV3_2,
		RUBY_LV4_2,
		RUBY_LV5_2,
		GREATER_RUBY_LV1_2, // + 1% p atk
		GREATER_RUBY_LV2_2, // + 2% p atk
		GREATER_RUBY_LV3_2, // + 3% p atk
		GREATER_RUBY_LV4_2, // + 5% p atk
		GREATER_RUBY_LV5_2, // + 7% p atk + crit. p. rate +10%

		// Onyx effect
		ONYX_LV1, // Soulshot / Spiritshot damage +2%
		ONYX_LV2, // Soulshot / Spiritshot damage +3%
		ONYX_LV3, // Soulshot / Spiritshot damage +5%
		ONYX_LV4, // Soulshot / Spiritshot damage +8%
		ONYX_LV5, // Soulshot / Spiritshot damage +12%
		ONYX_LV6, // Soulshot / Spiritshot damage +16%, P. Atk. +50, M. Atk. +50
		ONYX_LV7, // Soulshot / Spiritshot damage +20%, P. Atk. +150, M. Atk. +150
		ONYX_LV8, // Soulshot / Spiritshot damage +25%, P. Atk. +300, M. Atk. +300

		// Working effect
		SAPPHIRE_LV1,
		SAPPHIRE_LV2,
		SAPPHIRE_LV3,
		SAPPHIRE_LV4,
		SAPPHIRE_LV5,
		GREATER_SAPPHIRE_LV1, // + 2 % m attack
		GREATER_SAPPHIRE_LV2, // + 4 % m attack
		GREATER_SAPPHIRE_LV3, // + 6 % m attack
		GREATER_SAPPHIRE_LV4, // + 10 % m attack
		GREATER_SAPPHIRE_LV5, // + 14 % m attack + crit. m. rate +10%

		// Not show recharge effect - not used in classics maybe
		SAPPHIRE_LV1_2,
		SAPPHIRE_LV2_2,
		SAPPHIRE_LV3_2,
		SAPPHIRE_LV4_2,
		SAPPHIRE_LV5_2,
		GREATER_SAPPHIRE_LV1_2, // + 2 % m attack
		GREATER_SAPPHIRE_LV2_2, // + 4 % m attack
		GREATER_SAPPHIRE_LV3_2, // + 6 % m attack
		GREATER_SAPPHIRE_LV4_2, // + 10 % m attack
		GREATER_SAPPHIRE_LV5_2 // + 14 % m attack + crit. m. ata +10%
	}.ToImmutableDictionary(i => i.Jewel);
	
	private readonly BroochJewel _jewel;
	private readonly int _itemId;
	private readonly int _skillId;
	private readonly int _skillLevel;
	private readonly double _bonus;
	private readonly bool _isRuby;
	private readonly bool _isSapphire;
	
	private BroochJewelInfo(BroochJewel jewel, int itemId, int skillId, int skillLevel, double bonus, bool isRuby, bool isSapphire)
	{
		_jewel = jewel;
		_itemId = itemId;
		_skillId = skillId;
		_skillLevel = skillLevel;
		_bonus = bonus;
		_isRuby = isRuby;
		_isSapphire = isSapphire;
	}

	public BroochJewel Jewel => _jewel;
	
	public int getItemId()
	{
		return _itemId;
	}
	
	public int getSkillId()
	{
		return _skillId;
	}
	
	public int getSkillLevel()
	{
		return _skillLevel;
	}
	
	public double getBonus()
	{
		return _bonus;
	}
	
	public bool isRuby()
	{
		return _isRuby;
	}
	
	public bool isSapphire()
	{
		return _isSapphire;
	}
}