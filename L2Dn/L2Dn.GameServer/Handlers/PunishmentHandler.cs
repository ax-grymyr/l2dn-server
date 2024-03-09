using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model.Punishment;
using L2Dn.GameServer.Scripts.Handlers.PunishmentHandlers;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers;

/**
 * This class manages handlers of punishments.
 * @author UnAfraid
 */
public class PunishmentHandler: IHandler<IPunishmentHandler, PunishmentType>
{
	private readonly Map<PunishmentType, IPunishmentHandler> _handlers = new();
	
	protected PunishmentHandler()
	{
		registerHandler(new BanHandler());
		registerHandler(new ChatBanHandler());
		registerHandler(new JailHandler());
	}
	
	public void registerHandler(IPunishmentHandler handler)
	{
		_handlers.put(handler.getType(), handler);
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public void removeHandler(IPunishmentHandler handler)
	{
		_handlers.remove(handler.getType());
	}
	
	public IPunishmentHandler getHandler(PunishmentType val)
	{
		return _handlers.get(val);
	}
	
	public int size()
	{
		return _handlers.size();
	}
	
	public static PunishmentHandler getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly PunishmentHandler INSTANCE = new PunishmentHandler();
	}
}