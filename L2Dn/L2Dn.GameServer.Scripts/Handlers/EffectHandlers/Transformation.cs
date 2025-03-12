using System.Collections.Frozen;
using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Transformation effect implementation.
/// </summary>
public sealed class Transformation: AbstractEffect
{
    private readonly FrozenSet<int> _transformationIds;

    public Transformation(StatSet @params)
    {
        string ids = @params.getString("transformationId", string.Empty);
        _transformationIds = ParseUtil.ParseSet<int>(ids);
    }

    public override bool canStart(Creature effector, Creature effected, Skill skill)
    {
        return !effected.isDoor();
    }

    public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (_transformationIds.Count != 0)
            effected.transform(_transformationIds.Items.GetRandomElement(), true);
    }

    public override void onExit(Creature effector, Creature effected, Skill skill)
    {
        effected.stopTransformation(false);
    }

    public override int GetHashCode() => _transformationIds.GetSetHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._transformationIds.GetSetComparable());
}