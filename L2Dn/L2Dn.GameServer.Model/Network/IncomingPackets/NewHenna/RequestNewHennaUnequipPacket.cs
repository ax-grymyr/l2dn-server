using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Henna;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.NewHenna;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.NewHenna;

public struct RequestNewHennaUnequipPacket: IIncomingPacket<GameSession>
{
    private int _slotId;
    private int _itemId;

    public void ReadContent(PacketBitReader reader)
    {
        _slotId = reader.ReadByte();
        _itemId = reader.ReadInt32(); // CostItemId
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        // TODO: flood protection
        // if (!client.getFloodProtectors().canPerformTransaction())
        // {
        //     player.sendPacket(ActionFailedPacket.STATIC_PACKET);
        //     player.sendPacket(new NewHennaUnequipPacket(_slotId, 0));
        //     return ValueTask.CompletedTask;
        // }
		
        if (_slotId > player.getHennaPotenList().Length)
            return ValueTask.CompletedTask;
		
        Henna henna = player.getHenna(_slotId);
        if (henna == null)
        {
            PacketLogger.Instance.Warn(GetType().Name + ": " + player + " requested Henna Draw remove without any henna.");
            player.sendPacket(ActionFailedPacket.STATIC_PACKET);
            player.sendPacket(new NewHennaUnequipPacket(_slotId, 0));
            return ValueTask.CompletedTask;
        }
		
        int feeType = 0;
		
        if (_itemId == 57)
        {
            feeType = henna.getCancelFee();
        }
        else if (_itemId == 91663)
        {
            feeType = henna.getCancelL2CoinFee();
        }
		
        if (player.destroyItemByItemId("FeeType", _itemId, feeType, player, false))
        {
            player.removeHenna(_slotId);
            player.getStat().recalculateStats(true);
            player.sendPacket(new NewHennaUnequipPacket(_slotId, 1));
            player.sendPacket(new UserInfoPacket(player));
        }
        else
        {
            if (_itemId == 57)
            {
                player.sendPacket(SystemMessageId.YOU_DO_NOT_HAVE_ENOUGH_ADENA_TO_REGISTER_THE_ITEM);
            }
            else if (_itemId == 91663)
            {
                player.sendPacket(SystemMessageId.YOU_DO_NOT_HAVE_ENOUGH_L2_COINS_ADD_MORE_L2_COINS_AND_TRY_AGAIN);
            }
            
            player.sendPacket(ActionFailedPacket.STATIC_PACKET);
            player.sendPacket(new NewHennaUnequipPacket(_slotId, 0));
        }
       
        return ValueTask.CompletedTask;
    }
}