using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.Attendance;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Vip;

public struct RequestVipAttendanceItemRewardPacket: IIncomingPacket<GameSession>
{
    private int _day;

    public void ReadContent(PacketBitReader reader)
    {
        _day = reader.ReadByte();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (!Config.ENABLE_ATTENDANCE_REWARDS)
        {
            player.sendPacket(SystemMessageId.DUE_TO_A_SYSTEM_ERROR_THE_ATTENDANCE_REWARD_CANNOT_BE_RECEIVED_PLEASE_TRY_AGAIN_LATER_BY_GOING_TO_MENU_ATTENDANCE_CHECK);
            return ValueTask.CompletedTask;
        }
		
        if (Config.PREMIUM_ONLY_ATTENDANCE_REWARDS && !player.hasPremiumStatus())
        {
            player.sendPacket(SystemMessageId.YOUR_VIP_RANK_IS_TOO_LOW_TO_RECEIVE_THE_REWARD);
            return ValueTask.CompletedTask;
        }
        
        if (Config.VIP_ONLY_ATTENDANCE_REWARDS && player.getVipTier() <= 0)
        {
            player.sendPacket(SystemMessageId.YOUR_VIP_RANK_IS_TOO_LOW_TO_RECEIVE_THE_REWARD);
            return ValueTask.CompletedTask;
        }
		
        AttendanceInfoHolder attendanceInfo = player.getAttendanceInfo();
        int rewardIndex = attendanceInfo.getRewardIndex();
        List<ItemHolder> rewards = AttendanceRewardData.getInstance().getRewards();
		
        if (_day > 0 && _day <= rewards.Count)
        {
            // Claim all unreclaimed rewards before the current day.
            for (int i = rewardIndex; i < _day - 1; i++)
            {
                ItemHolder unreclaimedReward = rewards[i];
                player.addItem("Attendance Reward", unreclaimedReward, player, true);
            }
			
            // Claim the current day's reward
            ItemHolder reward = rewards[_day - 1]; // Subtract 1 because the index is 0-based.
            player.addItem("Attendance Reward", reward, player, true);
            player.setAttendanceInfo(_day); // Update reward index.
			
            // Send message.
            SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.YOU_VE_RECEIVED_YOUR_VIP_ATTENDANCE_REWARD_FOR_DAY_S1);
            msg.Params.addInt(_day);
            player.sendPacket(msg);
			
            // Send confirm packet.
            player.sendPacket(new ExVipAttendanceRewardPacket());
        }
        else
        {
            PacketLogger.Instance.Warn(GetType().Name + player + ": Invalid attendance day: " + _day);
        }
 
        return ValueTask.CompletedTask;
    }
}