using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Unsummon my servitors effect implementation.
 * @author Nik
 */
public class UnsummonServitors: AbstractEffect
{
	public UnsummonServitors(StatSet @params)
	{
	}

	public override bool isInstant()
	{
		return true;
	}

	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (effector.hasServitors())
		{
			effector.getServitors().Values.ForEach(servitor =>
			{
				servitor.abortAttack();
				servitor.abortCast();
				servitor.stopAllEffects();

				servitor.unSummon(effector.getActingPlayer());
			});
		}
	}
}