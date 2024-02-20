using L2Dn.GameServer.Model.Items.Instances;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author UnAfraid
 */
public class PetItemHolder
{
	private readonly Item _item;

	public PetItemHolder(Item item)
	{
		_item = item;
	}

	public Item getItem()
	{
		return _item;
	}
}