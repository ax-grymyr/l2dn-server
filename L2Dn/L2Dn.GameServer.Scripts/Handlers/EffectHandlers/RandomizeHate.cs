using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Randomize Hate effect implementation.
 */
public class RandomizeHate: AbstractEffect
{
	private readonly int _chance;
	
	public RandomizeHate(StatSet @params)
	{
		_chance = @params.getInt("chance", 100);
	}
	
	public override bool calcSuccess(Creature effector, Creature effected, Skill skill)
	{
		return Formulas.calcProbability(_chance, effector, effected, skill);
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if ((effected == effector) || !effected.isAttackable())
		{
			return;
		}
		
		Attackable effectedMob = (Attackable) effected;
		List<Creature> targetList = new();
		World.getInstance().forEachVisibleObject<Creature>(effected, cha =>
		{
			if ((cha != effectedMob) && (cha != effector))
			{
				// Aggro cannot be transfered to a mob of the same faction.
				if (cha.isAttackable() && ((Attackable) cha).isInMyClan(effectedMob))
				{
					return;
				}
				
				targetList.Add(cha);
			}
		});

		// if there is no target, exit function
		if (targetList.Count == 0)
		{
			return;
		}
		
		// Choosing randomly a new target
		Creature target = targetList.GetRandomElement();
		long hate = effectedMob.getHating(effector);
		effectedMob.stopHating(effector);
		effectedMob.addDamageHate(target, 0, hate);
	}
}