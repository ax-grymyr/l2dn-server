using L2Dn.GameServer.StaticData;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExPledgeEmblemPacket: IOutgoingPacket
{
    private const int TOTAL_SIZE = 65664;

    private readonly int _crestId;
    private readonly int _clanId;
    private readonly byte[] _data;
    private readonly int _chunkId;

    public ExPledgeEmblemPacket(int crestId, byte[] chunkedData, int clanId, int chunkId)
    {
        _crestId = crestId;
        _data = chunkedData;
        _clanId = clanId;
        _chunkId = chunkId;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PLEDGE_EMBLEM);

        writer.WriteInt32(Config.SERVER_ID);
        writer.WriteInt32(_clanId);
        writer.WriteInt32(_crestId);
        writer.WriteInt32(_chunkId);
        writer.WriteInt32(TOTAL_SIZE);
        if (_data != null)
        {
            writer.WriteInt32(_data.Length);
            writer.WriteBytes(_data);
        }
        else
        {
            writer.WriteInt32(0);
        }
    }
}