using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExAskCoupleActionPacket: IOutgoingPacket
{
    private readonly int _objectId;
    private readonly int _actionId;
	
    public ExAskCoupleActionPacket(int charObjId, int social)
    {
        _objectId = charObjId;
        _actionId = social;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ASK_COUPLE_ACTION);

        writer.WriteInt32(_actionId);
        writer.WriteInt32(_objectId);
    }
}