using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Data.Sql;

/**
 * @author Nyaran
 */
public sealed class SummonEffectTable
{
    /** Servitors **/
    // Map tree
    // => key: charObjectId, value: classIndex Map
    // --> key: classIndex, value: servitors Map
    // ---> key: servitorSkillId, value: Effects list
    private readonly Map<int, Map<int, Map<int, ICollection<SummonEffect>>>> _servitorEffects = new();

    /** Pets **/
    // key: petItemObjectId, value: Effects list
    private readonly Map<int, ICollection<SummonEffect>> _petEffects = new();

    public Map<int, Map<int, Map<int, ICollection<SummonEffect>>>> getServitorEffectsOwner() => _servitorEffects;

    public Map<int, ICollection<SummonEffect>>? getServitorEffects(Player owner)
    {
        Map<int, Map<int, ICollection<SummonEffect>>>? servitorMap = _servitorEffects.get(owner.ObjectId);
        if (servitorMap == null)
            return null;

        return servitorMap.get(owner.getClassIndex());
    }

    public Map<int, ICollection<SummonEffect>> getPetEffects() => _petEffects;

    public static SummonEffectTable getInstance()
    {
        return SingletonHolder.INSTANCE;
    }

    private static class SingletonHolder
    {
        public static readonly SummonEffectTable INSTANCE = new();
    }

    public sealed class SummonEffect(Skill skill, TimeSpan effectCurTime)
    {
        public Skill getSkill() => skill;
        public TimeSpan getEffectCurTime() => effectCurTime;
    }
}