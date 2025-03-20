using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Geometry;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class SummonHallucination: AbstractEffect
{
    private readonly int _despawnDelay;
    private readonly int _npcId;
    private readonly int _npcCount;

    public SummonHallucination(StatSet @params)
    {
        _despawnDelay = @params.getInt("despawnDelay", 20000);
        _npcId = @params.getInt("npcId", 0);
        _npcCount = @params.getInt("npcCount", 1);
    }

    public override EffectTypes EffectType => EffectTypes.SUMMON_NPC;

    public override bool IsInstant => true;

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected.isAlikeDead())
            return;

        if (_npcId <= 0 || _npcCount <= 0)
        {
            Logger.Warn(GetType().Name + ": Invalid NPC ID or count skill ID: " + skill.Id);
            return;
        }

        Player? player = effector.getActingPlayer();
        if (player == null)
            return;

        if (player.isMounted())
            return;

        NpcTemplate? npcTemplate = NpcData.getInstance().getTemplate(_npcId);
        if (npcTemplate == null)
        {
            Logger.Warn(GetType().Name + ": Spawn of the nonexisting NPC ID: " + _npcId + ", skill ID:" +
                skill.Id);

            return;
        }

        int x = effected.getX();
        int y = effected.getY();
        int z = effected.getZ();

        for (int i = 0; i < _npcCount; i++)
        {
            x += Rnd.nextBoolean() ? Rnd.get(0, 20) : Rnd.get(-20, 0);
            y += Rnd.nextBoolean() ? Rnd.get(0, 20) : Rnd.get(-20, 0);

            Doppelganger clone = new Doppelganger(npcTemplate, player);
            clone.setCurrentHp(clone.getMaxHp());
            clone.setCurrentMp(clone.getMaxMp());
            clone.setSummoner(player);
            clone.spawnMe(new Location3D(x, y, z));
            clone.scheduleDespawn(TimeSpan.FromMilliseconds(_despawnDelay));
            clone.startAttackTask(effected);
        }
    }

    public override int GetHashCode() => HashCode.Combine(_despawnDelay, _npcId, _npcCount);

    public override bool Equals(object? obj) =>
        this.EqualsTo(obj, static x => (x._despawnDelay, x._npcId, x._npcCount));
}