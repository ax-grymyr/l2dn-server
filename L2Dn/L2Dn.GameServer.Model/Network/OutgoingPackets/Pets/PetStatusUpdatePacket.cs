using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Pets;

/**
 * @version $Revision: 1.5.2.3.2.5 $ $Date: 2005/03/29 23:15:10 $
 */
public readonly struct PetStatusUpdatePacket: IOutgoingPacket
{
	private readonly Summon _summon;
	private readonly int _maxFed;
	private readonly int _curFed;

	public PetStatusUpdatePacket(Summon summon)
	{
		_summon = summon;
		if (_summon.isPet())
		{
			Pet pet = (Pet)_summon;
			_curFed = pet.getCurrentFed(); // how fed it is
			_maxFed = pet.getMaxFed(); // max fed it can be
		}
		else if (_summon.isServitor())
		{
			Servitor sum = (Servitor)_summon;
			_curFed = (int)(sum.getLifeTimeRemaining() ?? TimeSpan.Zero).TotalSeconds; // TODO: milliseconds?
			_maxFed = (int)(sum.getLifeTime() ?? TimeSpan.Zero).TotalSeconds;
		}
	}

	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.PET_STATUS_UPDATE);
		writer.WriteInt32(_summon.getSummonType());
		writer.WriteInt32(_summon.ObjectId);
		writer.WriteInt32(_summon.getX());
		writer.WriteInt32(_summon.getY());
		writer.WriteInt32(_summon.getZ());
		writer.WriteString(_summon.getTitle());
		writer.WriteInt32(_curFed);
		writer.WriteInt32(_maxFed);
		writer.WriteInt32((int) _summon.getCurrentHp());
		writer.WriteInt32(_summon.getMaxHp());
		writer.WriteInt32((int) _summon.getCurrentMp());
		writer.WriteInt32(_summon.getMaxMp());
		writer.WriteInt32(_summon.getLevel());
		writer.WriteInt64(_summon.getStat().getExp());
		writer.WriteInt64(_summon.getExpForThisLevel()); // 0% absolute value
		writer.WriteInt64(_summon.getExpForNextLevel()); // 100% absolute value
		writer.WriteInt32(1); // TODO: Find me!
	}
}