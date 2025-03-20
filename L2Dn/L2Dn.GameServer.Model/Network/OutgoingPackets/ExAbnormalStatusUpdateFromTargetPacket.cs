using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExAbnormalStatusUpdateFromTargetPacket: IOutgoingPacket
{
    private readonly Creature _creature;
    private readonly List<BuffInfo> _effects;

    public ExAbnormalStatusUpdateFromTargetPacket(Creature creature)
    {
        _creature = creature;
        _effects = new List<BuffInfo>();
        foreach (BuffInfo info in creature.getEffectList().getEffects())
        {
            if (info != null && info.isInUse() && !info.getSkill().IsToggle)
            {
                _effects.Add(info);
            }
        }
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ABNORMAL_STATUS_UPDATE_FROM_TARGET);

        writer.WriteInt32(_creature.ObjectId);
        writer.WriteInt16((short)_effects.Count);

        foreach (BuffInfo info in _effects)
        {
            Skill skill = info.getSkill();
            writer.WriteInt32(skill.DisplayId);
            writer.WriteInt16((short)skill.DisplayLevel);
            writer.WriteInt16((short)skill.SubLevel);
            writer.WriteInt16((short)skill.AbnormalType);
            if (skill.IsAura)
                writer.WriteVariableInt(-1);
            else
                writer.WriteVariableInt((int)(info.getTime() ?? TimeSpan.Zero).TotalSeconds);

            writer.WriteInt32(info.getEffectorObjectId());
        }
    }
}