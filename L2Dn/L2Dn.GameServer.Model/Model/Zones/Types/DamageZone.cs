using L2Dn.GameServer.Dto.ZoneForms;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * A damage zone
 * @author durgus
 */
public class DamageZone: ZoneType
{
	private int _damageHPPerSec;
	private int _damageMPPerSec;

	private int _castleId;
	private Castle? _castle;

	private int _startTask;
	private int _reuseTask;
	protected volatile ScheduledFuture? _task;

	public DamageZone(int id, ZoneForm form): base(id, form)
	{
		// Setup default damage
		_damageHPPerSec = 200;
		_damageMPPerSec = 0;

		// Setup default start / reuse time
		_startTask = 10;
		_reuseTask = 5000;

		// no castle by default
		_castleId = 0;
		_castle = null;

		setTargetType(InstanceType.Playable); // default only playabale
	}

	public override void setParameter(string name, string value)
	{
		if (name.equals("dmgHPSec"))
		{
			_damageHPPerSec = int.Parse(value);
		}
		else if (name.equals("dmgMPSec"))
		{
			_damageMPPerSec = int.Parse(value);
		}
		else if (name.equals("castleId"))
		{
			_castleId = int.Parse(value);
		}
		else if (name.equalsIgnoreCase("initialDelay"))
		{
			_startTask = int.Parse(value);
		}
		else if (name.equalsIgnoreCase("reuse"))
		{
			_reuseTask = int.Parse(value);
		}
		else
		{
			base.setParameter(name, value);
		}
	}

	protected override void onEnter(Creature creature)
	{
		ScheduledFuture? task = _task;
		if (task == null && (_damageHPPerSec != 0 || _damageMPPerSec != 0))
		{
			Player? player = creature.getActingPlayer();
			Castle? castle = getCastle();
			if (castle != null) // Castle zone
			{
				if (!(castle.getSiege().isInProgress() && player != null && player.getSiegeState() != 2)) // Siege and no defender
				{
					return;
				}
			}

			lock (this)
			{
				task = _task;
				if (task == null)
				{
					_task = task = ThreadPool.scheduleAtFixedRate(new ApplyDamage(this), _startTask, _reuseTask);
				}
			}
		}
	}

	protected override void onExit(Creature creature)
	{
		if (getCharactersInside().Count == 0 && _task != null)
		{
			_task.cancel(true);
			_task = null;
		}
	}

	protected int getHPDamagePerSecond()
	{
		return _damageHPPerSec;
	}

	protected int getMPDamagePerSecond()
	{
		return _damageMPPerSec;
	}

	protected Castle? getCastle()
	{
		if (_castleId > 0 && _castle == null)
		{
			_castle = CastleManager.getInstance().getCastleById(_castleId);
		}
		return _castle;
	}

	private sealed class ApplyDamage(DamageZone damageZone): Runnable
    {
        private readonly Castle? _castle = damageZone.getCastle();

        public void run()
		{
			if (!damageZone.isEnabled())
			{
				return;
			}

			if (damageZone.getCharactersInside().Count == 0)
			{
				damageZone._task?.cancel(false);
				damageZone._task = null;
				return;
			}

			bool siege = false;

			if (_castle != null)
			{
				siege = _castle.getSiege().isInProgress();
				// castle zones active only during siege
				if (!siege)
				{
					damageZone._task?.cancel(false);
					damageZone._task = null;
					return;
				}
			}

			foreach (Creature character in damageZone.getCharactersInside())
			{
				if (character != null && character.isPlayer() && !character.isDead())
				{
					if (siege)
					{
						// during siege defenders not affected
						Player? player = character.getActingPlayer();
						if (player != null && player.isInSiege() && player.getSiegeState() == 2)
						{
							continue;
						}
					}

					double multiplier = 1 + character.getStat().getValue(Stat.DAMAGE_ZONE_VULN, 0) / 100;
					if (damageZone.getHPDamagePerSecond() != 0)
					{
						character.reduceCurrentHp(damageZone.getHPDamagePerSecond() * multiplier, character, null);
					}
					if (damageZone.getMPDamagePerSecond() != 0)
					{
						character.reduceCurrentMp(damageZone.getMPDamagePerSecond() * multiplier);
					}
				}
			}
		}
	}
}