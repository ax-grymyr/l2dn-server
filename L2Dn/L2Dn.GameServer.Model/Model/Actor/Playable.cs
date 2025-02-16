using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor.Stats;
using L2Dn.GameServer.Model.Actor.Status;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events.Impl.Creatures;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Quests;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Actor;

/**
 * This class represents all Playable characters in the world.<br>
 * Playable:
 * <ul>
 * <li>Player</li>
 * <li>Summon</li>
 * </ul>
 */
public abstract class Playable: Creature
{
	private Creature _lockedTarget;
	private Player transferDmgTo;
	
	private readonly Map<int, int> _replacedSkills = new();
	private readonly Map<int, int> _originalSkills = new();
	
	/**
	 * Constructor of Playable.<br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>Call the Creature constructor to create an empty _skills slot and link copy basic Calculator set to this Playable</li>
	 * </ul>
	 * @param objectId the object id
	 * @param template The CreatureTemplate to apply to the Playable
	 */
	public Playable(int objectId, CreatureTemplate template): base(objectId, template)
	{
		InstanceType = InstanceType.Playable;
		setInvul(false);
	}
	
	public Playable(CreatureTemplate template): base(template)
	{
		InstanceType = InstanceType.Playable;
		setInvul(false);
	}
	
	public override PlayableStat getStat()
	{
		return (PlayableStat) base.getStat();
	}
	
	public override void initCharStat()
	{
		setStat(new PlayableStat(this));
	}
	
	public override PlayableStatus getStatus()
	{
		return (PlayableStatus) base.getStatus();
	}
	
	public override void initCharStatus()
	{
		setStatus(new PlayableStatus(this));
	}
	
	public override bool doDie(Creature killer)
	{
		if (Events.HasSubscribers<OnCreatureDeath>())
		{
			OnCreatureDeath onCreatureDeath = new(killer, this);
			if (Events.Notify(onCreatureDeath) && onCreatureDeath.Terminate)
			{
				return false;
			}
		}
		
		// killing is only possible one time
		lock (this)
		{
			if (isDead())
			{
				return false;
			}
			// now reset currentHp to zero
			setCurrentHp(0);
			setDead(true);
		}
		
		// Set target to null and cancel Attack or Cast
		setTarget(null);
		
		// Abort casting after target has been cancelled.
		abortAttack();
		abortCast();
		
		// Stop movement
		stopMove(null);
		
		// Stop HP/MP/CP Regeneration task
		getStatus().stopHpMpRegeneration();
		
		bool deleteBuffs = true;
		if (isNoblesseBlessedAffected())
		{
			stopEffects(EffectFlag.NOBLESS_BLESSING);
			deleteBuffs = false;
		}
		if (isResurrectSpecialAffected())
		{
			stopEffects(EffectFlag.RESURRECTION_SPECIAL);
			deleteBuffs = false;
		}
		if (isPlayer())
		{
			Player player = getActingPlayer();
			if (player.hasCharmOfCourage())
			{
				if (player.isInSiege())
				{
					getActingPlayer().reviveRequest(getActingPlayer(), false, 0, 0, 0, 0);
				}
				player.setCharmOfCourage(false);
				player.sendPacket(new EtcStatusUpdatePacket(player));
			}
		}
		
		if (deleteBuffs)
		{
			stopAllEffectsExceptThoseThatLastThroughDeath();
		}
		
		// Send the Server->Client packet StatusUpdate with current HP and MP to all other Player to inform
		broadcastStatusUpdate();
		
		ZoneManager.getInstance().getRegion(Location.Location2D)?.onDeath(this);
		
		// Notify Quest of Playable's death
		Player actingPlayer = getActingPlayer();
		if (!actingPlayer.isNotifyQuestOfDeathEmpty())
		{
			foreach (QuestState qs in actingPlayer.getNotifyQuestOfDeath())
			{
				qs.getQuest().notifyDeath((killer == null ? this : killer), this, qs);
			}
		}
		
		// Notify instance
		if (isPlayer())
		{
			Instance instance = getInstanceWorld();
			if (instance != null)
			{
				instance.onDeath(getActingPlayer());
			}
		}
		
		if (killer != null)
		{
			Player killerPlayer = killer.getActingPlayer();
			if (killerPlayer != null)
			{
				killerPlayer.onPlayerKill(this);
			}
		}
		
		// Notify Creature AI
		getAI().notifyEvent(CtrlEvent.EVT_DEAD);
		return true;
	}
	
	public bool checkIfPvP(Player target)
	{
		Player player = getActingPlayer();
		if ((player == null) //
			|| (target == null) //
			|| (player == target) //
			|| (target.getReputation() < 0) //
			|| (target.getPvpFlag() != PvpFlagStatus.None) //
			|| target.isOnDarkSide())
		{
			return true;
		}
		else if (player.isInParty() && player.getParty().containsPlayer(target))
		{
			return false;
		}
		
		Clan playerClan = player.getClan();
		if ((playerClan != null) && !player.isAcademyMember() && !target.isAcademyMember())
		{
			ClanWar war = playerClan.getWarWith(target.getClanId());
			return (war != null) && (war.getState() == ClanWarState.MUTUAL);
		}
		return false;
	}
	
	/**
	 * Return True.
	 */
	public override bool canBeAttacked()
	{
		return true;
	}
	
	// Support for Noblesse Blessing skill, where buffs are retained after resurrect
	public bool isNoblesseBlessedAffected()
	{
		return isAffected(EffectFlag.NOBLESS_BLESSING);
	}
	
	/**
	 * @return {@code true} if char can resurrect by himself, {@code false} otherwise
	 */
	public bool isResurrectSpecialAffected()
	{
		return isAffected(EffectFlag.RESURRECTION_SPECIAL);
	}
	
	/**
	 * @return {@code true} if the Silent Moving mode is active, {@code false} otherwise
	 */
	public bool isSilentMovingAffected()
	{
		return isAffected(EffectFlag.SILENT_MOVE);
	}
	
	/**
	 * For Newbie Protection Blessing skill, keeps you safe from an attack by a chaotic character >= 10 levels apart from you.
	 * @return
	 */
	public bool isProtectionBlessingAffected()
	{
		return isAffected(EffectFlag.PROTECTION_BLESSING);
	}
	
	public override void updateEffectIcons(bool partyOnly)
	{
		getEffectList().updateEffectIcons(partyOnly);
	}
	
	public bool isLockedTarget()
	{
		return _lockedTarget != null;
	}
	
	public Creature getLockedTarget()
	{
		return _lockedTarget;
	}
	
	public void setLockedTarget(Creature creature)
	{
		_lockedTarget = creature;
	}
	
	public void setTransferDamageTo(Player val)
	{
		transferDmgTo = val;
	}
	
	public Player getTransferingDamageTo()
	{
		return transferDmgTo;
	}
	
	/**
	 * Adds a replacement for an original skill.<br>
	 * Both original and replacement skill IDs are stored in their respective maps.
	 * @param originalId The ID of the original skill.
	 * @param replacementId The ID of the replacement skill.
	 */
	public void addReplacedSkill(int originalId, int replacementId)
	{
		_replacedSkills.put(originalId, replacementId);
		_originalSkills.put(replacementId, originalId);
	}
	
	/**
	 * Removes a replaced skill by the original skill ID.<br>
	 * The corresponding replacement skill ID is also removed from its map.
	 * @param originalId The ID of the original skill to be removed.
	 */
	public void removeReplacedSkill(int originalId)
	{
		int replacementId = _replacedSkills.remove(originalId);
		if (replacementId != null)
		{
			_originalSkills.remove(replacementId);
		}
	}
	
	/**
	 * Retrieves the replacement skill for a given original skill.
	 * @param originalId The ID of the original skill.
	 * @return The ID of the replacement skill if it exists, or the original skill ID.
	 */
	public int getReplacementSkill(int originalId)
	{
		return _replacedSkills.GetValueOrDefault(originalId, originalId);
	}
	
	/**
	 * Retrieves the original skill for a given replacement skill.
	 * @param replacementId The ID of the replacement skill.
	 * @return The ID of the original skill if it exists, or the replacement skill ID.
	 */
	public int getOriginalSkill(int replacementId)
	{
		return _originalSkills.GetValueOrDefault(replacementId, replacementId);
	}
	
	/**
	 * Retrieves a collection of all original skills that have been replaced.
	 * @return The collection of all replaced skill IDs.
	 */
	public ICollection<int> getReplacedSkills()
	{
		return _replacedSkills.Keys;
	}
	
	public abstract void doPickupItem(WorldObject @object);
	
	public abstract bool useMagic(Skill skill, Item item, bool forceUse, bool dontMove);
	
	public abstract void storeMe();
	
	public abstract void storeEffect(bool storeEffects);
	
	public abstract void restoreEffects();
	
	public virtual bool isOnEvent()
	{
		return false;
	}
	
	public override bool isPlayable()
	{
		return true;
	}
}