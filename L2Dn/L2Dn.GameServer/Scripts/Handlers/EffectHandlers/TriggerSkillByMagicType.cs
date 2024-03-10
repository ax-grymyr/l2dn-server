using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Creatures;
using L2Dn.GameServer.Model.Events.Listeners;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Skills.Targets;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Trigger skill by isMagic type.
 * @author Zealar
 */
public class TriggerSkillByMagicType: AbstractEffect
{
	private readonly int[] _magicTypes;
	private readonly int _chance;
	private readonly SkillHolder _skill;
	private readonly int _skillLevelScaleTo;
	private readonly TargetType _targetType;
	private readonly bool _replace;
	
	public TriggerSkillByMagicType(StatSet @params)
	{
		_magicTypes = @params.getIntArray("magicTypes", ";");
		_chance = @params.getInt("chance", 100);
		_skill = new SkillHolder(@params.getInt("skillId", 0), @params.getInt("skillLevel", 0));
		_skillLevelScaleTo = @params.getInt("skillLevelScaleTo", 0);
		_targetType = @params.getEnum("targetType", TargetType.TARGET);
		_replace = @params.getBoolean("replace", true);
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		if ((_chance == 0) || (_skill.getSkillId() == 0) || (_skill.getSkillLevel() == 0) || (_magicTypes.Length == 0))
		{
			return;
		}

		effected.addListener(new ConsumerEventListener(effected, EventType.ON_CREATURE_SKILL_FINISH_CAST,
			@event => onSkillUseEvent((OnCreatureSkillFinishCast)@event), this));
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		effected.removeListenerIf(EventType.ON_CREATURE_SKILL_FINISH_CAST, listener => listener.getOwner() == this);
	}
	
	private void onSkillUseEvent(OnCreatureSkillFinishCast @event)
	{
		WorldObject target = @event.getTarget();
		if (target == null)
		{
			return;
		}
		
		if (!target.isCreature())
		{
			return;
		}
		
		Skill eventSkill = @event.getSkill();
		if (!CommonUtil.contains(_magicTypes, eventSkill.getMagicType()))
		{
			return;
		}
		
		if ((_chance < 100) && (Rnd.get(100) > _chance))
		{
			return;
		}
		
		target = null;
		try
		{
			target = TargetHandler.getInstance().getHandler(_targetType).getTarget(@event.getCaster(), @event.getTarget(), _skill.getSkill(), false, false, false);
		}
		catch (Exception e)
		{
			LOGGER.Error("Exception in ITargetTypeHandler.getTarget(): " + e);
		}
		if ((target == null) || !target.isCreature())
		{
			return;
		}
		
		// Ignore common skills.
		if (EnumUtil.GetValues<CommonSkill>().Contains((CommonSkill)eventSkill.getId()))
			return;
		
		Skill triggerSkill;
		if (_skillLevelScaleTo <= 0)
		{
			triggerSkill = _skill.getSkill();
		}
		else
		{
			BuffInfo buffInfo = ((Creature) target).getEffectList().getBuffInfoBySkillId(_skill.getSkillId());
			if (buffInfo != null)
			{
				triggerSkill = SkillData.getInstance().getSkill(_skill.getSkillId(), Math.Min(_skillLevelScaleTo, buffInfo.getSkill().getLevel() + 1));
				
				if (@event.getCaster().isSkillDisabled(buffInfo.getSkill()))
				{
					if ((_replace) && (buffInfo.getSkill().getLevel() == _skillLevelScaleTo))
					{
						((Creature) target).stopSkillEffects(SkillFinishType.SILENT, triggerSkill.getId());
					}
					return;
				}
			}
			else
			{
				triggerSkill = _skill.getSkill();
			}
		}
		
		// Remove existing effect, otherwise time will not be renewed at max level.
		if (_replace)
		{
			((Creature) target).stopSkillEffects(SkillFinishType.SILENT, triggerSkill.getId());
		}
		
		SkillCaster.triggerCast(@event.getCaster(), (Creature) target, triggerSkill);
	}
}