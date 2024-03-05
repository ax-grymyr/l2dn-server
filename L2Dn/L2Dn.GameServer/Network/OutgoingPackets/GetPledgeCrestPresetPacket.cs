using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct GetPledgeCrestPresetPacket: IOutgoingPacket
{
    private readonly int _clanId;
    private readonly int _emblemId;
	
    public GetPledgeCrestPresetPacket(int pledgeId, int crestId)
    {
        _clanId = pledgeId;
        _emblemId = crestId;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_GET_PLEDGE_CREST_PRESET);
        
        writer.WriteInt32(1);
        writer.WriteInt32(_clanId);
        writer.WriteInt32(_emblemId);
    }
}