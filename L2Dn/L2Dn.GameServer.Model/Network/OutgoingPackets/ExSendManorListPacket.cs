using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExSendManorListPacket: IOutgoingPacket
{
    public static readonly ExSendManorListPacket STATIC_PACKET = new();
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SEND_MANOR_LIST);
        
        ICollection<Castle> castles = CastleManager.getInstance().getCastles();
        writer.WriteInt32(castles.Count);
        foreach (Castle castle in castles)
        {
            writer.WriteInt32(castle.getResidenceId());
        }
    }
}