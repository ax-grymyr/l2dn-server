using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events.Impl.Playables;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Soul Eating effect implementation.
/// </summary>
public sealed class SoulEating: AbstractEffect
{
    private readonly SoulType _type;
    private readonly int _expNeeded;
    private readonly double _maxSouls;

    public SoulEating(StatSet @params)
    {
        _type = @params.getEnum("type", SoulType.LIGHT);
        _expNeeded = @params.getInt("expNeeded");
        _maxSouls = @params.getDouble("maxSouls");
    }

    public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected.isPlayer())
        {
            Player player = (Player)effected;
            player.Events.Subscribe<OnPlayableExpChanged>(this, onExperienceReceived);
        }
    }

    public override void onExit(Creature effector, Creature effected, Skill skill)
    {
        if (effected.isPlayer())
        {
            Player player = (Player)effected;
            player.Events.Unsubscribe<OnPlayableExpChanged>(onExperienceReceived);
        }
    }

    public override void pump(Creature effected, Skill skill)
    {
        effected.getStat().mergeAdd(Stat.MAX_SOULS, _maxSouls);
    }

    private void onExperienceReceived(OnPlayableExpChanged ev)
    {
        Playable playable = ev.getPlayable();
        long exp = ev.getNewExp() - ev.getOldExp();

        // TODO: Verify logic.
        Player? player = playable.getActingPlayer();
        if (playable.isPlayer() && player != null && exp >= _expNeeded)
        {
            int maxSouls = (int)player.getStat().getValue(Stat.MAX_SOULS, 0);
            if (player.getChargedSouls(_type) >= maxSouls)
            {
                playable.sendPacket(SystemMessageId.YOU_CAN_T_ABSORB_MORE_SOULS);
                return;
            }

            player.increaseSouls(1, _type);

            WorldObject? target = player.getTarget();
            if (target != null && target.isNpc())
            {
                Npc npc = (Npc)target;
                player.broadcastPacket(new ExSpawnEmitterPacket(player, npc), 500);
            }
        }
    }

    public override int GetHashCode() => HashCode.Combine(_type, _expNeeded, _maxSouls);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._type, x._expNeeded, x._maxSouls));
}