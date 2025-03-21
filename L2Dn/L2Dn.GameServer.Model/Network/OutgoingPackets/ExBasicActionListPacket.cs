using System.Collections.Immutable;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.StaticData;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExBasicActionListPacket: IOutgoingPacket
{
    public static readonly ExBasicActionListPacket STATIC_PACKET = new(ActionData.Instance.GetActionIdList());
	
    private readonly ImmutableArray<int> _actionIds;
	
    public ExBasicActionListPacket(ImmutableArray<int> actionIds)
    {
        _actionIds = actionIds;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_BASIC_ACTION_LIST);

        if (_actionIds.IsDefaultOrEmpty)
        {
            writer.WriteInt32(0);
            return;
        }
        
        writer.WriteInt32(_actionIds.Length);
        foreach (int actionId in _actionIds)
            writer.WriteInt32(actionId);
    }
}