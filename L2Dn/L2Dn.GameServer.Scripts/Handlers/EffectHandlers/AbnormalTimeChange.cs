using System.Collections.Frozen;
using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class AbnormalTimeChange: AbstractEffect
{
    private readonly FrozenSet<AbnormalType> _abnormals;
    private readonly TimeSpan? _time;
    private readonly int _mode;

    public AbnormalTimeChange(StatSet @params)
    {
        string abnormals = @params.getString("slot", string.Empty);
        _abnormals = ParseUtil.ParseEnumSet<AbnormalType>(abnormals);

        int time = @params.getInt("time", -1);
        _time = time == -1 ? null : TimeSpan.FromSeconds(time);

        _mode = @params.getString("mode", "DEBUFF") switch
        {
            "DIFF" => 0,
            "DEBUFF" => 1,
            _ => throw new ArgumentException("Mode should be DIFF or DEBUFF for AbnormalTimeChange effect"),
        };
    }

    public override bool isInstant() => true;

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        AbnormalStatusUpdatePacket asu = new AbnormalStatusUpdatePacket([]);

        switch (_mode)
        {
            case 0: // DIFF
            {
                if (_abnormals.Count == 0)
                {
                    foreach (BuffInfo info in effected.getEffectList().getEffects())
                    {
                        if (info.getSkill().canBeDispelled())
                        {
                            info.resetAbnormalTime(info.getTime() + _time);
                            asu.addSkill(info);
                        }
                    }
                }
                else
                {
                    foreach (BuffInfo info in effected.getEffectList().getEffects())
                    {
                        if (info.getSkill().canBeDispelled() && _abnormals.Contains(info.getSkill().getAbnormalType()))
                        {
                            info.resetAbnormalTime(info.getTime() + _time);
                            asu.addSkill(info);
                        }
                    }
                }

                break;
            }
            case 1: // DEBUFF
            {
                if (_abnormals.Count == 0)
                {
                    foreach (BuffInfo info in effected.getEffectList().getDebuffs())
                    {
                        if (info.getSkill().canBeDispelled())
                        {
                            info.resetAbnormalTime(info.getAbnormalTime());
                            asu.addSkill(info);
                        }
                    }
                }
                else
                {
                    foreach (BuffInfo info in effected.getEffectList().getDebuffs())
                    {
                        if (info.getSkill().canBeDispelled() && _abnormals.Contains(info.getSkill().getAbnormalType()))
                        {
                            info.resetAbnormalTime(info.getAbnormalTime());
                            asu.addSkill(info);
                        }
                    }
                }

                break;
            }
        }

        effected.sendPacket(asu);

        ExAbnormalStatusUpdateFromTargetPacket upd = new ExAbnormalStatusUpdateFromTargetPacket(effected);
        foreach (Creature creature in effected.getStatus().getStatusListener())
        {
            if (creature.isPlayer())
                creature.sendPacket(upd);
        }

        if (effected.isPlayer() && effected.getTarget() == effected)
            effected.sendPacket(upd);
    }

    public override int GetHashCode() => HashCode.Combine(_abnormals.GetSetHashCode(), _time, _mode);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._abnormals.GetSetComparable(), x._time, x._mode));
}