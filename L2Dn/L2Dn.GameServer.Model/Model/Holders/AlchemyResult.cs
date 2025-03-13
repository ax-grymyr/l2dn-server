using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author UnAfraid
 */
public class AlchemyResult: ItemHolder
{
	private readonly TryMixCubeResultType _type;
	
	public AlchemyResult(int id, long count, TryMixCubeResultType type): base(id, count)
	{
		_type = type;
	}
	
	public TryMixCubeResultType getType()
	{
		return _type;
	}
}