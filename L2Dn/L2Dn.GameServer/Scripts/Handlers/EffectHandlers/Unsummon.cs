using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Unsummon effect implementation.
 * @author Adry_85
 */
public class Unsummon: AbstractEffect
{
	private readonly int _chance;
	
	public Unsummon(StatSet @params)
	{
		_chance = @params.getInt("chance", -1);
	}
	
	public override bool calcSuccess(Creature effector, Creature effected, Skill skill)
	{
		if (_chance < 0)
		{
			return true;
		}
		
		int magicLevel = skill.getMagicLevel();
		if ((magicLevel <= 0) || ((effected.getLevel() - 9) <= magicLevel))
		{
			double chance = _chance * Formulas.calcAttributeBonus(effector, effected, skill) * Formulas.calcGeneralTraitBonus(effector, effected, skill.getTraitType(), false);
			if ((chance >= 100) || (chance > (Rnd.nextDouble() * 100)))
			{
				return true;
			}
		}
		
		return false;
	}
	
	public override bool canStart(Creature effector, Creature effected, Skill skill)
	{
		return effected.isSummon();
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (effected.isServitor())
		{
			L2Dn.GameServer.Model.Actor.Summon servitor = (L2Dn.GameServer.Model.Actor.Summon) effected;
			Player summonOwner = servitor.getOwner();
			
			servitor.abortAttack();
			servitor.abortCast();
			servitor.stopAllEffects();
			
			servitor.unSummon(summonOwner);
			summonOwner.sendPacket(SystemMessageId.YOUR_SERVITOR_HAS_VANISHED_YOU_LL_NEED_TO_SUMMON_A_NEW_ONE);
		}
	}
}