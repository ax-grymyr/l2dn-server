using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.BypassHandlers;

public class SupportMagic: IBypassHandler
{
	private static readonly string[] COMMANDS =
	{
		"supportmagicservitor",
		"supportmagic"
	};
	
	// Buffs
	private static readonly SkillHolder HASTE_1 = new SkillHolder(4327, 1);
	private static readonly SkillHolder HASTE_2 = new SkillHolder(5632, 1);
	private static readonly SkillHolder CUBIC = new SkillHolder(4338, 1);
	private static readonly SkillHolder[] FIGHTER_BUFFS =
	{
		new SkillHolder(4322, 1), // Wind Walk
		new SkillHolder(4323, 1), // Shield
		new SkillHolder(5637, 1), // Magic Barrier
		new SkillHolder(4324, 1), // Bless the Body
		new SkillHolder(4325, 1), // Vampiric Rage
		new SkillHolder(4326, 1), // Regeneration
	};
	private static readonly SkillHolder[] MAGE_BUFFS =
	{
		new SkillHolder(4322, 1), // Wind Walk
		new SkillHolder(4323, 1), // Shield
		new SkillHolder(5637, 1), // Magic Barrier
		new SkillHolder(4328, 1), // Bless the Soul
		new SkillHolder(4329, 1), // Acumen
		new SkillHolder(4330, 1), // Concentration
		new SkillHolder(4331, 1), // Empower
	};
	private static readonly SkillHolder[] SUMMON_BUFFS =
	{
		new SkillHolder(4322, 1), // Wind Walk
		new SkillHolder(4323, 1), // Shield
		new SkillHolder(5637, 1), // Magic Barrier
		new SkillHolder(4324, 1), // Bless the Body
		new SkillHolder(4325, 1), // Vampiric Rage
		new SkillHolder(4326, 1), // Regeneration
		new SkillHolder(4328, 1), // Bless the Soul
		new SkillHolder(4329, 1), // Acumen
		new SkillHolder(4330, 1), // Concentration
		new SkillHolder(4331, 1), // Empower
	};
	
	// Levels
	private static readonly int LOWEST_LEVEL = 6;
	private static readonly int CUBIC_LOWEST = 16;
	private static readonly int CUBIC_HIGHEST = 34;
	private static readonly int HASTE_LEVEL_2 = Config.MAX_NEWBIE_BUFF_LEVEL + 1; // disabled
	
	public bool useBypass(String command, Player player, Creature target)
	{
		if (!target.isNpc() || player.isCursedWeaponEquipped())
		{
			return false;
		}
		
		if (command.equalsIgnoreCase(COMMANDS[0]))
		{
			makeSupportMagic(player, (Npc) target, true);
		}
		else if (command.equalsIgnoreCase(COMMANDS[1]))
		{
			makeSupportMagic(player, (Npc) target, false);
		}
		return true;
	}
	
	private void makeSupportMagic(Player player, Npc npc, bool isSummon)
	{
		int level = player.getLevel();
		if (isSummon && !player.hasServitors())
		{
			npc.showChatWindow(player, "html/default/SupportMagicNoSummon.htm");
			return;
		}
		else if (level < LOWEST_LEVEL)
		{
			npc.showChatWindow(player, "html/default/SupportMagicLowLevel.htm");
			return;
		}
		else if (level > Config.MAX_NEWBIE_BUFF_LEVEL)
		{
			npc.showChatWindow(player, "html/default/SupportMagicHighLevel.htm");
			return;
		}
		else if (player.getClassId().GetLevel() == 3)
		{
			player.sendMessage("Only adventurers who have not completed their 3rd class transfer may receive these buffs."); // Custom message
			return;
		}
		
		if (isSummon)
		{
			foreach (Summon s in player.getServitors().values())
			{
				npc.setTarget(s);
				foreach (SkillHolder skill in SUMMON_BUFFS)
				{
					SkillCaster.triggerCast(npc, s, skill.getSkill());
				}
				
				if (level >= HASTE_LEVEL_2)
				{
					SkillCaster.triggerCast(npc, s, HASTE_2.getSkill());
				}
				else
				{
					SkillCaster.triggerCast(npc, s, HASTE_1.getSkill());
				}
			}
		}
		else
		{
			npc.setTarget(player);
			if (player.isInCategory(CategoryType.BEGINNER_MAGE))
			{
				foreach (SkillHolder skill in MAGE_BUFFS)
				{
					SkillCaster.triggerCast(npc, player, skill.getSkill());
				}
			}
			else
			{
				foreach (SkillHolder skill in FIGHTER_BUFFS)
				{
					SkillCaster.triggerCast(npc, player, skill.getSkill());
				}
				
				if (level >= HASTE_LEVEL_2)
				{
					SkillCaster.triggerCast(npc, player, HASTE_2.getSkill());
				}
				else
				{
					SkillCaster.triggerCast(npc, player, HASTE_1.getSkill());
				}
			}
			
			if ((level >= CUBIC_LOWEST) && (level <= CUBIC_HIGHEST))
			{
				SkillCaster.triggerCast(npc, player, CUBIC.getSkill());
			}
		}
	}
	
	public String[] getBypassList()
	{
		return COMMANDS;
	}
}