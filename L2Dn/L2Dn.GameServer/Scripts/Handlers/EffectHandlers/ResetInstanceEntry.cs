using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class ResetInstanceEntry: AbstractEffect
{
	private readonly Set<int> _instanceId;
	
	public ResetInstanceEntry(StatSet @params)
	{
		String instanceIds = @params.getString("instanceId", null);
		if ((instanceIds != null) && !instanceIds.isEmpty())
		{
			_instanceId = new();
			foreach (String id in instanceIds.Split(";"))
			{
				_instanceId.add(int.Parse(id));
			}
		}
		else
		{
			_instanceId = new();
		}
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		foreach (int instanceId in _instanceId)
		{
			InstanceManager.getInstance().deleteInstanceTime(effector.getActingPlayer(), instanceId);
		}
	}
}