using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct PartyMemberPositionPacket: IOutgoingPacket
{
    private readonly Map<int, Location> locations;
	
    public PartyMemberPositionPacket(Party party)
    {
        locations = new Map<int, Location>();
        reuse(party);
    }
	
    public void reuse(Party party)
    {
        locations.clear();
        foreach (Player member in party.getMembers())
        {
            if (member == null)
            {
                continue;
            }
            locations.put(member.getObjectId(), member.getLocation());
        }
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.PARTY_MEMBER_POSITION);
        
        writer.WriteInt32(locations.size());
        foreach (var entry in locations)
        {
            Location loc = entry.Value;
            writer.WriteInt32(entry.Key);
            writer.WriteInt32(loc.getX());
            writer.WriteInt32(loc.getY());
            writer.WriteInt32(loc.getZ());
        }
    }
}