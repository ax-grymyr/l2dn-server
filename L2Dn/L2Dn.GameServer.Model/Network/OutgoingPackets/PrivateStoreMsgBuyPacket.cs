using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct PrivateStoreMsgBuyPacket: IOutgoingPacket
{
    private readonly int _objId;
    private readonly string _storeMsg;
	
    public PrivateStoreMsgBuyPacket(Player player)
    {
        _objId = player.getObjectId();
        if (player.getBuyList() != null)
        {
            _storeMsg = player.getBuyList().getTitle();
        }
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.PRIVATE_STORE_BUY_MSG);
        
        writer.WriteInt32(_objId);
        writer.WriteString(_storeMsg);
    }
}