using System.Globalization;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Attack Trait effect implementation.
 * @author NosBit
 */
public class AttackTrait: AbstractEffect
{
	private readonly Map<TraitType, float> _attackTraits = new();

	public AttackTrait(StatSet @params)
	{
		if (@params.isEmpty())
		{
			LOGGER.Warn(GetType().Name + ": this effect must have parameters!");
			return;
		}

		foreach (var param in @params.getSet())
		{
			_attackTraits.put(Enum.Parse<TraitType>(param.Key),
				float.Parse(param.Value?.ToString() ?? string.Empty, CultureInfo.InvariantCulture) / 100);
		}
	}

	public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
	{
		foreach (var trait in _attackTraits)
		{
			effected.getStat().mergeAttackTrait(trait.Key, trait.Value);
		}
	}

	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		foreach (var trait in _attackTraits)
		{
			effected.getStat().removeAttackTrait(trait.Key, trait.Value);
		}
	}
}