using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Double Casting effect implementation.
 * @author Nik
 */
public class DoubleCast: AbstractEffect
{
	private static readonly SkillHolder[] TOGGLE_SKILLS =
	[
		new SkillHolder(11007, 1),
		new SkillHolder(11009, 1),
		new SkillHolder(11008, 1),
		new SkillHolder(11010, 1)
	];

	private readonly Map<int, List<SkillHolder>> _addedToggles;

	public DoubleCast(StatSet @params)
	{
		_addedToggles = new();
	}

	public override long getEffectFlags()
	{
		return EffectFlag.DOUBLE_CAST.getMask();
	}

	public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
	{
		if (effected.isPlayer())
		{
			foreach (SkillHolder holder in TOGGLE_SKILLS)
			{
				Skill s = holder.getSkill();
				if (s != null && !effected.isAffectedBySkill(holder))
				{
					_addedToggles.GetOrAdd(effected.ObjectId, _ => []).Add(holder);
					s.applyEffects(effected, effected);
				}
			}
		}

		base.onStart(effector, effected, skill, item);
	}

	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		if (effected.isPlayer())
		{
			_addedToggles.computeIfPresent(effected.ObjectId, (_, v) =>
			{
				v.ForEach(h => effected.stopSkillEffects(h.getSkill()));
				return (object?)null;
			});
		}
	}
}