using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Subjugation;

public readonly struct ExSubjugationListPacket: IOutgoingPacket
{
    private readonly List<KeyValuePair<int, PurgePlayerHolder>> _playerHolder;
	
    public ExSubjugationListPacket(Map<int, PurgePlayerHolder> playerHolder)
    {
        _playerHolder = playerHolder.Where(it => it.Value != null).ToList();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SUBJUGATION_LIST);
        
        writer.WriteInt32(_playerHolder.size());
        foreach (var integerPurgePlayerHolderEntry in _playerHolder)
        {
            writer.WriteInt32(integerPurgePlayerHolderEntry.Key);
            writer.WriteInt32(integerPurgePlayerHolderEntry.Value != null ? integerPurgePlayerHolderEntry.Value.getPoints() : 0);
            writer.WriteInt32(integerPurgePlayerHolderEntry.Value != null ? integerPurgePlayerHolderEntry.Value.getKeys() : 0);
            writer.WriteInt32(integerPurgePlayerHolderEntry.Value != null ? integerPurgePlayerHolderEntry.Value.getRemainingKeys() : 40);
        }
    }
}