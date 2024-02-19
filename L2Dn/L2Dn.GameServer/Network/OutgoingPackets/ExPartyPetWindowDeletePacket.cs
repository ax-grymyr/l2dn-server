using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExPartyPetWindowDeletePacket: IOutgoingPacket
{
    private readonly Summon _summon;
	
    public ExPartyPetWindowDeletePacket(Summon summon)
    {
        _summon = summon;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PARTY_PET_WINDOW_DELETE);
        
        writer.WriteInt32(_summon.getObjectId());
        writer.WriteByte((byte)_summon.getSummonType());
        writer.WriteInt32(_summon.getOwner().getObjectId());
    }
}