using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Geometry;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Summon Npc effect implementation.
/// </summary>
[HandlerStringKey("SummonNpc")]
public sealed class SummonNpc: AbstractEffect
{
    private readonly int _despawnDelay;
    private readonly int _npcId;
    private readonly int _npcCount;
    private readonly bool _randomOffset;
    private readonly bool _isSummonSpawn;
    private readonly bool _singleInstance; // Only one instance of this NPC is allowed.
    private readonly bool _isAggressive;

    public SummonNpc(EffectParameterSet parameters)
    {
        _despawnDelay = parameters.GetInt32(XmlSkillEffectParameterType.DespawnDelay, 0);
        _npcId = parameters.GetInt32(XmlSkillEffectParameterType.NpcId, 0);
        _npcCount = parameters.GetInt32(XmlSkillEffectParameterType.NpcCount, 1);
        _randomOffset = parameters.GetBoolean(XmlSkillEffectParameterType.RandomOffset, false);
        _isSummonSpawn = parameters.GetBoolean(XmlSkillEffectParameterType.IsSummonSpawn, false);
        _singleInstance = parameters.GetBoolean(XmlSkillEffectParameterType.SingleInstance, false);
        _isAggressive = parameters.GetBoolean(XmlSkillEffectParameterType.Aggressive, true); // Used by Decoy.
    }

    public override EffectTypes EffectTypes => EffectTypes.SUMMON_NPC;

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effected.getActingPlayer();
        if (!effected.isPlayer() || player == null || effected.isAlikeDead() || player.inObserverMode())
            return;

        if (_npcId <= 0 || _npcCount <= 0)
        {
            Logger.Warn(GetType().Name + ": Invalid NPC ID or count skill ID: " + skill.Id);
            return;
        }

        if (player.isMounted())
            return;

        NpcTemplate? npcTemplate = NpcData.getInstance().getTemplate(_npcId);
        if (npcTemplate == null)
        {
            Logger.Warn(GetType().Name + ": Spawn of the nonexisting NPC ID: " + _npcId + ", skill ID:" +
                skill.Id);

            return;
        }

        int x = player.getX();
        int y = player.getY();
        int z = player.getZ();

        if (skill.TargetType == TargetType.GROUND)
        {
            Location3D? wordPosition = player.getActingPlayer().getCurrentSkillWorldPosition();
            if (wordPosition != null)
            {
                x = wordPosition.Value.X;
                y = wordPosition.Value.Y;
                z = wordPosition.Value.Z;
            }
        }
        else
        {
            x = effected.getX();
            y = effected.getY();
            z = effected.getZ();
        }

        if (_randomOffset)
        {
            x += Rnd.nextBoolean() ? Rnd.get(20, 50) : Rnd.get(-50, -20);
            y += Rnd.nextBoolean() ? Rnd.get(20, 50) : Rnd.get(-50, -20);
        }

        // If only single instance is allowed, delete previous NPCs.
        if (_singleInstance)
        {
            foreach (Npc npc in player.getSummonedNpcs())
            {
                if (npc.Id == _npcId)
                    npc.deleteMe();
            }
        }

        switch (npcTemplate.getType())
        {
            case "Decoy":
            {
                Decoy decoy = new Decoy(npcTemplate, player,
                    TimeSpan.FromMilliseconds(_despawnDelay > 0 ? _despawnDelay : 20000), _isAggressive);

                decoy.setCurrentHp(decoy.getMaxHp());
                decoy.setCurrentMp(decoy.getMaxMp());
                decoy.setHeading(player.getHeading());
                decoy.setInstance(player.getInstanceWorld());
                decoy.setSummoner(player);
                decoy.spawnMe(new Location3D(x, y, z));
                break;
            }
            case "EffectPoint":
            {
                EffectPoint effectPoint = new EffectPoint(npcTemplate, player);
                effectPoint.setCurrentHp(effectPoint.getMaxHp());
                effectPoint.setCurrentMp(effectPoint.getMaxMp());
                effectPoint.setInvul(true);
                effectPoint.setSummoner(player);
                effectPoint.setTitle(player.getName());
                effectPoint.spawnMe(new Location3D(x, y, z));
                // First consider NPC template despawn_time parameter.
                long despawnTime = (long)(effectPoint.getParameters().getFloat("despawn_time", 0) * 1000);
                if (despawnTime > 0)
                {
                    effectPoint.scheduleDespawn(TimeSpan.FromMilliseconds(despawnTime));
                }
                else if (_despawnDelay > 0) // Use skill despawnDelay parameter.
                {
                    effectPoint.scheduleDespawn(TimeSpan.FromMilliseconds(_despawnDelay));
                }

                break;
            }
            default:
            {
                Spawn spawn;
                try
                {
                    spawn = new Spawn(npcTemplate);
                }
                catch (Exception e)
                {
                    Logger.Warn(GetType().Name + ": Unable to create spawn. " + e);
                    return;
                }

                spawn.Location = new Location(x, y, z, player.getHeading());
                spawn.stopRespawn();

                Npc? npc = spawn.doSpawn(_isSummonSpawn);
                if (npc == null)
                {
                    Logger.Warn(GetType().Name + ": Unable to spawn NPC. ");
                    return;
                }

                player.addSummonedNpc(npc); // npc.setSummoner(player);
                npc.setName(npcTemplate.getName());
                npc.setTitle(npcTemplate.getName());
                if (_despawnDelay > 0)
                {
                    npc.scheduleDespawn(TimeSpan.FromMilliseconds(_despawnDelay));
                }

                npc.broadcastInfo();
                break;
            }
        }
    }

    public override int GetHashCode() =>
        HashCode.Combine(_despawnDelay, _npcId, _npcCount, _randomOffset, _isSummonSpawn, _singleInstance,
            _isAggressive);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj,
            static x => (x._despawnDelay, x._npcId, x._npcCount, x._randomOffset, x._isSummonSpawn, x._singleInstance,
                x._isAggressive));
}