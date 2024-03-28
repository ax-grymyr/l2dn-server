using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Model;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct PledgeCrestPacket: IOutgoingPacket
{
    private readonly int _crestId;
    private readonly byte[] _data;
    private readonly int _clanId;
	
    public PledgeCrestPacket(int crestId, int clanId)
    {
        _crestId = crestId;
        Crest crest = CrestTable.getInstance().getCrest(crestId);
        _data = crest != null ? crest.getData() : null;
        _clanId = clanId;
    }
	
    public PledgeCrestPacket(int crestId, byte[] data, int clanId)
    {
        _crestId = crestId;
        _data = data;
        _clanId = clanId;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.PLEDGE_CREST);
        
        writer.WriteInt32(_clanId);
        writer.WriteInt32(_crestId);
        if (_data != null)
        {
            writer.WriteInt32(_data.Length);
            writer.WriteInt32(_data.Length);
            writer.WriteBytes(_data);
        }
        else
        {
            writer.WriteInt32(0);
        }
    }
}