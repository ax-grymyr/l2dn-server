using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionSiegeZone.
 * @author Gigiikun
 */
public class ConditionSiegeZone : Condition
{
	// conditional values
	public const int COND_NOT_ZONE = 0x0001;
	public const int COND_CAST_ATTACK = 0x0002;
	public const int COND_CAST_DEFEND = 0x0004;
	public const int COND_CAST_NEUTRAL = 0x0008;
	public const int COND_FORT_ATTACK = 0x0010;
	public const int COND_FORT_DEFEND = 0x0020;
	public const int COND_FORT_NEUTRAL = 0x0040;
	
	private readonly int _value;
	private readonly bool _self;
	
	/**
	 * Instantiates a new condition siege zone.
	 * @param value the value
	 * @param self the self
	 */
	public ConditionSiegeZone(int value, bool self)
	{
		_value = value;
		_self = self;
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		Creature target = _self ? effector : effected;
		Castle castle = CastleManager.getInstance().getCastle(target);
		Fort fort = FortManager.getInstance().getFort(target);
		if ((castle == null) && (fort == null))
		{
			return (_value & COND_NOT_ZONE) != 0;
		}
		if (castle != null)
		{
			return checkIfOk(target, castle, _value);
		}
		return checkIfOk(target, fort, _value);
	}
	
	/**
	 * Check if ok.
	 * @param creature the creature
	 * @param castle the castle
	 * @param value the value
	 * @return true, if successful
	 */
	public static bool checkIfOk(Creature creature, Castle castle, int value)
	{
		if ((creature == null) || !creature.isPlayer())
		{
			return false;
		}
		
		Player player = (Player) creature;
		if (((castle == null) || (castle.getResidenceId() <= 0)))
		{
			if ((value & COND_NOT_ZONE) != 0)
			{
				return true;
			}
		}
		else if (!castle.getZone().isActive())
		{
			if ((value & COND_NOT_ZONE) != 0)
			{
				return true;
			}
		}
		else if (((value & COND_CAST_ATTACK) != 0) && player.isRegisteredOnThisSiegeField(castle.getResidenceId()) && (player.getSiegeState() == 1))
		{
			return true;
		}
		else if (((value & COND_CAST_DEFEND) != 0) && player.isRegisteredOnThisSiegeField(castle.getResidenceId()) && (player.getSiegeState() == 2))
		{
			return true;
		}
		else if (((value & COND_CAST_NEUTRAL) != 0) && (player.getSiegeState() == 0))
		{
			return true;
		}
		
		return false;
	}
	
	/**
	 * Check if ok.
	 * @param creature the creature
	 * @param fort the fort
	 * @param value the value
	 * @return true, if successful
	 */
	public static bool checkIfOk(Creature creature, Fort fort, int value)
	{
		if ((creature == null) || !creature.isPlayer())
		{
			return false;
		}
		
		Player player = (Player) creature;
		if (((fort == null) || (fort.getResidenceId() <= 0)))
		{
			if ((value & COND_NOT_ZONE) != 0)
			{
				return true;
			}
		}
		else if (!fort.getZone().isActive())
		{
			if ((value & COND_NOT_ZONE) != 0)
			{
				return true;
			}
		}
		else if (((value & COND_FORT_ATTACK) != 0) && player.isRegisteredOnThisSiegeField(fort.getResidenceId()) && (player.getSiegeState() == 1))
		{
			return true;
		}
		else if (((value & COND_FORT_DEFEND) != 0) && player.isRegisteredOnThisSiegeField(fort.getResidenceId()) && (player.getSiegeState() == 2))
		{
			return true;
		}
		else if (((value & COND_FORT_NEUTRAL) != 0) && (player.getSiegeState() == 0))
		{
			return true;
		}
		
		return false;
	}
}
