using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Model;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

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
            if (info != null && info.isInUse()) // TODO: does this make count invalid?????
            {
                writer.WriteInt32(info.getSkill().DisplayId);
                writer.WriteInt16((short)info.getSkill().DisplayLevel);

                if (ServerConfig.Instance.GameServerParams.ServerType != GameServerType.Classic)
                    writer.WriteInt16((short)info.getSkill().SubLevel);

                writer.WriteInt32((int)info.getSkill().AbnormalType);
                writer.WriteVariableInt(info.getSkill().IsAura || info.getSkill().IsToggle
                    ? -1
                    : (int)(info.getTime() ?? TimeSpan.Zero).TotalSeconds);
            }
        }
    }
}