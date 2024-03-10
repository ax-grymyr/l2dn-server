using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestAcquireSkillInfoPacket: IIncomingPacket<GameSession>
{
    private int _id;
    private int _level;
    private AcquireSkillType _skillType;

    public void ReadContent(PacketBitReader reader)
    {
        _id = reader.ReadInt32();
        _level = reader.ReadInt32();
        _skillType = (AcquireSkillType)reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
		if ((_id <= 0) || (_level <= 0))
		{
			PacketLogger.Instance.Warn(GetType().Name + ": Invalid Id: " + _id + " or level: " + _level + "!");
			return ValueTask.CompletedTask;
		}
		
		Player? player = session.Player;
		if (player == null)
			return ValueTask.CompletedTask;
		
		Npc trainer = player.getLastFolkNPC();
		if ((_skillType != AcquireSkillType.CLASS) &&
		    ((trainer == null) || !trainer.isNpc() || (!trainer.canInteract(player) && !player.isGM())))
			return ValueTask.CompletedTask;
		
		// Consider skill replacements.
		int id = player.getOriginalSkill(_id);
		
		Skill skill = SkillData.getInstance().getSkill(id, _level);
		if (skill == null)
		{
			PacketLogger.Instance.Warn($"Skill Id: {id} level: {_level} is undefined. {GetType().Name} failed.");
			return ValueTask.CompletedTask;
		}
		
		SkillLearn s = SkillTreeData.getInstance().getSkillLearn(_skillType, id, _level, player);
		if (s == null)
			return ValueTask.CompletedTask;
		
		switch (_skillType)
		{
			case AcquireSkillType.TRANSFORM:
			case AcquireSkillType.FISHING:
			case AcquireSkillType.SUBCLASS:
			case AcquireSkillType.COLLECT:
			case AcquireSkillType.TRANSFER:
			case AcquireSkillType.DUALCLASS:
			{
				connection.Send(new AcquireSkillInfoPacket(player, _skillType, s));
				break;
			}
			case AcquireSkillType.CLASS:
			{
				connection.Send(new ExAcquireSkillInfoPacket(player, s));
				break;
			}
			case AcquireSkillType.PLEDGE:
			{
				if (!player.isClanLeader())
					break;
				
				connection.Send(new AcquireSkillInfoPacket(player, _skillType, s));
				break;
			}
			case AcquireSkillType.SUBPLEDGE:
			{
				if (!player.isClanLeader() || !player.hasClanPrivilege(ClanPrivilege.CL_TROOPS_FAME))
					break;

				connection.Send(new AcquireSkillInfoPacket(player, _skillType, s));
				break;
			}
			case AcquireSkillType.ALCHEMY:
			{
				if (player.getRace() != Race.ERTHEIA)
					break;

				connection.Send(new AcquireSkillInfoPacket(player, _skillType, s));
				break;
			}
			case AcquireSkillType.REVELATION:
			{
				/*
				 * if ((player.getLevel() < 85) || !player.isInCategory(CategoryType.SIXTH_CLASS_GROUP)) { return; } client.sendPacket(new AcquireSkillInfo(player, _skillType, s));
				 */
				break;
			}
			case AcquireSkillType.REVELATION_DUALCLASS:
			{
				/*
				 * if (!player.isSubClassActive() || !player.isDualClassActive()) { return; } client.sendPacket(new AcquireSkillInfo(player, _skillType, s));
				 */
				break;
			}
		}

		return ValueTask.CompletedTask;
    }
}