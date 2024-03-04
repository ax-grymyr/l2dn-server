using L2Dn.GameServer.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExPledgeRecruitApplyInfoPacket: IOutgoingPacket
{
    private readonly ClanEntryStatus _status;
	
    public ExPledgeRecruitApplyInfoPacket(ClanEntryStatus status)
    {
        _status = status;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PLEDGE_RECRUIT_APPLY_INFO);

        writer.WriteInt32((int)_status);
    }
}