using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExShowFortressInfoPacket: IOutgoingPacket
{
    public static readonly ExShowFortressInfoPacket STATIC_PACKET = new();
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SHOW_FORTRESS_INFO);
        
        ICollection<Fort> forts = FortManager.getInstance().getForts();
        writer.WriteInt32(forts.Count);
        foreach (Fort fort in forts)
        {
            Clan clan = fort.getOwnerClan();
            writer.WriteInt32(fort.getResidenceId());
            writer.WriteString(clan != null ? clan.getName() : "");
            writer.WriteInt32(fort.getSiege().isInProgress());
            // Time of possession
            writer.WriteInt32((int)(fort.getOwnedTime() ?? TimeSpan.Zero).TotalSeconds);
        }
    }
}