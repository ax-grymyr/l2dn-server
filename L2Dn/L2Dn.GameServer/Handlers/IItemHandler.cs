using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;

namespace L2Dn.GameServer.Handlers;

/**
 * Mother class of all Item Handlers.
 */
public interface IItemHandler
{
	/**
	 * Launch task associated to the item.
	 * @param playable the non-NPC character using the item
	 * @param item Item designating the item to use
	 * @param forceUse ctrl hold on item use
	 * @return {@code true} if the item all conditions are met and the item is used, {@code false} otherwise.
	 */
	bool useItem(Playable playable, Item item, bool forceUse);
}