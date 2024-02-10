using L2Dn.GameServer.Model.Holders;

namespace L2Dn.GameServer.Model.Items;

/**
 * @author Zoey76
 */
public class PlayerItemTemplate: ItemHolder
{
	private readonly bool _equipped;

	/**
	 * @param set the set containing the values for this object
	 */
	public PlayerItemTemplate(StatSet set): base(set.getInt("id"), set.getInt("count"))
	{
		_equipped = set.getBoolean("equipped", false);
	}

	/**
	 * @return {@code true} if the items is equipped upon character creation, {@code false} otherwise
	 */
	public bool isEquipped()
	{
		return _equipped;
	}
}