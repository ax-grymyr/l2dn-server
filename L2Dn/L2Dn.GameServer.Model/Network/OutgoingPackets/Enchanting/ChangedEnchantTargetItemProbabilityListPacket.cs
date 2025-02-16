using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.Items.Enchant;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Enchanting;

public readonly struct ChangedEnchantTargetItemProbabilityListPacket: IOutgoingPacket
{
	private readonly Player _player;
	private readonly bool _isMulti;
	
	public ChangedEnchantTargetItemProbabilityListPacket(Player player, bool isMulti)
	{
		_player = player;
		_isMulti = isMulti;
	}
	
	public void WriteContent(PacketBitWriter writer)
	{
		EnchantItemRequest request = _player.getRequest<EnchantItemRequest>();
		if (request == null)
		{
			return;
		}
		
		if ((!_isMulti && (request.getEnchantingItem() == null)) || request.isProcessing() || (request.getEnchantingScroll() == null))
		{
			return;
		}
		
		int count = 1;
		if (_isMulti)
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
			if (!_isMulti || (request.getMultiEnchantingItemsBySlot(i) != 0))
			{
				baseRate = getBaseRate(request, i);
				passiveRate = getPassiveRate(request, i);
			}
			else
			{
				baseRate = 0;
				passiveRate = 0;
			}
			double passiveBaseRate = 0;
			double supportRate = getSupportRate(request);
			if (passiveRate != 0)
			{
				passiveBaseRate = (baseRate * passiveRate) / 10000;
			}
			double totalRate = baseRate + supportRate + passiveBaseRate;
			if (totalRate >= 10000)
			{
				totalRate = 10000;
			}
			if (!_isMulti)
			{
				writer.WriteInt32(request.getEnchantingItem().ObjectId);
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
	
	private int getBaseRate(EnchantItemRequest request, int iteration)
	{
		EnchantScroll? enchantScroll = EnchantItemData.getInstance().getEnchantScroll(request.getEnchantingScroll().getId());
		return (int) Math.Min(100, enchantScroll.getChance(_player, _isMulti ? _player.getInventory().getItemByObjectId(request.getMultiEnchantingItemsBySlot(iteration)) : request.getEnchantingItem()) + enchantScroll.getBonusRate()) * 100;
	}
	
	private int getSupportRate(EnchantItemRequest request)
	{
		double supportRate = 0;
		if (!_isMulti && (request.getSupportItem() != null))
		{
			supportRate = EnchantItemData.getInstance().getSupportItem(request.getSupportItem().getId()).getBonusRate();
			supportRate = supportRate * 100;
		}
		return (int) supportRate;
	}
	
	private int getPassiveRate(EnchantItemRequest request, int iteration)
	{
		double passiveRate = 0;
		if (_player.getStat().getValue(Stat.ENCHANT_RATE) != 0)
		{
			if (!_isMulti)
			{
				CrystalType crystalLevel = request.getEnchantingItem().getTemplate().getCrystalType().getLevel();
				if ((crystalLevel == CrystalType.NONE.getLevel()) || (crystalLevel == CrystalType.EVENT.getLevel()))
				{
					passiveRate = 0;
				}
				else
				{
					passiveRate = _player.getStat().getValue(Stat.ENCHANT_RATE) * 100;
				}
			}
			else
			{
				CrystalType crystalLevel = _player.getInventory().getItemByObjectId(request.getMultiEnchantingItemsBySlot(iteration)).getTemplate().getCrystalType().getLevel();
				if ((crystalLevel == CrystalType.NONE.getLevel()) || (crystalLevel == CrystalType.EVENT.getLevel()))
				{
					passiveRate = 0;
				}
				else
				{
					passiveRate = _player.getStat().getValue(Stat.ENCHANT_RATE) * 100;
				}
			}
		}
		
		return (int) passiveRate;
	}
}