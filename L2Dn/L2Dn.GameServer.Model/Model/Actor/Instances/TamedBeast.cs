using L2Dn.GameServer.AI;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Actor.Instances;

// While a tamed beast behaves a lot like a pet (ingame) and does have
// an owner, in all other aspects, it acts like a mob.
// In addition, it can be fed in order to increase its duration.
// This class handles the running tasks, AI, and feed of the mob.
// The (mostly optional) AI on feeding the spawn is handled by the datapack ai script
public class TamedBeast: FeedableBeast
{
	private int _foodSkillId;
	private const int MAX_DISTANCE_FROM_HOME = 30000;
	private const int MAX_DISTANCE_FROM_OWNER = 2000;
	private const int MAX_DURATION = 1200000; // 20 minutes
	private const int DURATION_CHECK_INTERVAL = 60000; // 1 minute
	private const int DURATION_INCREASE_INTERVAL = 20000; // 20 secs (gained upon feeding)
	private const int BUFF_INTERVAL = 5000; // 5 seconds
	private int _remainingTime = MAX_DURATION;
	private Location3D _homeLocation;
	private Player _owner;
	private ScheduledFuture? _buffTask;
	private ScheduledFuture? _durationCheckTask;
	private readonly bool _isFreyaBeast;
	private Set<Skill> _beastSkills = [];

	public TamedBeast(NpcTemplate npcTemplate, Player owner): base(npcTemplate)
	{
		InstanceType = InstanceType.TamedBeast;
        _owner = owner;
		setHome(this);
	}

	public TamedBeast(NpcTemplate npcTemplate, Player owner, int foodSkillId, Location3D location,
		bool isFreyaBeast = false): base(npcTemplate)
	{
		InstanceType = InstanceType.TamedBeast;
		_isFreyaBeast = isFreyaBeast;
        _owner = owner;
		setCurrentHp(getMaxHp());
		setCurrentMp(getMaxMp());
		setFoodType(foodSkillId);
		setHome(location);
		spawnMe(location);
		setOwner(owner);
		if (isFreyaBeast)
		{
			getAI().setIntention(CtrlIntention.AI_INTENTION_FOLLOW, _owner);
		}
	}

	public void onReceiveFood()
	{
		// Eating food extends the duration by 20secs, to a max of 20minutes
		_remainingTime += DURATION_INCREASE_INTERVAL;
		if (_remainingTime > MAX_DURATION)
		{
			_remainingTime = MAX_DURATION;
		}
	}

	public Location3D getHome()
	{
		return _homeLocation;
	}

	public void setHome(Location3D location)
	{
		_homeLocation = location;
	}

	public void setHome(Creature c)
	{
		setHome(c.Location.Location3D);
	}

	public int getRemainingTime()
	{
		return _remainingTime;
	}

	public void setRemainingTime(int duration)
	{
		_remainingTime = duration;
	}

	public int getFoodType()
	{
		return _foodSkillId;
	}

	public void setFoodType(int foodItemId)
	{
		if (foodItemId > 0)
		{
			_foodSkillId = foodItemId;

			// start the duration checks
			// start the buff tasks
			if (_durationCheckTask != null)
			{
				_durationCheckTask.cancel(true);
			}
			_durationCheckTask = ThreadPool.scheduleAtFixedRate(new CheckDuration(this), DURATION_CHECK_INTERVAL, DURATION_CHECK_INTERVAL);
		}
	}

	public override bool doDie(Creature? killer)
	{
		if (!base.doDie(killer))
		{
			return false;
		}

		getAI().stopFollow();
		if (_buffTask != null)
			_buffTask.cancel(true);

        if (_durationCheckTask != null)
			_durationCheckTask.cancel(true);

		// clean up variables
		if (_owner != null)
			_owner.getTrainedBeasts().remove(this);

        _buffTask = null;
		_durationCheckTask = null;
		_foodSkillId = 0;
		_remainingTime = 0;
		return true;
	}

	public override bool isAutoAttackable(Creature attacker)
	{
		return !_isFreyaBeast;
	}

	public bool isFreyaBeast()
	{
		return _isFreyaBeast;
	}

	public void addBeastSkill(Skill skill)
	{
		if (_beastSkills == null)
		{
			_beastSkills = new();
		}
		_beastSkills.add(skill);
	}

	public void castBeastSkills()
	{
		if (_owner == null || _beastSkills == null)
		{
			return;
		}

		TimeSpan delay = TimeSpan.FromMilliseconds(100);
		foreach (Skill skill in _beastSkills)
		{
			ThreadPool.schedule(new BuffCast(this, skill), delay);
			delay += TimeSpan.FromMilliseconds(100) + skill.HitTime;
		}
		ThreadPool.schedule(new BuffCast(this, null), delay);
	}

	private class BuffCast(TamedBeast tamedBeast, Skill? skill): Runnable
    {
        public void run()
		{
			if (skill == null)
			{
				tamedBeast.getAI().setIntention(CtrlIntention.AI_INTENTION_FOLLOW, tamedBeast._owner);
			}
			else
			{
				tamedBeast.sitCastAndFollow(skill, tamedBeast._owner);
			}
		}
	}

	public Player getOwner()
	{
		return _owner;
	}

	public void setOwner(Player owner)
	{
		if (owner != null)
		{
			_owner = owner;
			setTitle(owner.getName());
			// broadcast the new title
			setShowSummonAnimation(true);
			broadcastPacket(new NpcInfoPacket(this));
			owner.addTrainedBeast(this);

			// always and automatically follow the owner.
			getAI().startFollow(_owner, 100);
			if (!_isFreyaBeast)
			{
				// instead of calculating this value each time, let's get this now and pass it on
				int totalBuffsAvailable = 0;
				foreach (Skill skill in getTemplate().getSkills().Values)
				{
					// if the skill is a buff, check if the owner has it already [ owner.getEffect(Skill skill) ]
					if (skill.IsContinuous && !skill.IsDebuff)
					{
						totalBuffsAvailable++;
					}
				}

				// start the buff tasks
				if (_buffTask != null)
				{
					_buffTask.cancel(true);
				}

				_buffTask = ThreadPool.scheduleAtFixedRate(new CheckOwnerBuffs(this, totalBuffsAvailable),
					BUFF_INTERVAL, BUFF_INTERVAL);
			}
		}
		else
		{
			deleteMe(); // despawn if no owner
		}
	}

	public bool isTooFarFromHome()
	{
		return !this.IsInsideRadius3D(_homeLocation, MAX_DISTANCE_FROM_HOME);
	}

	public override bool deleteMe()
	{
		if (_buffTask != null)
		{
			_buffTask.cancel(true);
		}

		_durationCheckTask?.cancel(true);
		stopHpMpRegeneration();

		// clean up variables
		if (_owner != null)
		{
			_owner.getTrainedBeasts().remove(this);
		}
		setTarget(null);
		_buffTask = null;
		_durationCheckTask = null;
		_foodSkillId = 0;
		_remainingTime = 0;

		// remove the spawn
		return base.deleteMe();
	}

	// notification triggered by the owner when the owner is attacked.
	// tamed mobs will heal/recharge or debuff the enemy according to their skills
	public void onOwnerGotAttacked(Creature? attacker)
	{
		// check if the owner is no longer around...if so, despawn
		if (_owner == null || !_owner.isOnline())
		{
			deleteMe();
			return;
		}
		// if the owner is too far away, stop anything else and immediately run towards the owner.
		if (!_owner.IsInsideRadius3D(this, MAX_DISTANCE_FROM_OWNER))
		{
			getAI().startFollow(_owner);
			return;
		}
		// if the owner is dead, do nothing...
		if (_owner.isDead() || _isFreyaBeast)
		{
			return;
		}

		// if the tamed beast is currently in the middle of casting, let it complete its skill...
		if (isCastingNow(x => x.isAnyNormalType()))
		{
			return;
		}

		float HPRatio = (float) _owner.getCurrentHp() / _owner.getMaxHp();

		// if the owner has a lot of HP, then debuff the enemy with a random debuff among the available skills
		// use of more than one debuff at this moment is acceptable
		if (HPRatio >= 0.8)
		{
			foreach (Skill skill in getTemplate().getSkills().Values)
			{
				// if the skill is a debuff, check if the attacker has it already [ attacker.getEffect(Skill skill) ]
				if (skill.IsDebuff && Rnd.get(3) < 1 && attacker != null && attacker.isAffectedBySkill(skill.Id))
				{
					sitCastAndFollow(skill, attacker);
				}
			}
		}
		// for HP levels between 80% and 50%, do not react to attack events (so that MP can regenerate a bit)
		// for lower HP ranges, heal or recharge the owner with 1 skill use per attack.
		else if (HPRatio < 0.5)
		{
			int chance = 1;
			if (HPRatio < 0.25)
			{
				chance = 2;
			}

			// if the owner has a lot of HP, then debuff the enemy with a random debuff among the available skills
			foreach (Skill skill in getTemplate().getSkills().Values)
			{
				// if the skill is a buff, check if the owner has it already [ owner.getEffect(Skill skill) ]
				if (Rnd.get(5) < chance && skill.HasEffectType(EffectType.CPHEAL, EffectType.HEAL, EffectType.MANAHEAL_BY_LEVEL, EffectType.MANAHEAL_PERCENT))
				{
					sitCastAndFollow(skill, _owner);
				}
			}
		}
	}

	/**
	 * Prepare and cast a skill:<br>
	 * First smoothly prepare the beast for casting, by abandoning other actions.<br>
	 * Next, call super.doCast(skill) in order to actually cast the spell.<br>
	 * Finally, return to auto-following the owner.
	 * @param skill
	 * @param target
	 */
	protected void sitCastAndFollow(Skill skill, Creature target)
	{
		stopMove(null);
		broadcastPacket(new StopMovePacket(this));
		getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);

		setTarget(target);
		doCast(skill);
		getAI().setIntention(CtrlIntention.AI_INTENTION_FOLLOW, _owner);
	}

	private class CheckDuration: Runnable
	{
		private TamedBeast _tamedBeast;

		public CheckDuration(TamedBeast tamedBeast)
		{
			_tamedBeast = tamedBeast;
		}

		public void run()
		{
			int foodTypeSkillId = _tamedBeast.getFoodType();
			Player owner = _tamedBeast.getOwner();
			Item? item = null;
			if (_tamedBeast._isFreyaBeast)
			{
				item = owner.getInventory().getItemByItemId(foodTypeSkillId);
				if (item != null && item.getCount() >= 1)
				{
					owner.destroyItem("BeastMob", item, 1, _tamedBeast, true);
					_tamedBeast.broadcastPacket(new SocialActionPacket(_tamedBeast.ObjectId, 3));
				}
				else
				{
					_tamedBeast.deleteMe();
				}
			}
			else
			{
				_tamedBeast.setRemainingTime(_tamedBeast.getRemainingTime() - DURATION_CHECK_INTERVAL);
				// I tried to avoid this as much as possible...but it seems I can't avoid hardcoding
				// ids further, except by carrying an additional variable just for these two lines...
				// Find which food item needs to be consumed.
				if (foodTypeSkillId == 2188)
				{
					item = owner.getInventory().getItemByItemId(6643);
				}
				else if (foodTypeSkillId == 2189)
				{
					item = owner.getInventory().getItemByItemId(6644);
				}

				// if the owner has enough food, call the item handler (use the food and triffer all necessary actions)
				if (item != null && item.getCount() >= 1)
				{
					WorldObject? oldTarget = owner.getTarget();
					owner.setTarget(_tamedBeast);

					// emulate a call to the owner using food, but bypass all checks for range, etc
					// this also causes a call to the AI tasks handling feeding, which may call onReceiveFood as required.
                    Skill skill = SkillData.Instance.GetSkill(foodTypeSkillId, 1) ??
                        throw new InvalidOperationException($"Skill not found, id={foodTypeSkillId}");

					SkillCaster.triggerCast(owner, _tamedBeast, skill);
					owner.setTarget(oldTarget);
				}
				else
				{
					// if the owner has no food, the beast immediately despawns, except when it was only
					// newly spawned. Newly spawned beasts can last up to 5 minutes
					if (_tamedBeast.getRemainingTime() < MAX_DURATION - 300000)
					{
						_tamedBeast.setRemainingTime(-1);
					}
				}
				// There are too many conflicting reports about whether distance from home should be taken into consideration. Disabled for now.
				// if (_tamedBeast.isTooFarFromHome())
				// _tamedBeast.setRemainingTime(-1);
				if (_tamedBeast.getRemainingTime() <= 0)
				{
					_tamedBeast.deleteMe();
				}
			}
		}
	}

	private class CheckOwnerBuffs: Runnable
	{
		private TamedBeast _tamedBeast;
		private int _numBuffs;

		public CheckOwnerBuffs(TamedBeast tamedBeast, int numBuffs)
		{
			_tamedBeast = tamedBeast;
			_numBuffs = numBuffs;
		}

		public void run()
		{
			Player owner = _tamedBeast.getOwner();

			// check if the owner is no longer around...if so, despawn
			if (owner == null || !owner.isOnline())
			{
				_tamedBeast.deleteMe();
				return;
			}
			// if the owner is too far away, stop anything else and immediately run towards the owner.
			if (!_tamedBeast.IsInsideRadius3D(owner, MAX_DISTANCE_FROM_OWNER))
			{
				_tamedBeast.getAI().startFollow(owner);
				return;
			}
			// if the owner is dead, do nothing...
			if (owner.isDead())
			{
				return;
			}
			// if the tamed beast is currently casting a spell, do not interfere (do not attempt to cast anything new yet).
			if (_tamedBeast.isCastingNow(x => x.isAnyNormalType()))
			{
				return;
			}

			int totalBuffsOnOwner = 0;
			int i = 0;
			int rand = Rnd.get(_numBuffs);
			Skill? buffToGive = null;

			// get this npc's skills: getSkills()
			foreach (Skill skill in _tamedBeast.getTemplate().getSkills().Values)
			{
				// if the skill is a buff, check if the owner has it already [ owner.getEffect(Skill skill) ]
				if (skill.IsContinuous && !skill.IsDebuff)
				{
					if (i++ == rand)
					{
						buffToGive = skill;
					}
					if (owner.isAffectedBySkill(skill.Id))
					{
						totalBuffsOnOwner++;
					}
				}
			}

			// if the owner has less than 60% of this beast's available buff, cast a random buff
			if (buffToGive != null && _numBuffs * 2 / 3 > totalBuffsOnOwner)
			{
				_tamedBeast.sitCastAndFollow(buffToGive, owner);
			}

			_tamedBeast.getAI().setIntention(CtrlIntention.AI_INTENTION_FOLLOW, _tamedBeast.getOwner());
		}
	}

	public override void onAction(Player player, bool interact)
	{
		if (player == null || !canTarget(player))
		{
			return;
		}

		// Check if the Player already target the Npc
		if (this != player.getTarget())
		{
			// Set the target of the Player player
			player.setTarget(this);
		}
		else if (interact)
		{
			if (isAutoAttackable(player) && Math.Abs(player.getZ() - getZ()) < 100)
			{
				player.getAI().setIntention(CtrlIntention.AI_INTENTION_ATTACK, this);
			}
			else
			{
				// Send a Server->Client ActionFailed to the Player in order to avoid that the client wait another packet
				player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			}
		}
	}
}