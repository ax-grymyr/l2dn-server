using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Templates;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Options;

/**
 * @author UnAfraid
 */
public class Options
{
	private readonly int _id;
	private readonly List<AbstractEffect> _effects = [];
	private readonly List<Skill> _activeSkill = [];
	private readonly List<Skill> _passiveSkill = [];
	private readonly List<OptionSkillHolder> _activationSkills = [];

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
		_effects.Add(effect);
	}

	public List<AbstractEffect> getEffects()
	{
		return _effects;
	}

	public bool hasEffects()
	{
		return _effects.Count != 0;
	}

	public bool hasActiveSkills()
	{
		return _activeSkill.Count != 0;
	}

	public List<Skill> getActiveSkills()
	{
		return _activeSkill;
	}

	public void addActiveSkill(Skill holder)
	{
		_activeSkill.Add(holder);
	}

	public bool hasPassiveSkills()
	{
		return _passiveSkill.Count != 0;
	}

	public List<Skill> getPassiveSkills()
	{
		return _passiveSkill;
	}

	public void addPassiveSkill(Skill holder)
	{
		_passiveSkill.Add(holder);
	}

	public bool hasActivationSkills()
	{
		return _activationSkills.Count != 0;
	}

    public bool hasActivationSkills(OptionSkillType type)
    {
        foreach (OptionSkillHolder holder in _activationSkills)
        {
            if (holder.getSkillType() == type)
                return true;
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
		foreach (OptionSkillHolder holder in _activationSkills)
		{
			if (holder.getSkillType() == type)
				temp.Add(holder);
		}

		return temp;
	}

	public void addActivationSkill(OptionSkillHolder holder)
	{
		_activationSkills.Add(holder);
	}

	public void apply(Playable playable)
	{
		if (hasEffects())
		{
            Skill skill = null!; // TODO: WARNING! the skill is used in many AbstractEffects but here it is null
			BuffInfo info = new BuffInfo(playable, playable, skill, true, null, this);
			foreach (AbstractEffect effect in _effects)
			{
				if (effect.IsInstant)
				{
					if (effect.CalcSuccess(info.getEffector(), info.getEffected(), info.getSkill()))
					{
						effect.Instant(info.getEffector(), info.getEffected(), info.getSkill(), info.getItem());
					}
				}
				else
				{
					effect.ContinuousInstant(info.getEffector(), info.getEffected(), info.getSkill(), info.getItem());
					effect.Pump(playable, info.getSkill());
					if (effect.CanStart(info.getEffector(), info.getEffected(), info.getSkill()))
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

        Player? player = playable.getActingPlayer();
		if (playable.isPlayer() && player != null)
		{
			player.sendSkillList();
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

        Player? player = playable.getActingPlayer();
		if (playable.isPlayer() && player != null)
		{
			player.sendSkillList();
		}
	}

	private void addSkill(Playable playable, Skill skill)
	{
		bool updateTimeStamp = false;
		playable.addSkill(skill);
		if (skill.IsActive)
		{
			TimeSpan remainingTime = playable.getSkillRemainingReuseTime(skill.ReuseHashCode);
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