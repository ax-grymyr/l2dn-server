using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events.Impl.Playables;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.StaticData.Xml.Skills;
using L2Dn.GameServer.Templates;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

[HandlerName("GiveItemByExp")]
public sealed class GiveItemByExp: AbstractEffect
{
    private static readonly Map<Player, long> PLAYER_VALUES = new();

    private readonly long _exp;
    private readonly int _itemId;

    public GiveItemByExp(EffectParameterSet parameters)
    {
        _exp = parameters.GetInt64(XmlSkillEffectParameterType.Exp, 0);
        _itemId = parameters.GetInt32(XmlSkillEffectParameterType.ItemId, 0);
    }

    public override void OnStart(Creature effector, Creature effected, Skill skill, Item? item)
    {
        if (effected.isPlayer())
        {
            Player player = (Player)effected;
            player.Events.Subscribe<OnPlayableExpChanged>(this, onExperienceReceived);
        }
    }

    public override void OnExit(Creature effector, Creature effected, Skill skill)
    {
        Player? player = effected.getActingPlayer();
        if (effected.isPlayer() && player != null)
        {
            PLAYER_VALUES.remove(player);
            player.Events.Unsubscribe<OnPlayableExpChanged>(onExperienceReceived);
        }
    }

    private void onExperienceReceived(OnPlayableExpChanged ev)
    {
        Playable playable = ev.getPlayable();
        long exp = ev.getNewExp() - ev.getOldExp();

        if (exp < 1)
            return;

        Player? player = playable.getActingPlayer();
        if (player == null)
            return;

        long sum = PLAYER_VALUES.GetValueOrDefault(player) + exp;
        if (sum >= _exp)
        {
            PLAYER_VALUES.remove(player);
            player.addItem("GiveItemByExp effect", _itemId, 1, player, true);
        }
        else
        {
            PLAYER_VALUES.put(player, sum);
        }
    }

    public override int GetHashCode() => HashCode.Combine(_exp, _itemId);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._exp, x._itemId));
}