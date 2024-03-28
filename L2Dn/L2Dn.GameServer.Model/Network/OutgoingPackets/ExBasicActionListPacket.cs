using L2Dn.GameServer.Data.Xml;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExBasicActionListPacket: IOutgoingPacket
{
    public static readonly ExBasicActionListPacket STATIC_PACKET = new(ActionData.getInstance().getActionIdList());
	
    private readonly int[] _actionIds;
	
    public ExBasicActionListPacket(int[] actionIds)
    {
        _actionIds = actionIds;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_BASIC_ACTION_LIST);
        
        writer.WriteInt32(_actionIds.Length);
        foreach (int actionId in _actionIds)
        {
            writer.WriteInt32(actionId);
        }
    }
}