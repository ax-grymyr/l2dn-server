using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Geremy
 */
public class GivePetXp: AbstractEffect
{
	private readonly int _xp;
	
	public GivePetXp(StatSet @params)
	{
		_xp = @params.getInt("xp", 0);
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (!effector.hasPet())
		{
			return;
		}
		
		effected.getActingPlayer().getPet().addExpAndSp(_xp, 0);
	}
}