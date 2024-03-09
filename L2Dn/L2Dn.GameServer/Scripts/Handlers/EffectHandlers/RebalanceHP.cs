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
public class RebalanceHP: AbstractEffect
{
	public RebalanceHP(StatSet @params)
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
		Party party = effector.getParty();
		if (party != null)
		{
			foreach (Player member in party.getMembers())
			{
				if (!member.isDead() && Util.checkIfInRange(skill.getAffectRange(), effector, member, true))
				{
					fullHP += member.getMaxHp();
					currentHPs += member.getCurrentHp();
				}
				
				L2Dn.GameServer.Model.Actor.Summon summon = member.getPet();
				if ((summon != null) && (!summon.isDead() && Util.checkIfInRange(skill.getAffectRange(), effector, summon, true)))
				{
					fullHP += summon.getMaxHp();
					currentHPs += summon.getCurrentHp();
				}
				
				foreach (L2Dn.GameServer.Model.Actor.Summon servitors in member.getServitors().values())
				{
					if (!servitors.isDead() && Util.checkIfInRange(skill.getAffectRange(), effector, servitors, true))
					{
						fullHP += servitors.getMaxHp();
						currentHPs += servitors.getCurrentHp();
					}
				}
			}
			
			double percentHP = currentHPs / fullHP;
			foreach (Player member in party.getMembers())
			{
				if (!member.isDead() && Util.checkIfInRange(skill.getAffectRange(), effector, member, true))
				{
					double newHP = member.getMaxHp() * percentHP;
					if (newHP > member.getCurrentHp()) // The target gets healed
					{
						// The heal will be blocked if the current hp passes the limit
						if (member.getCurrentHp() > member.getMaxRecoverableHp())
						{
							newHP = member.getCurrentHp();
						}
						else if (newHP > member.getMaxRecoverableHp())
						{
							newHP = member.getMaxRecoverableHp();
						}
					}
					
					member.setCurrentHp(newHP);
				}
				
				L2Dn.GameServer.Model.Actor.Summon summon = member.getPet();
				if ((summon != null) && (!summon.isDead() && Util.checkIfInRange(skill.getAffectRange(), effector, summon, true)))
				{
					double newHP = summon.getMaxHp() * percentHP;
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
				
				foreach (L2Dn.GameServer.Model.Actor.Summon servitors in member.getServitors().values())
				{
					if (!servitors.isDead() && Util.checkIfInRange(skill.getAffectRange(), effector, servitors, true))
					{
						double newHP = servitors.getMaxHp() * percentHP;
						if (newHP > servitors.getCurrentHp()) // The target gets healed
						{
							// The heal will be blocked if the current hp passes the limit
							if (servitors.getCurrentHp() > servitors.getMaxRecoverableHp())
							{
								newHP = servitors.getCurrentHp();
							}
							else if (newHP > servitors.getMaxRecoverableHp())
							{
								newHP = servitors.getMaxRecoverableHp();
							}
						}
						servitors.setCurrentHp(newHP);
					}
				}
			}
		}
	}
}