using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Options;

/**
 * @author UnAfraid
 */
public class Options
{
	private readonly int _id;
	private List<AbstractEffect> _effects = [];
	private List<Skill> _activeSkill = [];
	private List<Skill> _passiveSkill = [];
	private List<OptionSkillHolder> _activationSkills = [];

	/**
	 * @param id
	 */
	public Options(int id)
	{
		_id = id;
	}

	public int getId()
	{
		return _id;
	}

	public void addEffect(AbstractEffect effect)
	{
		if (_effects == null)
		{
			_effects = new();
		}

		_effects.Add(effect);
	}

	public List<AbstractEffect> getEffects()
	{
		return _effects;
	}

	public bool hasEffects()
	{
		return _effects != null;
	}

	public bool hasActiveSkills()
	{
		return _activeSkill != null;
	}

	public List<Skill> getActiveSkills()
	{
		return _activeSkill;
	}

	public void addActiveSkill(Skill holder)
	{
		if (_activeSkill == null)
		{
			_activeSkill = new();
		}

		_activeSkill.Add(holder);
	}

	public bool hasPassiveSkills()
	{
		return _passiveSkill != null;
	}

	public List<Skill> getPassiveSkills()
	{
		return _passiveSkill;
	}

	public void addPassiveSkill(Skill holder)
	{
		if (_passiveSkill == null)
		{
			_passiveSkill = new();
		}

		_passiveSkill.Add(holder);
	}

	public bool hasActivationSkills()
	{
		return _activationSkills != null;
	}

	public bool hasActivationSkills(OptionSkillType type)
	{
		if (_activationSkills != null)
		{
			foreach (OptionSkillHolder holder in _activationSkills)
			{
				if (holder.getSkillType() == type)
				{
					return true;
				}
			}
		}

		return false;
	}

	public List<OptionSkillHolder> getActivationSkills()
	{
		return _activationSkills;
	}

	public List<OptionSkillHolder> getActivationSkills(OptionSkillType type)
	{
		List<OptionSkillHolder> temp = new();
		if (_activationSkills != null)
		{
			foreach (OptionSkillHolder holder in _activationSkills)
			{
				if (holder.getSkillType() == type)
				{
					temp.Add(holder);
				}
			}
		}

		return temp;
	}

	public void addActivationSkill(OptionSkillHolder holder)
	{
		if (_activationSkills == null)
		{
			_activationSkills = new();
		}

		_activationSkills.Add(holder);
	}

	public void apply(Playable playable)
	{
		if (hasEffects())
		{
			BuffInfo info = new BuffInfo(playable, playable, null, true, null, this);
			foreach (AbstractEffect effect in _effects)
			{
				if (effect.isInstant())
				{
					if (effect.calcSuccess(info.getEffector(), info.getEffected(), info.getSkill()))
					{
						effect.instant(info.getEffector(), info.getEffected(), info.getSkill(), info.getItem());
					}
				}
				else
				{
					effect.continuousInstant(info.getEffector(), info.getEffected(), info.getSkill(), info.getItem());
					effect.pump(playable, info.getSkill());
					if (effect.canStart(info.getEffector(), info.getEffected(), info.getSkill()))
					{
						info.addEffect(effect);
					}
				}
			}

			if (info.getEffects().Count != 0)
			{
				playable.getEffectList().add(info);
			}
		}

		if (hasActiveSkills())
		{
			foreach (Skill skill in _activeSkill)
			{
				addSkill(playable, skill);
			}
		}

		if (hasPassiveSkills())
		{
			foreach (Skill skill in _passiveSkill)
			{
				addSkill(playable, skill);
			}
		}

		if (hasActivationSkills())
		{
			foreach (OptionSkillHolder holder in _activationSkills)
			{
				playable.addTriggerSkill(holder);
			}
		}

		playable.getStat().recalculateStats(true);
		if (playable.isPlayer())
		{
			playable.getActingPlayer().sendSkillList();
		}
	}

	public void remove(Playable playable)
	{
		if (hasEffects())
		{
			foreach (BuffInfo info in playable.getEffectList().getOptions())
			{
				if (info.getOption() == this)
				{
					playable.getEffectList().remove(info, SkillFinishType.NORMAL, true, true);
				}
			}
		}

		if (hasActiveSkills())
		{
			foreach (Skill skill in _activeSkill)
			{
				playable.removeSkill(skill, false);
			}
		}

		if (hasPassiveSkills())
		{
			foreach (Skill skill in _passiveSkill)
			{
				playable.removeSkill(skill, true);
			}
		}

		if (hasActivationSkills())
		{
			foreach (OptionSkillHolder holder in _activationSkills)
			{
				playable.removeTriggerSkill(holder);
			}
		}

		playable.getStat().recalculateStats(true);
		if (playable.isPlayer())
		{
			playable.getActingPlayer().sendSkillList();
		}
	}

	private void addSkill(Playable playable, Skill skill)
	{
		bool updateTimeStamp = false;
		playable.addSkill(skill);
		if (skill.isActive())
		{
			TimeSpan remainingTime = playable.getSkillRemainingReuseTime(skill.getReuseHashCode());
			if (remainingTime > TimeSpan.Zero)
			{
				playable.addTimeStamp(skill, remainingTime);
				playable.disableSkill(skill, remainingTime);
			}

			updateTimeStamp = true;
		}

        Player? player = playable.getActingPlayer();
		if (updateTimeStamp && playable.isPlayer() && player != null)
		{
			playable.sendPacket(new SkillCoolTimePacket(player));
		}
	}
}