using System.Collections.Frozen;
using L2Dn.Extensions;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Transformation type effect, which disables attack or use of skills.
/// </summary>
public sealed class ChangeBody: AbstractEffect
{
    private readonly FrozenSet<TemplateChanceHolder> _transformations;

    public ChangeBody(EffectParameterSet parameters)
    {
        _transformations = FrozenSet<TemplateChanceHolder>.Empty;
        throw new NotImplementedException(); // TODO: currently not used, implement later
        // List<StatSet>? items = parameters.getList<StatSet>("templates");
        // if (items != null)
        // {
        //     _transformations = items.Select(item => new TemplateChanceHolder(item.getInt(".templateId"),
        //         item.getInt(".minChance"), item.getInt(".maxChance"))).ToFrozenSet();
        // }
    }

    public override bool CanStart(Creature effector, Creature effected, Skill skill)
    {
        return !effected.isDoor();
    }

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        int chance = Rnd.get(100);
        foreach (TemplateChanceHolder holder in _transformations)
        {
            if (holder.CalcChance(chance))
            {
                effected.transform(holder.TemplateId, false);
                return;
            }
        }
    }

    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        effected.stopTransformation(false);
    }

    public override int GetHashCode() => _transformations.GetSetHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._transformations.GetSetComparable());

    /// <summary>
    /// An object for holding template id and chance.
    /// </summary>
    private sealed record TemplateChanceHolder(int TemplateId, int MinChance, int MaxChance)
    {
        public bool CalcChance(int chance) => MaxChance > chance && chance >= MinChance;

        public override string ToString() =>
            $"[TemplateId: {TemplateId} minChance: {MinChance} maxChance: {MaxChance}]";
    }
}