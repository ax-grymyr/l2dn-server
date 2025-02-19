using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Teleport to previously set Player teleport location.
 * @author Mobius
 */
public class TeleportToTeleportLocation: AbstractEffect
{
	public TeleportToTeleportLocation(StatSet @params)
	{
	}

	public override bool isInstant()
	{
		return true;
	}

	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		Player? player = effector.getActingPlayer();
		if (player == null)
		{
			return;
		}

		Location? location = player.getTeleportLocation();
		if (location != null)
		{
			player.teleToLocation(location.Value);
			player.setTeleportLocation(null);
		}
	}
}