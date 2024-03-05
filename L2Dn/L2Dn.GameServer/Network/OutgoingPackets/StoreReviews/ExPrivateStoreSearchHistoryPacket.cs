using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.StoreReviews;

public readonly struct ExPrivateStoreSearchHistoryPacket: IOutgoingPacket
{
    private readonly int _page;
    private readonly int _maxPage;
    private readonly List<PrivateStoreHistoryManager.ItemHistoryTransaction> _history;
	
    public ExPrivateStoreSearchHistoryPacket(int page, int maxPage, List<PrivateStoreHistoryManager.ItemHistoryTransaction> history)
    {
        _page = page;
        _maxPage = maxPage;
        _history = history;
    }
	
    /**
     * 338 struct _S_EX_PRIVATE_STORE_SEARCH_HISTORY { var int cCurrentPage; var int cMaxPage; var array<_pkPSSearchHistory> histories; }; struct _pkPSSearchHistory { var int nClassID; var int cStoreType; var int cEnchant; var INT64 nPrice; var INT64 nAmount; }; // S: FE D502 01 - cPage 01 -
     * cMaxPage E6000000 - nSize nClassID cStoreType cEnchant nPrice nAmount 4E000000 00 00 7F96980000000000 0100000000000000 4F000000 00 00 7F96980000000000 0100000000000000 5B000000 00 00 80C3C90100000000 0100000000000000 62000000 00 00 002D310100000000 0100000000000000 6E000000 00 00
     * 80841E0000000000 0100000000000000 C6000000 00 00 FF117A0000000000 0100000000000000
     */
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PRIVATE_STORE_SEARCH_HISTORY);
        
        writer.WriteByte((byte)_page); // cPage
        writer.WriteByte((byte)_maxPage); // cMaxPage
		
        writer.WriteInt32(_history.Count); // nSize -> Items count for loop below
		
        for (int i = 0; i < _history.Count; i++)
        {
            PrivateStoreHistoryManager.ItemHistoryTransaction transaction = _history[i];
            writer.WriteInt32(transaction.getItemId()); // itemId
            writer.WriteByte((byte)(transaction.getTransactionType() == PrivateStoreType.SELL ? 0x00 : 0x01)); // cStoreType
            writer.WriteByte((byte)transaction.getEnchantLevel()); // cEnchant
            writer.WriteInt64(transaction.getPrice() / transaction.getCount()); // nPrice
            writer.WriteInt64(transaction.getCount()); // nAmount
        }
    }
}