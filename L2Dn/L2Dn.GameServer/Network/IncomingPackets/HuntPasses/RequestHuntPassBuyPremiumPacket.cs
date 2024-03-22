using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets.HuntPasses;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.HuntPasses;

public struct RequestHuntPassBuyPremiumPacket: IIncomingPacket<GameSession>
{
    private int _huntPassType;

    public void ReadContent(PacketBitReader reader)
    {
        _huntPassType = reader.ReadByte();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        DateTime calendar = DateTime.Now;
        if (calendar.Day == Config.HUNT_PASS_PERIOD && calendar.Hour == 6 && calendar.Minute < 30)
        {
            player.sendPacket(SystemMessageId
                .CURRENTLY_UNAVAILABLE_FOR_PURCHASE_YOU_CAN_BUY_THE_SEASON_PASS_ADDITIONAL_REWARDS_ONLY_UNTIL_6_30_A_M_OF_THE_SEASON_S_LAST_DAY);

            return ValueTask.CompletedTask;
        }

        if (!player.destroyItemByItemId("RequestHuntPassBuyPremium", Config.HUNT_PASS_PREMIUM_ITEM_ID,
                Config.HUNT_PASS_PREMIUM_ITEM_COUNT, player, true))
        {
            player.sendPacket(SystemMessageId.NOT_ENOUGH_MONEY_TO_USE_THE_FUNCTION);
            return ValueTask.CompletedTask;
        }

        player.getHuntPass().setPremium(true);
        player.sendPacket(new HuntPassSayhasSupportInfoPacket(player));
        player.sendPacket(new HuntPassInfoPacket(player, _huntPassType));
        
        return ValueTask.CompletedTask;
    }
}