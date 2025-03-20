using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Punishment;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Block Chat effect implementation.
/// </summary>
[AbstractEffectName("BlockChat")]
public sealed class BlockChat: AbstractEffect
{
    public override bool CanStart(Creature effector, Creature effected, Skill skill)
    {
        return effected != null && effected.isPlayer();
    }

    public override EffectFlags EffectFlags => EffectFlags.CHAT_BLOCK;

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        PunishmentManager.getInstance().startPunishment(new PunishmentTask(0, effected.ObjectId.ToString(),
            PunishmentAffect.CHARACTER, PunishmentType.CHAT_BAN, null, "Chat banned bot report", "system", true));
    }

    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        PunishmentManager.getInstance().stopPunishment(effected.ObjectId.ToString(), PunishmentAffect.CHARACTER,
            PunishmentType.CHAT_BAN);
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}