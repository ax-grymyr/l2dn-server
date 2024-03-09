using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model.Skills.Targets;
using L2Dn.GameServer.Scripts.Handlers.TargetHandlers.AffectScopes;
using L2Dn.GameServer.Utilities;
using Range = System.Range;
using Single = System.Single;

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
		
		registerHandler(new BalakasScope());
		registerHandler(new DeadParty());
		registerHandler(new DeadPartyPledge());
		registerHandler(new DeadPledge());
		registerHandler(new DeadUnion());
		registerHandler(new Fan());
		registerHandler(new FanPB());
		registerHandler(new Party());
		registerHandler(new PartyPledge());
		registerHandler(new Pledge());
		registerHandler(new PointBlank());
		registerHandler(new Scripts.Handlers.TargetHandlers.AffectScopes.Range());
		registerHandler(new RangeSortByHp());
		registerHandler(new RingRange());
		registerHandler(new Scripts.Handlers.TargetHandlers.AffectScopes.Single());
		registerHandler(new Square());
		registerHandler(new SquarePB());
		registerHandler(new StaticObjectScope());
		registerHandler(new SummonExceptMaster());
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
		return _datatable.size();
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