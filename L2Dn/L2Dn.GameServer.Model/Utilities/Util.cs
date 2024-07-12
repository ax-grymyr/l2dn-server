using System.Globalization;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Tasks.PlayerTasks;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Network;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Utilities;

/**
 * General Utility functions related to game server.
 */
public class Util
{
	private static readonly NumberFormatInfo _adenaFormat;

	static Util()
	{
		_adenaFormat = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
		_adenaFormat.CurrencyGroupSeparator = ",";
		_adenaFormat.CurrencyGroupSizes = [3];
	}

	public static void handleIllegalPlayerAction(Player actor, string message, IllegalActionPunishmentType punishment)
	{
		ThreadPool.schedule(new IllegalPlayerActionTask(actor, message, punishment), 5000);
	}

	/**
	 * @param range
	 * @param obj1
	 * @param obj2
	 * @param includeZAxis
	 * @return {@code true} if the two objects are within specified range between each other, {@code false} otherwise
	 */
	public static bool checkIfInRange(int range, WorldObject obj1, WorldObject obj2, bool includeZAxis)
	{
		if (obj1 == null || obj2 == null || obj1.getInstanceWorld() != obj2.getInstanceWorld())
		{
			return false;
		}

		if (range == -1)
		{
			return true; // not limited
		}

		int radius = 0;
		if (obj1.isCreature())
		{
			radius += ((Creature)obj1).getTemplate().getCollisionRadius();
		}

		if (obj2.isCreature())
		{
			radius += ((Creature)obj2).getTemplate().getCollisionRadius();
		}

		double distance = includeZAxis ? obj1.Distance3D(obj2) : obj1.Distance2D(obj2);
		return distance <= range + radius;
	}

	/**
	 * Checks if object is within short (Sqrt(int.max_value)) radius, not using collisionRadius. Faster calculation than checkIfInRange if distance is short and collisionRadius isn't needed. Not for long distance checks (potential teleports, far away castles etc).
	 * @param range
	 * @param obj1
	 * @param obj2
	 * @param includeZAxis if true, check also Z axis (3-dimensional check), otherwise only 2D
	 * @return {@code true} if objects are within specified range between each other, {@code false} otherwise
	 */
	public static bool checkIfInShortRange(int range, WorldObject obj1, WorldObject obj2, bool includeZAxis)
	{
		if (obj1 == null || obj2 == null)
		{
			return false;
		}

		if (range == -1)
		{
			return true; // not limited
		}

		double distance = includeZAxis ? obj1.Distance3D(obj2) : obj1.Distance2D(obj2);
		return distance <= range;
	}

	/**
	 * Format the specified digit using the digit grouping symbol "," (comma).<br>
	 * For example, 123456789 becomes 123,456,789.
	 * @param amount - the amount of adena
	 * @return the formatted adena amount
	 */
	public static string formatAdena(long amount)
	{
		return amount.ToString(_adenaFormat);
	}

	/**
	 * Helper method to send a community board html to the specified player.<br>
	 * HtmlActionCache will be build with npc origin 0 which means the<br>
	 * links on the html are not bound to a specific npc.
	 * @param player the player
	 * @param html the html content
	 */
	public static void sendCBHtml(Player player, string html)
	{
		sendCBHtml(player, html, 0);
	}

	/**
	 * Helper method to send a community board html to the specified player.<br>
	 * When {@code npcObjId} is greater -1 the HtmlActionCache will be build<br>
	 * with the npcObjId as origin. An origin of 0 means the cached bypasses<br>
	 * are not bound to a specific npc.
	 * @param player the player to send the html content to
	 * @param html the html content
	 * @param npcObjId bypass origin to use
	 */
	public static void sendCBHtml(Player player, string html, int npcObjId)
	{
		sendCBHtml(player, html, null, npcObjId);
	}

	/**
	 * Helper method to send a community board html to the specified player.<br>
	 * HtmlActionCache will be build with npc origin 0 which means the<br>
	 * links on the html are not bound to a specific npc. It also fills a<br>
	 * multiedit field in the send html if fillMultiEdit is not null.
	 * @param player the player
	 * @param html the html content
	 * @param fillMultiEdit text to fill the multiedit field with(may be null)
	 */
	public static void sendCBHtml(Player player, string html, string fillMultiEdit)
	{
		sendCBHtml(player, html, fillMultiEdit, 0);
	}

	/**
	 * Helper method to send a community board html to the specified player.<br>
	 * It fills a multiedit field in the send html if {@code fillMultiEdit}<br>
	 * is not null. When {@code npcObjId} is greater -1 the HtmlActionCache will be build<br>
	 * with the npcObjId as origin. An origin of 0 means the cached bypasses<br>
	 * are not bound to a specific npc.
	 * @param player the player
	 * @param html the html content
	 * @param fillMultiEdit text to fill the multiedit field with(may be null)
	 * @param npcObjId bypass origin to use
	 */
	public static void sendCBHtml(Player player, string html, string fillMultiEdit, int npcObjId)
	{
		GameSession? session = player?.getClient();
		if (session == null || html == null)
		{
			return;
		}

		session.HtmlActionValidator.ClearActions(HtmlActionScope.COMM_BOARD_HTML);

		if (npcObjId > -1)
		{
			session.HtmlActionValidator.BuildActions(HtmlActionScope.COMM_BOARD_HTML, html,
				npcObjId == 0 ? null : npcObjId);
		}

		if (fillMultiEdit != null)
		{
			player.sendPacket(new ShowBoardPacket(html, "1001"));
			fillMultiEditContent(player, fillMultiEdit);
		}
		else if (html.Length < 16250)
		{
			player.sendPacket(new ShowBoardPacket(html, "101"));
			player.sendPacket(new ShowBoardPacket(null, "102"));
			player.sendPacket(new ShowBoardPacket(null, "103"));
		}
		else if (html.Length < 16250 * 2)
		{
			player.sendPacket(new ShowBoardPacket(html.Substring(0, 16250), "101"));
			player.sendPacket(new ShowBoardPacket(html.Substring(16250), "102"));
			player.sendPacket(new ShowBoardPacket(null, "103"));
		}
		else if (html.Length < 16250 * 3)
		{
			player.sendPacket(new ShowBoardPacket(html.Substring(0, 16250), "101"));
			player.sendPacket(new ShowBoardPacket(html.Substring(16250, 16250), "102"));
			player.sendPacket(new ShowBoardPacket(html.Substring(16250 * 2), "103"));
		}
		else
		{
			player.sendPacket(
				new ShowBoardPacket("<html><body><br><center>Error: HTML was too long!</center></body></html>", "101"));
			player.sendPacket(new ShowBoardPacket(null, "102"));
			player.sendPacket(new ShowBoardPacket(null, "103"));
		}
	}

	/**
	 * Fills the community board's multiedit window with text. Must send after sendCBHtml
	 * @param player
	 * @param text
	 */
	public static void fillMultiEditContent(Player player, string text)
	{
		player.sendPacket(new ShowBoardPacket([
			"0", "0", "0", "0", "0", "0", player.getName(),
			player.getObjectId().ToString(), player.getAccountName(), "9", " ", " ",
			text.Replace("<br>", Environment.NewLine), "0", "0", "0", "0",
		]));
	}

	public static bool isInsideRangeOfObjectId(WorldObject obj, int targetObjId, int radius)
	{
		WorldObject? target = World.getInstance().findObject(targetObjId);
		return target != null && obj.Distance3D(target) <= radius;
	}
}