using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.LuckyGame;

public readonly struct ExBettingLuckyGameResultPacket: IOutgoingPacket
{
    public static readonly ExBettingLuckyGameResultPacket NORMAL_INVALID_ITEM_COUNT = new(LuckyGameResultType.INVALID_ITEM_COUNT, LuckyGameType.NORMAL);
    public static readonly ExBettingLuckyGameResultPacket LUXURY_INVALID_ITEM_COUNT = new(LuckyGameResultType.INVALID_ITEM_COUNT, LuckyGameType.LUXURY);
    public static readonly ExBettingLuckyGameResultPacket NORMAL_INVALID_CAPACITY = new(LuckyGameResultType.INVALID_CAPACITY, LuckyGameType.NORMAL);
    public static readonly ExBettingLuckyGameResultPacket LUXURY_INVALID_CAPACITY = new(LuckyGameResultType.INVALID_CAPACITY, LuckyGameType.LUXURY);

    private readonly LuckyGameResultType _result;
    private readonly LuckyGameType _type;
    private readonly Map<LuckyGameItemType, List<ItemHolder>> _rewards;
    private readonly int _ticketCount;
    private readonly int _size;

    public ExBettingLuckyGameResultPacket(LuckyGameResultType result, LuckyGameType type)
    {
        _result = result;
        _type = type;
        _rewards = new();
        _ticketCount = 0;
        _size = 0;
    }

    public ExBettingLuckyGameResultPacket(LuckyGameResultType result, LuckyGameType type, Map<LuckyGameItemType, List<ItemHolder>> rewards, int ticketCount)
    {
        _result = result;
        _type = type;
        _rewards = rewards;
        _ticketCount = ticketCount;
        _size = rewards.Count == 0 ? 0 : rewards.Values.Select(i => i.Count).Sum();
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_BETTING_LUCKY_GAME_RESULT);

        writer.WriteInt32((int)_result);
        writer.WriteInt32((int)_type);
        writer.WriteInt32(_ticketCount);
        writer.WriteInt32(_size);
        foreach (var reward in _rewards)
        {
            foreach (ItemHolder item in reward.Value)
            {
                writer.WriteInt32((int)reward.Key);
                writer.WriteInt32(item.Id);
                writer.WriteInt32((int) item.getCount());
            }
        }
    }
}