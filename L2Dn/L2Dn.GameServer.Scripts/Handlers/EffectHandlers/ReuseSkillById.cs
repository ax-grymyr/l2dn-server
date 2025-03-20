using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class ReuseSkillById: AbstractEffect
{
    private readonly int _skillId;
    private readonly int _amount;

    public ReuseSkillById(StatSet @params)
    {
        _skillId = @params.getInt("skillId", 0);
        _amount = @params.getInt("amount", 0);
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effector.getActingPlayer();
        if (player != null)
        {
            Skill? s = player.getKnownSkill(_skillId);
            if (s != null)
            {
                if (_amount > 0)
                {
                    TimeSpan reuse = player.getSkillRemainingReuseTime(s.ReuseHashCode);
                    if (reuse > TimeSpan.Zero)
                    {
                        TimeSpan diff = reuse - TimeSpan.FromMilliseconds(_amount);
                        diff = Algorithms.Max(diff, TimeSpan.Zero);

                        player.removeTimeStamp(s);
                        player.addTimeStamp(s, diff);
                        player.sendPacket(new SkillCoolTimePacket(player));
                    }
                }
                else
                {
                    player.removeTimeStamp(s);
                    player.enableSkill(s);
                    player.sendPacket(new SkillCoolTimePacket(player));
                }
            }
        }
    }

    public override int GetHashCode() => HashCode.Combine(_skillId, _amount);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._skillId, x._amount));
}