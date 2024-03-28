using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExChangePostStatePacket: IOutgoingPacket
{
    private readonly bool _receivedBoard;
    private readonly int[] _changedMsgIds;
    private readonly int _changeId;
	
    public ExChangePostStatePacket(bool receivedBoard, int[] changedMsgIds, int changeId)
    {
        _receivedBoard = receivedBoard;
        _changedMsgIds = changedMsgIds;
        _changeId = changeId;
    }
	
    public ExChangePostStatePacket(bool receivedBoard, int changedMsgId, int changeId)
    {
        _receivedBoard = receivedBoard;
        _changedMsgIds = new int[]
        {
            changedMsgId
        };
        _changeId = changeId;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_CHANGE_POST_STATE);
        
        writer.WriteInt32(_receivedBoard);
        writer.WriteInt32(_changedMsgIds.Length);
        foreach (int postId in _changedMsgIds)
        {
            writer.WriteInt32(postId); // postId
            writer.WriteInt32(_changeId); // state
        }
    }
}