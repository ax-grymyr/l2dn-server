using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.OutgoingPackets.Teleports;

public readonly struct ExShowSharedLocationTeleportUiPacket(SharedTeleportHolder teleport): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SHARED_POSITION_TELEPORT_UI);

        writer.WriteSizedString(teleport.getName());
        writer.WriteInt32(teleport.getId());
        writer.WriteInt32(teleport.getCount());
        writer.WriteInt16(150);
        writer.WriteLocation3D(teleport.getLocation());
        writer.WriteInt64(Config.General.TELEPORT_SHARE_LOCATION_COST);
    }
}