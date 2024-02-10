using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Utilities;
using ThreadPool = System.Threading.ThreadPool;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * A damage zone
 * @author durgus
 */
public class DamageZone : ZoneType
{
	private int _damageHPPerSec;
	private int _damageMPPerSec;
	
	private int _castleId;
	private Castle _castle;
	
	private int _startTask;
	private int _reuseTask;
	protected volatile Future<?> _task;
	
	public DamageZone(int id)
	{
		base(id);
		
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
	
	public void setParameter(String name, String value)
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
		Future<?> task = _task;
		if ((task == null) && ((_damageHPPerSec != 0) || (_damageMPPerSec != 0)))
		{
			Player player = creature.getActingPlayer();
			Castle castle = getCastle();
			if (castle != null) // Castle zone
			{
				if (!(castle.getSiege().isInProgress() && (player != null) && (player.getSiegeState() != 2))) // Siege and no defender
				{
					return;
				}
			}
			
			lock (this)
			{
				task = _task;
				if (task == null)
				{
					_task = task = ThreadPool.scheduleAtFixedRate(new ApplyDamage(), _startTask, _reuseTask);
				}
			}
		}
	}
	
	protected override void onExit(Creature creature)
	{
		if (getCharactersInside().isEmpty() && (_task != null))
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
	
	protected Castle getCastle()
	{
		if ((_castleId > 0) && (_castle == null))
		{
			_castle = CastleManager.getInstance().getCastleById(_castleId);
		}
		return _castle;
	}
	
	private class ApplyDamage: Runnable
	{
		private readonly Castle _castle;
		
		protected ApplyDamage()
		{
			_castle = getCastle();
		}
		
		public void run()
		{
			if (!isEnabled())
			{
				return;
			}
			
			if (getCharactersInside().isEmpty())
			{
				_task.cancel(false);
				_task = null;
				return;
			}
			
			bool siege = false;
			
			if (_castle != null)
			{
				siege = _castle.getSiege().isInProgress();
				// castle zones active only during siege
				if (!siege)
				{
					_task.cancel(false);
					_task = null;
					return;
				}
			}
			
			foreach (Creature character in getCharactersInside())
			{
				if ((character != null) && character.isPlayer() && !character.isDead())
				{
					if (siege)
					{
						// during siege defenders not affected
						Player player = character.getActingPlayer();
						if ((player != null) && player.isInSiege() && (player.getSiegeState() == 2))
						{
							continue;
						}
					}
					
					double multiplier = 1 + (character.getStat().getValue(Stat.DAMAGE_ZONE_VULN, 0) / 100);
					if (getHPDamagePerSecond() != 0)
					{
						character.reduceCurrentHp(getHPDamagePerSecond() * multiplier, character, null);
					}
					if (getMPDamagePerSecond() != 0)
					{
						character.reduceCurrentMp(getMPDamagePerSecond() * multiplier);
					}
				}
			}
		}
	}
}
