using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Item Effect: Increase/decrease PK count permanently.
 * @author Nik
 */
public class PkCount: AbstractEffect
{
	private readonly int _amount;

	public PkCount(StatSet @params)
	{
		_amount = @params.getInt("amount", 0);
	}

	public override bool isInstant()
	{
		return true;
	}

	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		Player? player = effected.getActingPlayer();
		if (player == null)
		{
			return;
		}

		if (player.getPkKills() > 0)
		{
			int newPkCount = Math.Max(player.getPkKills() + _amount, 0);
			player.setPkKills(newPkCount);
			player.updateUserInfo();
		}
	}
}