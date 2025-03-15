using System.Collections.Immutable;
using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public static partial class Config
{
    public static class RandomSpawns
    {
        public static bool ENABLE_RANDOM_MONSTER_SPAWNS;
        public static int MOB_MIN_SPAWN_RANGE;
        public static int MOB_MAX_SPAWN_RANGE;
        public static ImmutableSortedSet<int> MOBS_LIST_NOT_RANDOM = ImmutableSortedSet<int>.Empty;

        internal static void Load(ConfigurationParser parser)
        {
            parser.LoadConfig(FileNames.Configs.RandomSpawns);

            ENABLE_RANDOM_MONSTER_SPAWNS = parser.getBoolean("EnableRandomMonsterSpawns");
            MOB_MAX_SPAWN_RANGE = parser.getInt("MaxSpawnMobRange", 150);
            MOB_MIN_SPAWN_RANGE = -MOB_MAX_SPAWN_RANGE;
            if (ENABLE_RANDOM_MONSTER_SPAWNS)
            {
                MOBS_LIST_NOT_RANDOM = parser.GetIntList("MobsSpawnNotRandom", ',', 18812, 18813, 18814, 22138).
                    ToImmutableSortedSet();
            }
        }
    }
}