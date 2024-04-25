using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Geremy
 */
public class AdditionalPhysicalAttack: PhysicalAttack
{
	private readonly int _chance;
	
	public AdditionalPhysicalAttack(StatSet @params): base(@params)
	{
		_chance = @params.getInt("chance", 100);
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (effector.isPlayer() && effector.hasAbnormalType(AbnormalType.SEAL_OF_DAMAGE))
		{
			return;
		}
		
		if (Rnd.get(100) < _chance)
		{
			return;
		}
		
		base.instant(effector, effected, skill, item);
	}
}