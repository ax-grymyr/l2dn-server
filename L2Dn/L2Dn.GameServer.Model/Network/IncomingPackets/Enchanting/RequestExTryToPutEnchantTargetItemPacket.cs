using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.Items.Enchant;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets.Enchanting;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;
using L2Dn.Network;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.IncomingPackets.Enchanting;

public struct RequestExTryToPutEnchantTargetItemPacket: IIncomingPacket<GameSession>
{
    private int _objectId;

    public void ReadContent(PacketBitReader reader)
    {
        _objectId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

		EnchantItemRequest? request = player.getRequest<EnchantItemRequest>();
		if (request == null || request.isProcessing())
			return ValueTask.CompletedTask;

		Item? scroll = request.getEnchantingScroll();
		if (scroll == null)
			return ValueTask.CompletedTask;

		Item? item = player.getInventory().getItemByObjectId(_objectId);
		if (item == null)
		{
			Util.handleIllegalPlayerAction(player,
				"RequestExTryToPutEnchantTargetItem: " + player +
				" tried to cheat using a packet manipulation tool! Ban this player!", Config.DEFAULT_PUNISH);

			return ValueTask.CompletedTask;
		}

		EnchantScroll? scrollTemplate = EnchantItemData.getInstance().getEnchantScroll(scroll.getId());
		if (!item.isEnchantable() || scrollTemplate == null || !scrollTemplate.isValid(item, null) ||
		    item.getEnchantLevel() >= scrollTemplate.getMaxEnchantLevel())
		{
			player.sendPacket(SystemMessageId.DOES_NOT_FIT_STRENGTHENING_CONDITIONS_OF_THE_SCROLL);
			request.setEnchantingItem(0);
			player.sendPacket(new ExPutEnchantTargetItemResultPacket(0));
			player.sendPacket(new EnchantResultPacket(EnchantResultPacket.ERROR, null, null, 0));
			player.sendPacket(new ExPutEnchantScrollItemResultPacket(1));
			if (scrollTemplate == null)
			{
				PacketLogger.Instance.Warn("RequestExTryToPutEnchantTargetItem: " + player +
				                           " has used undefined scroll with id " + scroll.getId());
			}

			return ValueTask.CompletedTask;
		}

		request.setEnchantingItem(_objectId);
		request.setEnchantLevel(item.getEnchantLevel());

		request.setTimestamp(DateTime.UtcNow);
		player.sendPacket(new ExPutEnchantTargetItemResultPacket(_objectId));
		player.sendPacket(new ChangedEnchantTargetItemProbabilityListPacket(player, false));

		double chance = scrollTemplate.getChance(player, item);
		if (chance > 0)
		{
			double challengePointsChance = 0;
			EnchantChallengePointData.EnchantChallengePointsItemInfo? info = EnchantChallengePointData.getInstance()
				.getInfoByItemId(item.getId());

			if (info != null)
			{
				int groupId = info.GroupId;
				int pendingGroupId = player.getChallengeInfo().getChallengePointsPendingRecharge()[0];
				int pendingOptionIndex = player.getChallengeInfo().getChallengePointsPendingRecharge()[1];
				if (pendingGroupId == groupId &&
				    (pendingOptionIndex == EnchantChallengePointData.OptionProbInc1 ||
				     pendingOptionIndex == EnchantChallengePointData.OptionProbInc2))
				{
					EnchantChallengePointData.EnchantChallengePointsOptionInfo? optionInfo = EnchantChallengePointData
						.getInstance().getOptionInfo(pendingGroupId, pendingOptionIndex);

					if (optionInfo != null && item.getEnchantLevel() >= optionInfo.MinEnchant &&
					    item.getEnchantLevel() <= optionInfo.MaxEnchant)
					{
						challengePointsChance = optionInfo.Chance;
						player.getChallengeInfo().setChallengePointsPendingRecharge(-1, -1);
					}
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
		}

		return ValueTask.CompletedTask;
    }
}