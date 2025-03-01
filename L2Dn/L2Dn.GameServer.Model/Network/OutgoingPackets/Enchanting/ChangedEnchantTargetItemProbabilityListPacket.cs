using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.Items.Enchant;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Enchanting;

public readonly struct ChangedEnchantTargetItemProbabilityListPacket(Player player, bool isMulti): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
	{
        // TODO: packet writes 6 integer values, but requires a lot of calculations
        //       to get the values. Logic must be moved out of packets.

		EnchantItemRequest? request = player.getRequest<EnchantItemRequest>();
		if (request == null)
			return;

        Item? enchantingItem = request.getEnchantingItem();
        Item? enchantingScroll = request.getEnchantingScroll();
		if ((!isMulti && enchantingItem == null) || request.isProcessing() || enchantingScroll == null)
			return;

        EnchantScroll? enchantScroll = EnchantItemData.getInstance().getEnchantScroll(enchantingScroll.getId());
        if (enchantScroll == null)
            return;

		int count = 1;
		if (isMulti)
		{
			count = request.getMultiEnchantingItemsCount();
		}

		writer.WritePacketCode(OutgoingPacketCodes.EX_CHANGED_ENCHANT_TARGET_ITEM_PROB_LIST);

		writer.WriteInt32(count);
		for (int i = 1; i <= count; i++)
		{
			// 100,00 % = 10000, because last 2 numbers going after float comma.
			double baseRate;
			double passiveRate;
			if (!isMulti || request.getMultiEnchantingItemsBySlot(i) != 0)
			{
				baseRate = GetBaseRate(request, enchantScroll, i);
				passiveRate = getPassiveRate(request, i);
			}
			else
			{
				baseRate = 0;
				passiveRate = 0;
			}
			double passiveBaseRate = 0;
			double supportRate = GetSupportRate(request);
			if (passiveRate != 0)
			{
				passiveBaseRate = baseRate * passiveRate / 10000;
			}
			double totalRate = baseRate + supportRate + passiveBaseRate;
			if (totalRate >= 10000)
			{
				totalRate = 10000;
			}

            if (!isMulti && enchantingItem != null)
			{
				writer.WriteInt32(enchantingItem.ObjectId);
			}
			else
			{
				writer.WriteInt32(request.getMultiEnchantingItemsBySlot(i));
			}

			writer.WriteInt32((int) totalRate); // Total success.
			writer.WriteInt32((int) baseRate); // Base success.
			writer.WriteInt32((int) supportRate); // Support success.
			writer.WriteInt32((int) passiveBaseRate); // Passive success (items, skills).
		}
	}

    private int GetBaseRate(EnchantItemRequest request, EnchantScroll enchantScroll, int iteration)
    {
        Item? item = isMulti
            ? player.getInventory().getItemByObjectId(request.getMultiEnchantingItemsBySlot(iteration))
            : request.getEnchantingItem();

        if (item == null)
            throw new InvalidOperationException("No enchanting item"); // TODO: verify

        return (int)Math.Min(100, enchantScroll.getChance(player, item) + enchantScroll.getBonusRate()) * 100;
    }

    private int GetSupportRate(EnchantItemRequest request)
    {
        double supportRate = 0;
        Item? supportItem = request.getSupportItem();
        if (!isMulti && supportItem != null)
        {
            // TODO: null check suppressed
            supportRate = EnchantItemData.getInstance().getSupportItem(supportItem.getId())?.getBonusRate() ?? 0;
            supportRate *= 100;
        }

        return (int)supportRate;
    }

    private int getPassiveRate(EnchantItemRequest request, int iteration)
	{
		double passiveRate = 0;
		if (player.getStat().getValue(Stat.ENCHANT_RATE) != 0)
        {
            Item? enchantingItem = request.getEnchantingItem();
			if (!isMulti && enchantingItem != null)
			{
				CrystalType crystalLevel = enchantingItem.getTemplate().getCrystalType().getLevel();
				if (crystalLevel == CrystalType.NONE.getLevel() || crystalLevel == CrystalType.EVENT.getLevel())
				{
					passiveRate = 0;
				}
				else
				{
					passiveRate = player.getStat().getValue(Stat.ENCHANT_RATE) * 100;
				}
			}
			else
            {
                CrystalType crystalLevel = player.getInventory().
                    getItemByObjectId(request.getMultiEnchantingItemsBySlot(iteration))?.getTemplate().getCrystalType().
                    getLevel() ?? CrystalType.NONE;

				if (crystalLevel == CrystalType.NONE.getLevel() || crystalLevel == CrystalType.EVENT.getLevel())
				{
					passiveRate = 0;
				}
				else
				{
					passiveRate = player.getStat().getValue(Stat.ENCHANT_RATE) * 100;
				}
			}
		}

		return (int) passiveRate;
	}
}