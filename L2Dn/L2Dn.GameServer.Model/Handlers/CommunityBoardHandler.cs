using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Handlers;

/**
 * Community Board handler.
 * @author Zoey76
 */
public class CommunityBoardHandler: IHandler<IParseBoardHandler, string>
{
	private static readonly Logger LOG = LogManager.GetLogger(nameof(CommunityBoardHandler));
	/** The registered handlers. */
	private readonly Map<string, IParseBoardHandler> _datatable = new();
	/** The bypasses used by the players. */
	private readonly Map<int, string> _bypasses = new();

	protected CommunityBoardHandler()
	{
		// Prevent external initialization.
	}

	public void registerHandler(IParseBoardHandler handler)
	{
		foreach (string cmd in handler.getCommunityBoardCommands())
		{
			_datatable.put(cmd.ToLower(), handler);
		}
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public void removeHandler(IParseBoardHandler handler)
	{
		foreach (string cmd in handler.getCommunityBoardCommands())
		{
			_datatable.remove(cmd.ToLower());
		}
	}

	public IParseBoardHandler getHandler(string cmd)
	{
		foreach (IParseBoardHandler cb in _datatable.Values)
		{
			foreach (string command in cb.getCommunityBoardCommands())
			{
				if (cmd.ToLower().startsWith(command.ToLower()))
				{
					return cb;
				}
			}
		}
		return null;
	}

	public int size()
	{
		return _datatable.Count;
	}

	/**
	 * Verifies if the string is a registered community board command.
	 * @param cmd the command to verify
	 * @return {@code true} if the command has been registered, {@code false} otherwise
	 */
	public bool isCommunityBoardCommand(string cmd)
	{
		return getHandler(cmd) != null;
	}

	/**
	 * Parses a community board command.
	 * @param command the command
	 * @param player the player
	 */
	public void handleParseCommand(string command, Player player)
	{
		if (player == null)
		{
			return;
		}

		if (!Config.ENABLE_COMMUNITY_BOARD)
		{
			player.sendPacket(SystemMessageId.THE_COMMUNITY_SERVER_IS_CURRENTLY_OFFLINE);
			return;
		}

		IParseBoardHandler cb = getHandler(command);
		if (cb == null)
		{
			LOG.Warn(nameof(CommunityBoardHandler) + ": Couldn't find parse handler for command " + command + "!");
			return;
		}

		cb.parseCommunityBoardCommand(command, player);
	}

	/**
	 * Writes a command into the client.
	 * @param player the player
	 * @param url the command URL
	 * @param arg1 the first argument
	 * @param arg2 the second argument
	 * @param arg3 the third argument
	 * @param arg4 the fourth argument
	 * @param arg5 the fifth argument
	 */
	public void handleWriteCommand(Player player, string url, string arg1, string arg2, string arg3, string arg4, string arg5)
	{
		if (player == null)
		{
			return;
		}

		if (!Config.ENABLE_COMMUNITY_BOARD)
		{
			player.sendPacket(SystemMessageId.THE_COMMUNITY_SERVER_IS_CURRENTLY_OFFLINE);
			return;
		}

		string cmd = "";
		switch (url)
		{
			case "Topic":
			{
				cmd = "_bbstop";
				break;
			}
			case "Post":
			{
				cmd = "_bbspos"; // TODO: Implement.
				break;
			}
			case "Region":
			{
				cmd = "_bbsloc";
				break;
			}
			case "Notice":
			{
				cmd = "_bbsclan";
				break;
			}
			default:
			{
				separateAndSend("<html><body><br><br><center>The command: " + url + " is not implemented yet.</center><br><br></body></html>", player);
				return;
			}
		}

		IParseBoardHandler cb = getHandler(cmd);
		if (cb == null)
		{
			LOG.Warn(nameof(CommunityBoardHandler) + ": Couldn't find write handler for command " + cmd + "!");
			return;
		}

		if (!(cb is IWriteBoardHandler))
		{
			LOG.Warn(nameof(CommunityBoardHandler) + ": " + cb.GetType().Name + " doesn't implement write!");
			return;
		}
		((IWriteBoardHandler) cb).writeCommunityBoardCommand(player, arg1, arg2, arg3, arg4, arg5);
	}

	/**
	 * Sets the last bypass used by the player.
	 * @param player the player
	 * @param title the title
	 * @param bypass the bypass
	 */
	public void addBypass(Player player, string title, string bypass)
	{
		_bypasses.put(player.ObjectId, title + "&" + bypass);
	}

	/**
	 * Removes the last bypass used by the player.
	 * @param player the player
	 * @return the last bypass used
	 */
	public string? removeBypass(Player player)
	{
		return _bypasses.remove(player.ObjectId);
	}

	/**
	 * Separates and send an HTML into multiple packets, to display into the community board.<br>
	 * The limit is 16383 characters.
	 * @param html the HTML to send
	 * @param player the player
	 */
	public static void separateAndSend(string html, Player player)
	{
		Util.sendCBHtml(player, html);
	}

	public static CommunityBoardHandler getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly CommunityBoardHandler INSTANCE = new CommunityBoardHandler();
	}
}