using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Geometry;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Headquarter Create effect implementation.
/// </summary>
[HandlerName("HeadquarterCreate")]
public sealed class HeadquarterCreate: AbstractEffect
{
    private const int _hqNpcId = 35062;
    private readonly bool _isAdvanced;

    public HeadquarterCreate(EffectParameterSet parameters)
    {
        _isAdvanced = parameters.GetBoolean(XmlSkillEffectParameterType.IsAdvanced, false);
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effector.getActingPlayer();
        Clan? clan = player?.getClan();
        if (player == null || clan == null || clan.getLeaderId() != player.ObjectId)
            return;

        NpcTemplate? npcTemplate = NpcData.getInstance().getTemplate(_hqNpcId);
        if (npcTemplate is null)
            return;

        SiegeFlag flag = new SiegeFlag(player, npcTemplate, _isAdvanced);
        flag.setTitle(clan.getName());
        flag.setCurrentHpMp(flag.getMaxHp(), flag.getMaxMp());
        flag.setHeading(player.getHeading());
        flag.spawnMe(new Location3D(player.getX(), player.getY(), player.getZ() + 50));
    }

    public override int GetHashCode() => HashCode.Combine(_isAdvanced);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => x._isAdvanced);
}