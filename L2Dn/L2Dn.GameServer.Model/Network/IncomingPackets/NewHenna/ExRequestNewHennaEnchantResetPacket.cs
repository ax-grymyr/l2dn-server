using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Henna;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets.NewHenna;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.NewHenna;

public struct ExRequestNewHennaEnchantResetPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
        //reader.ReadInt32(); // nCostItemId
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        int dailyReset = player.getDyePotentialDailyEnchantReset();
        ItemHolder enchant;
        try
        {
            enchant = HennaPatternPotentialData.getInstance().getEnchantReset()[dailyReset];
        }
        catch (Exception e)
        {
            PacketLogger.Instance.Warn(e);
            return ValueTask.CompletedTask;
        }
		
        if (dailyReset <= 9)
        {
            if (player.destroyItemByItemId("Reset fee", enchant.getId(), enchant.getCount(), player, true))
            {
                DyePotentialFee newFee = HennaPatternPotentialData.getInstance().getFee(1 /* daily step */);
                player.setDyePotentialDailyCount(newFee.getDailyCount());
                player.setDyePotentialDailyEnchantReset(dailyReset + 1);
                player.sendPacket(new NewHennaPotenEnchantResetPacket(true));
                player.sendPacket(new NewHennaListPacket(player, 1));
            }
            else
            {
                player.sendPacket(SystemMessageId.NOT_ENOUGH_MONEY_TO_USE_THE_FUNCTION);
            }
        }
        
        return ValueTask.CompletedTask;
    }
}