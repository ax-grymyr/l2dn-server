using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Transformation type effect, which disables attack or use of skills.
 * @author Nik, Mobius
 */
public class ChangeBody: AbstractEffect
{
	private readonly Set<TemplateChanceHolder> _transformations = [];

	public ChangeBody(StatSet @params)
	{
		foreach (StatSet item in @params.getList<StatSet>("templates"))
		{
			_transformations.add(new TemplateChanceHolder(item.getInt(".templateId"), item.getInt(".minChance"),
				item.getInt(".maxChance")));
		}
	}

	public override bool canStart(Creature effector, Creature effected, Skill skill)
	{
		return !effected.isDoor();
	}

	public override void onStart(Creature effector, Creature effected, Skill skill, Item? item)
	{
		int chance = Rnd.get(100);
		foreach (TemplateChanceHolder holder in _transformations)
		{
			if (holder.calcChance(chance))
			{
				effected.transform(holder.getTemplateId(), false);
				return;
			}
		}
	}

	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		effected.stopTransformation(false);
	}
}