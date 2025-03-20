using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Increase PcCafe points permanently.
/// </summary>
public sealed class AddPcCafePoints: AbstractEffect
{
    private readonly int _amount;

    public AddPcCafePoints(EffectParameterSet parameters)
    {
        _amount = parameters.GetInt32(XmlSkillEffectParameterType.Amount, 0);
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effected.getActingPlayer();
        if (!effected.isPlayer() || player == null)
            return;

        int currentPoints = player.getPcCafePoints();
        int upgradePoints = currentPoints + _amount;
        player.setPcCafePoints(upgradePoints);
        SystemMessagePacket message = new SystemMessagePacket(SystemMessageId.YOU_EARNED_S1_PA_POINT_S);
        message.Params.addInt(_amount);
        player.sendPacket(message);
        player.sendPacket(new ExPcCafePointInfoPacket(currentPoints, upgradePoints, 1));
    }

    public override int GetHashCode() => _amount;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._amount);
}