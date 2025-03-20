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

[HandlerName("GetMomentum")]
public sealed class GetMomentum: AbstractEffect
{
    public GetMomentum(EffectParameterSet parameters)
    {
        Ticks = parameters.GetInt32(XmlSkillEffectParameterType.Ticks, 0);
    }

    public override bool OnActionTime(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effected.getActingPlayer();
        if (effected.isPlayer() && player != null)
        {
            int maxCharge = (int)player.getStat().getValue(Stat.MAX_MOMENTUM, 1);
            int newCharge = Math.Min(player.getCharges() + 1, maxCharge);

            player.setCharges(newCharge);

            if (newCharge == maxCharge)
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

        return skill.IsToggle;
    }

    public override int GetHashCode() => Ticks;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x.Ticks);
}