using System.Collections.Frozen;
using L2Dn.Extensions;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[AbstractEffectName("ResetInstanceEntry")]
public sealed class ResetInstanceEntry: AbstractEffect
{
    private readonly FrozenSet<int> _instanceIds;

    public ResetInstanceEntry(EffectParameterSet parameters)
    {
        string instanceIds = parameters.GetString(XmlSkillEffectParameterType.InstanceId, string.Empty);
        _instanceIds = ParseUtil.ParseSet<int>(instanceIds);
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effector.getActingPlayer();
        if (player == null)
            return;

        foreach (int instanceId in _instanceIds)
        {
            InstanceManager.getInstance().deleteInstanceTime(player, instanceId);
            string? instanceName = InstanceManager.getInstance().getInstanceName(instanceId);
            effector.sendMessage(instanceName + " entry has been reset.");
        }
    }

    public override int GetHashCode() => _instanceIds.GetSetHashCode();
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._instanceIds.GetSetComparable());
}