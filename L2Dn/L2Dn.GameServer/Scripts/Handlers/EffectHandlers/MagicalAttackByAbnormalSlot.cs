using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Magical Attack By Abnormal Slot effect implementation.
 * @author Sdw
 */
public class MagicalAttackByAbnormalSlot: AbstractEffect
{
	private readonly double _power;
	private readonly Set<AbnormalType> _abnormals;
	
	public MagicalAttackByAbnormalSlot(StatSet @params)
	{
		_power = @params.getDouble("power", 0);
		
		String abnormals = @params.getString("abnormalType", null);
		if ((abnormals != null) && !abnormals.isEmpty())
		{
			_abnormals = new();
			foreach (String slot in abnormals.Split(";"))
			{
				if (Enum.TryParse(slot, out AbnormalType val))
					_abnormals.add(val);
			}
		}
		else
		{
			_abnormals = new();
		}
	}
	
	public override bool calcSuccess(Creature effector, Creature effected, Skill skill)
	{
		return !Formulas.calcSkillEvasion(effector, effected, skill);
	}
	
	public override EffectType getEffectType()
	{
		return EffectType.MAGICAL_ATTACK;
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (effector.isAlikeDead())
		{
			return;
		}
		
		bool hasAbnormalType = false;
		if (!_abnormals.isEmpty())
		{
			foreach (AbnormalType abnormal in _abnormals)
			{
				if (effected.hasAbnormalType(abnormal))
				{
					hasAbnormalType = true;
					break;
				}
			}
		}
		if (!hasAbnormalType)
		{
			return;
		}
		
		if (effected.isPlayer() && effected.getActingPlayer().isFakeDeath() && Config.FAKE_DEATH_DAMAGE_STAND)
		{
			effected.stopFakeDeath(true);
		}
		
		bool sps = skill.useSpiritShot() && effector.isChargedShot(ShotType.SPIRITSHOTS);
		bool bss = skill.useSpiritShot() && effector.isChargedShot(ShotType.BLESSED_SPIRITSHOTS);
		bool mcrit = Formulas.calcCrit(skill.getMagicCriticalRate(), effector, effected, skill);
		double damage = Formulas.calcMagicDam(effector, effected, skill, effector.getMAtk(), _power, effected.getMDef(), sps, bss, mcrit);
		
		effector.doAttack(damage, effected, skill, false, false, mcrit, false);
	}
}