using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Pets;

/**
 * @author Berezkin Nikolay
 */
public readonly struct ExPetSkillListPacket: IOutgoingPacket
{
	private readonly bool _onEnter;
	private readonly Pet _pet;
	
	public ExPetSkillListPacket(bool onEnter, Pet pet)
	{
		_onEnter = onEnter;
		_pet = pet;
	}
	
	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.EX_PET_SKILL_LIST);
		writer.WriteByte(_onEnter);
		writer.WriteInt32(_pet.getAllSkills().Count);
		foreach (Skill sk in _pet.getAllSkills())
		{
			writer.WriteInt32(sk.getDisplayId());
			writer.WriteInt32(sk.getDisplayLevel());
			writer.WriteInt32(sk.getReuseDelayGroup());
			writer.WriteByte(0);
			writer.WriteByte(0);
		}
	}
}