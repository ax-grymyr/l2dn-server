using L2Dn.GameServer.Model.Skills;
using L2Dn.Model;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct AbnormalStatusUpdatePacket: IOutgoingPacket
{
    private readonly List<BuffInfo> _effects;

    public AbnormalStatusUpdatePacket(List<BuffInfo> effects)
    {
        _effects = effects;
    }

    public void addSkill(BuffInfo info)
    {
        _effects.Add(info);
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.ABNORMAL_STATUS_UPDATE);
        
        writer.WriteInt16((short)_effects.Count);
        foreach (BuffInfo info in _effects)
        {
            if ((info != null) && info.isInUse()) // TODO: does this make count invalid?????
            {
                writer.WriteInt32(info.getSkill().getDisplayId());
                writer.WriteInt16((short)info.getSkill().getDisplayLevel());
                
                if (Config.SERVER_LIST_TYPE != GameServerType.Classic)
                    writer.WriteInt16((short)info.getSkill().getSubLevel());
                
                writer.WriteInt32((int)info.getSkill().getAbnormalType());
                writer.WriteVariableInt(info.getSkill().isAura() || info.getSkill().isToggle()
                    ? -1
                    : (int)(info.getTime() ?? TimeSpan.Zero).TotalSeconds);
            }
        }
    }
}