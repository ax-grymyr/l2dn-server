namespace L2Dn.GameServer.Cache;

/**
 * @author Sahar
 */
public class RelationCache
{
	private readonly long _relation;
	private readonly bool _isAutoAttackable;
	
	public RelationCache(long relation, bool isAutoAttackable)
	{
		_relation = relation;
		_isAutoAttackable = isAutoAttackable;
	}
	
	public long getRelation()
	{
		return _relation;
	}
	
	public bool isAutoAttackable()
	{
		return _isAutoAttackable;
	}
}