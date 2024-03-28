using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Pets;

public readonly struct PetDeletePacket: IOutgoingPacket
{
	private readonly int _petType;
	private readonly int _petObjId;
	
	public PetDeletePacket(int petType, int petObjId)
	{
		_petType = petType; // Summon Type
		_petObjId = petObjId; // objectId
	}
	
	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.PET_DELETE);
		writer.WriteInt32(_petType);
		writer.WriteInt32(_petObjId);
	}
}