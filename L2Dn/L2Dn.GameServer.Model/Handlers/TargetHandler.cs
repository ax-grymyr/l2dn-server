using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model.Skills.Targets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers;

/**
 * @author UnAfraid
 */
public class TargetHandler: IHandler<ITargetTypeHandler, TargetType>
{
	private readonly Map<TargetType, ITargetTypeHandler> _datatable;
	
	protected TargetHandler()
	{
		_datatable = new();
	}
	
	public void registerHandler(ITargetTypeHandler handler)
	{
		_datatable.put(handler.getTargetType(), handler);
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public void removeHandler(ITargetTypeHandler handler)
	{
		_datatable.remove(handler.getTargetType());
	}
	
	public ITargetTypeHandler getHandler(TargetType targetType)
	{
		return _datatable.get(targetType);
	}
	
	public int size()
	{
		return _datatable.Count;
	}
	
	public static TargetHandler getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly TargetHandler INSTANCE = new TargetHandler();
	}
}