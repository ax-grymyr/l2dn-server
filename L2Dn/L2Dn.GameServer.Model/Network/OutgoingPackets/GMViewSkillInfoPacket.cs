using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct GMViewSkillInfoPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly ICollection<Skill> _skills;

    public GMViewSkillInfoPacket(Player player)
    {
        _player = player;
        _skills = _player.getSkillList();
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.GM_VIEW_SKILL_INFO);

        writer.WriteString(_player.getName());
        writer.WriteInt32(_skills.Count);

        Clan? clan = _player.getClan();
        bool isDisabled = clan != null && clan.getReputationScore() < 0;
        foreach (Skill skill in _skills)
        {
            writer.WriteInt32(skill.IsPassive);
            writer.WriteInt16((short)skill.DisplayLevel);
            writer.WriteInt16((short)skill.SubLevel);
            writer.WriteInt32(skill.DisplayId);
            writer.WriteInt32(0);
            writer.WriteByte(isDisabled && skill.IsClanSkill());
            writer.WriteByte(skill.IsEnchantable());
        }
    }
}