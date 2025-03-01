using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;

namespace L2Dn.GameServer.Model.Events.Impl.Items;

/**
 * @author UnAfraid
 */
public class OnPlayerItemAugment: EventBase
{
	private readonly Player _player;
	private readonly Item _item;
	private readonly VariationInstance? _augmentation;
	private readonly bool _isAugment; // true = is being augmented // false = augment is being removed

	public OnPlayerItemAugment(Player player, Item item, VariationInstance? augment, bool isAugment)
	{
		_player = player;
		_item = item;
		_augmentation = augment;
		_isAugment = isAugment;
	}

	public Player getPlayer()
	{
		return _player;
	}

	public Item getItem()
	{
		return _item;
	}

	public VariationInstance? getAugmentation()
	{
		return _augmentation;
	}

	public bool isAugment()
	{
		return _isAugment;
	}
}