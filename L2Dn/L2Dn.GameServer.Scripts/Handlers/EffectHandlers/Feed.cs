using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.Model.Enums;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[AbstractEffectName("Feed")]
public sealed class Feed: AbstractEffect
{
    private readonly int _normal;
    private readonly int _ride;
    private readonly int _wyvern;

    public Feed(EffectParameterSet parameters)
    {
        _normal = parameters.GetInt32(XmlSkillEffectParameterType.Normal, 0);
        _ride = parameters.GetInt32(XmlSkillEffectParameterType.Ride, 0);
        _wyvern = parameters.GetInt32(XmlSkillEffectParameterType.Wyvern, 0);
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effected.getActingPlayer();
        if (effected.isPet())
        {
            Pet pet = (Pet)effected;
            int feedEffect = (int)pet.getStat().getValue(Stat.FEED_MODIFY, 0);
            pet.setCurrentFed(pet.getCurrentFed() + _normal * Config.Rates.PET_FOOD_RATE + feedEffect * (_normal / 100));
        }
        else if (effected.isPlayer() && player != null)
        {
            if (player.getMountType() == MountType.WYVERN)
                player.setCurrentFeed(player.getCurrentFeed() + _wyvern);
            else
                player.setCurrentFeed(player.getCurrentFeed() + _ride);
        }
    }

    public override int GetHashCode() => HashCode.Combine(_normal, _ride, _wyvern);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._normal, x._ride, x._wyvern));
}