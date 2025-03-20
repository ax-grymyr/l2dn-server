using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Templates;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Rebalance HP effect implementation.
/// </summary>
[HandlerName("RebalanceHP")]
public sealed class RebalanceHP: AbstractEffect
{
    public override EffectTypes EffectTypes => EffectTypes.REBALANCE_HP;

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (!effector.isPlayer())
            return;

        double fullHp = 0;
        double currentHPs = 0;
        Party? party = effector.getParty();
        if (party != null)
        {
            foreach (Player member in party.getMembers())
            {
                if (!member.isDead() && Util.checkIfInRange(skill.AffectRange, effector, member, true))
                {
                    fullHp += member.getMaxHp();
                    currentHPs += member.getCurrentHp();
                }

                L2Dn.GameServer.Model.Actor.Summon? summon = member.getPet();
                if (summon != null && !summon.isDead() &&
                    Util.checkIfInRange(skill.AffectRange, effector, summon, true))
                {
                    fullHp += summon.getMaxHp();
                    currentHPs += summon.getCurrentHp();
                }

                foreach (L2Dn.GameServer.Model.Actor.Summon servitors in member.getServitors().Values)
                {
                    if (!servitors.isDead() && Util.checkIfInRange(skill.AffectRange, effector, servitors, true))
                    {
                        fullHp += servitors.getMaxHp();
                        currentHPs += servitors.getCurrentHp();
                    }
                }
            }

            double percentHP = currentHPs / fullHp;
            foreach (Player member in party.getMembers())
            {
                if (!member.isDead() && Util.checkIfInRange(skill.AffectRange, effector, member, true))
                {
                    double newHP = member.getMaxHp() * percentHP;
                    if (newHP > member.getCurrentHp()) // The target gets healed
                    {
                        // The heal will be blocked if the current hp passes the limit
                        if (member.getCurrentHp() > member.getMaxRecoverableHp())
                        {
                            newHP = member.getCurrentHp();
                        }
                        else if (newHP > member.getMaxRecoverableHp())
                        {
                            newHP = member.getMaxRecoverableHp();
                        }
                    }

                    member.setCurrentHp(newHP);
                }

                L2Dn.GameServer.Model.Actor.Summon? summon = member.getPet();
                if (summon != null && !summon.isDead() &&
                    Util.checkIfInRange(skill.AffectRange, effector, summon, true))
                {
                    double newHP = summon.getMaxHp() * percentHP;
                    if (newHP > summon.getCurrentHp()) // The target gets healed
                    {
                        // The heal will be blocked if the current hp passes the limit
                        if (summon.getCurrentHp() > summon.getMaxRecoverableHp())
                        {
                            newHP = summon.getCurrentHp();
                        }
                        else if (newHP > summon.getMaxRecoverableHp())
                        {
                            newHP = summon.getMaxRecoverableHp();
                        }
                    }

                    summon.setCurrentHp(newHP);
                }

                foreach (L2Dn.GameServer.Model.Actor.Summon servitors in member.getServitors().Values)
                {
                    if (!servitors.isDead() && Util.checkIfInRange(skill.AffectRange, effector, servitors, true))
                    {
                        double newHP = servitors.getMaxHp() * percentHP;
                        if (newHP > servitors.getCurrentHp()) // The target gets healed
                        {
                            // The heal will be blocked if the current hp passes the limit
                            if (servitors.getCurrentHp() > servitors.getMaxRecoverableHp())
                            {
                                newHP = servitors.getCurrentHp();
                            }
                            else if (newHP > servitors.getMaxRecoverableHp())
                            {
                                newHP = servitors.getMaxRecoverableHp();
                            }
                        }

                        servitors.setCurrentHp(newHP);
                    }
                }
            }
        }
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}