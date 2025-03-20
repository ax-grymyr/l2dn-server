using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct SkillCoolTimePacket: IOutgoingPacket
{
    private readonly List<TimeStamp> _reuseTimestamps;

    public SkillCoolTimePacket(Player player)
    {
        _reuseTimestamps = new List<TimeStamp>();
        foreach (TimeStamp ts in player.getSkillReuseTimeStamps().Values)
        {
            Skill? skill = SkillData.getInstance().getSkill(ts.getSkillId(), ts.getSkillLevel(), ts.getSkillSubLevel());
            bool isNotBroadcastable = skill?.IsNotBroadcastable ?? false;
            if (ts.hasNotPassed() && !isNotBroadcastable)
            {
                _reuseTimestamps.Add(ts);
            }
        }
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.SKILL_COOL_TIME);

        writer.WriteInt32(_reuseTimestamps.Count);
        foreach (TimeStamp ts in _reuseTimestamps)
        {
            TimeSpan reuse = ts.getReuse();
            TimeSpan remaining = ts.getRemaining();
            int sharedReuseGroup = ts.getSharedReuseGroup();
            writer.WriteInt32(sharedReuseGroup > 0 ? sharedReuseGroup : ts.getSkillId());
            writer.WriteInt32(ts.getSkillLevel());
            writer.WriteInt32((int)(reuse > TimeSpan.Zero ? reuse : remaining).TotalSeconds);
            writer.WriteInt32((int)remaining.TotalSeconds);
        }
    }
}