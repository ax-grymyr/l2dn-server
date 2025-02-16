using L2Dn.GameServer.Model.Interfaces;

namespace L2Dn.GameServer.Model.Holders;

/**
 * A DTO for items; contains item ID, object ID and count.
 * @author xban1x
 */
public class UniqueItemHolder: ItemHolder, IUniqueId
{
	private readonly int _objectId;
	
	public UniqueItemHolder(int id, int objectId): this(id, objectId, 1)
	{
	}
	
	public UniqueItemHolder(int id, int objectId, long count): base(id, count)
	{
		_objectId = objectId;
	}

	public int ObjectId => _objectId;
	
	public override string ToString() =>
		$"[{GetType().Name}] ID: {getId()}, object ID: {_objectId}, count: {getCount()}";
}