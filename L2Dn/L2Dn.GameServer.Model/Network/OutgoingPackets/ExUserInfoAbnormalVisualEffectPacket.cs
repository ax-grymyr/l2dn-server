using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExUserInfoAbnormalVisualEffectPacket: IOutgoingPacket
{
    private readonly Player _player;

    public ExUserInfoAbnormalVisualEffectPacket(Player player)
    {
        _player = player;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_USER_INFO_ABNORMAL_VISUAL_EFFECT);
        writer.WriteInt32(_player.ObjectId);
        writer.WriteInt32(_player.getTransformationId());
        Set<AbnormalVisualEffect> abnormalVisualEffects = _player.getEffectList().getCurrentAbnormalVisualEffects();
        Team team = Config.BLUE_TEAM_ABNORMAL_EFFECT != AbnormalVisualEffect.None && Config.RED_TEAM_ABNORMAL_EFFECT != AbnormalVisualEffect.None ? _player.getTeam() : Team.NONE;
        bool isInvisible = _player.isInvisible();
        writer.WriteInt32(abnormalVisualEffects.size() + (isInvisible ? 1 : 0) + (team != Team.NONE ? 1 : 0));
        foreach (AbnormalVisualEffect abnormalVisualEffect in abnormalVisualEffects)
        {
            writer.WriteInt16((short)abnormalVisualEffect);
        }
        if (isInvisible)
        {
            writer.WriteInt16((short)AbnormalVisualEffect.STEALTH);
        }
        if (team == Team.BLUE)
        {
            if (Config.BLUE_TEAM_ABNORMAL_EFFECT != AbnormalVisualEffect.None)
            {
                writer.WriteInt16((short)Config.BLUE_TEAM_ABNORMAL_EFFECT);
            }
        }
        else if (team == Team.RED && Config.RED_TEAM_ABNORMAL_EFFECT != AbnormalVisualEffect.None)
        {
            writer.WriteInt16((short)Config.RED_TEAM_ABNORMAL_EFFECT);
        }
    }
}