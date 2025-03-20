using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Dto.ZoneForms;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData.Xml.Zones;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * Another type of damage zone with skills.
 * @author kerberos
 */
public class EffectZone : Zone
{
	int _chance;
	private int _initialDelay;
	private int _reuse;
	protected bool _bypassConditions;
	private bool _isShowDangerIcon;
	private bool _removeEffectsOnExit;
	protected Map<int, int> _skills = [];
	protected volatile ScheduledFuture? _task;

	public EffectZone(int id, ZoneForm form): base(id, form)
	{
		_chance = 100;
		_initialDelay = 0;
		_reuse = 30000;
		setTargetType(InstanceType.Playable); // default only playable
		_bypassConditions = false;
		_isShowDangerIcon = true;
		_removeEffectsOnExit = false;
	}

	public override void setParameter(XmlZoneStatName name, string value)
	{
		switch (name)
		{
			case XmlZoneStatName.chance:
			{
				_chance = int.Parse(value);
				break;
			}
			case XmlZoneStatName.initialDelay:
			{
				_initialDelay = int.Parse(value);
				break;
			}
			case XmlZoneStatName.reuse:
			{
				_reuse = int.Parse(value);
				break;
			}
			case XmlZoneStatName.bypassSkillConditions:
			{
				_bypassConditions = bool.Parse(value);
				break;
			}
			case XmlZoneStatName.maxDynamicSkillCount:
			{
				_skills = new(); // int.Parse(value)
				break;
			}
			case XmlZoneStatName.showDangerIcon:
			{
				_isShowDangerIcon = bool.Parse(value);
				break;
			}
			case XmlZoneStatName.skillIdLvl:
			{
				string[] propertySplit =
					(value.EndsWith(';') ? value[..^1] : value).Split(";");

				_skills = new();
				foreach (string skill in propertySplit)
				{
					string[] skillSplit = skill.Split("-");
					if (skillSplit.Length != 2)
					{
						Logger.Warn(GetType().Name + ": invalid config property -> skillsIdLvl \"" + skill + "\"");
					}
					else
					{
						try
						{
							_skills.put(int.Parse(skillSplit[0]), int.Parse(skillSplit[1]));
						}
						catch (Exception nfe)
						{
							if (!string.IsNullOrEmpty(skill))
							{
								Logger.Warn(GetType().Name + ": invalid config property -> skillsIdLvl \"" +
								            skillSplit[0] + "\"" + skillSplit[1] + ": " + nfe);
							}
						}
					}
				}
				break;
			}
			case XmlZoneStatName.removeEffectsOnExit:
			{
				_removeEffectsOnExit = bool.Parse(value);
				break;
			}
			default:
			{
				base.setParameter(name, value);
				break;
			}
		}
	}

	protected override void onEnter(Creature creature)
	{
		if (_skills != null)
		{
			ScheduledFuture? task = _task;
			if (task == null)
			{
				lock (this)
				{
					task = _task;
					if (task == null)
					{
						_task = task = ThreadPool.scheduleAtFixedRate(new ApplySkill(this), _initialDelay, _reuse);
					}
				}
			}
		}

        Player? player = creature.getActingPlayer();
		if (creature.isPlayer() && player != null)
		{
			creature.setInsideZone(ZoneId.ALTERED, true);
			if (_isShowDangerIcon)
			{
				creature.setInsideZone(ZoneId.DANGER_AREA, true);
				creature.sendPacket(new EtcStatusUpdatePacket(player));
			}
		}
	}

	protected override void onExit(Creature creature)
    {
        Player? player = creature.getActingPlayer();
		if (creature.isPlayer() && player != null)
		{
			creature.setInsideZone(ZoneId.ALTERED, false);
			if (_isShowDangerIcon)
			{
				creature.setInsideZone(ZoneId.DANGER_AREA, false);
				if (!creature.isInsideZone(ZoneId.DANGER_AREA))
				{
					creature.sendPacket(new EtcStatusUpdatePacket(player));
				}
			}
			if (_removeEffectsOnExit && _skills != null)
			{
				foreach (var e in _skills)
				{
					Skill? skill = SkillData.getInstance().getSkill(e.Key, e.Value);
					if (skill != null && creature.isAffectedBySkill(skill.Id))
					{
						creature.stopSkillEffects(SkillFinishType.REMOVED, skill.Id);
					}
				}
			}
		}

		if (getCharactersInside().Count == 0 && _task != null)
		{
			_task.cancel(true);
			_task = null;
		}
	}

	public int getChance()
	{
		return _chance;
	}

	public void addSkill(int skillId, int skillLevel)
	{
		if (skillLevel < 1) // remove skill
		{
			removeSkill(skillId);
			return;
		}

		if (_skills == null)
		{
			lock (this)
			{
				if (_skills == null)
				{
					_skills = new();
				}
			}
		}
		_skills.put(skillId, skillLevel);
	}

	public void removeSkill(int skillId)
	{
		if (_skills != null)
		{
			_skills.remove(skillId);
		}
	}

	public void clearSkills()
	{
		if (_skills != null)
		{
			_skills.Clear();
		}
	}

	public int getSkillLevel(int skillId)
	{
		if (_skills == null || !_skills.TryGetValue(skillId, out int level))
		{
			return 0;
		}
		return level;
	}

	private class ApplySkill: Runnable
	{
		private readonly EffectZone _effectZone;

		public ApplySkill(EffectZone effectZone)
		{
			_effectZone = effectZone;
			if (effectZone._skills == null)
			{
				throw new InvalidOperationException("No skills defined.");
			}
		}

		public void run()
		{
			if (!_effectZone.isEnabled())
			{
				return;
			}

			if (_effectZone.getCharactersInside().Count == 0)
			{
				if (_effectZone._task != null)
				{
					_effectZone._task.cancel(false);
					_effectZone._task = null;
				}
				return;
			}

			foreach (Creature character in _effectZone.getCharactersInside())
			{
				if (character != null && character.isPlayer() && !character.isDead() && Rnd.get(100) < _effectZone._chance)
				{
					foreach (var e in _effectZone._skills)
					{
						Skill? skill = SkillData.getInstance().getSkill(e.Key, e.Value);
						if (skill != null && (_effectZone._bypassConditions || skill.CheckCondition(character, character, false)))
						{
							if (character.getAffectedSkillLevel(skill.Id) < skill.Level)
							{
								skill.ActivateSkill(character, [character]);
							}
						}
					}
				}
			}
		}
	}
}