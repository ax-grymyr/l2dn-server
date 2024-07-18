using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Ensoul;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.WorldExchange;

public readonly struct WorldExchangeSettleListPacket: IOutgoingPacket
{
	private readonly Player _player;
	
	public WorldExchangeSettleListPacket(Player player)
	{
		_player = player;
	}
	
	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.EX_WORLD_EXCHANGE_SETTLE_LIST);
		
		Map<WorldExchangeItemStatusType, List<WorldExchangeHolder>> holders = WorldExchangeManager.getInstance().getPlayerBids(_player.getObjectId());
		if (holders.Count == 0)
		{
			writer.WriteInt32(0); // RegiItemDataList
			writer.WriteInt32(0); // RecvItemDataList
			writer.WriteInt32(0); // TimeOutItemDataList
			return;
		}
		
		writer.WriteInt32(holders.get(WorldExchangeItemStatusType.WORLD_EXCHANGE_REGISTERED).Count);
		foreach (WorldExchangeHolder holder in holders.get(WorldExchangeItemStatusType.WORLD_EXCHANGE_REGISTERED))
			getItemInfo(writer, holder);
		
		writer.WriteInt32(holders.get(WorldExchangeItemStatusType.WORLD_EXCHANGE_SOLD).Count);
		foreach (WorldExchangeHolder holder in holders.get(WorldExchangeItemStatusType.WORLD_EXCHANGE_SOLD))
			getItemInfo(writer, holder);
		
		writer.WriteInt32(holders.get(WorldExchangeItemStatusType.WORLD_EXCHANGE_OUT_TIME).Count);
		foreach (WorldExchangeHolder holder in holders.get(WorldExchangeItemStatusType.WORLD_EXCHANGE_OUT_TIME))
			getItemInfo(writer, holder);
	}
	
	private void getItemInfo(PacketBitWriter writer, WorldExchangeHolder holder)
	{
		writer.WriteInt64(holder.getWorldExchangeId());
		writer.WriteInt64(holder.getPrice());
		writer.WriteInt32(holder.getEndTime().getEpochSecond()); // TODO can be wrong
		
		Item item = holder.getItemInstance();
		writer.WriteInt32(item.getId());
		writer.WriteInt64(item.getCount());
		writer.WriteInt32(item.getEnchantLevel() < 1 ? 0 : item.getEnchantLevel());
		
		VariationInstance iv = item.getAugmentation();
		writer.WriteInt32(iv != null ? iv.getOption1Id() : 0);
		writer.WriteInt32(iv != null ? iv.getOption2Id() : 0);
		writer.WriteInt32(-1); // IntensiveItemClassID
		writer.WriteInt16(item.getAttackAttribute() != null ? (short)item.getAttackAttribute().getType() : (short)0);
		writer.WriteInt16(item.getAttackAttribute() != null ? (short)item.getAttackAttribute().getValue() : (short)0);
		writer.WriteInt16((short)item.getDefenceAttribute(AttributeType.FIRE));
		writer.WriteInt16((short)item.getDefenceAttribute(AttributeType.WATER));
		writer.WriteInt16((short)item.getDefenceAttribute(AttributeType.WIND));
		writer.WriteInt16((short)item.getDefenceAttribute(AttributeType.EARTH));
		writer.WriteInt16((short)item.getDefenceAttribute(AttributeType.HOLY));
		writer.WriteInt16((short)item.getDefenceAttribute(AttributeType.DARK));
		writer.WriteInt32(item.getVisualId());
		
		List<EnsoulOption> soul = (List<EnsoulOption>) holder.getItemInfo().getSoulCrystalOptions();
		try
		{
			writer.WriteInt32(soul[0].getId());
		}
		catch (IndexOutOfRangeException ignored)
		{
			writer.WriteInt32(0); // TODO logging
		}
		
		try
		{
			writer.WriteInt32(soul[1].getId());
		}
		catch (IndexOutOfRangeException ignored)
		{
			writer.WriteInt32(0); // TODO logging
		}
		
		List<EnsoulOption> specialSoul = (List<EnsoulOption>) holder.getItemInfo().getSoulCrystalSpecialOptions();
		try
		{
			writer.WriteInt32(specialSoul[0].getId());
		}
		catch (IndexOutOfRangeException ignored)
		{
			writer.WriteInt32(0); // TODO logging
		}
		
		writer.WriteInt16(item.isBlessed());
	}
}