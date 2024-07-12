using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model.Skills.Targets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers;

/**
 * @author Nik
 */
public class AffectScopeHandler: IHandler<IAffectScopeHandler, AffectScope>
{
	private readonly Map<AffectScope, IAffectScopeHandler> _datatable;
	
	protected AffectScopeHandler()
	{
		_datatable = new();
	}
	
	public void registerHandler(IAffectScopeHandler handler)
	{
		_datatable.put(handler.getAffectScopeType(), handler);
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public void removeHandler(IAffectScopeHandler handler)
	{
		_datatable.remove(handler.getAffectScopeType());
	}
	
	public IAffectScopeHandler getHandler(AffectScope affectScope)
	{
		return _datatable.get(affectScope);
	}
	
	public int size()
	{
		return _datatable.Count;
	}
	
	public static AffectScopeHandler getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly AffectScopeHandler INSTANCE = new AffectScopeHandler();
	}
}