using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class TriggerSkillByStat: AbstractEffect
{
	private readonly Stat _stat;
	private readonly int _skillId;
	private readonly int _skillLevel;
	private readonly int _skillSubLevel;
	private readonly int _min;
	private readonly int _max;
	
	public TriggerSkillByStat(StatSet @params)
	{
		_stat = @params.getEnum<Stat>("stat");
		_skillId = @params.getInt("skillId", 0);
		_skillLevel = @params.getInt("skillLevel", 1);
		_skillSubLevel = @params.getInt("skillSubLevel", 0);
		_min = @params.getInt("min", 0);
		_max = @params.getInt("max", 9999);
	}
	
	public override void pump(Creature effected, Skill skill)
	{
		Creature target = effected;
		
		// In some cases, without ThreadPool, values did not apply.
		ThreadPool.schedule(() =>
		{
			int currentValue = (int) effected.getStat().getValue(_stat);
			
			// Synchronized because the same skill could be used twice and isAffectedBySkill ignored.
			lock (target)
			{
				if ((currentValue >= _min) && (currentValue <= _max))
				{
					if (!target.isAffectedBySkill(_skillId))
					{
						SkillCaster.triggerCast(target, target, SkillData.getInstance().getSkill(_skillId, _skillLevel, _skillSubLevel));
					}
				}
				else
				{
					target.getEffectList().stopSkillEffects(SkillFinishType.REMOVED, _skillId);
				}
			}
		}, 100);
	}
}