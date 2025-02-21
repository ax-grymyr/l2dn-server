using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Skills;

/**
 * Skill Channelizer implementation.
 * @author UnAfraid
 */
public class SkillChannelizer: Runnable
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(SkillChannelizer));

	private readonly Creature _channelizer;
	private List<Creature> _channelized;

	private Skill _skill;
	private ScheduledFuture _task;

	public SkillChannelizer(Creature channelizer)
	{
		_channelizer = channelizer;
	}

	public Creature getChannelizer()
	{
		return _channelizer;
	}

	public List<Creature> getChannelized()
	{
		return _channelized;
	}

	public bool hasChannelized()
	{
		return _channelized != null;
	}

	public void startChanneling(Skill skill)
	{
		// Verify for same status.
		if (isChanneling())
		{
			LOGGER.Warn("Character: " + ToString() + " is attempting to channel skill but he already does!");
			return;
		}

		// Start channeling.
		_skill = skill;
		_task = ThreadPool.scheduleAtFixedRate(this, skill.getChannelingTickInitialDelay(),
			skill.getChannelingTickInterval());
	}

	public void stopChanneling()
	{
		// Verify for same status.
		if (!isChanneling())
		{
			LOGGER.Warn("Character: " + ToString() + " is attempting to stop channel skill but he does not!");
			return;
		}

		// Cancel the task and unset it.
		_task.cancel(false);
		_task = null;

		// Cancel target channelization and unset it.
		if (_channelized != null)
		{
			foreach (Creature creature in _channelized)
			{
				creature.getSkillChannelized().removeChannelizer(_skill.getChannelingSkillId(), _channelizer);
			}

			_channelized = null;
		}

		// unset skill.
		_skill = null;
	}

	public Skill getSkill()
	{
		return _skill;
	}

	public bool isChanneling()
	{
		return _task != null;
	}

	public void run()
	{
		if (!isChanneling())
		{
			return;
		}

		Skill skill = _skill;
		List<Creature> channelized = _channelized;

		try
		{
			if (skill.getMpPerChanneling() > 0)
			{
				// Validate mana per tick.
				if (_channelizer.getCurrentMp() < skill.getMpPerChanneling())
				{
					if (_channelizer.isPlayer())
					{
						_channelizer.sendPacket(SystemMessageId.YOUR_SKILL_WAS_DEACTIVATED_DUE_TO_LACK_OF_MP);
					}

					_channelizer.abortCast();
					return;
				}

				// Reduce mana per tick
				_channelizer.reduceCurrentMp(skill.getMpPerChanneling());
			}

			// Apply channeling skills on the targets.
			List<Creature> targetList = new();
			WorldObject target = skill.getTarget(_channelizer, false, false, false);
			if (target != null)
			{
				skill.forEachTargetAffected<Creature>(_channelizer, target, o =>
				{
					if (o.isCreature())
					{
						targetList.Add(o);
						o.getSkillChannelized().addChannelizer(skill.getChannelingSkillId(), _channelizer);
					}
				});
			}

			if (targetList.Count == 0)
			{
				return;
			}

			channelized = targetList;
			foreach (Creature creature in channelized)
			{
				if (!Util.checkIfInRange(skill.getEffectRange(), _channelizer, creature, true))
				{
					continue;
				}
				else if (!GeoEngine.getInstance().canSeeTarget(_channelizer, creature))
				{
					continue;
				}

				if (skill.getChannelingSkillId() > 0)
				{
					int maxSkillLevel = SkillData.getInstance().getMaxLevel(skill.getChannelingSkillId());
					int skillLevel =
						Math.Min(creature.getSkillChannelized().getChannerlizersSize(skill.getChannelingSkillId()),
							maxSkillLevel);
					if (skillLevel == 0)
					{
						continue;
					}

					BuffInfo info = creature.getEffectList().getBuffInfoBySkillId(skill.getChannelingSkillId());
					if (info == null || info.getSkill().getLevel() < skillLevel)
					{
						Skill channeledSkill =
							SkillData.getInstance().getSkill(skill.getChannelingSkillId(), skillLevel);
						if (channeledSkill == null)
						{
							LOGGER.Warn(GetType().Name + ": Non existent channeling skill requested: " + skill);
							_channelizer.abortCast();
							return;
						}

						// Update PvP status
						if (creature.isPlayable() && _channelizer.isPlayer())
						{
							((Player)_channelizer).updatePvPStatus(creature);
						}

						// Be warned, this method has the possibility to call doDie->abortCast->stopChanneling method. Variable cache above try{} is used in this case to avoid NPEs.
						channeledSkill.applyEffects(_channelizer, creature);
					}

					if (!skill.isToggle())
					{
						_channelizer.broadcastPacket(new MagicSkillLaunchedPacket(_channelizer, skill.getId(),
							skill.getLevel(), SkillCastingType.NORMAL, creature));
					}
				}
				else
				{
					skill.applyChannelingEffects(_channelizer, creature);
				}

				// Reduce shots.
				if (skill.useSpiritShot())
				{
					_channelizer.unchargeShot(_channelizer.isChargedShot(ShotType.BLESSED_SPIRITSHOTS)
						? ShotType.BLESSED_SPIRITSHOTS
						: ShotType.SPIRITSHOTS);
				}
				else
				{
					_channelizer.unchargeShot(_channelizer.isChargedShot(ShotType.BLESSED_SOULSHOTS)
						? ShotType.BLESSED_SOULSHOTS
						: ShotType.SOULSHOTS);
				}

				// Shots are re-charged every cast.
				_channelizer.rechargeShots(skill.useSoulShot(), skill.useSpiritShot(), false);
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(
				"Error while channelizing skill: " + skill + " channelizer: " + _channelizer + " channelized: " +
				channelized + "; ", e);
		}
	}
}