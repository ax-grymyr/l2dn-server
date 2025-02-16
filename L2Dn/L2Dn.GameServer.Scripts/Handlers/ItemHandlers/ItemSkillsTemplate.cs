using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Skills.Targets;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using NLog;

namespace L2Dn.GameServer.Scripts.Handlers.ItemHandlers;

/**
 * Template for item skills handler.
 * @author Zoey76
 */
public class ItemSkillsTemplate: IItemHandler
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(ItemSkillsTemplate));

	public virtual bool useItem(Playable playable, Item item, bool forceUse)
	{
		if (!playable.isPlayer() && !playable.isPet())
		{
			return false;
		}
		
		// Pets can use items only when they are tradable.
		if (playable.isPet() && !item.isTradeable())
		{
			playable.sendPacket(SystemMessageId.YOUR_PET_CANNOT_CARRY_THIS_ITEM);
			return false;
		}
		
		// Verify that item is not under reuse.
		if (!checkReuse(playable, null, item))
		{
			return false;
		}
		
		List<ItemSkillHolder> skills = item.getTemplate().getSkills(ItemSkillType.NORMAL);
		if (skills == null)
		{
			_logger.Info("Item " + item + " does not have registered any skill for handler.");
			return false;
		}
		
		bool hasConsumeSkill = false;
		bool successfulUse = false;
		foreach (SkillHolder skillInfo in skills)
		{
			if (skillInfo == null)
			{
				continue;
			}
			
			Skill itemSkill = skillInfo.getSkill();
			if (itemSkill != null)
			{
				if (itemSkill.hasEffectType(EffectType.EXTRACT_ITEM) && (playable.getActingPlayer() != null) && !playable.getActingPlayer().isInventoryUnder80(false))
				{
					playable.getActingPlayer().sendPacket(SystemMessageId.NOT_ENOUGH_SPACE_IN_INVENTORY_UNABLE_TO_PROCESS_THIS_REQUEST_UNTIL_YOUR_INVENTORY_S_WEIGHT_IS_LESS_THAN_80_AND_SLOT_COUNT_IS_LESS_THAN_90_OF_CAPACITY);
					return false;
				}
				
				if (itemSkill.getItemConsumeId() > 0)
				{
					hasConsumeSkill = true;
				}
				
				if (!itemSkill.hasEffectType(EffectType.SUMMON_PET) && !itemSkill.checkCondition(playable, playable.getTarget(), true))
				{
					continue;
				}
				
				if (playable.isSkillDisabled(itemSkill))
				{
					continue;
				}
				
				// Verify that skill is not under reuse.
				if (!checkReuse(playable, itemSkill, item))
				{
					continue;
				}
				
				if (!item.isPotion() && !item.isElixir() && !item.isScroll() && playable.isCastingNow())
				{
					continue;
				}
				
				// Send message to the master.
				if (playable.isPet())
				{
					SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOUR_PET_USES_S1);
					sm.Params.addSkillName(itemSkill);
					playable.sendPacket(sm);
				}
				
				if (playable.isPlayer() && itemSkill.hasEffectType(EffectType.SUMMON_PET))
				{
					playable.doCast(itemSkill);
					successfulUse = true;
				}
				else if (itemSkill.isWithoutAction() || item.getTemplate().hasImmediateEffect() || item.getTemplate().hasExImmediateEffect())
				{
					SkillCaster.triggerCast(playable, itemSkill.getTargetType() == TargetType.OTHERS ? playable.getTarget() : null, itemSkill, item, false);
					successfulUse = true;
				}
				else
				{
					playable.getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
					if (playable.useMagic(itemSkill, item, forceUse, false))
					{
						successfulUse = true;
					}
					else
					{
						continue;
					}
				}
				
				if (itemSkill.getReuseDelay() > TimeSpan.Zero)
				{
					playable.addTimeStamp(itemSkill, itemSkill.getReuseDelay());
				}
			}
		}
		
		if (successfulUse && checkConsume(item, hasConsumeSkill) && !playable.destroyItem("Consume", item.ObjectId, 1, playable, false))
		{
			playable.sendPacket(SystemMessageId.INCORRECT_ITEM_COUNT_2);
			return false;
		}
		
		return successfulUse;
	}
	
	/**
	 * @param item the item being used
	 * @param hasConsumeSkill
	 * @return {@code true} check if item use consume item, {@code false} otherwise
	 */
	private bool checkConsume(Item item, bool hasConsumeSkill)
	{
		switch (item.getTemplate().getDefaultAction())
		{
			case ActionType.CAPSULE:
			case ActionType.SKILL_REDUCE:
			{
				if (!hasConsumeSkill && item.getTemplate().hasImmediateEffect())
				{
					return true;
				}
				break;
			}
			case ActionType.SKILL_REDUCE_ON_SKILL_SUCCESS:
			{
				return false;
			}
		}
		return hasConsumeSkill;
	}
	
	/**
	 * @param playable the character using the item or skill
	 * @param skill the skill being used, can be null
	 * @param item the item being used
	 * @return {@code true} if the the item or skill to check is available, {@code false} otherwise
	 */
	private bool checkReuse(Playable playable, Skill skill, Item item)
	{
		TimeSpan remainingTime = (skill != null)
			? playable.getSkillRemainingReuseTime(skill.getReuseHashCode())
			: playable.getItemRemainingReuseTime(item.ObjectId);
		
		bool isAvailable = remainingTime <= TimeSpan.Zero;
		if (playable.isPlayer() && !isAvailable)
		{
			int hours = (int)remainingTime.TotalHours;
			int minutes = remainingTime.Minutes;
			int seconds = remainingTime.Seconds;
			SystemMessagePacket sm;
			if (hours > 0)
			{
				sm = new SystemMessagePacket(SystemMessageId.S1_WILL_BE_AVAILABLE_AGAIN_IN_S2_H_S3_MIN_S4_SEC);
				if ((skill == null) || skill.isStatic())
				{
					sm.Params.addItemName(item);
				}
				else
				{
					sm.Params.addSkillName(skill);
				}
				sm.Params.addInt(hours);
				sm.Params.addInt(minutes);
			}
			else if (minutes > 0)
			{
				sm = new SystemMessagePacket(SystemMessageId.S1_WILL_BE_AVAILABLE_AGAIN_IN_S2_MIN_S3_SEC);
				if ((skill == null) || skill.isStatic())
				{
					sm.Params.addItemName(item);
				}
				else
				{
					sm.Params.addSkillName(skill);
				}
				sm.Params.addInt(minutes);
			}
			else
			{
				sm = new SystemMessagePacket(SystemMessageId.S1_WILL_BE_AVAILABLE_AGAIN_IN_S2_SEC);
				if ((skill == null) || skill.isStatic())
				{
					sm.Params.addItemName(item);
				}
				else
				{
					sm.Params.addSkillName(skill);
				}
			}
			
			sm.Params.addInt(seconds);
			playable.sendPacket(sm);
		}
		
		return isAvailable;
	}
}