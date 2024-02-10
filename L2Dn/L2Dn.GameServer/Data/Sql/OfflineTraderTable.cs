using System.Runtime.CompilerServices;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Data.Sql;

public class OfflineTraderTable
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(OfflineTraderTable));
	
	// SQL DEFINITIONS
	private const string SAVE_OFFLINE_STATUS = "INSERT INTO character_offline_trade (`charId`,`time`,`type`,`title`) VALUES (?,?,?,?)";
	private const string SAVE_ITEMS = "INSERT INTO character_offline_trade_items (`charId`,`item`,`count`,`price`) VALUES (?,?,?,?)";
	private const string CLEAR_OFFLINE_TABLE = "DELETE FROM character_offline_trade";
	private const string CLEAR_OFFLINE_TABLE_PLAYER = "DELETE FROM character_offline_trade WHERE `charId`=?";
	private const string CLEAR_OFFLINE_TABLE_ITEMS = "DELETE FROM character_offline_trade_items";
	private const string CLEAR_OFFLINE_TABLE_ITEMS_PLAYER = "DELETE FROM character_offline_trade_items WHERE `charId`=?";
	private const string LOAD_OFFLINE_STATUS = "SELECT * FROM character_offline_trade";
	private const string LOAD_OFFLINE_ITEMS = "SELECT * FROM character_offline_trade_items WHERE `charId`=?";
	
	protected OfflineTraderTable()
	{
	}
	
	public void storeOffliners()
	{
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement stm1 = con.prepareStatement(CLEAR_OFFLINE_TABLE);
			PreparedStatement stm2 = con.prepareStatement(CLEAR_OFFLINE_TABLE_ITEMS);
			PreparedStatement stm3 = con.prepareStatement(SAVE_OFFLINE_STATUS);
			PreparedStatement stmItems = con.prepareStatement(SAVE_ITEMS);
			stm1.execute();
			stm2.execute();
			con.setAutoCommit(false); // avoid halfway done
			foreach (Player pc in World.getInstance().getPlayers())
			{
				try
				{
					if ((pc.getPrivateStoreType() != PrivateStoreType.NONE) && ((pc.getClient() == null) || pc.getClient().isDetached()))
					{
						stm3.setInt(1, pc.getObjectId()); // Char Id
						stm3.setLong(2, pc.getOfflineStartTime());
						stm3.setInt(3, pc.isSellingBuffs() ? PrivateStoreType.SELL_BUFFS.getId() : pc.getPrivateStoreType().getId()); // store type
						String title = null;
						
						switch (pc.getPrivateStoreType())
						{
							case PrivateStoreType.BUY:
							{
								if (!Config.OFFLINE_TRADE_ENABLE)
								{
									continue;
								}
								title = pc.getBuyList().getTitle();
								foreach (TradeItem i in pc.getBuyList().getItems())
								{
									stmItems.setInt(1, pc.getObjectId());
									stmItems.setInt(2, i.getItem().getId());
									stmItems.setLong(3, i.getCount());
									stmItems.setLong(4, i.getPrice());
									stmItems.executeUpdate();
									stmItems.clearParameters();
								}
								break;
							}
							case PrivateStoreType.SELL:
							case PrivateStoreType.PACKAGE_SELL:
							{
								if (!Config.OFFLINE_TRADE_ENABLE)
								{
									continue;
								}
								title = pc.getSellList().getTitle();
								if (pc.isSellingBuffs())
								{
									foreach (SellBuffHolder holder in pc.getSellingBuffs())
									{
										stmItems.setInt(1, pc.getObjectId());
										stmItems.setInt(2, holder.getSkillId());
										stmItems.setLong(3, 0);
										stmItems.setLong(4, holder.getPrice());
										stmItems.executeUpdate();
										stmItems.clearParameters();
									}
								}
								else
								{
									foreach (TradeItem i in pc.getSellList().getItems())
									{
										stmItems.setInt(1, pc.getObjectId());
										stmItems.setInt(2, i.getObjectId());
										stmItems.setLong(3, i.getCount());
										stmItems.setLong(4, i.getPrice());
										stmItems.executeUpdate();
										stmItems.clearParameters();
									}
								}
								break;
							}
							case PrivateStoreType.MANUFACTURE:
							{
								if (!Config.OFFLINE_CRAFT_ENABLE)
								{
									continue;
								}
								title = pc.getStoreName();
								foreach (ManufactureItem i in pc.getManufactureItems().values())
								{
									stmItems.setInt(1, pc.getObjectId());
									stmItems.setInt(2, i.getRecipeId());
									stmItems.setLong(3, 0);
									stmItems.setLong(4, i.getCost());
									stmItems.executeUpdate();
									stmItems.clearParameters();
								}
								break;
							}
						}
						stm3.setString(4, title);
						stm3.executeUpdate();
						stm3.clearParameters();
						con.commit(); // flush
					}
				}
				catch (Exception e)
				{
					LOGGER.Warn(GetType().Name + ": Error while saving offline trader: " + pc.getObjectId() + " " + e, e);
				}
			}
			LOGGER.Info(GetType().Name + ": Offline traders stored.");
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Error while saving offline traders: " + e, e);
		}
	}
	
	public void restoreOfflineTraders()
	{
		LOGGER.Info(GetType().Name + ": Loading offline traders...");
		int nTraders = 0;
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			Statement stm = con.createStatement();
			ResultSet rs = stm.executeQuery(LOAD_OFFLINE_STATUS);
			while (rs.next())
			{
				long time = rs.getLong("time");
				if (Config.OFFLINE_MAX_DAYS > 0)
				{
					Calendar cal = Calendar.getInstance();
					cal.setTimeInMillis(time);
					cal.add(Calendar.DAY_OF_YEAR, Config.OFFLINE_MAX_DAYS);
					if (cal.getTimeInMillis() <= System.currentTimeMillis())
					{
						continue;
					}
				}
				
				int typeId = rs.getInt("type");
				bool isSellBuff = false;
				if (typeId == PrivateStoreType.SELL_BUFFS.getId())
				{
					isSellBuff = true;
				}
				
				PrivateStoreType type = isSellBuff ? PrivateStoreType.PACKAGE_SELL : PrivateStoreType.findById(typeId);
				if (type == null)
				{
					LOGGER.Warn(GetType().Name + ": PrivateStoreType with id " + rs.getInt("type") + " could not be found.");
					continue;
				}
				
				if (type == PrivateStoreType.NONE)
				{
					continue;
				}
				
				Player player = null;
				
				try
				{
					GameClient client = new GameClient();
					client.setDetached(true);
					player = Player.load(rs.getInt("charId"));
					client.setPlayer(player);
					player.setOnlineStatus(true, false);
					client.setAccountName(player.getAccountNamePlayer());
					player.setClient(client);
					player.setOfflineStartTime(time);
					
					if (isSellBuff)
					{
						player.setSellingBuffs(true);
					}
					
					player.spawnMe(player.getX(), player.getY(), player.getZ());
					try
					{
						PreparedStatement stmItems = con.prepareStatement(LOAD_OFFLINE_ITEMS);
						stmItems.setInt(1, player.getObjectId());
						{
							ResultSet items = stmItems.executeQuery();
							switch (type)
							{
								case PrivateStoreType.BUY:
								{
									while (items.next())
									{
										if (player.getBuyList().addItemByItemId(items.getInt(2), items.getLong(3), items.getLong(4)) == null)
										{
											continue;
											// throw new NullPointerException();
										}
									}
									player.getBuyList().setTitle(rs.getString("title"));
									break;
								}
								case PrivateStoreType.SELL:
								case PrivateStoreType.PACKAGE_SELL:
								{
									if (player.isSellingBuffs())
									{
										while (items.next())
										{
											player.getSellingBuffs().Add(new SellBuffHolder(items.getInt("item"), items.getLong("price")));
										}
									}
									else
									{
										while (items.next())
										{
											if (player.getSellList().addItem(items.getInt(2), items.getLong(3), items.getLong(4)) == null)
											{
												continue;
												// throw new NullPointerException();
											}
										}
									}
									player.getSellList().setTitle(rs.getString("title"));
									player.getSellList().setPackaged(type == PrivateStoreType.PACKAGE_SELL);
									break;
								}
								case PrivateStoreType.MANUFACTURE:
								{
									while (items.next())
									{
										player.getManufactureItems().put(items.getInt(2), new ManufactureItem(items.getInt(2), items.getLong(4)));
									}
									player.setStoreName(rs.getString("title"));
									break;
								}
							}
						}
					}
					player.sitDown();
					if (Config.OFFLINE_SET_NAME_COLOR)
					{
						player.getAppearance().setNameColor(Config.OFFLINE_NAME_COLOR);
					}
					player.setPrivateStoreType(type);
					player.setOnlineStatus(true, true);
					player.restoreEffects();
					if (!Config.OFFLINE_ABNORMAL_EFFECTS.isEmpty())
					{
						player.getEffectList().startAbnormalVisualEffect(Config.OFFLINE_ABNORMAL_EFFECTS.get(Rnd.get(Config.OFFLINE_ABNORMAL_EFFECTS.size())));
					}
					player.broadcastUserInfo();
					nTraders++;
				}
				catch (Exception e)
				{
					LOGGER.Warn(GetType().Name + ": Error loading trader: " + player, e);
					if (player != null)
					{
						Disconnection.of(player).defaultSequence(LeaveWorld.STATIC_PACKET);
					}
				}
			}
			
			World.OFFLINE_TRADE_COUNT = nTraders;
			LOGGER.Info(GetType().Name + ": Loaded " + nTraders + " offline traders.");
			
			if (!Config.STORE_OFFLINE_TRADE_IN_REALTIME)
			{
				{
					Statement stm1 = con.createStatement();
					stm1.execute(CLEAR_OFFLINE_TABLE);
					stm1.execute(CLEAR_OFFLINE_TABLE_ITEMS);
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Error while loading offline traders: ", e);
		}
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void onTransaction(Player trader, bool finished, bool firstCall)
	{
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement stm1 = con.prepareStatement(CLEAR_OFFLINE_TABLE_ITEMS_PLAYER);
			PreparedStatement stm2 = con.prepareStatement(CLEAR_OFFLINE_TABLE_PLAYER);
			PreparedStatement stm3 = con.prepareStatement(SAVE_ITEMS);
			PreparedStatement stm4 = con.prepareStatement(SAVE_OFFLINE_STATUS);
			String title = null;
			stm1.setInt(1, trader.getObjectId()); // Char Id
			stm1.execute();
			
			// Trade is done - clear info
			if (finished)
			{
				stm2.setInt(1, trader.getObjectId()); // Char Id
				stm2.execute();
			}
			else
			{
				try
				{
					if ((trader.getClient() == null) || trader.getClient().isDetached())
					{
						switch (trader.getPrivateStoreType())
						{
							case PrivateStoreType.BUY:
							{
								if (firstCall)
								{
									title = trader.getBuyList().getTitle();
								}
								foreach (TradeItem i in trader.getBuyList().getItems())
								{
									stm3.setInt(1, trader.getObjectId());
									stm3.setInt(2, i.getItem().getId());
									stm3.setLong(3, i.getCount());
									stm3.setLong(4, i.getPrice());
									stm3.executeUpdate();
									stm3.clearParameters();
								}
								break;
							}
							case PrivateStoreType.SELL:
							case PrivateStoreType.PACKAGE_SELL:
							{
								if (firstCall)
								{
									title = trader.getSellList().getTitle();
								}
								if (trader.isSellingBuffs())
								{
									foreach (SellBuffHolder holder in trader.getSellingBuffs())
									{
										stm3.setInt(1, trader.getObjectId());
										stm3.setInt(2, holder.getSkillId());
										stm3.setLong(3, 0);
										stm3.setLong(4, holder.getPrice());
										stm3.executeUpdate();
										stm3.clearParameters();
									}
								}
								else
								{
									foreach (TradeItem i in trader.getSellList().getItems())
									{
										stm3.setInt(1, trader.getObjectId());
										stm3.setInt(2, i.getObjectId());
										stm3.setLong(3, i.getCount());
										stm3.setLong(4, i.getPrice());
										stm3.executeUpdate();
										stm3.clearParameters();
									}
								}
								break;
							}
							case PrivateStoreType.MANUFACTURE:
							{
								if (firstCall)
								{
									title = trader.getStoreName();
								}
								foreach (ManufactureItem i in trader.getManufactureItems().values())
								{
									stm3.setInt(1, trader.getObjectId());
									stm3.setInt(2, i.getRecipeId());
									stm3.setLong(3, 0);
									stm3.setLong(4, i.getCost());
									stm3.executeUpdate();
									stm3.clearParameters();
								}
								break;
							}
						}
						if (firstCall)
						{
							stm4.setInt(1, trader.getObjectId()); // Char Id
							stm4.setLong(2, trader.getOfflineStartTime());
							stm4.setInt(3, trader.isSellingBuffs() ? PrivateStoreType.SELL_BUFFS.getId() : trader.getPrivateStoreType().getId()); // store type
							stm4.setString(4, title);
							stm4.executeUpdate();
							stm4.clearParameters();
						}
					}
				}
				catch (Exception e)
				{
					LOGGER.Warn(GetType().Name + ": Error while saving offline trader: " + trader.getObjectId() + " " + e, e);
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Error while saving offline traders: " + e, e);
		}
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void removeTrader(int traderObjId)
	{
		World.OFFLINE_TRADE_COUNT--;
		
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement stm1 = con.prepareStatement(CLEAR_OFFLINE_TABLE_ITEMS_PLAYER);
			PreparedStatement stm2 = con.prepareStatement(CLEAR_OFFLINE_TABLE_PLAYER);
			stm1.setInt(1, traderObjId);
			stm1.execute();
			
			stm2.setInt(1, traderObjId);
			stm2.execute();
		}
		catch (Exception e)
		{
			LOGGER.Warn(GetType().Name + ": Error while removing offline trader: " + traderObjId + " " + e, e);
		}
	}
	
	/**
	 * Gets the single instance of OfflineTradersTable.
	 * @return single instance of OfflineTradersTable
	 */
	public static OfflineTraderTable getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly OfflineTraderTable INSTANCE = new OfflineTraderTable();
	}
}