using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
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
            if ((info != null) && info.isInUse() && !info.getSkill().isToggle())
            {
                _effects.Add(info);
            }
        }
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ABNORMAL_STATUS_UPDATE_FROM_TARGET);

        writer.WriteInt32(_creature.getObjectId());
        writer.WriteInt16((short)_effects.Count);
        
        foreach (BuffInfo info in _effects)
        {
            writer.WriteInt32(info.getSkill().getDisplayId());
            writer.WriteInt16((short)info.getSkill().getDisplayLevel());
            writer.WriteInt16((short)info.getSkill().getSubLevel());
            writer.WriteInt16((short)info.getSkill().getAbnormalType());
            writer.WriteVariableInt(info.getSkill().isAura() ? -1 : info.getTime());
            writer.WriteInt32(info.getEffectorObjectId());
        }
    }
}