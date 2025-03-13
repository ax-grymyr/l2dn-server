using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct NpcInfoAbnormalVisualEffectPacket: IOutgoingPacket
{
    private readonly Npc _npc;

    public NpcInfoAbnormalVisualEffectPacket(Npc npc)
    {
        _npc = npc;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.NPC_INFO_ABNORMAL_VISUAL_EFFECT);

        writer.WriteInt32(_npc.ObjectId);
        writer.WriteInt32(_npc.getTransformationDisplayId());
        Set<AbnormalVisualEffect> abnormalVisualEffects = _npc.getEffectList().getCurrentAbnormalVisualEffects();
        Team team = Config.BLUE_TEAM_ABNORMAL_EFFECT != AbnormalVisualEffect.None && Config.RED_TEAM_ABNORMAL_EFFECT != AbnormalVisualEffect.None
            ? _npc.getTeam()
            : Team.NONE;

        writer.WriteInt32(abnormalVisualEffects.size() + (team != Team.NONE ? 1 : 0));
        foreach (AbnormalVisualEffect abnormalVisualEffect in abnormalVisualEffects)
        {
            writer.WriteInt16((short)abnormalVisualEffect);
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