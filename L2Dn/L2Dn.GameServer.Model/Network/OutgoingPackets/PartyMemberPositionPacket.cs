using System.Collections.Immutable;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct PartyMemberPositionPacket: IOutgoingPacket
{
    private readonly ImmutableArray<(int, Location3D)> _locations;

    public PartyMemberPositionPacket(Party party)
    {
        _locations = party.getMembers().Select(x => (x.getObjectId(), x.getLocation().Location3D))
            .ToImmutableArray();
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.PARTY_MEMBER_POSITION);

        writer.WriteInt32(_locations.Length);
        foreach ((int objectId, Location3D location) in _locations)
        {
            writer.WriteInt32(objectId);
            writer.WriteLocation3D(location);
        }
    }
}