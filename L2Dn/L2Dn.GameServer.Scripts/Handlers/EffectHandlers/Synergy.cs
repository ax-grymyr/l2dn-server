using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Synergy effect implementation.
 */
public class Synergy: AbstractEffect
{
	private readonly Set<AbnormalType> _requiredSlots;
	private readonly Set<AbnormalType> _optionalSlots;
	private readonly int _partyBuffSkillId;
	private readonly int _skillLevelScaleTo;
	private readonly int _minSlot;

	public Synergy(StatSet @params)
	{
		string requiredSlots = @params.getString("requiredSlots", string.Empty);
		if (!string.IsNullOrEmpty(requiredSlots))
		{
			_requiredSlots = [];
			foreach (string slot in requiredSlots.Split(";"))
			{
				_requiredSlots.add(Enum.Parse<AbnormalType>(slot));
			}
		}
		else
		{
			_requiredSlots = [];
		}

		string optionalSlots = @params.getString("optionalSlots", string.Empty);
		if (!string.IsNullOrEmpty(optionalSlots))
		{
			_optionalSlots = [];
			foreach (string slot in optionalSlots.Split(";"))
			{
				_optionalSlots.add(Enum.Parse<AbnormalType>(slot));
			}
		}
		else
		{
			_optionalSlots = [];
		}

		_partyBuffSkillId = @params.getInt("partyBuffSkillId");
		_skillLevelScaleTo = @params.getInt("skillLevelScaleTo", 1);
		_minSlot = @params.getInt("minSlot", 2);
		setTicks(@params.getInt("ticks"));
	}

	public override bool onActionTime(Creature effector, Creature effected, Skill skill, Item? item)
	{
		if (effector.isDead())
		{
			return false;
		}

		foreach (AbnormalType required in _requiredSlots)
		{
			if (!effector.hasAbnormalType(required))
			{
				return skill.isToggle();
			}
		}

		int abnormalCount = 0;
		foreach (AbnormalType abnormalType in _optionalSlots)
		{
			if (effector.hasAbnormalType(abnormalType))
			{
				abnormalCount++;
			}
		}

		if (abnormalCount >= _minSlot)
		{
			SkillHolder partyBuff = new SkillHolder(_partyBuffSkillId, Math.Min(abnormalCount - 1, _skillLevelScaleTo));
			Skill partyBuffSkill = partyBuff.getSkill();
			if (partyBuffSkill != null)
			{
				WorldObject? target = partyBuffSkill.getTarget(effector, effected, false, false, false);
				if (target != null && target.isCreature())
				{
					BuffInfo? abnormalBuffInfo = effector.getEffectList().getFirstBuffInfoByAbnormalType(partyBuffSkill.getAbnormalType());
					if (abnormalBuffInfo != null && abnormalBuffInfo.getSkill().getAbnormalLevel() != abnormalCount - 1)
					{
						effector.getEffectList().stopSkillEffects(SkillFinishType.REMOVED, _partyBuffSkillId);
					}
					else
					{
						SkillCaster.triggerCast(effector, (Creature) target, partyBuffSkill);
					}
				}
			}
			else
			{
				LOGGER.Warn("Skill not found effect called from " + skill);
			}
		}
		else
		{
			effector.getEffectList().stopSkillEffects(SkillFinishType.REMOVED, _partyBuffSkillId);
		}

		return skill.isToggle();
	}
}