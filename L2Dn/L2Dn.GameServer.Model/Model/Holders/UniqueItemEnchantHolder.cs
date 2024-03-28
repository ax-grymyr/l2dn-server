using L2Dn.GameServer.Model.Interfaces;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Index, Mobius
 */
public class UniqueItemEnchantHolder: ItemEnchantHolder, IUniqueId
{
	private readonly int _objectId;

	public UniqueItemEnchantHolder(int id, int objectId): this(id, objectId, 1)
	{
	}

	public UniqueItemEnchantHolder(int id, int objectId, long count): base(id, count)
	{
		_objectId = objectId;
	}

	public UniqueItemEnchantHolder(ItemEnchantHolder itemHolder, int objectId): base(itemHolder.getId(),
		itemHolder.getCount(), itemHolder.getEnchantLevel())
	{
		_objectId = objectId;
	}

	public int getObjectId()
	{
		return _objectId;
	}

	public String toString()
	{
		return "[" + GetType().Name + "] ID: " + getId() + ", object ID: " + _objectId + ", count: " +
		       getCount() + ", enchant level: " + getEnchantLevel();
	}
}