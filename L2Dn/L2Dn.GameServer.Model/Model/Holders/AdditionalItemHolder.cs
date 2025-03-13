using L2Dn.GameServer.Dto;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author UnAfraid
 */
public class AdditionalItemHolder: ItemHolder
{
	private readonly bool _allowed;
	
	public AdditionalItemHolder(int id, bool allowed): base(id, 0)
	{
		_allowed = allowed;
	}
	
	public bool isAllowedToUse()
	{
		return _allowed;
	}
}