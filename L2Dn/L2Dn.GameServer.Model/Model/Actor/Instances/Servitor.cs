using L2Dn.GameServer.AI;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Model.Enums;
using Microsoft.EntityFrameworkCore;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Actor.Instances;

public class Servitor : Summon, Runnable
{
	private float _expMultiplier;
	private ItemHolder _itemConsume;
	private TimeSpan? _lifeTime;
	private TimeSpan? _lifeTimeRemaining;
	private TimeSpan _consumeItemInterval;
	private TimeSpan _consumeItemIntervalRemaining;
	private ScheduledFuture? _summonLifeTask;
	private int _referenceSkill;

	public Servitor(NpcTemplate template, Player owner): base(template, owner)
	{
		InstanceType = InstanceType.Servitor;
		setShowSummonAnimation(true);
	}

	public override void onSpawn()
	{
		base.onSpawn();
		if (_lifeTime != null && _summonLifeTask == null)
		{
			_summonLifeTask = ThreadPool.scheduleAtFixedRate(this, 0, 5000);
		}
	}

	public override int getLevel()
	{
		return getTemplate() != null ? getTemplate().getLevel() : 0;
	}

	public override int getSummonType()
	{
		return 1;
	}

	// ************************************/

	public void setExpMultiplier(float expMultiplier)
	{
		_expMultiplier = expMultiplier;
	}

	public float getExpMultiplier()
	{
		return _expMultiplier;
	}

	// ************************************/

	public void setItemConsume(ItemHolder item)
	{
		_itemConsume = item;
	}

	public ItemHolder getItemConsume()
	{
		return _itemConsume;
	}

	// ************************************/

	public void setItemConsumeInterval(TimeSpan interval)
	{
		_consumeItemInterval = interval;
		_consumeItemIntervalRemaining = interval;
	}

	public TimeSpan getItemConsumeInterval()
	{
		return _consumeItemInterval;
	}

	// ************************************/

	public void setLifeTime(TimeSpan? lifeTime)
	{
		_lifeTime = lifeTime;
		_lifeTimeRemaining = lifeTime;
	}

	public TimeSpan? getLifeTime()
	{
		return _lifeTime;
	}

	// ************************************/

	public void setLifeTimeRemaining(TimeSpan? time)
	{
		_lifeTimeRemaining = time;
	}

	public TimeSpan? getLifeTimeRemaining()
	{
		return _lifeTimeRemaining;
	}

	// ************************************/

	public void setReferenceSkill(int skillId)
	{
		_referenceSkill = skillId;
	}

	public int getReferenceSkill()
	{
		return _referenceSkill;
	}

	public override bool doDie(Creature killer)
	{
		if (!base.doDie(killer))
		{
			return false;
		}

		if (_summonLifeTask != null)
		{
			_summonLifeTask.cancel(false);
		}

		return true;
	}

	/**
	 * Servitors' skills automatically change their level based on the servitor's level.<br>
	 * Until level 70, the servitor gets 1 lv of skill per 10 levels.<br>
	 * After that, it is 1 skill level per 5 servitor levels.<br>
	 * If the resulting skill level doesn't exist use the max that does exist!
	 */
	public override void doCast(Skill skill)
	{
		int petLevel = getLevel();
		int skillLevel = petLevel / 10;
		if (petLevel >= 70)
		{
			skillLevel += (petLevel - 65) / 10;
		}

		// Adjust the level for servitors less than level 1.
		if (skillLevel < 1)
		{
			skillLevel = 1;
		}

		Skill? skillToCast = SkillData.getInstance().getSkill(skill.getId(), skillLevel);
		if (skillToCast != null)
		{
			base.doCast(skillToCast);
		}
		else
		{
			base.doCast(skill);
		}
	}

	public override void setRestoreSummon(bool value)
	{
		_restoreSummon = value;
	}

	public override void stopSkillEffects(SkillFinishType type, int skillId)
	{
		base.stopSkillEffects(type, skillId);
		Map<int, ICollection<SummonEffectTable.SummonEffect>>? servitorEffects = SummonEffectTable.getInstance().getServitorEffects(getOwner());
		if (servitorEffects != null)
		{
			ICollection<SummonEffectTable.SummonEffect>? effects = servitorEffects.get(_referenceSkill);
			if (effects != null && effects.Count != 0)
			{
				foreach (SummonEffectTable.SummonEffect effect in effects)
				{
					Skill skill = effect.getSkill();
					if (skill != null && skill.getId() == skillId)
					{
						effects.Remove(effect);
					}
				}
			}
		}
	}

	public override void storeMe()
	{
		if (_referenceSkill == 0)
		{
			return;
		}

		if (Config.RESTORE_SERVITOR_ON_RECONNECT)
		{
			if (isDead())
			{
				CharSummonTable.getInstance().removeServitor(getOwner(), ObjectId);
			}
			else
			{
				CharSummonTable.getInstance().saveSummon(this);
			}
		}
	}

	public override void storeEffect(bool storeEffects)
	{
		if (!Config.SUMMON_STORE_SKILL_COOLTIME)
		{
			return;
		}

		if (getOwner() == null || getOwner().isInOlympiadMode())
		{
			return;
		}

		// Clear list for overwrite
		if (SummonEffectTable.getInstance().getServitorEffectsOwner().GetValueOrDefault(getOwner().ObjectId)?
		    .ContainsKey(getOwner().getClassIndex()) ?? false)
		{
			SummonEffectTable.getInstance().getServitorEffects(getOwner()).GetValueOrDefault(getReferenceSkill())
				?.Clear();
		}

		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();

			// Delete all current stored effects for summon to avoid dupe
			int ownerId = getOwner().ObjectId;
			int ownerClassIndex = getOwner().getClassIndex();
			ctx.SummonSkillReuses.Where(r =>
					r.OwnerId == ownerId && r.OwnerClassIndex == ownerClassIndex && r.SummonSkillId == _referenceSkill)
				.ExecuteDelete();

			int buffIndex = 0;

			Set<long> storedSkills = new();

			// Store all effect data along with calculated remaining
			if (storeEffects)
			{
				foreach (BuffInfo info in getEffectList().getEffects())
				{
					if (info == null)
					{
						continue;
					}

					Skill skill = info.getSkill();

					// Do not store those effects.
					if (skill.isDeleteAbnormalOnLeave())
					{
						continue;
					}

					// Do not save heals.
					if (skill.getAbnormalType() == AbnormalType.LIFE_FORCE_OTHERS)
					{
						continue;
					}

					// Toggles are skipped, unless they are necessary to be always on.
					if (skill.isToggle() && !skill.isNecessaryToggle())
					{
						continue;
					}

					// Dances and songs are not kept in retail.
					if (skill.isDance() && !Config.ALT_STORE_DANCES)
					{
						continue;
					}

					if (storedSkills.Contains(skill.getReuseHashCode()))
					{
						continue;
					}

					storedSkills.add(skill.getReuseHashCode());

					int skillId = skill.getId();
					int skillLevel = skill.getLevel();

					// Search by primary key
					DbSummonSkillReuse? record = ctx.SummonSkillReuses.SingleOrDefault(r =>
						r.OwnerId == ownerId && r.OwnerClassIndex == ownerClassIndex &&
						r.SummonSkillId == _referenceSkill && r.SkillId == skillId && r.SkillLevel == skillLevel);

					if (record is null)
					{
						record = new DbSummonSkillReuse()
						{
							OwnerId = ownerId,
							OwnerClassIndex = (byte)ownerClassIndex,
							SummonSkillId = _referenceSkill,
							SkillId = skillId,
							SkillLevel = (short)skillLevel
						};

						ctx.SummonSkillReuses.Add(record);
					}

					record.RemainingTime = info.getTime() ?? TimeSpan.Zero; // TODO ???
					++buffIndex;
					record.BuffIndex = (byte)buffIndex;

					// XXX: Rework me!
					if (!SummonEffectTable.getInstance().getServitorEffectsOwner()
						    .ContainsKey(getOwner().ObjectId))
					{
						SummonEffectTable.getInstance().getServitorEffectsOwner().put(getOwner().ObjectId, new());
					}

					if (!SummonEffectTable.getInstance().getServitorEffectsOwner().get(getOwner().ObjectId)
						    .ContainsKey(getOwner().getClassIndex()))
					{
						SummonEffectTable.getInstance().getServitorEffectsOwner().get(getOwner().ObjectId)
							.put(getOwner().getClassIndex(), new());
					}

					if (!SummonEffectTable.getInstance().getServitorEffects(getOwner())
						    .ContainsKey(getReferenceSkill()))
					{
						SummonEffectTable.getInstance().getServitorEffects(getOwner()).put(getReferenceSkill(),
							new List<SummonEffectTable.SummonEffect>());
					}

					SummonEffectTable.getInstance().getServitorEffects(getOwner()).get(getReferenceSkill())
						.Add(new SummonEffectTable.SummonEffect(skill, info.getTime() ?? TimeSpan.Zero)); // TODO ???
				}

				ctx.SaveChanges();
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Could not store summon effect data: " + e);
		}
	}

	public override void restoreEffects()
	{
		if (getOwner().isInOlympiadMode())
		{
			return;
		}

		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			int ownerId = getOwner().ObjectId;
			int ownerClassIndex = getOwner().getClassIndex();
			if (!SummonEffectTable.getInstance().getServitorEffectsOwner().ContainsKey(getOwner().ObjectId) ||
			    !SummonEffectTable.getInstance().getServitorEffectsOwner().get(getOwner().ObjectId).ContainsKey(getOwner().getClassIndex()) ||
			    !SummonEffectTable.getInstance().getServitorEffects(getOwner()).ContainsKey(getReferenceSkill()))
			{
				IQueryable<DbSummonSkillReuse> query = ctx.SummonSkillReuses.Where(r =>
					r.OwnerId == ownerId && r.OwnerClassIndex == ownerClassIndex && r.SummonSkillId == _referenceSkill);

				foreach (DbSummonSkillReuse record in query)
				{
					TimeSpan effectCurTime = record.RemainingTime;
					Skill skill = SkillData.getInstance().getSkill(record.SkillId, record.SkillLevel);
					if (skill == null)
					{
						continue;
					}

					// TODO: Rework me!
					if (skill.hasEffects(EffectScope.GENERAL))
					{
						if (!SummonEffectTable.getInstance().getServitorEffectsOwner()
							    .ContainsKey(getOwner().ObjectId))
						{
							SummonEffectTable.getInstance().getServitorEffectsOwner()
								.put(getOwner().ObjectId, new());
						}

						if (!SummonEffectTable.getInstance().getServitorEffectsOwner().get(getOwner().ObjectId)
							    .ContainsKey(getOwner().getClassIndex()))
						{
							SummonEffectTable.getInstance().getServitorEffectsOwner().get(getOwner().ObjectId)
								.put(getOwner().getClassIndex(), new());
						}

						if (!SummonEffectTable.getInstance().getServitorEffects(getOwner())
							    .ContainsKey(getReferenceSkill()))
						{
							SummonEffectTable.getInstance().getServitorEffects(getOwner()).put(getReferenceSkill(),
								new List<SummonEffectTable.SummonEffect>());
						}

						SummonEffectTable.getInstance().getServitorEffects(getOwner()).get(getReferenceSkill())
							.Add(new SummonEffectTable.SummonEffect(skill, effectCurTime));
					}
				}
			}

			ctx.SummonSkillReuses.Where(r =>
					r.OwnerId == ownerId && r.OwnerClassIndex == ownerClassIndex && r.SummonSkillId == _referenceSkill)
				.ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Warn("Could not restore " + this + " active effect data: " + e);
		}
		finally
		{
			if (SummonEffectTable.getInstance().getServitorEffectsOwner().ContainsKey(getOwner().ObjectId) &&
			    SummonEffectTable.getInstance().getServitorEffectsOwner().get(getOwner().ObjectId).ContainsKey(getOwner().getClassIndex()) &&
			    SummonEffectTable.getInstance().getServitorEffects(getOwner()).ContainsKey(getReferenceSkill()))
			{
				foreach (SummonEffectTable.SummonEffect se in SummonEffectTable.getInstance().getServitorEffects(getOwner()).get(getReferenceSkill()))
				{
					if (se != null)
					{
						se.getSkill().applyEffects(this, this, false, se.getEffectCurTime());
					}
				}
			}
		}
	}

	public override void unSummon(Player owner)
	{
		if (_summonLifeTask != null)
		{
			_summonLifeTask.cancel(false);
		}

		base.unSummon(owner);

		if (!_restoreSummon)
		{
			CharSummonTable.getInstance().removeServitor(owner, ObjectId);
		}
	}

	public override bool destroyItem(string process, int objectId, long count, WorldObject reference, bool sendMessage)
	{
		return getOwner().destroyItem(process, objectId, count, reference, sendMessage);
	}

	public override bool destroyItemByItemId(string process, int itemId, long count, WorldObject reference, bool sendMessage)
	{
		return getOwner().destroyItemByItemId(process, itemId, count, reference, sendMessage);
	}

	public override AttributeType getAttackElement()
	{
		if (getOwner() != null)
		{
			return getOwner().getAttackElement();
		}
		return base.getAttackElement();
	}

	public override int getAttackElementValue(AttributeType attackAttribute)
	{
		if (getOwner() != null)
		{
			return getOwner().getAttackElementValue(attackAttribute);
		}
		return base.getAttackElementValue(attackAttribute);
	}

	public override int getDefenseElementValue(AttributeType defenseAttribute)
	{
		if (getOwner() != null)
		{
			return getOwner().getDefenseElementValue(defenseAttribute);
		}
		return base.getDefenseElementValue(defenseAttribute);
	}

	public override bool isServitor()
	{
		return true;
	}

	public void run()
	{
		TimeSpan usedtime = TimeSpan.FromMilliseconds(5000);
		_lifeTimeRemaining -= usedtime;
		if (isDead() || !isSpawned())
		{
			if (_summonLifeTask != null)
			{
				_summonLifeTask.cancel(false);
			}
			return;
		}

		// check if the summon's lifetime has ran out
		if (_lifeTimeRemaining < TimeSpan.Zero)
		{
			sendPacket(SystemMessageId.YOUR_SERVITOR_PASSED_AWAY);
			unSummon(getOwner());
			return;
		}

		if (_consumeItemInterval > TimeSpan.Zero)
		{
			_consumeItemIntervalRemaining -= usedtime;

			// check if it is time to consume another item
			if (_consumeItemIntervalRemaining <= TimeSpan.Zero && _itemConsume.getCount() > 0 && _itemConsume.getId() > 0 && !isDead())
			{
				if (destroyItemByItemId("Consume", _itemConsume.getId(), _itemConsume.getCount(), this, false))
				{
					SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.A_SUMMONED_MONSTER_USES_S1);
					msg.Params.addItemName(_itemConsume.getId());
					sendPacket(msg);

					// Reset
					_consumeItemIntervalRemaining = _consumeItemInterval;
				}
				else
				{
					sendPacket(SystemMessageId.SINCE_YOU_DO_NOT_HAVE_ENOUGH_ITEMS_TO_MAINTAIN_THE_SERVITOR_S_STAY_THE_SERVITOR_HAS_DISAPPEARED);
					unSummon(getOwner());
				}
			}
		}

		int lifeTimeInMs = (int)(_lifeTime ?? TimeSpan.Zero).TotalMilliseconds;
		int lifeTimeRemainingInMs = (int)(_lifeTimeRemaining ?? TimeSpan.Zero).TotalMilliseconds;
		sendPacket(new SetSummonRemainTimePacket(lifeTimeInMs, lifeTimeRemainingInMs));

		// Using same task to check if owner is in visible range
		if (this.Distance3D(getOwner()) > 2000)
		{
			getAI().setIntention(CtrlIntention.AI_INTENTION_FOLLOW, getOwner());
		}
	}

	public override void doPickupItem(WorldObject @object)
	{
	}
}