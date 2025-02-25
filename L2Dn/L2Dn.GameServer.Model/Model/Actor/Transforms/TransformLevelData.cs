using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model.Actor.Transforms;

public class TransformLevelData
{
    private readonly int _level;
    private readonly double _levelMod;
    private Map<Stat, double> _stats = [];

    public TransformLevelData(StatSet set)
    {
        _level = set.getInt("val");
        _levelMod = set.getDouble("levelMod");
        addStats(Stat.MAX_HP, set.getDouble("hp"));
        addStats(Stat.MAX_MP, set.getDouble("mp"));
        addStats(Stat.MAX_CP, set.getDouble("cp"));
        addStats(Stat.REGENERATE_HP_RATE, set.getDouble("hpRegen"));
        addStats(Stat.REGENERATE_MP_RATE, set.getDouble("mpRegen"));
        addStats(Stat.REGENERATE_CP_RATE, set.getDouble("cpRegen"));
    }

    private void addStats(Stat stat, double value)
    {
        _stats.put(stat, value);
    }

    public double getStats(Stat stat, double defaultValue)
    {
        return _stats.GetValueOrDefault(stat, defaultValue);
    }

    public int getLevel()
    {
        return _level;
    }

    public double getLevelMod()
    {
        return _levelMod;
    }
}