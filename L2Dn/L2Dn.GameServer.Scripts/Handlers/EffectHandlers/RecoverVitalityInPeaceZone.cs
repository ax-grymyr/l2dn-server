using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Stats;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Recover Vitality in Peace Zone effect implementation.
/// </summary>
[HandlerStringKey("RecoverVitalityInPeaceZone")]
public sealed class RecoverVitalityInPeaceZone: AbstractEffect
{
    private readonly double _amount;

    public RecoverVitalityInPeaceZone(EffectParameterSet parameters)
    {
        _amount = parameters.GetDouble(XmlSkillEffectParameterType.Amount, 0);
        Ticks = parameters.GetInt32(XmlSkillEffectParameterType.Ticks, 10);
    }

    public override bool OnActionTime(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? effectedPlayer = effected.getActingPlayer();
        if (effectedPlayer == null //
            || effected.isDead() //
            || !effected.isPlayer() //
            || !effected.isInsideZone(ZoneId.PEACE))
        {
            return false;
        }

        double vitality = effectedPlayer.getVitalityPoints();
        vitality += _amount;
        if (vitality >= PlayerStat.MAX_VITALITY_POINTS)
            vitality = PlayerStat.MAX_VITALITY_POINTS;

        effectedPlayer.setVitalityPoints((int)vitality, true);
        return skill.IsToggle;
    }

    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        Player? effectedPlayer = effected.getActingPlayer();
        if (effectedPlayer != null //
            && effected.isPlayer())
        {
            BuffInfo? info = effected.getEffectList().getBuffInfoBySkillId(skill.Id);
            if (info != null && !info.isRemoved())
            {
                double vitality = effectedPlayer.getVitalityPoints();
                vitality += _amount * 100;
                if (vitality >= PlayerStat.MAX_VITALITY_POINTS)
                {
                    vitality = PlayerStat.MAX_VITALITY_POINTS;
                }

                effectedPlayer.setVitalityPoints((int)vitality, true);
            }
        }
    }

    public override int GetHashCode() => HashCode.Combine(_amount);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._amount);
}