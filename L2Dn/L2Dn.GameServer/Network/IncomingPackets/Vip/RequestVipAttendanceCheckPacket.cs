using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets.Attendance;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Vip;

public struct RequestVipAttendanceCheckPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (Config.PREMIUM_ONLY_ATTENDANCE_REWARDS && !player.hasPremiumStatus())
        {
            player.sendPacket(SystemMessageId.YOUR_VIP_RANK_IS_TOO_LOW_TO_RECEIVE_THE_REWARD);
            return ValueTask.CompletedTask;
        }
        
        if (Config.VIP_ONLY_ATTENDANCE_REWARDS && (player.getVipTier() <= 0))
        {
            player.sendPacket(SystemMessageId.YOUR_VIP_RANK_IS_TOO_LOW_TO_RECEIVE_THE_REWARD);
            return ValueTask.CompletedTask;
        }
		
        if (!player.destroyItemByItemId("RequestVipAttendanceCheck", Inventory.LCOIN_ID, 100, player, true))
        {
            player.sendPacket(SystemMessageId.NOT_ENOUGH_MONEY_TO_USE_THE_FUNCTION);
            return ValueTask.CompletedTask;
        }
		
        player.sendPacket(new ExVipAttendanceCheckPacket(true));
        
        return ValueTask.CompletedTask;
    }
}