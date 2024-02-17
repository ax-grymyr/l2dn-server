using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExPrivateStoreSetWholeMsgPacket: IOutgoingPacket
{
    private readonly int _objectId;
    private readonly string _msg;
	
    public ExPrivateStoreSetWholeMsgPacket(Player player, string msg)
    {
        _objectId = player.getObjectId();
        _msg = msg;
    }
	
    public ExPrivateStoreSetWholeMsgPacket(Player player): this(player, player.getSellList().getTitle())
    {
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PRIVATE_STORE_WHOLE_MSG);
        writer.WriteInt32(_objectId);
        writer.WriteString(_msg);
    }
}