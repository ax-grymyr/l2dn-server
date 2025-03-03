using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct JoinPartyPacket(int response, Player requestor): IOutgoingPacket
{
    private readonly PartyDistributionType _type = requestor.getClientSettings().getPartyContributionType();

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.JOIN_PARTY);

        writer.WriteInt32(response);
        writer.WriteInt32((int)_type);
        if (_type != 0)
        {
            writer.WriteInt32(0); // TODO: Find me!
            writer.WriteInt32(0); // TODO: Find me!
        }
    }
}