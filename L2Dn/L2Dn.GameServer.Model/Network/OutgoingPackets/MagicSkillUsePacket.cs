using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

/**
 * MagicSkillUse server packet implementation.
 * @author UnAfraid, NosBit, Mobius
 */
public readonly struct MagicSkillUsePacket: IOutgoingPacket
{
	private readonly int _skillId;
	private readonly int _skillLevel;
	private readonly TimeSpan _hitTime;
	private readonly int _reuseGroup;
	private readonly TimeSpan _reuseDelay;
	private readonly int _actionId; // If skill is called from RequestActionUse, use that ID.
	private readonly SkillCastingType _castingType; // Defines which client bar is going to use.
	private readonly Creature _creature;
	private readonly int _targetObjectId;
	private readonly Location3D _targetLocation;
	private readonly bool _isGroundTargetSkill;
	private readonly Location3D? _groundLocation;

	public MagicSkillUsePacket(Creature creature, WorldObject target, int skillId, int skillLevel, TimeSpan hitTime,
		TimeSpan reuseDelay, int reuseGroup, int actionId, SkillCastingType castingType, bool isGroundTargetSkill)
	{
		_creature = creature;
		_targetObjectId = target.getObjectId();
		_targetLocation = target.getLocation().Location3D;
		_skillId = skillId;
		_skillLevel = skillLevel;
		_hitTime = hitTime;
		_reuseGroup = reuseGroup;
		_reuseDelay = reuseDelay;
		_actionId = actionId;
		_castingType = castingType;
		_isGroundTargetSkill = isGroundTargetSkill;
		_groundLocation = creature.isPlayer() ? creature.getActingPlayer().getCurrentSkillWorldPosition() : null;
	}

	public MagicSkillUsePacket(Creature creature, WorldObject target, int skillId, int skillLevel, TimeSpan hitTime,
		TimeSpan reuseDelay, int reuseGroup, int actionId, SkillCastingType castingType)
		: this(creature, target, skillId, skillLevel, hitTime, reuseDelay, reuseGroup, actionId, castingType, false)
	{
	}

	public MagicSkillUsePacket(Creature creature, WorldObject target, int skillId, int skillLevel, TimeSpan hitTime,
		TimeSpan reuseDelay)
		: this(creature, target, skillId, skillLevel, hitTime, reuseDelay, -1, -1, SkillCastingType.NORMAL)
	{
	}

	public MagicSkillUsePacket(Creature creature, int skillId, int skillLevel, TimeSpan hitTime, TimeSpan reuseDelay)
		: this(creature, creature, skillId, skillLevel, hitTime, reuseDelay, -1, -1, SkillCastingType.NORMAL)
	{
	}

	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.MAGIC_SKILL_USE);

		writer.WriteInt32((int)_castingType); // Casting bar type: 0 - default, 1 - default up, 2 - blue, 3 - green, 4 - red.
		writer.WriteInt32(_creature.getObjectId());
		writer.WriteInt32(_targetObjectId);
		writer.WriteInt32(_skillId);
		writer.WriteInt32(_skillLevel);
		writer.WriteInt32((int)_hitTime.TotalMilliseconds);
		writer.WriteInt32(_reuseGroup);
		writer.WriteInt32((int)_reuseDelay.TotalMilliseconds);
		writer.WriteInt32(_creature.getX());
		writer.WriteInt32(_creature.getY());
		writer.WriteInt32(_creature.getZ());
		writer.WriteInt16(_isGroundTargetSkill ? (short)-1 : (short)0);
		if (_groundLocation == null)
		{
			writer.WriteInt16(0);
		}
		else
		{
			writer.WriteInt16(1);
			writer.WriteLocation3D(_groundLocation.Value);
		}

		writer.WriteLocation3D(_targetLocation);
		writer.WriteInt32(_actionId >= 0); // 1 when ID from RequestActionUse is used
		writer.WriteInt32(_actionId >= 0
			? _actionId
			: 0); // ID from RequestActionUse. Used to set cooldown on summon skills.
		
		if (_groundLocation == null)
		{
			writer.WriteInt32(-1); // 306
		}
	}
}