using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("ModifyMagicLampPoints")]
public sealed class ModifyMagicLampPoints: AbstractEffect
{
    private readonly int _amount;

    public ModifyMagicLampPoints(EffectParameterSet parameters)
    {
        _amount = parameters.GetInt32(XmlSkillEffectParameterType.Amount);
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected == null)
            return;

        Player? player = effected.getActingPlayer();
        if (player == null)
            return;

        MagicLampManager.getInstance().addLampExp(player, _amount, false);
    }

    public override int GetHashCode() => _amount;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._amount);
}