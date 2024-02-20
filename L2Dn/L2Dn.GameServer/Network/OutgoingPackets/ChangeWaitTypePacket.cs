using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ChangeWaitTypePacket: IOutgoingPacket
{
	public const int WT_SITTING = 0;
	public const int WT_STANDING = 1;
	public const int WT_START_FAKEDEATH = 2;
	public const int WT_STOP_FAKEDEATH = 3;
	
	private readonly int _objectId;
	private readonly int _moveType;
	private readonly int _x;
	private readonly int _y;
	private readonly int _z;
	
	public ChangeWaitTypePacket(Creature creature, int newMoveType)
	{
		_objectId = creature.getObjectId();
		_moveType = newMoveType;
		_x = creature.getX();
		_y = creature.getY();
		_z = creature.getZ();
	}
	
	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.CHANGE_WAIT_TYPE);

		writer.WriteInt32(_objectId);
		writer.WriteInt32(_moveType);
		writer.WriteInt32(_x);
		writer.WriteInt32(_y);
		writer.WriteInt32(_z);
	}
}