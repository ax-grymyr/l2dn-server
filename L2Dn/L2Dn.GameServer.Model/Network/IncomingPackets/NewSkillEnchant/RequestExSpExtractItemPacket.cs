using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Variables;
using L2Dn.GameServer.Network.OutgoingPackets.NewSkillEnchant;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.NewSkillEnchant;

public struct RequestExSpExtractItemPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if ((player.getSp() >= 5000000000L) && (player.getAdena() >= 3000000) && (player.getVariables()
                .getInt(PlayerVariables.DAILY_EXTRACT_ITEM + Inventory.SP_POUCH, 5) > 0))
        {
            player.removeExpAndSp(0, 5000000000L);
            player.broadcastUserInfo();
            player.reduceAdena("SpExtract", 3000000, null, true);
            player.addItem("AddSpExtract", Inventory.SP_POUCH, 1, null, true);
            int current = player.getVariables().getInt(PlayerVariables.DAILY_EXTRACT_ITEM + Inventory.SP_POUCH, 5);
            player.getVariables().set(PlayerVariables.DAILY_EXTRACT_ITEM + Inventory.SP_POUCH, current - 1);
            player.sendPacket(new ExSpExtractItemPacket());
        }

        return ValueTask.CompletedTask;
    }
}