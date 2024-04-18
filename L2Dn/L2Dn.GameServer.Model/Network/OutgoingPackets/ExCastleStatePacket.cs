using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.Model.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExCastleStatePacket: IOutgoingPacket
{
    private readonly int _castleId;
    private readonly CastleSide _castleSide;
	
    public ExCastleStatePacket(Castle castle)
    {
        _castleId = castle.getResidenceId();
        _castleSide = castle.getSide();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_CASTLE_STATE);
        
        writer.WriteInt32(_castleId);
        writer.WriteInt32((int)_castleSide);
    }
}