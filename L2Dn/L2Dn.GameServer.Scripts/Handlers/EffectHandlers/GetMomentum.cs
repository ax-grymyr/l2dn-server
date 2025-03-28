using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class GetMomentum: AbstractEffect
{
    public GetMomentum(StatSet @params)
    {
        Ticks = @params.getInt("ticks", 0);
    }

    public override bool onActionTime(Creature effector, Creature effected, Skill skill, Item? item)
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

        return skill.isToggle();
    }

    public override int GetHashCode() => Ticks;
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x.Ticks);
}