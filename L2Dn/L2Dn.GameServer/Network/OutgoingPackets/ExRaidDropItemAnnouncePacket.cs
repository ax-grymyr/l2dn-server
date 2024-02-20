using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExRaidDropItemAnnouncePacket: IOutgoingPacket
{
    private readonly string _killerName;
    private readonly int _npcId;
    private readonly ICollection<int> _items;
	
    public ExRaidDropItemAnnouncePacket(string killerName, int npcId, ICollection<int> items)
    {
        _killerName = killerName;
        _npcId = npcId + 1000000;
        _items = items;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_RAID_DROP_ITEM_ANNOUNCE);
        writer.WriteSizedString(_killerName);
        writer.WriteInt32(_npcId);
        writer.WriteInt32(_items.Count);

        foreach (int itemId in _items)
            writer.WriteInt32(itemId);
    }
}