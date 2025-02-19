using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author dims
 */
public class TriggerSkillByBaseStat: AbstractEffect
{
	private readonly BaseStat _baseStat;
	private readonly int _skillId;
	private readonly int _skillLevel;
	private readonly int _skillSubLevel;
	private readonly int _min;
	private readonly int _max;

	public TriggerSkillByBaseStat(StatSet @params)
	{
		_baseStat = @params.getEnum<BaseStat>("baseStat");
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
			int currentValue;
			switch (_baseStat)
			{
				case BaseStat.STR:
				{
					currentValue = target.getSTR();
					break;
				}
				case BaseStat.INT:
				{
					currentValue = target.getINT();
					break;
				}
				case BaseStat.DEX:
				{
					currentValue = target.getDEX();
					break;
				}
				case BaseStat.WIT:
				{
					currentValue = target.getWIT();
					break;
				}
				case BaseStat.CON:
				{
					currentValue = target.getCON();
					break;
				}
				case BaseStat.MEN:
				{
					currentValue = target.getMEN();
					break;
				}
				default:
				{
					currentValue = 0;
					break;
				}
			}

			// Synchronized because the same skill could be used twice and isAffectedBySkill ignored.
            Skill? triggerSkill = SkillData.getInstance().getSkill(_skillId, _skillLevel, _skillSubLevel);
            if (triggerSkill == null)
                return;

			lock (target)
			{
				if (currentValue >= _min && currentValue <= _max)
				{
					if (!target.isAffectedBySkill(_skillId))
					{
						SkillCaster.triggerCast(target, target, triggerSkill);
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