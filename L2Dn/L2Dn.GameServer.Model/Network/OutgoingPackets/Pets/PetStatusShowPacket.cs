using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Pets;

/**
 * @author Yme
 */
public readonly struct PetStatusShowPacket: IOutgoingPacket
{
	private readonly int _summonType;
	private readonly int _summonObjectId;
	
	public PetStatusShowPacket(Summon summon)
	{
		_summonType = summon.getSummonType();
		_summonObjectId = summon.ObjectId;
	}
	
	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.PET_STATUS_SHOW);
		writer.WriteInt32(_summonType);
		writer.WriteInt32(_summonObjectId);
	}
}