using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * Give SP effect implementation.
 * @author Adry_85
 */
public class GiveSp: AbstractEffect
{
	private readonly int _sp;
	
	public GiveSp(StatSet @params)
	{
		_sp = @params.getInt("sp", 0);
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
		
		effector.getActingPlayer().addExpAndSp(0, _sp);
	}
}