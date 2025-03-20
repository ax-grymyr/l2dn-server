using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Focus Energy effect implementation.
/// </summary>
[AbstractEffectName("FocusMomentum")]
public sealed class FocusMomentum: AbstractEffect
{
    private readonly int _amount;
    private readonly int _maxCharges;

    public FocusMomentum(EffectParameterSet parameters)
    {
        _amount = parameters.GetInt32(XmlSkillEffectParameterType.Amount, 1);
        _maxCharges = parameters.GetInt32(XmlSkillEffectParameterType.MaxCharges, 0);
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effected.getActingPlayer();
        if (!effected.isPlayer() || player == null)
            return;

        int currentCharges = player.getCharges();
        int maxCharges = Math.Min(_maxCharges, (int)effected.getStat().getValue(Stat.MAX_MOMENTUM, 1));

        if (currentCharges >= maxCharges)
        {
            if (!skill.IsTriggeredSkill)
                player.sendPacket(SystemMessageId.YOUR_FORCE_HAS_REACHED_MAXIMUM_CAPACITY);

            return;
        }

        int newCharge = Math.Min(currentCharges + _amount, maxCharges);

        player.setCharges(newCharge);

        if (newCharge == maxCharges)
        {
            player.sendPacket(SystemMessageId.YOUR_FORCE_HAS_REACHED_MAXIMUM_CAPACITY);
        }
        else
        {
            SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOUR_FORCE_HAS_INCREASED_TO_LEVEL_S1);
            sm.Params.addInt(newCharge);
            player.sendPacket(sm);
        }

        player.sendPacket(new EtcStatusUpdatePacket(player));
    }

    public override int GetHashCode() => HashCode.Combine(_amount, _maxCharges);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._amount, x._maxCharges));
}