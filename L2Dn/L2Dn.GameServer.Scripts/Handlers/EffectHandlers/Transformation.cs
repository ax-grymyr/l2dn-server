using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Transformation effect implementation.
 * @author nBd
 */
public class Transformation: AbstractEffect
{
	private readonly List<int> _id;
	
	public Transformation(StatSet @params)
	{
		String ids = @params.getString("transformationId", null);
		_id = new();
		if ((ids != null) && !ids.isEmpty())
		{
			foreach (String id in ids.Split(";"))
			{
				_id.Add(int.Parse(id));
			}
		}
	}
	
	public override bool canStart(Creature effector, Creature effected, Skill skill)
	{
		return !effected.isDoor();
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (_id.Count != 0)
		{
			effected.transform(_id.GetRandomElement(), true);
		}
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		effected.stopTransformation(false);
	}
}
