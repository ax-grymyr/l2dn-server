using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

internal readonly struct MagicSkillLaunchedPacket: IOutgoingPacket
{
    private readonly int _objectId;
    private readonly int _skillId;
    private readonly int _skillLevel;
    private readonly SkillCastingType _castingType;
    private readonly IReadOnlyCollection<WorldObject>? _targets;

    public MagicSkillLaunchedPacket(Creature creature, int skillId, int skillLevel,
        SkillCastingType castingType = SkillCastingType.NORMAL,
        IReadOnlyCollection<WorldObject>? targets = null)
    {
        _objectId = creature.ObjectId;
        _skillId = skillId;
        _skillLevel = skillLevel;
        _castingType = castingType;
        _targets = targets;
    }

    public MagicSkillLaunchedPacket(Creature creature, int skillId, int skillLevel,
        SkillCastingType castingType, WorldObject? target)
    {
        _objectId = creature.ObjectId;
        _skillId = skillId;
        _skillLevel = skillLevel;
        _castingType = castingType;
        _targets = target is null ? null : [target];
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.MAGIC_SKILL_LAUNCHED);

        writer.WriteInt32((int)_castingType); // MagicSkillUse castingType
        writer.WriteInt32(_objectId);
        writer.WriteInt32(_skillId);
        writer.WriteInt32(_skillLevel);

        if (_targets is null || _targets.Count == 0)
        {
            writer.WriteInt32(1);
            writer.WriteInt32(_objectId);
        }
        else
        {
            writer.WriteInt32(_targets.Count);
            foreach (WorldObject target in _targets)
                writer.WriteInt32(target.ObjectId);
        }
    }
}