using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Give XP effect implementation.
 * @author Mobius
 */
public class GiveXp: AbstractEffect
{
	private readonly long _xp;
	private readonly int _level;
	private readonly double _percentage;
	
	public GiveXp(StatSet @params)
	{
		_xp = @params.getLong("xp", 0);
		_level = @params.getInt("level", 0);
		_percentage = @params.getDouble("percentage", 0);
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (!effector.isPlayer() || !effected.isPlayer() || effected.isAlikeDead())
		{
			return;
		}
		
		Player player = effector.getActingPlayer();
		double amount;
		if (player.getLevel() < _level)
		{
			amount = (_xp / 100.0) * _percentage;
		}
		else
		{
			amount = _xp;
		}
		
		player.addExpAndSp(amount, 0);
	}
}