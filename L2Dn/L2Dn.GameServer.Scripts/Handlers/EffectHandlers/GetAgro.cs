using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Get Agro effect implementation.
/// </summary>
public sealed class GetAgro: AbstractEffect
{
    public GetAgro(StatSet @params)
    {
    }

    public override EffectTypes EffectType => EffectTypes.AGGRESSION;

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected != null && effected.isAttackable())
        {
            effected.getAI().setIntention(CtrlIntention.AI_INTENTION_ATTACK, effector);

            // Monsters from the same clan should assist.
            NpcTemplate template = ((Attackable)effected).getTemplate();
            Set<int>? clans = template.getClans();
            if (clans != null)
            {
                World.getInstance().forEachVisibleObjectInRange<Attackable>(effected, template.getClanHelpRange(),
                    nearby =>
                    {
                        if (!nearby.isMovementDisabled() && nearby.getTemplate().isClan(clans))
                        {
                            nearby.addDamageHate(effector, 1, 200);
                            nearby.getAI().setIntention(CtrlIntention.AI_INTENTION_ATTACK, effector);
                        }
                    });
            }
        }
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}