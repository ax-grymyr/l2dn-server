using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestDispelPacket: IIncomingPacket<GameSession>
{
    private int _objectId;
    private int _skillId;
    private int _skillLevel;
    private int _skillSubLevel;

    public void ReadContent(PacketBitReader reader)
    {
        _objectId = reader.ReadInt32();
        _skillId = reader.ReadInt32();
        _skillLevel = reader.ReadInt16();
        _skillSubLevel = reader.ReadInt16();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        if (_skillId <= 0 || _skillLevel <= 0)
            return ValueTask.CompletedTask;

        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Skill skill = SkillData.getInstance().getSkill(_skillId, _skillLevel, _skillSubLevel);
        if (skill == null)
            return ValueTask.CompletedTask;

        if (!skill.canBeDispelled() || skill.isDebuff())
            return ValueTask.CompletedTask;

        if (skill.getAbnormalType() == AbnormalType.TRANSFORM)
            return ValueTask.CompletedTask;

        if (skill.isDance() && !Config.DANCE_CANCEL_BUFF)
            return ValueTask.CompletedTask;

        if (player.ObjectId == _objectId)
            player.stopSkillEffects(SkillFinishType.REMOVED, _skillId);
        else
        {
            Summon pet = player.getPet();
            if (pet != null && pet.ObjectId == _objectId)
                pet.stopSkillEffects(SkillFinishType.REMOVED, _skillId);
			
            Summon servitor = player.getServitor(_objectId);
            if (servitor != null)
                servitor.stopSkillEffects(SkillFinishType.REMOVED, _skillId);
        }

        return ValueTask.CompletedTask;
    }
}