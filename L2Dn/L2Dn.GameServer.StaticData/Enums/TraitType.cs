using System.Collections.Immutable;

namespace L2Dn.GameServer.Enums;

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
    INFECTION,
    BLUFF,
}

public static class TraitTypeUtil
{
    private static readonly int[] _types = CreateTypes();

    public static ImmutableArray<TraitType> WeaknessList { get; } =
    [
        TraitType.BUG_WEAKNESS, TraitType.ANIMAL_WEAKNESS, TraitType.PLANT_WEAKNESS, TraitType.BEAST_WEAKNESS,
        TraitType.DRAGON_WEAKNESS, TraitType.GIANT_WEAKNESS, TraitType.CONSTRUCT_WEAKNESS, TraitType.VALAKAS,
        TraitType.ANESTHESIA, TraitType.DEMONIC_WEAKNESS, TraitType.DIVINE_WEAKNESS, TraitType.ELEMENTAL_WEAKNESS,
        TraitType.FAIRY_WEAKNESS, TraitType.HUMAN_WEAKNESS, TraitType.HUMANOID_WEAKNESS, TraitType.UNDEAD_WEAKNESS,
        TraitType.EMBRYO_WEAKNESS, TraitType.SPIRIT_WEAKNESS,
    ];

    public static int GetTypeOfTrait(this TraitType traitType) =>
        traitType >= 0 && (int)traitType < _types.Length ? _types[(int)traitType] : 0;

    private static int[] CreateTypes()
    {
        int[] types = new int[Enum.GetValues<TraitType>().Length];

        // 1 = weapon, 2 = weakness, 3 = resistance
        types[(int)TraitType.NONE] = 0;
        types[(int)TraitType.SWORD] = 1;
        types[(int)TraitType.BLUNT] = 1;
        types[(int)TraitType.DAGGER] = 1;
        types[(int)TraitType.POLE] = 1;
        types[(int)TraitType.FIST] = 1;
        types[(int)TraitType.BOW] = 1;
        types[(int)TraitType.ETC] = 1;
        types[(int)TraitType.UNK_8] = 0;
        types[(int)TraitType.POISON] = 3;
        types[(int)TraitType.HOLD] = 3;
        types[(int)TraitType.BLEED] = 3;
        types[(int)TraitType.SLEEP] = 3;
        types[(int)TraitType.SHOCK] = 3;
        types[(int)TraitType.DERANGEMENT] = 3;
        types[(int)TraitType.BUG_WEAKNESS] = 2;
        types[(int)TraitType.ANIMAL_WEAKNESS] = 2;
        types[(int)TraitType.PLANT_WEAKNESS] = 2;
        types[(int)TraitType.BEAST_WEAKNESS] = 2;
        types[(int)TraitType.DRAGON_WEAKNESS] = 2;
        types[(int)TraitType.PARALYZE] = 3;
        types[(int)TraitType.DUAL] = 1;
        types[(int)TraitType.DUALFIST] = 1;
        types[(int)TraitType.BOSS] = 3;
        types[(int)TraitType.GIANT_WEAKNESS] = 2;
        types[(int)TraitType.CONSTRUCT_WEAKNESS] = 2;
        types[(int)TraitType.DEATH] = 3;
        types[(int)TraitType.VALAKAS] = 2;
        types[(int)TraitType.ANESTHESIA] = 2;
        types[(int)TraitType.CRITICAL_POISON] = 3;
        types[(int)TraitType.ROOT_PHYSICALLY] = 3;
        types[(int)TraitType.ROOT_MAGICALLY] = 3;
        types[(int)TraitType.RAPIER] = 1;
        types[(int)TraitType.CROSSBOW] = 1;
        types[(int)TraitType.ANCIENTSWORD] = 1;
        types[(int)TraitType.TURN_STONE] = 3;
        types[(int)TraitType.GUST] = 3;
        types[(int)TraitType.PHYSICAL_BLOCKADE] = 3;
        types[(int)TraitType.TARGET] = 3;
        types[(int)TraitType.PHYSICAL_WEAKNESS] = 3;
        types[(int)TraitType.MAGICAL_WEAKNESS] = 3;
        types[(int)TraitType.DUALDAGGER] = 1;
        types[(int)TraitType.DEMONIC_WEAKNESS] = 2; // CT26_P4
        types[(int)TraitType.DIVINE_WEAKNESS] = 2;
        types[(int)TraitType.ELEMENTAL_WEAKNESS] = 2;
        types[(int)TraitType.FAIRY_WEAKNESS] = 2;
        types[(int)TraitType.HUMAN_WEAKNESS] = 2;
        types[(int)TraitType.HUMANOID_WEAKNESS] = 2;
        types[(int)TraitType.UNDEAD_WEAKNESS] = 2;

        // The values from below are custom.
        types[(int)TraitType.DUALBLUNT] = 1;
        types[(int)TraitType.KNOCKBACK] = 3;
        types[(int)TraitType.KNOCKDOWN] = 3;
        types[(int)TraitType.PULL] = 3;
        types[(int)TraitType.HATE] = 3;
        types[(int)TraitType.AGGRESSION] = 3;
        types[(int)TraitType.AIRBIND] = 3;
        types[(int)TraitType.DISARM] = 3;
        types[(int)TraitType.DEPORT] = 3;
        types[(int)TraitType.CHANGEBODY] = 3;
        types[(int)TraitType.TWOHANDCROSSBOW] = 1;
        types[(int)TraitType.ZONE] = 3;
        types[(int)TraitType.PSYCHIC] = 3;
        types[(int)TraitType.EMBRYO_WEAKNESS] = 2;
        types[(int)TraitType.SPIRIT_WEAKNESS] = 2;
        types[(int)TraitType.PISTOLS] = 1;
        types[(int)TraitType.ANOMALY] = 3;
        types[(int)TraitType.SUPPRESSION] = 3;
        types[(int)TraitType.IMPRISON] = 3;
        types[(int)TraitType.FEAR] = 3;
        types[(int)TraitType.SILENCE] = 3;
        types[(int)TraitType.INFECTION] = 3;
        types[(int)TraitType.BLUFF] = 3;

        return types;
    }
}