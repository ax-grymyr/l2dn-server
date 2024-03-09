using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Creatures.Players;
using L2Dn.GameServer.Model.Events.Listeners;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Mobius
 */
public class GiveItemByExp: AbstractEffect
{
	private static readonly Map<Player, long> PLAYER_VALUES = new();
	
	private readonly long _exp;
	private readonly int _itemId;
	
	public GiveItemByExp(StatSet @params)
	{
		_exp = @params.getLong("exp", 0);
		_itemId = @params.getInt("itemId", 0);
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (effected.isPlayer())
		{
			effected.addListener(new ConsumerEventListener(effected, EventType.ON_PLAYABLE_EXP_CHANGED,
				@event =>
				{
					OnPlayableExpChanged onPlayableExpChanged = (OnPlayableExpChanged)@event;
					onExperienceReceived(onPlayableExpChanged.getPlayable(),
						onPlayableExpChanged.getNewExp() - onPlayableExpChanged.getOldExp());
				}, this));
		}
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		if (effected.isPlayer())
		{
			PLAYER_VALUES.remove(effected.getActingPlayer());
			effected.removeListenerIf(EventType.ON_PLAYABLE_EXP_CHANGED, listener => listener.getOwner() == this);
		}
	}
	
	private void onExperienceReceived(Playable playable, long exp)
	{
		if (exp < 1)
		{
			return;
		}
		
		Player player = playable.getActingPlayer();
		long sum = PLAYER_VALUES.getOrDefault(player, 0L) + exp;
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
}