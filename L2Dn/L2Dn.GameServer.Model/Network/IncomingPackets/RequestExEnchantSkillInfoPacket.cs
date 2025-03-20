using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestExEnchantSkillInfoPacket: IIncomingPacket<GameSession>
{
    private int _skillId;
    private int _skillLevel;
    private int _skillSubLevel;

    public void ReadContent(PacketBitReader reader)
    {
        _skillId = reader.ReadInt32();
        _skillLevel = reader.ReadInt16();
        _skillSubLevel = reader.ReadInt16();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (_skillId <= 0 || _skillLevel <= 0 || _skillSubLevel < 0)
            return ValueTask.CompletedTask;

        // if (!player.isInCategory(CategoryType.SIXTH_CLASS_GROUP))
        // {
        // return;
        // }

        Skill? skill = SkillData.Instance.GetSkill(_skillId, _skillLevel, _skillSubLevel);
        if (skill == null || skill.Id != _skillId)
            return ValueTask.CompletedTask;

        Set<int> route = EnchantSkillGroupsData.getInstance().getRouteForSkill(_skillId, _skillLevel);
        if (route.isEmpty())
            return ValueTask.CompletedTask;

        Skill? playerSkill = player.getKnownSkill(_skillId);
        if (playerSkill == null || playerSkill.Level != _skillLevel || playerSkill.SubLevel != _skillSubLevel)
            return ValueTask.CompletedTask;

        player.sendPacket(new ExEnchantSkillInfoPacket(_skillId, _skillLevel, _skillSubLevel, playerSkill.SubLevel));

        // ExEnchantSkillInfoDetail - not really necessary I think
        // player.sendPacket(new ExEnchantSkillInfoDetail(SkillEnchantType.NORMAL, _skillId, _skillLevel, _skillSubLevel , activeChar));

        return ValueTask.CompletedTask;
    }
}