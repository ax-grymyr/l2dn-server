using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model.Events.Impl.Creatures;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Actor.Status;

public class CreatureStatus
{
	protected static Logger LOGGER = LogManager.GetLogger(nameof(CreatureStatus));

	private readonly Creature _creature;

	private double _currentHp; // Current HP of the Creature
	private double _currentMp; // Current MP of the Creature

	/** Array containing all clients that need to be notified about hp/mp updates of the Creature */
	private Set<Creature> _statusListener;

	private ScheduledFuture? _regTask;

	protected int _flagsRegenActive;

	protected const int REGEN_FLAG_CP = 4; // TODO: enum
	private const int REGEN_FLAG_HP = 1;
	private const int REGEN_FLAG_MP = 2;

	public CreatureStatus(Creature creature)
	{
		_creature = creature;
	}

	/**
	 * Add the object to the list of Creature that must be informed of HP/MP updates of this Creature.<br>
	 * <br>
	 * <b><u>Concept</u>:</b><br>
	 * <br>
	 * Each Creature owns a list called <b>_statusListener</b> that contains all Player to inform of HP/MP updates.<br>
	 * Players who must be informed are players that target this Creature.<br>
	 * When a RegenTask is in progress sever just need to go through this list to send Server->Client packet StatusUpdate.<br>
	 * <br>
	 * <b><u>Example of use</u>:</b>
	 * <ul>
	 * <li>Target a PC or NPC</li>
	 * <ul>
	 * @param object Creature to add to the listener
	 */
	public void addStatusListener(Creature obj)
	{
		if (obj == _creature)
		{
			return;
		}

		getStatusListener().add(obj);
	}

	/**
	 * Remove the object from the list of Creature that must be informed of HP/MP updates of this Creature.<br>
	 * <br>
	 * <b><u>Concept</u>:</b><br>
	 * <br>
	 * Each Creature owns a list called <b>_statusListener</b> that contains all Player to inform of HP/MP updates.<br>
	 * Players who must be informed are players that target this Creature.<br>
	 * When a RegenTask is in progress sever just need to go through this list to send Server->Client packet StatusUpdate.<br>
	 * <br>
	 * <b><u>Example of use </u>:</b>
	 * <ul>
	 * <li>Untarget a PC or NPC</li>
	 * </ul>
	 * @param object Creature to add to the listener
	 */
	public void removeStatusListener(Creature @object)
	{
		getStatusListener().remove(@object);
	}

	/**
	 * Return the list of Creature that must be informed of HP/MP updates of this Creature.<br>
	 * <br>
	 * <b><u>Concept</u>:</b><br>
	 * <br>
	 * Each Creature owns a list called <b>_statusListener</b> that contains all Player to inform of HP/MP updates.<br>
	 * Players who must be informed are players that target this Creature.<br>
	 * When a RegenTask is in progress sever just need to go through this list to send Server->Client packet StatusUpdate.
	 * @return The list of Creature to inform or null if empty
	 */
	public Set<Creature> getStatusListener()
	{
		if (_statusListener == null)
		{
			_statusListener = new();
		}
		return _statusListener;
	}

	// place holder, only PcStatus has CP
	public virtual void reduceCp(int value)
	{
	}

	/**
	 * Reduce the current HP of the Creature and launch the doDie Task if necessary.
	 * @param value
	 * @param attacker
	 */
	public virtual void reduceHp(double value, Creature attacker)
	{
		reduceHp(value, attacker, true, false, false);
	}

	public void reduceHp(double value, Creature attacker, bool isHpConsumption)
	{
		reduceHp(value, attacker, true, false, isHpConsumption);
	}

	public virtual void reduceHp(double value, Creature attacker, bool awake, bool isDOT, bool isHPConsumption)
	{
		Creature creature = _creature;
		if (creature.isDead())
		{
			return;
		}

		// invul handling
		if (creature.isHpBlocked() && !(isDOT || isHPConsumption))
		{
			return;
		}

		if (attacker != null)
		{
			Player attackerPlayer = attacker.getActingPlayer();
			if (attackerPlayer != null && attackerPlayer.isGM() && !attackerPlayer.getAccessLevel().canGiveDamage())
			{
				return;
			}
		}

		if (!isDOT && !isHPConsumption)
		{
			if (awake)
			{
				creature.stopEffectsOnDamage();
			}
			if (Formulas.calcStunBreak(creature))
			{
				creature.stopStunning(true);
			}
			if (Formulas.calcRealTargetBreak())
			{
				_creature.getEffectList().stopEffects(AbnormalType.REAL_TARGET);
			}
		}

		if (value > 0)
		{
			setCurrentHp(Math.Max(_currentHp - value, creature.isUndying() ? 1 : 0));
		}

		if (creature.getCurrentHp() < 0.5) // Die
		{
			creature.doDie(attacker);
		}
	}

	public void reduceMp(double value)
	{
		setCurrentMp(Math.Max(_currentMp - value, 0));
	}

	/**
	 * Start the HP/MP/CP Regeneration task.<br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>Calculate the regen task period</li>
	 * <li>Launch the HP/MP/CP Regeneration task with Medium priority</li>
	 * </ul>
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void startHpMpRegeneration()
	{
		if (_regTask == null && !_creature.isDead())
		{
			// Get the Regeneration period
			int period = Formulas.getRegeneratePeriod(_creature);

			// Create the HP/MP/CP Regeneration task
			_regTask = ThreadPool.scheduleAtFixedRate(doRegeneration, period, period);
		}
	}

	/**
	 * Stop the HP/MP/CP Regeneration task.<br>
	 * <br>
	 * <b><u>Actions</u>:</b>
	 * <ul>
	 * <li>Set the RegenActive flag to False</li>
	 * <li>Stop the HP/MP/CP Regeneration task</li>
	 * </ul>
	 */
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void stopHpMpRegeneration()
	{
		if (_regTask != null)
		{
			// Stop the HP/MP/CP Regeneration task
			_regTask.cancel(false);
			_regTask = null;

			// Set the RegenActive flag to false
			_flagsRegenActive = 0;
		}
	}

	// place holder, only PcStatus has CP
	public virtual double getCurrentCp()
	{
		return 0;
	}

	// place holder, only PcStatus has CP
	public virtual void setCurrentCp(double newCp)
	{
	}

	// place holder, only PcStatus has CP
	public virtual void setCurrentCp(double newCp, bool broadcastPacket)
	{
	}

	public double getCurrentHp()
	{
		return _currentHp;
	}

	public void setCurrentHp(double newHp)
	{
		setCurrentHp(newHp, true);
	}

	/**
	 * Sets the current hp of this character.
	 * @param newHp the new hp
	 * @param broadcastPacket if true StatusUpdate packet will be broadcasted.
	 * @return @{code true} if hp was changed, @{code false} otherwise.
	 */
	public virtual bool setCurrentHp(double newHp, bool broadcastPacket)
	{
		// Get the Max HP of the Creature
		int oldHp = (int) _currentHp;
		double maxHp = _creature.getStat().getMaxHp();

		lock (this)
		{
			if (_creature.isDead())
			{
				return false;
			}

			if (newHp >= maxHp)
			{
				// Set the RegenActive flag to false
				_currentHp = maxHp;
				_flagsRegenActive &= ~REGEN_FLAG_HP;

				// Stop the HP/MP/CP Regeneration task
				if (_flagsRegenActive == 0)
				{
					stopHpMpRegeneration();
				}
			}
			else
			{
				// Set the RegenActive flag to true
				_currentHp = newHp;
				_flagsRegenActive |= REGEN_FLAG_HP;

				// Start the HP/MP/CP Regeneration task with Medium priority
				startHpMpRegeneration();
			}
		}

		bool hpWasChanged = oldHp != _currentHp;

		// Send the Server->Client packet StatusUpdate with current HP and MP to all other Player to inform
		if (hpWasChanged)
		{
			if (broadcastPacket)
			{
				_creature.broadcastStatusUpdate();
			}

			if (_creature.Events.HasSubscribers<OnCreatureHpChange>())
			{
				_creature.Events.NotifyAsync(new OnCreatureHpChange(getActiveChar(), oldHp, _currentHp));
			}
		}

		return hpWasChanged;
	}

	public void setCurrentHpMp(double newHp, double newMp)
	{
		bool hpOrMpWasChanged = setCurrentHp(newHp, false);
		hpOrMpWasChanged |= setCurrentMp(newMp, false);
		if (hpOrMpWasChanged)
		{
			_creature.broadcastStatusUpdate();
		}
	}

	public double getCurrentMp()
	{
		return _currentMp;
	}

	public void setCurrentMp(double newMp)
	{
		setCurrentMp(newMp, true);
	}

	/**
	 * Sets the current mp of this character.
	 * @param newMp the new mp
	 * @param broadcastPacket if true StatusUpdate packet will be broadcasted.
	 * @return @{code true} if mp was changed, @{code false} otherwise.
	 */
	public bool setCurrentMp(double newMp, bool broadcastPacket)
	{
		// Get the Max MP of the Creature
		int currentMp = (int) _currentMp;
		int maxMp = _creature.getStat().getMaxMp();

		lock (this)
		{
			if (_creature.isDead())
			{
				return false;
			}

			if (newMp >= maxMp)
			{
				// Set the RegenActive flag to false
				_currentMp = maxMp;
				_flagsRegenActive &= ~REGEN_FLAG_MP;

				// Stop the HP/MP/CP Regeneration task
				if (_flagsRegenActive == 0)
				{
					stopHpMpRegeneration();
				}
			}
			else
			{
				// Set the RegenActive flag to true
				_currentMp = newMp;
				_flagsRegenActive |= REGEN_FLAG_MP;

				// Start the HP/MP/CP Regeneration task with Medium priority
				startHpMpRegeneration();
			}
		}

		bool mpWasChanged = currentMp != _currentMp;

		// Send the Server->Client packet StatusUpdate with current HP and MP to all other Player to inform
		if (mpWasChanged && broadcastPacket)
		{
			_creature.broadcastStatusUpdate();
		}

		return mpWasChanged;
	}

	protected virtual void doRegeneration()
	{
		// Modify the current HP/MP of the Creature and broadcast Server->Client packet StatusUpdate
		if (!_creature.isDead() && (_currentHp < _creature.getMaxRecoverableHp() || _currentMp < _creature.getMaxRecoverableMp()))
		{
			double newHp = _currentHp + _creature.getStat().getValue(Stat.REGENERATE_HP_RATE);
			double newMp = _currentMp + _creature.getStat().getValue(Stat.REGENERATE_MP_RATE);
			setCurrentHpMp(newHp, newMp);
		}
		else
		{
			stopHpMpRegeneration();
		}
	}

	public virtual Creature getActiveChar()
	{
		return _creature;
	}
}