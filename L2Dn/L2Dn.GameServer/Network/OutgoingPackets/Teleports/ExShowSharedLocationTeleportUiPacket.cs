using L2Dn.GameServer.Model.Holders;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Teleports;

public readonly struct ExShowSharedLocationTeleportUiPacket: IOutgoingPacket
{
    private readonly SharedTeleportHolder _teleport;

    public ExShowSharedLocationTeleportUiPacket(SharedTeleportHolder teleport)
    {
        _teleport = teleport;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SHARED_POSITION_TELEPORT_UI);
        writer.WriteSizedString(_teleport.getName());
        writer.WriteInt32(_teleport.getId());
        writer.WriteInt32(_teleport.getCount());
        writer.WriteInt16(150);
        writer.WriteInt32(_teleport.getLocation().getX());
        writer.WriteInt32(_teleport.getLocation().getY());
        writer.WriteInt32(_teleport.getLocation().getZ());
        writer.WriteInt64(Config.TELEPORT_SHARE_LOCATION_COST);
    }
}