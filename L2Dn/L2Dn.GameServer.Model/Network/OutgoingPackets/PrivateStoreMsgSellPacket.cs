using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct PrivateStoreMsgSellPacket: IOutgoingPacket
{
    private readonly int _objId;
    private readonly string _storeMsg;

    public PrivateStoreMsgSellPacket(Player player)
    {
        _objId = player.ObjectId;
        _storeMsg = string.Empty;

        TradeList sellList = player.getSellList();
        if (sellList != null || player.isSellingBuffs())
            _storeMsg = sellList.getTitle();
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.PRIVATE_STORE_MSG);

        writer.WriteInt32(_objId);
        writer.WriteString(_storeMsg);
    }
}