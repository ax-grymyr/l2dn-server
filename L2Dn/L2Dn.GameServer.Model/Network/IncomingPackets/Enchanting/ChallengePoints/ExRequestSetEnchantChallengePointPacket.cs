using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.Items.Enchant;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.OutgoingPackets.Enchanting;
using L2Dn.Model.Enums;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Enchanting.ChallengePoints;

public struct ExRequestSetEnchantChallengePointPacket: IIncomingPacket<GameSession>
{
    private int _useType;
    private bool _useTicket;

    public void ReadContent(PacketBitReader reader)
    {
        _useType = reader.ReadInt32();
        _useTicket = reader.ReadBoolean();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
	    Player? player = session.Player;
	    if (player == null)
		    return ValueTask.CompletedTask;

	    EnchantItemRequest? request = player.getRequest<EnchantItemRequest>();
	    if (request == null || request.isProcessing())
	    {
		    player.sendPacket(new ExSetEnchantChallengePointPacket(false));
		    return ValueTask.CompletedTask;
	    }

	    Item? item = request.getEnchantingItem();
	    if (item == null)
	    {
		    player.sendPacket(new ExSetEnchantChallengePointPacket(false));
		    return ValueTask.CompletedTask;
	    }

	    EnchantChallengePointData.EnchantChallengePointsItemInfo? info =
            EnchantChallengePointData.getInstance().getInfoByItemId(item.Id);

	    if (info == null)
	    {
		    player.sendPacket(new ExSetEnchantChallengePointPacket(false));
		    return ValueTask.CompletedTask;
	    }

	    int groupId = info.GroupId;
	    if (_useTicket)
	    {
		    int remainingRecharges = player.getChallengeInfo().getChallengePointsRecharges(groupId, _useType);
		    if (remainingRecharges > 0)
		    {
			    player.getChallengeInfo().setChallengePointsPendingRecharge(groupId, _useType);
			    player.sendPacket(new ExSetEnchantChallengePointPacket(true));
			    player.sendPacket(new ExEnchantChallengePointInfoPacket(player));
		    }
		    else
		    {
			    player.sendPacket(new ExSetEnchantChallengePointPacket(false));
			    return ValueTask.CompletedTask;
		    }
	    }
	    else
	    {
		    int remainingRecharges = player.getChallengeInfo().getChallengePointsRecharges(groupId, _useType);
		    if (remainingRecharges < EnchantChallengePointData.getInstance().getMaxTicketCharge())
		    {
			    int remainingPoints = player.getChallengeInfo().getChallengePoints().GetValueOrDefault(groupId);
			    int fee = EnchantChallengePointData.getInstance().getFeeForOptionIndex(_useType);
			    if (remainingPoints >= fee)
			    {
				    remainingPoints -= fee;
				    player.getChallengeInfo().getChallengePoints().put(groupId, remainingPoints);
				    player.getChallengeInfo().addChallengePointsRecharge(groupId, _useType, 1);
				    player.getChallengeInfo().setChallengePointsPendingRecharge(groupId, _useType);
				    player.sendPacket(new ExSetEnchantChallengePointPacket(true));
				    player.sendPacket(new ExEnchantChallengePointInfoPacket(player));
			    }
			    else
			    {
				    player.sendPacket(new ExSetEnchantChallengePointPacket(false));
				    return ValueTask.CompletedTask;
			    }
		    }
	    }

        Item? enchantingScroll = request.getEnchantingScroll();
        if (enchantingScroll == null)
        {
            player.sendPacket(new ExSetEnchantChallengePointPacket(false));
            player.removeRequest<EnchantItemRequest>();
            return ValueTask.CompletedTask;
        }

	    EnchantScroll? scrollTemplate = EnchantItemData.getInstance().getEnchantScroll(enchantingScroll.Id);
        if (scrollTemplate == null)
        {
            player.sendPacket(new ExSetEnchantChallengePointPacket(false));
            player.removeRequest<EnchantItemRequest>();
            return ValueTask.CompletedTask;
        }

        double chance = scrollTemplate.getChance(player, item);

	    double challengePointsChance = 0;
	    int pendingGroupId = player.getChallengeInfo().getChallengePointsPendingRecharge()[0];
	    int pendingOptionIndex = player.getChallengeInfo().getChallengePointsPendingRecharge()[1];
	    if (pendingGroupId == groupId && (pendingOptionIndex == EnchantChallengePointData.OptionProbInc1 ||
	                                      pendingOptionIndex == EnchantChallengePointData.OptionProbInc2))
	    {
		    EnchantChallengePointData.EnchantChallengePointsOptionInfo? optionInfo = EnchantChallengePointData
			    .getInstance().getOptionInfo(pendingGroupId, pendingOptionIndex);

		    if (optionInfo != null && item.getEnchantLevel() >= optionInfo.MinEnchant &&
		        item.getEnchantLevel() <= optionInfo.MaxEnchant)
		    {
			    challengePointsChance = optionInfo.Chance;
		    }
	    }

	    CrystalType crystalLevel = item.getTemplate().getCrystalType().getLevel();
	    double enchantRateStat =
		    crystalLevel > CrystalType.NONE.getLevel() && crystalLevel < CrystalType.EVENT.getLevel()
			    ? player.getStat().getValue(Stat.ENCHANT_RATE)
			    : 0;

	    player.sendPacket(new ExChangedEnchantTargetItemProbListPacket(
		    new ExChangedEnchantTargetItemProbListPacket.EnchantProbInfo(item.ObjectId,
			    (int)((chance + challengePointsChance + enchantRateStat) * 100), (int)(chance * 100),
			    (int)(challengePointsChance * 100), (int)(enchantRateStat * 100))));

	    return ValueTask.CompletedTask;
    }
}