using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Stats;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Zones;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Recover Vitality in Peace Zone effect implementation.
 * @author Mobius
 */
public class RecoverVitalityInPeaceZone: AbstractEffect
{
	private readonly double _amount;
	private readonly int _ticks;
	
	public RecoverVitalityInPeaceZone(StatSet @params)
	{
		_amount = @params.getDouble("amount", 0);
		_ticks = @params.getInt("ticks", 10);
	}
	
	public override int getTicks()
	{
		return _ticks;
	}
	
	public override bool onActionTime(Creature effector, Creature effected, Skill skill, Item item)
	{
		if ((effected == null) //
			|| effected.isDead() //
			|| !effected.isPlayer() //
			|| !effected.isInsideZone(ZoneId.PEACE))
		{
			return false;
		}
		
		double vitality = effected.getActingPlayer().getVitalityPoints();
		vitality += _amount;
		if (vitality >= PlayerStat.MAX_VITALITY_POINTS)
		{
			vitality = PlayerStat.MAX_VITALITY_POINTS;
		}
		effected.getActingPlayer().setVitalityPoints((int) vitality, true);
		
		return skill.isToggle();
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		if ((effected != null) //
			&& effected.isPlayer())
		{
			BuffInfo info = effected.getEffectList().getBuffInfoBySkillId(skill.getId());
			if ((info != null) && !info.isRemoved())
			{
				double vitality = effected.getActingPlayer().getVitalityPoints();
				vitality += _amount * 100;
				if (vitality >= PlayerStat.MAX_VITALITY_POINTS)
				{
					vitality = PlayerStat.MAX_VITALITY_POINTS;
				}
				effected.getActingPlayer().setVitalityPoints((int) vitality, true);
			}
		}
	}
}