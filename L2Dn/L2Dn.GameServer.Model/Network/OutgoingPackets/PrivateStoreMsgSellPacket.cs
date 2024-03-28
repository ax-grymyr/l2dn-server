using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct PrivateStoreMsgSellPacket: IOutgoingPacket
{
    private readonly int _objId;
    private readonly string _storeMsg;
	
    public PrivateStoreMsgSellPacket(Player player)
    {
        _objId = player.getObjectId();
        if ((player.getSellList() != null) || player.isSellingBuffs())
        {
            _storeMsg = player.getSellList().getTitle();
        }
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.PRIVATE_STORE_MSG);
        
        writer.WriteInt32(_objId);
        writer.WriteString(_storeMsg);
    }
}