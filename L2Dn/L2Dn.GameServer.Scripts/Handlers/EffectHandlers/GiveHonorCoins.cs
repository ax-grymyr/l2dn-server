using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class GiveHonorCoins: AbstractEffect
{
	private readonly long _amount;

	public GiveHonorCoins(StatSet @params)
	{
		_amount = @params.getLong("amount", 0);
	}

	public override bool isInstant()
	{
		return true;
	}

	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
        Player? player = effector.getActingPlayer();
		if (!effector.isPlayer() || player == null || !effected.isPlayer() || effected.isAlikeDead())
		{
			return;
		}

		player.setHonorCoins(player.getHonorCoins() + _amount);
	}
}