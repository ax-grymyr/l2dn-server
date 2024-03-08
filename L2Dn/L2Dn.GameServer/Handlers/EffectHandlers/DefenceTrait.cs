using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * Defence Trait effect implementation.
 * @author NosBit
 */
public class DefenceTrait: AbstractEffect
{
	private readonly Map<TraitType, float> _defenceTraits = new();
	
	public DefenceTrait(StatSet @params)
	{
		if (@params.isEmpty())
		{
			LOGGER.Warn(GetType().Name + ": must have parameters.");
			return;
		}
		
		foreach (var param in @params.getSet())
		{
			_defenceTraits.put(Enum.Parse<TraitType>(param.Key),
				float.Parse(param.Value.ToString() ?? string.Empty) / 100);
		}
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		foreach (var trait in _defenceTraits)
		{
			if (trait.Value < 1.0f)
			{
				effected.getStat().mergeDefenceTrait(trait.Key, trait.Value);
			}
			else
			{
				effected.getStat().mergeInvulnerableTrait(trait.Key);
			}
		}
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		foreach (var trait in _defenceTraits)
		{
			if (trait.Value < 1.0f)
			{
				effected.getStat().removeDefenceTrait(trait.Key, trait.Value);
			}
			else
			{
				effected.getStat().removeInvulnerableTrait(trait.Key);
			}
		}
	}
}
