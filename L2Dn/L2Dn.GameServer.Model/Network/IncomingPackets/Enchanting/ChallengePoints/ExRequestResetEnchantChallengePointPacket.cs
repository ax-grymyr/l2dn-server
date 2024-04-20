using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.Items.Enchant;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.OutgoingPackets.Enchanting;
using L2Dn.Model.Enums;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Enchanting.ChallengePoints;

public struct ExRequestResetEnchantChallengePointPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        player.getChallengeInfo().setChallengePointsPendingRecharge(-1, -1);
        player.sendPacket(new ExResetEnchantChallengePointPacket(true));

        EnchantItemRequest request = player.getRequest<EnchantItemRequest>();
        if (request == null || request.isProcessing())
            return ValueTask.CompletedTask;

        EnchantScroll scrollTemplate = EnchantItemData.getInstance().getEnchantScroll(request.getEnchantingScroll());
        double chance = scrollTemplate.getChance(player, request.getEnchantingItem());

        CrystalType crystalLevel = request.getEnchantingItem().getTemplate().getCrystalType().getLevel();
        double enchantRateStat =
            crystalLevel > CrystalType.NONE.getLevel() && crystalLevel < CrystalType.EVENT.getLevel()
                ? player.getStat().getValue(Stat.ENCHANT_RATE)
                : 0;

        player.sendPacket(new ExChangedEnchantTargetItemProbListPacket(
            new ExChangedEnchantTargetItemProbListPacket.EnchantProbInfo(request.getEnchantingItem().getObjectId(),
                (int)((chance + enchantRateStat) * 100), (int)(chance * 100), 0, (int)(enchantRateStat * 100))));

        return ValueTask.CompletedTask;
    }
}