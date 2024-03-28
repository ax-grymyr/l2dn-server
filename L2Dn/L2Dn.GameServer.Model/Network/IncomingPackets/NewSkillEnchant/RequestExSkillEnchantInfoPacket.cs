using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets.NewSkillEnchant;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.NewSkillEnchant;

public struct RequestExSkillEnchantInfoPacket: IIncomingPacket<GameSession>
{
    private int _skillId;
    private int _skillLevel;
    private int _skillSubLevel;

    public void ReadContent(PacketBitReader reader)
    {
        _skillId = reader.ReadInt32();
        _skillLevel = reader.ReadInt32();
        _skillSubLevel = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Skill skill = player.getKnownSkill(_skillId);
        if (skill == null)
        {
            PacketLogger.Instance.Warn(player.getName() + " trying enchant skill, what missed on server" + _skillId +
                                       " level-" + _skillLevel + " subLevel-" + _skillSubLevel);
            
            return ValueTask.CompletedTask;
        }

        SkillEnchantHolder skillEnchantHolder = SkillEnchantData.getInstance().getSkillEnchant(skill.getId());
        if (skillEnchantHolder == null)
        {
            PacketLogger.Instance.Warn("Skill does not exist at SkillEnchantData id-" + _skillId);
            return ValueTask.CompletedTask;
        }
		
        player.sendPacket(new ExSkillEnchantInfoPacket(skill, player));
        
        return ValueTask.CompletedTask;
    }
}