using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Cubics;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Summon Cubic effect implementation.
/// </summary>
[HandlerStringKey("SummonCubic")]
public sealed class SummonCubic: AbstractEffect
{
    private readonly int _cubicId;
    private readonly int _cubicLvl;

    public SummonCubic(EffectParameterSet parameters)
    {
        _cubicId = parameters.GetInt32(XmlSkillEffectParameterType.CubicId, -1);
        _cubicLvl = parameters.GetInt32(XmlSkillEffectParameterType.CubicLvl, 0);
    }

    public int getCubicId() => _cubicId;

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effected.getActingPlayer();
        if (!effected.isPlayer() || player == null || effected.isAlikeDead() || player.inObserverMode())
            return;

        if (_cubicId < 0)
        {
            Logger.Warn(GetType().Name + ": Invalid Cubic ID:" + _cubicId + " in skill ID: " + skill.Id);
            return;
        }

        if (player.inObserverMode() || player.isMounted())
            return;

        // If cubic is already present, it's replaced.
        Cubic? cubic = player.getCubicById(_cubicId);
        if (cubic != null)
        {
            if (cubic.getTemplate().getLevel() > _cubicLvl)
            {
                // What do we do in such case?
                return;
            }

            cubic.deactivate();
        }
        else
        {
            // If maximum amount is reached, random cubic is removed.
            // Players with no mastery can have only one cubic.
            double allowedCubicCount = player.getStat().getValue(Stat.MAX_CUBIC, 1);

            // Extra cubics are removed, one by one, randomly.
            int currentCubicCount = player.getCubics().Count;
            if (currentCubicCount >= allowedCubicCount)
            {
                player.getCubics().Values.Skip((int)(currentCubicCount * Rnd.nextDouble())).FirstOrDefault()?.
                    deactivate();
            }
        }

        CubicTemplate? template = CubicData.getInstance().getCubicTemplate(_cubicId, _cubicLvl);
        if (template == null)
        {
            Logger.Warn("Attempting to summon cubic without existing template id: " + _cubicId + " level: " +
                _cubicLvl);

            return;
        }

        // Adding a new cubic.
        player.addCubic(new Cubic(player, effector.getActingPlayer(), template));
        player.sendPacket(new ExUserInfoCubicPacket(player));
        player.broadcastCharInfo();
    }

    public override int GetHashCode() => HashCode.Combine(_cubicId, _cubicLvl);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._cubicId, x._cubicLvl));
}