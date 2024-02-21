using L2Dn.GameServer.Model.Clans;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExPledgeCountPacket: IOutgoingPacket
{
    private readonly int _count;
	
    public ExPledgeCountPacket(Clan clan)
    {
        _count = clan.getOnlineMembersCount();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PLEDGE_COUNT);

        writer.WriteInt32(_count);
    }
}