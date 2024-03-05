using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Enchant;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Enchanting;

public readonly struct ResetEnchantItemFailRewardInfoPacket: IOutgoingPacket
{
	private readonly Player _player;
	
	public ResetEnchantItemFailRewardInfoPacket(Player player)
	{
		_player = player;
	}
	
	public void WriteContent(PacketBitWriter writer)
	{
		EnchantItemRequest request = _player.getRequest<EnchantItemRequest>();
		if (request == null)
		{
			return;
		}
		
		if ((request.getEnchantingItem() == null) || request.isProcessing() || (request.getEnchantingScroll() == null))
		{
			return;
		}
		
		EnchantScroll enchantScroll = EnchantItemData.getInstance().getEnchantScroll(request.getEnchantingScroll());
		Item enchantItem = request.getEnchantingItem();
		Item addedItem = new Item(enchantItem.getId());
		addedItem.setOwnerId(_player.getObjectId());
		addedItem.setEnchantLevel(request.getEnchantingItem().getEnchantLevel());
		EnchantSupportItem enchantSupportItem = null;
		ItemHolder result = null;
		if (request.getSupportItem() != null)
		{
			enchantSupportItem = EnchantItemData.getInstance().getSupportItem(request.getSupportItem());
		}
		if (enchantScroll.isBlessed() || ((request.getSupportItem() != null) && (enchantSupportItem != null) && enchantSupportItem.isBlessed()))
		{
			addedItem.setEnchantLevel(0);
		}
		else if (enchantScroll.isBlessedDown() || enchantScroll.isCursed() /* || ((request.getSupportItem() != null) && (enchantSupportItem != null) && enchantSupportItem.isDown()) */)
		{
			addedItem.setEnchantLevel(enchantItem.getEnchantLevel() - 1);
		}
		else if (enchantScroll.isSafe())
		{
			addedItem.setEnchantLevel(enchantItem.getEnchantLevel());
		}
		else
		{
			addedItem = null;
			if (enchantItem.getTemplate().isCrystallizable())
			{
				result = new ItemHolder(enchantItem.getTemplate().getCrystalItemId(), Math.Max(0, enchantItem.getCrystalCount() - ((enchantItem.getTemplate().getCrystalCount() + 1) / 2)));
			}
		}
		
		writer.WritePacketCode(OutgoingPacketCodes.EX_RES_ENCHANT_ITEM_FAIL_REWARD_INFO);
		
		writer.WriteInt32(enchantItem.getObjectId());
		
		int challengeGroup = _player.getChallengeInfo().getNowGroup();
		int challengePoint = _player.getChallengeInfo().getNowPoint();
		writer.WriteInt32(challengeGroup);
		writer.WriteInt32(challengePoint);
		
		if (result != null)
		{
			writer.WriteInt32(1); // Loop count.
			writer.WriteInt32(result.getId());
			writer.WriteInt32((int) result.getCount());
		}
		else if (addedItem != null)
		{
			writer.WriteInt32(1); // Loop count.
			writer.WriteInt32(enchantItem.getId());
			writer.WriteInt32(1);
		}
		else
		{
			writer.WriteInt32(0); // Loop count.
		}
	}
}