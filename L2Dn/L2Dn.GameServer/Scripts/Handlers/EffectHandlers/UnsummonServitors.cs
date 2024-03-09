using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

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
			effector.getServitors().values().forEach(servitor =>
			{
				servitor.abortAttack();
				servitor.abortCast();
				servitor.stopAllEffects();
				
				servitor.unSummon(effector.getActingPlayer());
			});
		}
	}
}