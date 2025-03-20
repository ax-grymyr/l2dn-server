using L2Dn.GameServer.Model.Clans;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct PledgeStatusChangedPacket: IOutgoingPacket
{
    private readonly Clan _clan;

    public PledgeStatusChangedPacket(Clan clan)
    {
        _clan = clan;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.PLEDGE_STATUS_CHANGED);

        writer.WriteInt32(0);
        writer.WriteInt32(_clan.getLeaderId());
        writer.WriteInt32(_clan.Id);
        writer.WriteInt32(_clan.getCrestId() ?? 0);
        writer.WriteInt32(_clan.getAllyId() ?? 0);
        writer.WriteInt32(_clan.getAllyCrestId() ?? 0);
        writer.WriteInt32(_clan.getCrestLargeId() ?? 0);
        writer.WriteInt32(0); // pledge type ?
    }
}