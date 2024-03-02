using System.Collections.Immutable;

namespace L2Dn.GameServer.Model.Stats;

public enum TraitType
{
	NONE,
	SWORD,
	BLUNT,
	DAGGER,
	POLE,
	FIST,
	BOW,
	ETC,
	UNK_8,
	POISON,
	HOLD,
	BLEED,
	SLEEP,
	SHOCK,
	DERANGEMENT,
	BUG_WEAKNESS,
	ANIMAL_WEAKNESS,
	PLANT_WEAKNESS,
	BEAST_WEAKNESS,
	DRAGON_WEAKNESS,
	PARALYZE,
	DUAL,
	DUALFIST,
	BOSS,
	GIANT_WEAKNESS,
	CONSTRUCT_WEAKNESS,
	DEATH,
	VALAKAS,
	ANESTHESIA,
	CRITICAL_POISON,
	ROOT_PHYSICALLY,
	ROOT_MAGICALLY,
	RAPIER,
	CROSSBOW,
	ANCIENTSWORD,
	TURN_STONE,
	GUST,
	PHYSICAL_BLOCKADE,
	TARGET,
	PHYSICAL_WEAKNESS,
	MAGICAL_WEAKNESS,
	DUALDAGGER,
	DEMONIC_WEAKNESS, // CT26_P4
	DIVINE_WEAKNESS,
	ELEMENTAL_WEAKNESS,
	FAIRY_WEAKNESS,
	HUMAN_WEAKNESS,
	HUMANOID_WEAKNESS,
	UNDEAD_WEAKNESS,

	// The values from below are custom.
	DUALBLUNT,
	KNOCKBACK,
	KNOCKDOWN,
	PULL,
	HATE,
	AGGRESSION,
	AIRBIND,
	DISARM,
	DEPORT,
	CHANGEBODY,
	TWOHANDCROSSBOW,
	ZONE,
	PSYCHIC,
	EMBRYO_WEAKNESS,
	SPIRIT_WEAKNESS,
	PISTOLS,
	ANOMALY,
	SUPPRESSION,
	IMPRISON,
	FEAR,
	SILENCE,
	INFECTION
}

public static class TraitTypeUtil
{
	private static readonly ImmutableArray<TraitType> _weaknesses =
	[
		TraitType.BUG_WEAKNESS,
		TraitType.ANIMAL_WEAKNESS,
		TraitType.PLANT_WEAKNESS,
		TraitType.BEAST_WEAKNESS,
		TraitType.DRAGON_WEAKNESS,
		TraitType.GIANT_WEAKNESS,
		TraitType.CONSTRUCT_WEAKNESS,
		TraitType.VALAKAS,
		TraitType.ANESTHESIA,
		TraitType.DEMONIC_WEAKNESS,
		TraitType.DIVINE_WEAKNESS,
		TraitType.ELEMENTAL_WEAKNESS,
		TraitType.FAIRY_WEAKNESS,
		TraitType.HUMAN_WEAKNESS,
		TraitType.HUMANOID_WEAKNESS,
		TraitType.UNDEAD_WEAKNESS,
		TraitType.EMBRYO_WEAKNESS,
		TraitType.SPIRIT_WEAKNESS,
	];

	public static ImmutableArray<TraitType> getAllWeakness() => _weaknesses;
	
	public static int getType(this TraitType traitType) =>
		traitType switch
		{
			// 1 = weapon, 2 = weakness, 3 = resistance
			TraitType.NONE => 0,
			TraitType.SWORD => 1,
			TraitType.BLUNT => 1,
			TraitType.DAGGER => 1,
			TraitType.POLE => 1,
			TraitType.FIST => 1,
			TraitType.BOW => 1,
			TraitType.ETC => 1,
			TraitType.UNK_8 => 0,
			TraitType.POISON => 3,
			TraitType.HOLD => 3,
			TraitType.BLEED => 3,
			TraitType.SLEEP => 3,
			TraitType.SHOCK => 3,
			TraitType.DERANGEMENT => 3,
			TraitType.BUG_WEAKNESS => 2,
			TraitType.ANIMAL_WEAKNESS => 2,
			TraitType.PLANT_WEAKNESS => 2,
			TraitType.BEAST_WEAKNESS => 2,
			TraitType.DRAGON_WEAKNESS => 2,
			TraitType.PARALYZE => 3,
			TraitType.DUAL => 1,
			TraitType.DUALFIST => 1,
			TraitType.BOSS => 3,
			TraitType.GIANT_WEAKNESS => 2,
			TraitType.CONSTRUCT_WEAKNESS => 2,
			TraitType.DEATH => 3,
			TraitType.VALAKAS => 2,
			TraitType.ANESTHESIA => 2,
			TraitType.CRITICAL_POISON => 3,
			TraitType.ROOT_PHYSICALLY => 3,
			TraitType.ROOT_MAGICALLY => 3,
			TraitType.RAPIER => 1,
			TraitType.CROSSBOW => 1,
			TraitType.ANCIENTSWORD => 1,
			TraitType.TURN_STONE => 3,
			TraitType.GUST => 3,
			TraitType.PHYSICAL_BLOCKADE => 3,
			TraitType.TARGET => 3,
			TraitType.PHYSICAL_WEAKNESS => 3,
			TraitType.MAGICAL_WEAKNESS => 3,
			TraitType.DUALDAGGER => 1,
			TraitType.DEMONIC_WEAKNESS => 2, // CT26_P4
			TraitType.DIVINE_WEAKNESS => 2,
			TraitType.ELEMENTAL_WEAKNESS => 2,
			TraitType.FAIRY_WEAKNESS => 2,
			TraitType.HUMAN_WEAKNESS => 2,
			TraitType.HUMANOID_WEAKNESS => 2,
			TraitType.UNDEAD_WEAKNESS => 2,
			
			// The values from below are custom.
			TraitType.DUALBLUNT => 1,
			TraitType.KNOCKBACK => 3,
			TraitType.KNOCKDOWN => 3,
			TraitType.PULL => 3,
			TraitType.HATE => 3,
			TraitType.AGGRESSION => 3,
			TraitType.AIRBIND => 3,
			TraitType.DISARM => 3,
			TraitType.DEPORT => 3,
			TraitType.CHANGEBODY => 3,
			TraitType.TWOHANDCROSSBOW => 1,
			TraitType.ZONE => 3,
			TraitType.PSYCHIC => 3,
			TraitType.EMBRYO_WEAKNESS => 2,
			TraitType.SPIRIT_WEAKNESS => 2,
			TraitType.PISTOLS => 1,
			TraitType.ANOMALY => 3,
			TraitType.SUPPRESSION => 3,
			TraitType.IMPRISON => 3,
			TraitType.FEAR => 3,
			TraitType.SILENCE => 3,
			TraitType.INFECTION => 3,
			
			_ => 0
		};
}
//
// public enum TraitType
// {
// 	NONE(0),
// 	SWORD(1),
// 	BLUNT(1),
// 	DAGGER(1),
// 	POLE(1),
// 	FIST(1),
// 	BOW(1),
// 	ETC(1),
// 	UNK_8(0),
// 	POISON(3),
// 	HOLD(3),
// 	BLEED(3),
// 	SLEEP(3),
// 	SHOCK(3),
// 	DERANGEMENT(3),
// 	BUG_WEAKNESS(2),
// 	ANIMAL_WEAKNESS(2),
// 	PLANT_WEAKNESS(2),
// 	BEAST_WEAKNESS(2),
// 	DRAGON_WEAKNESS(2),
// 	PARALYZE(3),
// 	DUAL(1),
// 	DUALFIST(1),
// 	BOSS(3),
// 	GIANT_WEAKNESS(2),
// 	CONSTRUCT_WEAKNESS(2),
// 	DEATH(3),
// 	VALAKAS(2),
// 	ANESTHESIA(2),
// 	CRITICAL_POISON(3),
// 	ROOT_PHYSICALLY(3),
// 	ROOT_MAGICALLY(3),
// 	RAPIER(1),
// 	CROSSBOW(1),
// 	ANCIENTSWORD(1),
// 	TURN_STONE(3),
// 	GUST(3),
// 	PHYSICAL_BLOCKADE(3),
// 	TARGET(3),
// 	PHYSICAL_WEAKNESS(3),
// 	MAGICAL_WEAKNESS(3),
// 	DUALDAGGER(1),
// 	DEMONIC_WEAKNESS(2), // CT26_P4
// 	DIVINE_WEAKNESS(2),
// 	ELEMENTAL_WEAKNESS(2),
// 	FAIRY_WEAKNESS(2),
// 	HUMAN_WEAKNESS(2),
// 	HUMANOID_WEAKNESS(2),
// 	UNDEAD_WEAKNESS(2),
// 	// The values from below are custom.
// 	DUALBLUNT(1),
// 	KNOCKBACK(3),
// 	KNOCKDOWN(3),
// 	PULL(3),
// 	HATE(3),
// 	AGGRESSION(3),
// 	AIRBIND(3),
// 	DISARM(3),
// 	DEPORT(3),
// 	CHANGEBODY(3),
// 	TWOHANDCROSSBOW(1),
// 	ZONE(3),
// 	PSYCHIC(3),
// 	EMBRYO_WEAKNESS(2),
// 	SPIRIT_WEAKNESS(2),
// 	PISTOLS(1),
// 	ANOMALY(3),
// 	SUPPRESSION(3),
// 	IMPRISON(3),
// 	FEAR(3),
// 	SILENCE(3),
// 	INFECTION(3);
// 	
// 	private final int _type; // 1 = weapon, 2 = weakness, 3 = resistance
// 	private static final List<TraitType> _weaknesses = new ArrayList<>();
// 	static
// 	{
// 		_weaknesses.add(BUG_WEAKNESS);
// 		_weaknesses.add(ANIMAL_WEAKNESS);
// 		_weaknesses.add(PLANT_WEAKNESS);
// 		_weaknesses.add(BEAST_WEAKNESS);
// 		_weaknesses.add(DRAGON_WEAKNESS);
// 		_weaknesses.add(GIANT_WEAKNESS);
// 		_weaknesses.add(CONSTRUCT_WEAKNESS);
// 		_weaknesses.add(VALAKAS);
// 		_weaknesses.add(ANESTHESIA);
// 		_weaknesses.add(DEMONIC_WEAKNESS);
// 		_weaknesses.add(DIVINE_WEAKNESS);
// 		_weaknesses.add(ELEMENTAL_WEAKNESS);
// 		_weaknesses.add(FAIRY_WEAKNESS);
// 		_weaknesses.add(HUMAN_WEAKNESS);
// 		_weaknesses.add(HUMANOID_WEAKNESS);
// 		_weaknesses.add(UNDEAD_WEAKNESS);
// 		_weaknesses.add(EMBRYO_WEAKNESS);
// 		_weaknesses.add(SPIRIT_WEAKNESS);
// 	}
// 	
// 	public static List<TraitType> getAllWeakness()
// 	{
// 		return _weaknesses;
// 	}
// 	
// 	TraitType(int type)
// 	{
// 		_type = type;
// 	}
// 	
// 	public int getType()
// 	{
// 		return _type;
// 	}
// }
