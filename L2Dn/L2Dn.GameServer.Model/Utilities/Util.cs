using System.Globalization;
using System.Text;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Tasks.PlayerTasks;
using L2Dn.GameServer.Model.Interfaces;
using L2Dn.GameServer.Network;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Utilities;
using NLog;
using Org.BouncyCastle.Utilities;

namespace L2Dn.GameServer.Utilities;

/**
 * General Utility functions related to game server.
 */
public class Util
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(Util));
	
	private static readonly NumberFormatInfo _adenaFormat;

	static Util()
	{
		_adenaFormat = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
		_adenaFormat.CurrencyGroupSeparator = ",";
		_adenaFormat.CurrencyGroupSizes = [3];
	}
	
	public static void handleIllegalPlayerAction(Player actor, String message, IllegalActionPunishmentType punishment)
	{
		ThreadPool.schedule(new IllegalPlayerActionTask(actor, message, punishment), 5000);
	}
	
	/**
	 * @param from
	 * @param to
	 * @return degree value of object 2 to the horizontal line with object 1 being the origin.
	 */
	public static double calculateAngleFrom(ILocational from, ILocational to)
	{
		return calculateAngleFrom(from.getX(), from.getY(), to.getX(), to.getY());
	}
	
	/**
	 * @param fromX
	 * @param fromY
	 * @param toX
	 * @param toY
	 * @return degree value of object 2 to the horizontal line with object 1 being the origin
	 */
	public static double calculateAngleFrom(int fromX, int fromY, int toX, int toY)
	{
		double angleTarget = double.RadiansToDegrees(Math.Atan2(toY - fromY, toX - fromX));
		if (angleTarget < 0)
		{
			angleTarget += 360;
		}
		return angleTarget;
	}
	
	/**
	 * Gets a random position around the specified location.
	 * @param loc the center location
	 * @param minRange the minimum range from the center to pick a point.
	 * @param maxRange the maximum range from the center to pick a point.
	 * @return a random location between minRange and maxRange of the center location.
	 */
	public static Location getRandomPosition(ILocational loc, int minRange, int maxRange)
	{
		int randomX = Rnd.get(minRange, maxRange);
		int randomY = Rnd.get(minRange, maxRange);
		double rndAngle = double.DegreesToRadians(Rnd.get(360));
		int newX = (int) (loc.getX() + (randomX * Math.Cos(rndAngle)));
		int newY = (int) (loc.getY() + (randomY * Math.Sin(rndAngle)));
		return new Location(newX, newY, loc.getZ());
	}
	
	public static double convertHeadingToDegree(int clientHeading)
	{
		double degree = clientHeading / 182.044444444;
		return degree;
	}
	
	public static int calculateHeadingFrom(ILocational from, ILocational to)
	{
		return calculateHeadingFrom(from.getX(), from.getY(), to.getX(), to.getY());
	}
	
	public static int calculateHeadingFrom(int fromX, int fromY, int toX, int toY)
	{
		double angleTarget = double.RadiansToDegrees(Math.Atan2(toY - fromY, toX - fromX));
		if (angleTarget < 0)
		{
			angleTarget += 360;
		}
		return (int) (angleTarget * 182.044444444);
	}
	
	public static int calculateHeadingFrom(double dx, double dy)
	{
		double angleTarget = double.RadiansToDegrees(Math.Atan2(dy, dx));
		if (angleTarget < 0)
		{
			angleTarget += 360;
		}
		return (int) (angleTarget * 182.044444444);
	}
	
	/**
	 * Calculates distance between one set of x, y, z and another set of x, y, z.
	 * @param x1 - X coordinate of first point.
	 * @param y1 - Y coordinate of first point.
	 * @param z1 - Z coordinate of first point.
	 * @param x2 - X coordinate of second point.
	 * @param y2 - Y coordinate of second point.
	 * @param z2 - Z coordinate of second point.
	 * @param includeZAxis - If set to true, Z coordinates will be included.
	 * @param squared - If set to true, distance returned will be squared.
	 * @return {@code double} - Distance between object and given x, y , z.
	 */
	public static double calculateDistance(double x1, double y1, double z1, double x2, double y2, double z2, bool includeZAxis, bool squared)
	{
		double distance = Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2) + (includeZAxis ? Math.Pow(z1 - z2, 2) : 0);
		return (squared) ? distance : Math.Sqrt(distance);
	}
	
	/**
	 * Calculates distance between 2 locations.
	 * @param loc1 - First location.
	 * @param loc2 - Second location.
	 * @param includeZAxis - If set to true, Z coordinates will be included.
	 * @param squared - If set to true, distance returned will be squared.
	 * @return {@code double} - Distance between object and given location.
	 */
	public static double calculateDistance(ILocational loc1, ILocational loc2, bool includeZAxis, bool squared)
	{
		return calculateDistance(loc1.getX(), loc1.getY(), loc1.getZ(), loc2.getX(), loc2.getY(), loc2.getZ(), includeZAxis, squared);
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
		if ((obj1 == null) || (obj2 == null) || (obj1.getInstanceWorld() != obj2.getInstanceWorld()))
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
			radius += ((Creature) obj1).getTemplate().getCollisionRadius();
		}
		if (obj2.isCreature())
		{
			radius += ((Creature) obj2).getTemplate().getCollisionRadius();
		}
		
		return calculateDistance(obj1, obj2, includeZAxis, false) <= (range + radius);
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
		if ((obj1 == null) || (obj2 == null))
		{
			return false;
		}
		if (range == -1)
		{
			return true; // not limited
		}
		return calculateDistance(obj1, obj2, includeZAxis, false) <= range;
	}
	
	/**
	 * @param text - the text to check
	 * @return {@code true} if {@code text} contains only numbers, {@code false} otherwise
	 */
	public static bool isDigit(String text)
	{
		if ((text == null) || text.isEmpty())
		{
			return false;
		}
		foreach (char c in text)
		{
			if (!char.IsDigit(c))
			{
				return false;
			}
		}
		return true;
	}
	
	/**
	 * @param text - the text to check
	 * @return {@code true} if {@code text} is integer, {@code false} otherwise
	 */
	public static bool isInteger(String text)
	{
		if ((text == null) || text.isEmpty())
		{
			return false;
		}

		return int.TryParse(text, CultureInfo.InvariantCulture, out _);
	}
	
	/**
	 * @param text - the text to check
	 * @return {@code true} if {@code text} is float, {@code false} otherwise
	 */
	public static bool isFloat(String text)
	{
		if ((text == null) || text.isEmpty())
		{
			return false;
		}
		
		return float.TryParse(text, CultureInfo.InvariantCulture, out _);
	}
	
	/**
	 * @param text - the text to check
	 * @return {@code true} if {@code text} is double, {@code false} otherwise
	 */
	public static bool isDouble(String text)
	{
		if ((text == null) || text.isEmpty())
		{
			return false;
		}

		return double.TryParse(text, CultureInfo.InvariantCulture, out _);
	}
	
	/**
	 * @param <T>
	 * @param name - the text to check
	 * @param enumType
	 * @return {@code true} if {@code text} is enum, {@code false} otherwise
	 */
	public static bool isEnum<T>(String name)
		where T: struct, Enum
	{
		if ((name == null) || name.isEmpty())
		{
			return false;
		}

		return Enum.TryParse<T>(name, true, out _);
	}
	
	/**
	 * @param text - the text to check
	 * @return {@code true} if {@code text} contains only letters and/or numbers, {@code false} otherwise
	 */
	public static bool isAlphaNumeric(String text)
	{
		if ((text == null) || text.isEmpty())
		{
			return false;
		}
		foreach (char c in text)
		{
			if (!char.IsLetterOrDigit(c))
			{
				return false;
			}
		}
		return true;
	}
	
	/**
	 * Format the specified digit using the digit grouping symbol "," (comma).<br>
	 * For example, 123456789 becomes 123,456,789.
	 * @param amount - the amount of adena
	 * @return the formatted adena amount
	 */
	public static String formatAdena(long amount)
	{
		return amount.ToString(_adenaFormat);
	}
	
	/**
	 * @param value
	 * @param format
	 * @return formatted double value by specified format.
	 */
	public static String formatDouble(double value, String format)
	{
		return value.ToString();
	}
	
	/**
	 * Format the given date on the given format
	 * @param date : the date to format.
	 * @param format : the format to correct by.
	 * @return a string representation of the formatted date.
	 */
	// public static String formatDate(DateOnly date, String format)
	// {
	// 	return date.ToString(format);
	// }
	
	public static String getDateString(DateOnly date)
	{
		return date.ToString("yyyy-MM-dd");
	}
	
	/**
	 * Helper method to send a community board html to the specified player.<br>
	 * HtmlActionCache will be build with npc origin 0 which means the<br>
	 * links on the html are not bound to a specific npc.
	 * @param player the player
	 * @param html the html content
	 */
	public static void sendCBHtml(Player player, String html)
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
	public static void sendCBHtml(Player player, String html, int npcObjId)
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
	public static void sendCBHtml(Player player, String html, String fillMultiEdit)
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
	public static void sendCBHtml(Player player, String html, String fillMultiEdit, int npcObjId)
	{
		GameSession? session = player?.getClient();
		if ((session == null) || (html == null))
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
		else if (html.Length < (16250 * 2))
		{
			player.sendPacket(new ShowBoardPacket(html.Substring(0, 16250), "101"));
			player.sendPacket(new ShowBoardPacket(html.Substring(16250), "102"));
			player.sendPacket(new ShowBoardPacket(null, "103"));
		}
		else if (html.Length < (16250 * 3))
		{
			player.sendPacket(new ShowBoardPacket(html.Substring(0, 16250), "101"));
			player.sendPacket(new ShowBoardPacket(html.Substring(16250, 16250), "102"));
			player.sendPacket(new ShowBoardPacket(html.Substring(16250 * 2), "103"));
		}
		else
		{
			player.sendPacket(new ShowBoardPacket("<html><body><br><center>Error: HTML was too long!</center></body></html>", "101"));
			player.sendPacket(new ShowBoardPacket(null, "102"));
			player.sendPacket(new ShowBoardPacket(null, "103"));
		}
	}
	
	/**
	 * Fills the community board's multiedit window with text. Must send after sendCBHtml
	 * @param player
	 * @param text
	 */
	public static void fillMultiEditContent(Player player, String text)
	{
		player.sendPacket(new ShowBoardPacket([
			"0", "0", "0", "0", "0", "0", player.getName(),
			player.getObjectId().ToString(), player.getAccountName(), "9", " ", " ",
			text.Replace("<br>", Environment.NewLine), "0", "0", "0", "0"
		]));
	}
	
	public static bool isInsideRangeOfObjectId(WorldObject obj, int targetObjId, int radius)
	{
		WorldObject target = World.getInstance().findObject(targetObjId);
		return (target != null) && (obj.calculateDistance3D(target) <= radius);
	}
	
	public static String readAllLines(string filePath, Encoding encoding)
	{
		return File.ReadAllText(filePath, encoding);
	}
	
	/**
	 * Re-Maps a value from one range to another.
	 * @param input
	 * @param inputMin
	 * @param inputMax
	 * @param outputMin
	 * @param outputMax
	 * @return The mapped value
	 */
	public static int map(int input, int inputMin, int inputMax, int outputMin, int outputMax)
	{
		return (((constrain(input, inputMin, inputMax) - inputMin) * (outputMax - outputMin)) / (inputMax - inputMin)) + outputMin;
	}
	
	/**
	 * Re-Maps a value from one range to another.
	 * @param input
	 * @param inputMin
	 * @param inputMax
	 * @param outputMin
	 * @param outputMax
	 * @return The mapped value
	 */
	public static long map(long input, long inputMin, long inputMax, long outputMin, long outputMax)
	{
		return (((constrain(input, inputMin, inputMax) - inputMin) * (outputMax - outputMin)) / (inputMax - inputMin)) + outputMin;
	}
	
	/**
	 * Re-Maps a value from one range to another.
	 * @param input
	 * @param inputMin
	 * @param inputMax
	 * @param outputMin
	 * @param outputMax
	 * @return The mapped value
	 */
	public static double map(double input, double inputMin, double inputMax, double outputMin, double outputMax)
	{
		return (((constrain(input, inputMin, inputMax) - inputMin) * (outputMax - outputMin)) / (inputMax - inputMin)) + outputMin;
	}
	
	/**
	 * Constrains a number to be within a range.
	 * @param input the number to constrain, all data types
	 * @param min the lower end of the range, all data types
	 * @param max the upper end of the range, all data types
	 * @return input: if input is between min and max, min: if input is less than min, max: if input is greater than max
	 */
	public static int constrain(int input, int min, int max)
	{
		return (input < min) ? min : (input > max) ? max : input;
	}
	
	/**
	 * Constrains a number to be within a range.
	 * @param input the number to constrain, all data types
	 * @param min the lower end of the range, all data types
	 * @param max the upper end of the range, all data types
	 * @return input: if input is between min and max, min: if input is less than min, max: if input is greater than max
	 */
	public static long constrain(long input, long min, long max)
	{
		return (input < min) ? min : (input > max) ? max : input;
	}
	
	/**
	 * Constrains a number to be within a range.
	 * @param input the number to constrain, all data types
	 * @param min the lower end of the range, all data types
	 * @param max the upper end of the range, all data types
	 * @return input: if input is between min and max, min: if input is less than min, max: if input is greater than max
	 */
	public static double constrain(double input, double min, double max)
	{
		return (input < min) ? min : (input > max) ? max : input;
	}
	
	/**
	 * This will sort a Map according to the values. Default sort direction is ascending.
	 * @param <K> keyType
	 * @param <V> valueType
	 * @param map Map to be sorted.
	 * @param descending If you want to sort descending.
	 * @return A new Map sorted by the values.
	 */
	// public static <K, V extends Comparable<? super V>> Map<,> K, V> sortByValue(Map<K, V> map, bool descending)
	// {
	// 	if (descending)
	// 	{
	// 		return map.entrySet().stream().sorted(Entry.comparingByValue(Collections.reverseOrder())).collect(Collectors.toMap(Entry::getKey, Entry::getValue, (e1, e2) -> e1, LinkedHashMap::new));
	// 	}
	// 	return map.entrySet().stream().sorted(Entry.comparingByValue()).collect(Collectors.toMap(Entry::getKey, Entry::getValue, (e1, e2) -> e1, LinkedHashMap::new));
	// }
	//
	// public static <K, V extends Comparable<? super V>> Map<K, V> sortByValue(Map<K, V> map)
	// {
	// 	return map.entrySet().stream().sorted(Entry.comparingByValue()).collect(Collectors.toMap(Entry::getKey, Entry::getValue, (e1, e2) -> e1, LinkedHashMap::new));
	// }
}