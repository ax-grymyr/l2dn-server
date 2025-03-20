using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Summon Trap effect implementation.
/// </summary>
public sealed class SummonTrap: AbstractEffect
{
    private readonly int _despawnTime;
    private readonly int _npcId;

    public SummonTrap(EffectParameterSet parameters)
    {
        _despawnTime = parameters.GetInt32(XmlSkillEffectParameterType.DespawnTime, 0);
        _npcId = parameters.GetInt32(XmlSkillEffectParameterType.NpcId, 0);
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effected.getActingPlayer();
        if (!effected.isPlayer() || player == null || effected.isAlikeDead() || player.inObserverMode())
            return;

        if (_npcId <= 0)
        {
            Logger.Warn(GetType().Name + ": Invalid NPC ID:" + _npcId + " in skill ID: " + skill.Id);
            return;
        }

        if (player.inObserverMode() || player.isMounted())
        {
            return;
        }

        // Unsummon previous trap
        player.getTrap()?.unSummon();

        NpcTemplate? npcTemplate = NpcData.getInstance().getTemplate(_npcId);
        if (npcTemplate == null)
        {
            Logger.Warn(GetType().Name + ": Spawn of the non-existing Trap ID: " + _npcId + " in skill ID:" +
                skill.Id);

            return;
        }

        Trap trap = new Trap(npcTemplate, player, _despawnTime);
        trap.setCurrentHp(trap.getMaxHp());
        trap.setCurrentMp(trap.getMaxMp());
        trap.setInvul(true);
        trap.setHeading(player.getHeading());
        trap.spawnMe(player.Location.Location3D);
        player.addSummonedNpc(trap); // player.setTrap(trap);
    }

    public override int GetHashCode() => HashCode.Combine(_despawnTime, _npcId);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._despawnTime, x._npcId));
}