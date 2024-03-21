using System.Text;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.NewSkillEnchant;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;
using NLog;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestExEnchantSkillPacket: IIncomingPacket<GameSession>
{
    private static readonly Logger LOGGER = LogManager.GetLogger(nameof(RequestExEnchantSkillPacket));
    private static readonly Logger LOGGER_ENCHANT = LogManager.GetLogger("enchant.skills");

    private SkillEnchantType _type;
    private int _skillId;
    private int _skillLevel;
    private int _skillSubLevel;

    public void ReadContent(PacketBitReader reader)
    {
        _type = (SkillEnchantType)reader.ReadInt32();
        _skillId = reader.ReadInt32();
        _skillLevel = reader.ReadInt16();
        _skillSubLevel = reader.ReadInt16();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
		Player? player = session.Player;
		if (player == null)
			return ValueTask.CompletedTask;

		if (!Enum.IsDefined(_type))
		{
			PacketLogger.Instance.Warn("Client send incorrect type " + _type + " on packet: " + GetType().Name);
			return ValueTask.CompletedTask;
		}

		if (_skillId <= 0 || _skillLevel <= 0 || _skillSubLevel < 0)
		{
			PacketLogger.Instance.Warn(player + " tried to exploit RequestExEnchantSkill!");
			return ValueTask.CompletedTask;
		}
		
		// TODO: flood protection
		// if (!client.getFloodProtectors().canPerformPlayerAction())
		// {
		// 	return ValueTask.CompletedTask;
		// }
		
		if (!player.isAllowedToEnchantSkills())
			return ValueTask.CompletedTask;
		
		if (player.isSellingBuffs())
			return ValueTask.CompletedTask;
		
		if (player.isInOlympiadMode())
			return ValueTask.CompletedTask;
		
		if (player.getPrivateStoreType() != PrivateStoreType.NONE)
			return ValueTask.CompletedTask;
		
		Skill skill = player.getKnownSkill(_skillId);
		if (skill == null)
			return ValueTask.CompletedTask;
		
		if (!skill.isEnchantable())
			return ValueTask.CompletedTask;
		
		if (skill.getLevel() != _skillLevel)
			return ValueTask.CompletedTask;
		
		if (skill.getSubLevel() > 0)
		{
			if (_type == SkillEnchantType.CHANGE)
			{
				int group1 = _skillSubLevel % 1000;
				int group2 = skill.getSubLevel() % 1000;
				if (group1 != group2)
				{
					LOGGER.Warn(GetType().Name + ": Client: " + player +
					            " send incorrect sub level group: " + group1 + " expected: " + group2 +
					            " for skill " + _skillId);
					
					return ValueTask.CompletedTask;
				}
			}
			else if (skill.getSubLevel() + 1 != _skillSubLevel)
			{
				LOGGER.Warn(GetType().Name + ": Client: " + player + " send incorrect sub level: " + _skillSubLevel +
				            " expected: " + (skill.getSubLevel() + 1) + " for skill " + _skillId);
				
				return ValueTask.CompletedTask;
			}
		}
		
		SkillEnchantHolder skillEnchantHolder = SkillEnchantData.getInstance().getSkillEnchant(skill.getId());
		if (skillEnchantHolder == null)
		{
			LOGGER.Warn(GetType().Name + " request enchant skill dont have star lvl skillId-" + skill.getId());
			return ValueTask.CompletedTask;
		}
		
		EnchantStarHolder starHolder = SkillEnchantData.getInstance().getEnchantStar(skillEnchantHolder.getStarLevel());
		if (starHolder == null)
		{
			LOGGER.Warn(GetType().Name + " request enchant skill dont have star lvl-" + skill.getId());
			return ValueTask.CompletedTask;
		}
		
		if (player.getAdena() < 1000000)
			return ValueTask.CompletedTask;
		
		int starLevel = starHolder.getLevel();
		if (Rnd.get(100) <= SkillEnchantData.getInstance().getChanceEnchantMap(skill))
		{
			Skill enchantedSkill = SkillData.getInstance().getSkill(_skillId, _skillLevel, _skillSubLevel);
			if (Config.LOG_SKILL_ENCHANTS)
			{
				StringBuilder sb = new StringBuilder();
				LOGGER_ENCHANT.Info(sb.Append("Success, Character:").Append(player.getName()).Append(" [")
					.Append(player.getObjectId()).Append("] Account:").Append(player.getAccountName()).Append(" IP:")
					.Append(player.getIPAddress()).Append(", +").Append(enchantedSkill.getLevel()).Append(" ")
					.Append(enchantedSkill.getSubLevel()).Append(" - ").Append(enchantedSkill.getName()).Append(" (")
					.Append(enchantedSkill.getId()).Append("), ").ToString());
			}
			
			player.addSkill(enchantedSkill, true);
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.SKILL_ENCHANT_WAS_SUCCESSFUL_S1_HAS_BEEN_ENCHANTED);
			sm.Params.addSkillName(_skillId);
			player.sendPacket(sm);
			
			// player.setSkillEnchantExp(starHolder.getLvl(), 0);
			player.setSkillEnchantExp(starLevel, 0);
			player.sendPacket(ExEnchantSkillResultPacket.STATIC_PACKET_TRUE);
			player.setSkillTryEnchant(starLevel);
		}
		else
		{
			player.sendPacket(ExEnchantSkillResultPacket.STATIC_PACKET_FALSE);
			// player.setSkillEnchantExp(starHolder.getLvl(), 90000);
			int stepExp = 90_000;
			int curTry = player.getSkillTryEnchant(starLevel);
			int finalResult = stepExp * curTry;
			player.setSkillEnchantExp(starLevel, finalResult);
			player.increaseTrySkillEnchant(starLevel);
		}
		player.broadcastUserInfo();
		player.sendSkillList();
		
		skill = player.getKnownSkill(_skillId);
		player.reduceAdena("Try enchant skill", 1_000_000, null, true);
		player.sendPacket(new ExSkillEnchantInfoPacket(skill, player));
		player.updateShortCuts(skill.getId(), skill.getLevel(), skill.getSubLevel());

		return ValueTask.CompletedTask;
    }
}