using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * Rebalance HP effect implementation.
 * @author Adry_85, earendil
 */
public class RebalanceHPSummon: AbstractEffect
{
	public RebalanceHPSummon(StatSet @params)
	{
	}
	
	public override EffectType getEffectType()
	{
		return EffectType.REBALANCE_HP;
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (!effector.isPlayer())
		{
			return;
		}
		
		double fullHP = 0;
		double currentHPs = 0;
		
		foreach (L2Dn.GameServer.Model.Actor.Summon summon in effector.getServitors().values())
		{
			if (!summon.isDead() && Util.checkIfInRange(skill.getAffectRange(), effector, summon, true))
			{
				fullHP += summon.getMaxHp();
				currentHPs += summon.getCurrentHp();
			}
		}
		
		fullHP += effector.getMaxHp();
		currentHPs += effector.getCurrentHp();
		
		double percentHP = currentHPs / fullHP;
		double newHP;
		foreach (L2Dn.GameServer.Model.Actor.Summon summon in effector.getServitors().values())
		{
			if (!summon.isDead() && Util.checkIfInRange(skill.getAffectRange(), effector, summon, true))
			{
				newHP = summon.getMaxHp() * percentHP;
				if (newHP > summon.getCurrentHp()) // The target gets healed
				{
					// The heal will be blocked if the current hp passes the limit
					if (summon.getCurrentHp() > summon.getMaxRecoverableHp())
					{
						newHP = summon.getCurrentHp();
					}
					else if (newHP > summon.getMaxRecoverableHp())
					{
						newHP = summon.getMaxRecoverableHp();
					}
				}
				
				summon.setCurrentHp(newHP);
			}
		}
		
		newHP = effector.getMaxHp() * percentHP;
		if (newHP > effector.getCurrentHp()) // The target gets healed
		{
			// The heal will be blocked if the current hp passes the limit
			if (effector.getCurrentHp() > effector.getMaxRecoverableHp())
			{
				newHP = effector.getCurrentHp();
			}
			else if (newHP > effector.getMaxRecoverableHp())
			{
				newHP = effector.getMaxRecoverableHp();
			}
		}
		effector.setCurrentHp(newHP);
	}
}