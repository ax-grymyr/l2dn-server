using L2Dn.GameServer.InstanceManagers;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExShowSeedMapInfoPacket: IOutgoingPacket
{
    public static readonly ExShowSeedMapInfoPacket STATIC_PACKET = new();
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SHOW_SEED_MAP_INFO);
        
        writer.WriteInt32(2); // seed count
        // Seed of Destruction
        writer.WriteInt32(1); // id 1? Grand Crusade
        writer.WriteInt32(2770 + GraciaSeedsManager.getInstance().getSoDState()); // sys msg id
        // Seed of Infinity
        writer.WriteInt32(2); // id 2? Grand Crusade
        // Manager not implemented yet
        writer.WriteInt32(2766); // sys msg id
    }
}