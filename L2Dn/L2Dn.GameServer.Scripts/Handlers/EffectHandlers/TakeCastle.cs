using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Take Castle effect implementation.
 * @author Adry_85, St3eT
 */
public class TakeCastle: AbstractEffect
{
	private readonly CastleSide _side;

	public TakeCastle(StatSet @params)
	{
		_side = @params.getEnum<CastleSide>("side");
	}

	public override bool isInstant()
	{
		return true;
	}

	public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
	{
		if (!effector.isPlayer())
		    return;

        Clan? clan = effector.getClan();
        if (clan is null)
            return;

		Castle? castle = CastleManager.getInstance().getCastle(effector);
        if (castle is null)
            return;

		castle.engrave(clan, effected, _side);
	}
}