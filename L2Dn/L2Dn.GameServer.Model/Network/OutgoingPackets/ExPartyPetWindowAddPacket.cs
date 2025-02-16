using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExPartyPetWindowAddPacket: IOutgoingPacket
{
    private readonly Summon _summon;
	
    public ExPartyPetWindowAddPacket(Summon summon)
    {
        _summon = summon;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PARTY_PET_WINDOW_ADD);
        writer.WriteInt32(_summon.ObjectId);
        writer.WriteInt32(_summon.getTemplate().getDisplayId() + 1000000);
        writer.WriteByte((byte)_summon.getSummonType());
        writer.WriteInt32(_summon.getOwner().ObjectId);
        writer.WriteInt32((int) _summon.getCurrentHp());
        writer.WriteInt32(_summon.getMaxHp());
        writer.WriteInt32((int) _summon.getCurrentMp());
        writer.WriteInt32(_summon.getMaxMp());
    }
}