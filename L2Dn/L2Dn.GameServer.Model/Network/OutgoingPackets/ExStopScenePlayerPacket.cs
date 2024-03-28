using L2Dn.GameServer.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExStopScenePlayerPacket: IOutgoingPacket
{
    private readonly Movie _movie;
	
    public ExStopScenePlayerPacket(Movie movie)
    {
        _movie = movie;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_STOP_SCENE_PLAYER);
        
        writer.WriteInt32((int)_movie);
    }
}