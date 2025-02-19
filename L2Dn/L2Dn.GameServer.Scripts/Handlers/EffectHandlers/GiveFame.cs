using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class GiveFame: AbstractEffect
{
	private readonly int _fame;

	public GiveFame(StatSet @params)
	{
		_fame = @params.getInt("fame", 0);
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

		player.setFame(player.getFame() + _fame);
	}
}