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
	private List<Creature>? _channelized;

	private Skill? _skill;
	private ScheduledFuture? _task;

	public SkillChannelizer(Creature channelizer)
	{
		_channelizer = channelizer;
	}

	public Creature getChannelizer()
	{
		return _channelizer;
	}

	public List<Creature>? getChannelized()
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
		_task = ThreadPool.scheduleAtFixedRate(this, skill.ChannelingTickInitialDelay,
			skill.ChannelingTickInterval);
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
		_task?.cancel(false);
		_task = null;

		// Cancel target channelization and unset it.
		if (_channelized != null && _skill != null)
		{
			foreach (Creature creature in _channelized)
			{
				creature.getSkillChannelized().removeChannelizer(_skill.ChannelingSkillId, _channelizer);
			}
		}

        _channelized = null;

        // unset skill.
		_skill = null;
	}

	public Skill? getSkill()
	{
		return _skill;
	}

	public bool isChanneling()
	{
		return _task != null;
	}

	public void run()
	{
		if (!isChanneling() || _skill is null)
		{
			return;
		}

		Skill skill = _skill;
		List<Creature>? channelized = _channelized;

		try
		{
			if (skill.MpPerChanneling > 0)
			{
				// Validate mana per tick.
				if (_channelizer.getCurrentMp() < skill.MpPerChanneling)
				{
					if (_channelizer.isPlayer())
					{
						_channelizer.sendPacket(SystemMessageId.YOUR_SKILL_WAS_DEACTIVATED_DUE_TO_LACK_OF_MP);
					}

					_channelizer.abortCast();
					return;
				}

				// Reduce mana per tick
				_channelizer.reduceCurrentMp(skill.MpPerChanneling);
			}

			// Apply channeling skills on the targets.
			List<Creature> targetList = new();
			WorldObject? target = skill.GetTarget(_channelizer, false, false, false);
			if (target != null)
			{
				skill.ForEachTargetAffected<Creature>(_channelizer, target, o =>
				{
					if (o.isCreature())
					{
						targetList.Add(o);
						o.getSkillChannelized().addChannelizer(skill.ChannelingSkillId, _channelizer);
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
				if (!Util.checkIfInRange(skill.EffectRange, _channelizer, creature, true))
				{
					continue;
				}

                if (!GeoEngine.getInstance().canSeeTarget(_channelizer, creature))
                {
                    continue;
                }

                if (skill.ChannelingSkillId > 0)
				{
					int maxSkillLevel = SkillData.getInstance().getMaxLevel(skill.ChannelingSkillId);
					int skillLevel =
						Math.Min(creature.getSkillChannelized().getChannerlizersSize(skill.ChannelingSkillId),
							maxSkillLevel);
					if (skillLevel == 0)
					{
						continue;
					}

					BuffInfo? info = creature.getEffectList().getBuffInfoBySkillId(skill.ChannelingSkillId);
					if (info == null || info.getSkill().Level < skillLevel)
					{
						Skill? channeledSkill =
							SkillData.getInstance().getSkill(skill.ChannelingSkillId, skillLevel);
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
						channeledSkill.ApplyEffects(_channelizer, creature);
					}

					if (!skill.IsToggle)
					{
						_channelizer.broadcastPacket(new MagicSkillLaunchedPacket(_channelizer, skill.Id,
							skill.Level, SkillCastingType.NORMAL, creature));
					}
				}
				else
				{
					skill.ApplyChannelingEffects(_channelizer, creature);
				}

				// Reduce shots.
				if (skill.UseSpiritShot)
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
				_channelizer.rechargeShots(skill.UseSoulShot, skill.UseSpiritShot, false);
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