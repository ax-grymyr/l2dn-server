using System.Collections.Immutable;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Attendance;

public readonly struct ExVipAttendanceListPacket: IOutgoingPacket
{
    private readonly TimeSpan _delayreward;
    private readonly byte _index;
    private readonly bool _available;
    private readonly ImmutableArray<ItemHolder> _rewardItems;

    public ExVipAttendanceListPacket(Player player)
    {
        AttendanceInfoHolder attendanceInfo = player.getAttendanceInfo();
        _index = (byte)attendanceInfo.getRewardIndex();
        _delayreward = player.getAttendanceDelay();
        _available = attendanceInfo.isRewardAvailable();
        _rewardItems = AttendanceRewardData.getInstance().getRewards();
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_VIP_ATTENDANCE_LIST);

        writer.WriteInt32(_rewardItems.Length);
        foreach (ItemHolder reward in _rewardItems)
        {
            writer.WriteInt32(reward.getId());
            writer.WriteInt64(reward.getCount());
            writer.WriteByte(0); // Enchant level?
        }

        writer.WriteInt32(1); // MinimumLevel
        writer.WriteInt32((int)_delayreward.TotalSeconds); // RemainCheckTime
        if (_available)
        {
            writer.WriteByte((byte)(_index + 1)); // RollBookDay
            if ((_delayreward == TimeSpan.Zero) && (_available))
            {
                writer.WriteByte((byte)(_index + 1)); // AttendanceDay
            }
            else
            {
                writer.WriteByte(_index); // AttendanceDay
            }

            writer.WriteByte(_index); // RewardDay
            writer.WriteByte(0); // FollowBaseDay
            // writeByte(_available);
            writer.WriteByte(0); // FollowBaseDay
        }
        else
        {
            writer.WriteByte(_index); // RollBookDay
            if ((_delayreward == TimeSpan.Zero) && (_available))
            {
                writer.WriteByte((byte)(_index + 1)); // AttendanceDay
            }
            else
            {
                writer.WriteByte(_index); // AttendanceDay
            }

            writer.WriteByte(_index); // RewardDay
            writer.WriteByte(0); // FollowBaseDay
            // writeByte(_available);
            writer.WriteByte(1); // FollowBaseDay
        }
    }
}