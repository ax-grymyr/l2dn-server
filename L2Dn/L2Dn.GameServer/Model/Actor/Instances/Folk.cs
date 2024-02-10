using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Status;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Actor.Instances;

public class Folk: Npc
{
	public Folk(NpcTemplate template): base(template)
	{
		setInstanceType(InstanceType.Folk);
		setInvul(false);
	}
	
	public override FolkStatus getStatus()
	{
		return (FolkStatus) base.getStatus();
	}
	
	public override void initCharStatus()
	{
		setStatus(new FolkStatus(this));
	}
	
	/**
	 * Displays Skill Tree for a given player, npc and class Id.
	 * @param player the active character.
	 * @param npc the last folk.
	 * @param classId player's active class id.
	 */
	public static void showSkillList(Player player, Npc npc, ClassId classId)
	{
		int npcId = npc.getTemplate().getId();
		if (npcId == 32611) // Tolonis (Officer)
		{
			List<SkillLearn> skills = SkillTreeData.getInstance().getAvailableCollectSkills(player);
			if (skills.isEmpty()) // No more skills to learn, come back when you level.
			{
				int minLevel = SkillTreeData.getInstance().getMinLevelForNewSkill(player, SkillTreeData.getInstance().getCollectSkillTree());
				if (minLevel > 0)
				{
					SystemMessage sm = new SystemMessage(SystemMessageId.YOU_DO_NOT_HAVE_ANY_FURTHER_SKILLS_TO_LEARN_COME_BACK_WHEN_YOU_HAVE_REACHED_LEVEL_S1);
					sm.addInt(minLevel);
					player.sendPacket(sm);
				}
				else
				{
					player.sendPacket(SystemMessageId.THERE_ARE_NO_OTHER_SKILLS_TO_LEARN);
				}
			}
			else
			{
				player.sendPacket(new ExAcquirableSkillListByClass(skills, AcquireSkillType.COLLECT));
			}
			return;
		}
		
		// Normal skills, No LearnedByFS, no AutoGet skills.
		ICollection<SkillLearn> skills = SkillTreeData.getInstance().getAvailableSkills(player, classId, false, false);
		if (skills.isEmpty())
		{
			Map<long, SkillLearn> skillTree = SkillTreeData.getInstance().getCompleteClassSkillTree(classId);
			int minLevel = SkillTreeData.getInstance().getMinLevelForNewSkill(player, skillTree);
			if (minLevel > 0)
			{
				SystemMessage sm = new SystemMessage(SystemMessageId.YOU_DO_NOT_HAVE_ANY_FURTHER_SKILLS_TO_LEARN_COME_BACK_WHEN_YOU_HAVE_REACHED_LEVEL_S1);
				sm.addInt(minLevel);
				player.sendPacket(sm);
			}
			else if (player.getClassId().level() == 1)
			{
				SystemMessage sm = new SystemMessage(SystemMessageId.THERE_ARE_NO_OTHER_SKILLS_TO_LEARN_PLEASE_COME_BACK_AFTER_S1ND_CLASS_CHANGE);
				sm.addInt(2);
				player.sendPacket(sm);
			}
			else
			{
				player.sendPacket(SystemMessageId.THERE_ARE_NO_OTHER_SKILLS_TO_LEARN);
			}
		}
		else
		{
			player.sendPacket(new ExAcquirableSkillListByClass(skills, AcquireSkillType.CLASS));
		}
	}
}
