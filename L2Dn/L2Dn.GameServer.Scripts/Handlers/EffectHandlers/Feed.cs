using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Model.Enums;
using L2Dn.Utilities;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

public sealed class Feed: AbstractEffect
{
    private readonly int _normal;
    private readonly int _ride;
    private readonly int _wyvern;

    public Feed(StatSet @params)
    {
        _normal = @params.getInt("normal", 0);
        _ride = @params.getInt("ride", 0);
        _wyvern = @params.getInt("wyvern", 0);
    }

    public override bool isInstant() => true;

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effected.getActingPlayer();
        if (effected.isPet())
        {
            Pet pet = (Pet)effected;
            int feedEffect = (int)pet.getStat().getValue(Stat.FEED_MODIFY, 0);
            pet.setCurrentFed(pet.getCurrentFed() + _normal * Config.PET_FOOD_RATE + feedEffect * (_normal / 100));
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