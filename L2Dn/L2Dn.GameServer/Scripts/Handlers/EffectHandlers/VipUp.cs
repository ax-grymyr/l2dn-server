using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Gabriel Costa Souza
 */
public class VipUp: AbstractEffect
{
	private readonly long _amount;
	
	public VipUp(StatSet @params)
	{
		_amount = @params.getLong("amount", 0L);
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (effected == null)
		{
			return;
		}
		
		Player player = effected.getActingPlayer();
		if (player == null)
		{
			return;
		}
		
		player.updateVipPoints(_amount);
	}
}