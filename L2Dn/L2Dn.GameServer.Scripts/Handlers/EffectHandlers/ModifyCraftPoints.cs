using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author quangnguyen
 */
public class ModifyCraftPoints: AbstractEffect
{
	private readonly int _amount;

	public ModifyCraftPoints(StatSet @params)
	{
		_amount = @params.getInt("amount");
	}

	public override bool isInstant()
	{
		return true;
	}

	public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
	{
		if (effected == null)
		{
			return;
		}

		Player? player = effected.getActingPlayer();
		if (player == null)
		{
			return;
		}

		player.getRandomCraft().addCraftPoints(_amount);
	}
}