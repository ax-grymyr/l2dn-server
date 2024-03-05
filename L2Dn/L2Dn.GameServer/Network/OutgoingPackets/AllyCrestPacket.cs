using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Model;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct AllyCrestPacket: IOutgoingPacket
{
    private readonly int _clanId;
    private readonly int _crestId;
    private readonly byte[] _data;
	
    public AllyCrestPacket(int crestId, int clanId)
    {
        _crestId = crestId;
        _clanId = clanId;
        Crest crest = CrestTable.getInstance().getCrest(crestId);
        _data = crest != null ? crest.getData() : null;
    }
	
    public AllyCrestPacket(int crestId, int clanId, byte[] data)
    {
        _crestId = crestId;
        _clanId = clanId;
        _data = data;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.ALLIANCE_CREST);
        
        writer.WriteInt32(_crestId);
        writer.WriteInt32(_clanId);
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