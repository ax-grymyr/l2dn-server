using System.Runtime.CompilerServices;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers;

/**
 * This class handles all chat handlers
 * @author durgus, UnAfraid
 */
public class ChatHandler: IHandler<IChatHandler, ChatType>
{
	private readonly Map<ChatType, IChatHandler> _datatable = new();

	/**
	 * Singleton constructor
	 */
	protected ChatHandler()
	{
	}

	/**
	 * Register a new chat handler
	 * @param handler
	 */
	public void registerHandler(IChatHandler handler)
	{
		foreach (ChatType type in handler.getChatTypeList())
		{
			_datatable.put(type, handler);
		}
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public void removeHandler(IChatHandler handler)
	{
		foreach (ChatType type in handler.getChatTypeList())
		{
			_datatable.remove(type);
		}
	}

	/**
	 * Get the chat handler for the given chat type
	 * @param chatType
	 * @return
	 */
	public IChatHandler? getHandler(ChatType chatType)
	{
		return _datatable.get(chatType);
	}

	/**
	 * Returns the size
	 * @return
	 */
	public int size()
	{
		return _datatable.Count;
	}

	public static ChatHandler getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly ChatHandler INSTANCE = new ChatHandler();
	}
}