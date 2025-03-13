using System.Globalization;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;
using Config = L2Dn.GameServer.Configuration.Config;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * @author Mobius
 */
public class CustomMailManager
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(CustomMailManager));

	protected CustomMailManager()
	{
		ThreadPool.scheduleAtFixedRate(() =>
		{
			try
			{
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				foreach (DbCustomMail record in ctx.CustomMails)
				{
					int playerId = record.ReceiverId;
					Player? player = World.getInstance().getPlayer(playerId);
					if (player != null && player.isOnline())
					{
						// Create message.
						string items = record.Items;
						Message msg = new Message(playerId, record.Subject, record.Message, items.Length > 0 ? MailType.PRIME_SHOP_GIFT : MailType.REGULAR);
						List<ItemEnchantHolder> itemHolders = new();
						foreach (string str in items.Split(";"))
						{
							if (str.Contains(' '))
							{
								string[] split = str.Split(" ");
								string itemIdStr = split[0];
								string itemCountStr = split[1];
								string enchantStr = split.Length > 2 ? split[2] : "0";
								if (int.TryParse(itemIdStr, CultureInfo.InvariantCulture, out int itemId) &&
								    long.TryParse(itemCountStr, CultureInfo.InvariantCulture, out long itemCount) &&
								    int.TryParse(enchantStr, CultureInfo.InvariantCulture, out int enchant))
								{
									itemHolders.Add(new ItemEnchantHolder(itemId, itemCount, enchant));
								}
							}
							else if (int.TryParse(str, CultureInfo.InvariantCulture, out int itemId))
							{
								itemHolders.Add(new ItemEnchantHolder(itemId, 1));
							}
						}

						if (itemHolders.Count != 0)
						{
							Mail attachments = msg.createAttachments();
							foreach (ItemEnchantHolder itemHolder in itemHolders)
							{
								Item? item = attachments.addItem("Custom-Mail", itemHolder.getId(),
									itemHolder.getCount(), null, null);

                                if (item == null)
                                {
                                    LOGGER.Error("Item not found: " + itemHolder.getId());
                                    continue;
                                }

								if (itemHolder.getEnchantLevel() > 0)
									item.setEnchantLevel(itemHolder.getEnchantLevel());
							}
						}

						// Delete entry from database.
						try
						{
							DateTime time = record.Time;

							// TODO: deletion during querying, refactor this
							ctx.CustomMails.Where(m => m.Time == time && m.ReceiverId == playerId).ExecuteDelete();

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