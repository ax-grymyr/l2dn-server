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
		string instanceIds = @params.getString("instanceId", string.Empty);
		if (!string.IsNullOrEmpty(instanceIds))
		{
			_instanceId = [];
			foreach (string id in instanceIds.Split(";"))
			{
				_instanceId.add(int.Parse(id));
			}
		}
		else
		{
			_instanceId = [];
		}
	}

	public override bool isInstant()
	{
		return true;
	}

	public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effector.getActingPlayer();
        if (player == null)
            return;

		foreach (int instanceId in _instanceId)
		{
			InstanceManager.getInstance().deleteInstanceTime(player, instanceId);
			string? instanceName = InstanceManager.getInstance().getInstanceName(instanceId);
			effector.sendMessage(instanceName + " entry has been reset.");
		}
	}
}