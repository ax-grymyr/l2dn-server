using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Utilities;
using NLog;
using ThreadPool = System.Threading.ThreadPool;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * @author Mobius
 */
public class CustomMailManager
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(CustomMailManager));
	
	// SQL Statements
	private const string READ_SQL = "SELECT * FROM custom_mail";
	private const string DELETE_SQL = "DELETE FROM custom_mail WHERE date=? AND receiver=?";
	
	protected CustomMailManager()
	{
		ThreadPool.scheduleAtFixedRate(() =>
		{
			try 
			{
				using GameServerDbContext ctx = new();
				Statement ps = con.createStatement();
				ResultSet rs = ps.executeQuery(READ_SQL);
				while (rs.next())
				{
					int playerId = rs.getInt("receiver");
					Player player = World.getInstance().getPlayer(playerId);
					if ((player != null) && player.isOnline())
					{
						// Create message.
						String items = rs.getString("items");
						Message msg = new Message(playerId, rs.getString("subject"), rs.getString("message"), items.Length > 0 ? MailType.PRIME_SHOP_GIFT : MailType.REGULAR);
						List<ItemHolder> itemHolders = new();
						foreach (String str in items.Split(";"))
						{
							if (str.Contains(" "))
							{
								String itemId = str.Split(" ")[0];
								String itemCount = str.Split(" ")[1];
								if (Util.isDigit(itemId) && Util.isDigit(itemCount))
								{
									itemHolders.add(new ItemHolder(int.Parse(itemId), long.Parse(itemCount)));
								}
							}
							else if (Util.isDigit(str))
							{
								itemHolders.add(new ItemHolder(int.Parse(str), 1));
							}
						}
						if (!itemHolders.isEmpty())
						{
							Mail attachments = msg.createAttachments();
							foreach (ItemHolder itemHolder in itemHolders)
							{
								attachments.addItem("Custom-Mail", itemHolder.getId(), itemHolder.getCount(), null, null);
							}
						}
						
						// Delete entry from database.
						try
						{
							PreparedStatement stmt = con.prepareStatement(DELETE_SQL);
							stmt.setString(1, rs.getString("date"));
							stmt.setInt(2, playerId);
							stmt.execute();
						}
						catch (Exception e)
						{
							LOGGER.Warn(GetType().Name + ": Error deleting entry from database: " + e);
						}
						
						// Send message.
						MailManager.getInstance().sendMessage(msg);
						LOGGER.Info(GetType().Name +": Message sent to " + player.getName() + ".");
					}
				}
			}
			catch (Exception e)
			{
				LOGGER.Warn(GetType().Name + ": Error reading from database: " + e);
			}
		}, Config.CUSTOM_MAIL_MANAGER_DELAY, Config.CUSTOM_MAIL_MANAGER_DELAY);
		
		LOGGER.Info(GetType().Name +": Enabled.");
	}
	
	public static CustomMailManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly CustomMailManager INSTANCE = new CustomMailManager();
	}
}