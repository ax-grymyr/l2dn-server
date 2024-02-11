using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using NLog;
using ThreadPool = System.Threading.ThreadPool;

namespace L2Dn.GameServer.Model.Actor.Instances;

public class Servitor : Summon, Runnable
{
	protected static readonly Logger log = LogManager.GetLogger(nameof(Servitor));
	
	private const String ADD_SKILL_SAVE = "REPLACE INTO character_summon_skills_save (ownerId,ownerClassIndex,summonSkillId,skill_id,skill_level,remaining_time,buff_index) VALUES (?,?,?,?,?,?,?)";
	private const String RESTORE_SKILL_SAVE = "SELECT skill_id,skill_level,remaining_time,buff_index FROM character_summon_skills_save WHERE ownerId=? AND ownerClassIndex=? AND summonSkillId=? ORDER BY buff_index ASC";
	private const String DELETE_SKILL_SAVE = "DELETE FROM character_summon_skills_save WHERE ownerId=? AND ownerClassIndex=? AND summonSkillId=?";
	
	private float _expMultiplier = 0;
	private ItemHolder _itemConsume;
	private int _lifeTime;
	private int _lifeTimeRemaining;
	private int _consumeItemInterval;
	private int _consumeItemIntervalRemaining;
	protected Future<?> _summonLifeTask;
	
	private int _referenceSkill;
	
	public Servitor(NpcTemplate template, Player owner): base(template)
	{
		setInstanceType(InstanceType.Servitor);
		setShowSummonAnimation(true);
	}
	
	public override void onSpawn()
	{
		base.onSpawn();
		if ((_lifeTime > 0) && (_summonLifeTask == null))
		{
			_summonLifeTask = ThreadPool.scheduleAtFixedRate(this, 0, 5000);
		}
	}
	
	public override int getLevel()
	{
		return (getTemplate() != null ? getTemplate().getLevel() : 0);
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
	
	public void setItemConsumeInterval(int interval)
	{
		_consumeItemInterval = interval;
		_consumeItemIntervalRemaining = interval;
	}
	
	public int getItemConsumeInterval()
	{
		return _consumeItemInterval;
	}
	
	// ************************************/
	
	public void setLifeTime(int lifeTime)
	{
		_lifeTime = lifeTime;
		_lifeTimeRemaining = lifeTime;
	}
	
	public int getLifeTime()
	{
		return _lifeTime;
	}
	
	// ************************************/
	
	public void setLifeTimeRemaining(int time)
	{
		_lifeTimeRemaining = time;
	}
	
	public int getLifeTimeRemaining()
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
		
		Skill skillToCast = SkillData.getInstance().getSkill(skill.getId(), skillLevel);
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
		Map<int, Collection<SummonEffect>> servitorEffects = SummonEffectTable.getInstance().getServitorEffects(getOwner());
		if (servitorEffects != null)
		{
			Collection<SummonEffect> effects = servitorEffects.get(_referenceSkill);
			if ((effects != null) && !effects.isEmpty())
			{
				foreach (SummonEffect effect in effects)
				{
					Skill skill = effect.getSkill();
					if ((skill != null) && (skill.getId() == skillId))
					{
						effects.remove(effect);
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
				CharSummonTable.getInstance().removeServitor(getOwner(), getObjectId());
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
		
		if ((getOwner() == null) || getOwner().isInOlympiadMode())
		{
			return;
		}
		
		// Clear list for overwrite
		if (SummonEffectTable.getInstance().getServitorEffectsOwner().getOrDefault(getOwner().getObjectId(), Collections.emptyMap()).containsKey(getOwner().getClassIndex()))
		{
			SummonEffectTable.getInstance().getServitorEffects(getOwner()).getOrDefault(getReferenceSkill(), Collections.emptyList()).clear();
		}
		
		try 
		{
			using GameServerDbContext ctx = new();
			PreparedStatement statement = con.prepareStatement(DELETE_SKILL_SAVE);
			
			// Delete all current stored effects for summon to avoid dupe
			statement.setInt(1, getOwner().getObjectId());
			statement.setInt(2, getOwner().getClassIndex());
			statement.setInt(3, _referenceSkill);
			statement.execute();
			
			int buffIndex = 0;
			
			Collection<Long> storedSkills = ConcurrentHashMap.newKeySet();
			
			// Store all effect data along with calculated remaining
			if (storeEffects)
			{
				PreparedStatement ps2 = con.prepareStatement(ADD_SKILL_SAVE);
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
						
						if (storedSkills.contains(skill.getReuseHashCode()))
						{
							continue;
						}
						
						storedSkills.add(skill.getReuseHashCode());
						
						ps2.setInt(1, getOwner().getObjectId());
						ps2.setInt(2, getOwner().getClassIndex());
						ps2.setInt(3, _referenceSkill);
						ps2.setInt(4, skill.getId());
						ps2.setInt(5, skill.getLevel());
						ps2.setInt(6, info.getTime());
						ps2.setInt(7, ++buffIndex);
						ps2.addBatch();
						
						// XXX: Rework me!
						if (!SummonEffectTable.getInstance().getServitorEffectsOwner().containsKey(getOwner().getObjectId()))
						{
							SummonEffectTable.getInstance().getServitorEffectsOwner().put(getOwner().getObjectId(), new HashMap<>());
						}
						if (!SummonEffectTable.getInstance().getServitorEffectsOwner().get(getOwner().getObjectId()).containsKey(getOwner().getClassIndex()))
						{
							SummonEffectTable.getInstance().getServitorEffectsOwner().get(getOwner().getObjectId()).put(getOwner().getClassIndex(), new HashMap<>());
						}
						if (!SummonEffectTable.getInstance().getServitorEffects(getOwner()).containsKey(getReferenceSkill()))
						{
							SummonEffectTable.getInstance().getServitorEffects(getOwner()).put(getReferenceSkill(), ConcurrentHashMap.newKeySet());
						}
						
						SummonEffectTable.getInstance().getServitorEffects(getOwner()).get(getReferenceSkill()).add(new SummonEffect(skill, info.getTime()));
					}
					ps2.executeBatch();
				}
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
			using GameServerDbContext ctx = new();
			if (!SummonEffectTable.getInstance().getServitorEffectsOwner().containsKey(getOwner().getObjectId()) || !SummonEffectTable.getInstance().getServitorEffectsOwner().get(getOwner().getObjectId()).containsKey(getOwner().getClassIndex()) || !SummonEffectTable.getInstance().getServitorEffects(getOwner()).containsKey(getReferenceSkill()))
			{
				PreparedStatement statement = con.prepareStatement(RESTORE_SKILL_SAVE);
				try
				{
					statement.setInt(1, getOwner().getObjectId());
					statement.setInt(2, getOwner().getClassIndex());
					statement.setInt(3, _referenceSkill);
					ResultSet rset = statement.executeQuery();
					{
						while (rset.next())
						{
							int effectCurTime = rset.getInt("remaining_time");
							Skill skill = SkillData.getInstance().getSkill(rset.getInt("skill_id"), rset.getInt("skill_level"));
							if (skill == null)
							{
								continue;
							}
							
							// XXX: Rework me!
							if (skill.hasEffects(EffectScope.GENERAL))
							{
								if (!SummonEffectTable.getInstance().getServitorEffectsOwner().containsKey(getOwner().getObjectId()))
								{
									SummonEffectTable.getInstance().getServitorEffectsOwner().put(getOwner().getObjectId(), new HashMap<>());
								}
								if (!SummonEffectTable.getInstance().getServitorEffectsOwner().get(getOwner().getObjectId()).containsKey(getOwner().getClassIndex()))
								{
									SummonEffectTable.getInstance().getServitorEffectsOwner().get(getOwner().getObjectId()).put(getOwner().getClassIndex(), new HashMap<>());
								}
								if (!SummonEffectTable.getInstance().getServitorEffects(getOwner()).containsKey(getReferenceSkill()))
								{
									SummonEffectTable.getInstance().getServitorEffects(getOwner()).put(getReferenceSkill(), ConcurrentHashMap.newKeySet());
								}
								
								SummonEffectTable.getInstance().getServitorEffects(getOwner()).get(getReferenceSkill()).add(new SummonEffect(skill, effectCurTime));
							}
						}
					}
				}
			}

			PreparedStatement statement = con.prepareStatement(DELETE_SKILL_SAVE);
			{
				statement.setInt(1, getOwner().getObjectId());
				statement.setInt(2, getOwner().getClassIndex());
				statement.setInt(3, _referenceSkill);
				statement.executeUpdate();
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Could not restore " + this + " active effect data: " + e);
		}
		finally
		{
			if (SummonEffectTable.getInstance().getServitorEffectsOwner().containsKey(getOwner().getObjectId()) && SummonEffectTable.getInstance().getServitorEffectsOwner().get(getOwner().getObjectId()).containsKey(getOwner().getClassIndex()) && SummonEffectTable.getInstance().getServitorEffects(getOwner()).containsKey(getReferenceSkill()))
			{
				foreach (SummonEffect se in SummonEffectTable.getInstance().getServitorEffects(getOwner()).get(getReferenceSkill()))
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
			CharSummonTable.getInstance().removeServitor(owner, getObjectId());
		}
	}
	
	public override bool destroyItem(String process, int objectId, long count, WorldObject reference, bool sendMessage)
	{
		return getOwner().destroyItem(process, objectId, count, reference, sendMessage);
	}
	
	public override bool destroyItemByItemId(String process, int itemId, long count, WorldObject reference, bool sendMessage)
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
			return (getOwner().getAttackElementValue(attackAttribute));
		}
		return base.getAttackElementValue(attackAttribute);
	}
	
	public override int getDefenseElementValue(AttributeType defenseAttribute)
	{
		if (getOwner() != null)
		{
			return (getOwner().getDefenseElementValue(defenseAttribute));
		}
		return base.getDefenseElementValue(defenseAttribute);
	}
	
	public override bool isServitor()
	{
		return true;
	}
	
	public override void run()
	{
		int usedtime = 5000;
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
		if (_lifeTimeRemaining < 0)
		{
			sendPacket(SystemMessageId.YOUR_SERVITOR_PASSED_AWAY);
			unSummon(getOwner());
			return;
		}
		
		if (_consumeItemInterval > 0)
		{
			_consumeItemIntervalRemaining -= usedtime;
			
			// check if it is time to consume another item
			if ((_consumeItemIntervalRemaining <= 0) && (_itemConsume.getCount() > 0) && (_itemConsume.getId() > 0) && !isDead())
			{
				if (destroyItemByItemId("Consume", _itemConsume.getId(), _itemConsume.getCount(), this, false))
				{
					SystemMessage msg = new SystemMessage(SystemMessageId.A_SUMMONED_MONSTER_USES_S1);
					msg.addItemName(_itemConsume.getId());
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
		
		sendPacket(new SetSummonRemainTime(_lifeTime, _lifeTimeRemaining));
		
		// Using same task to check if owner is in visible range
		if (calculateDistance3D(getOwner()) > 2000)
		{
			getAI().setIntention(CtrlIntention.AI_INTENTION_FOLLOW, getOwner());
		}
	}
	
	public override void doPickupItem(WorldObject @object)
	{
	}
}
