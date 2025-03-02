using L2Dn.GameServer.InstanceManagers;
using L2Dn.Geometry;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Ranking;

public readonly struct ExRankingBuffZoneNpcPositionPacket: IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_RANKING_CHAR_BUFFZONE_NPC_POSITION);

        if (GlobalVariablesManager.getInstance().Get(GlobalVariablesManager.RANKING_POWER_COOLDOWN, DateTime.MinValue) > DateTime.UtcNow)
        {
            Location3D location = GlobalVariablesManager.getInstance().Get<Location3D>(GlobalVariablesManager.RANKING_POWER_LOCATION);
            writer.WriteByte(1);
            writer.WriteInt32(location.X);
            writer.WriteInt32(location.Y);
            writer.WriteInt32(location.Z);
        }
        else
        {
            writer.WriteByte(0);
            writer.WriteInt32(0);
            writer.WriteInt32(0);
            writer.WriteInt32(0);
        }
    }
}