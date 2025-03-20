using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class HairAccessorySet: AbstractEffect
{
    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effected.getActingPlayer();
        if (!effector.isPlayer() || player == null || !effected.isPlayer() || effected.isAlikeDead())
            return;

        player.setHairAccessoryEnabled(!player.isHairAccessoryEnabled());
        player.broadcastUserInfo(UserInfoType.APPAREANCE);
        player.sendPacket(player.isHairAccessoryEnabled()
            ? SystemMessageId.HEAD_ACCESSORIES_ARE_VISIBLE_FROM_NOW_ON
            : SystemMessageId.HEAD_ACCESSORIES_ARE_NO_LONGER_SHOWN);
    }

    public override int GetHashCode() => this.GetSingletonHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj);
}