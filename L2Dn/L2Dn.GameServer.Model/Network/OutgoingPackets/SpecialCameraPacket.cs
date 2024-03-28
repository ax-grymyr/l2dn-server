using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct SpecialCameraPacket: IOutgoingPacket
{
	private readonly int _id;
	private readonly int _force;
	private readonly int _angle1;
	private readonly int _angle2;
	private readonly int _time;
	private readonly int _duration;
	private readonly int _relYaw;
	private readonly int _relPitch;
	private readonly int _isWide;
	private readonly int _relAngle;
	private readonly int _unk;

	/**
	 * Special Camera packet constructor.
	 * @param creature the creature
	 * @param force
	 * @param angle1
	 * @param angle2
	 * @param time
	 * @param range
	 * @param duration
	 * @param relYaw
	 * @param relPitch
	 * @param isWide
	 * @param relAngle
	 */
	public SpecialCameraPacket(Creature creature, int force, int angle1, int angle2, int time, int range, int duration,
		int relYaw, int relPitch, int isWide, int relAngle)
		: this(creature, force, angle1, angle2, time, duration, range, relYaw, relPitch, isWide, relAngle, 0)
	{
	}

	/**
	 * Special Camera Ex packet constructor.
	 * @param creature the creature
	 * @param talker
	 * @param force
	 * @param angle1
	 * @param angle2
	 * @param time
	 * @param duration
	 * @param relYaw
	 * @param relPitch
	 * @param isWide
	 * @param relAngle
	 */
	public SpecialCameraPacket(Creature creature, Creature talker, int force, int angle1, int angle2, int time,
		int duration, int relYaw, int relPitch, int isWide, int relAngle)
		: this(creature, force, angle1, angle2, time, duration, 0, relYaw, relPitch, isWide, relAngle, 0)
	{
	}

	/**
	 * Special Camera 3 packet constructor.
	 * @param creature the creature
	 * @param force
	 * @param angle1
	 * @param angle2
	 * @param time
	 * @param range
	 * @param duration
	 * @param relYaw
	 * @param relPitch
	 * @param isWide
	 * @param relAngle
	 * @param unk unknown post-C4 parameter
	 */
	public SpecialCameraPacket(Creature creature, int force, int angle1, int angle2, int time, int range, int duration,
		int relYaw, int relPitch, int isWide, int relAngle, int unk)
	{
		_id = creature.getObjectId();
		_force = force;
		_angle1 = angle1;
		_angle2 = angle2;
		_time = time;
		_duration = duration;
		_relYaw = relYaw;
		_relPitch = relPitch;
		_isWide = isWide;
		_relAngle = relAngle;
		_unk = unk;
	}

	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.SPECIAL_CAMERA);

		writer.WriteInt32(_id);
		writer.WriteInt32(_force);
		writer.WriteInt32(_angle1);
		writer.WriteInt32(_angle2);
		writer.WriteInt32(_time);
		writer.WriteInt32(_duration);
		writer.WriteInt32(_relYaw);
		writer.WriteInt32(_relPitch);
		writer.WriteInt32(_isWide);
		writer.WriteInt32(_relAngle);
		writer.WriteInt32(_unk);
	}
}