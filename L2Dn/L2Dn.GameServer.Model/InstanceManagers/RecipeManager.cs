using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.TaskManagers;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;
using L2Dn.Utilities;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.InstanceManagers;

public class RecipeManager
{
	protected static readonly Map<int, RecipeItemMaker> _activeMakers = new();
	
	protected RecipeManager()
	{
		// Prevent external initialization.
	}
	
	public void requestBookOpen(Player player, bool isDwarvenCraft)
	{
		// Check if player is trying to alter recipe book while engaged in manufacturing.
		if (!_activeMakers.ContainsKey(player.getObjectId()))
		{
			ICollection<RecipeList> recipes = isDwarvenCraft ? player.getDwarvenRecipeBook() : player.getCommonRecipeBook();
			RecipeBookItemListPacket response = new RecipeBookItemListPacket(recipes, isDwarvenCraft, player.getMaxMp());
			player.sendPacket(response);
			return;
		}
		
		player.sendPacket(SystemMessageId.YOU_MAY_NOT_ALTER_YOUR_RECIPE_BOOK_WHILE_ENGAGED_IN_MANUFACTURING);
	}
	
	public void requestMakeItemAbort(Player player)
	{
		_activeMakers.remove(player.getObjectId());
	}
	
	public void requestManufactureItem(Player manufacturer, int recipeListId, Player player)
	{
		RecipeList recipeList = RecipeData.getInstance().getValidRecipeList(player, recipeListId);
		if (recipeList == null)
		{
			return;
		}
		
		if (!manufacturer.getDwarvenRecipeBook().Contains(recipeList) && !manufacturer.getCommonRecipeBook().Contains(recipeList))
		{
			Util.handleIllegalPlayerAction(player,
				"Warning!! Character " + player.getName() + " of account " + player.getAccountName() +
				" sent a false recipe id.", Config.DEFAULT_PUNISH);
			return;
		}
		
		// Check if manufacturer is under manufacturing store or private store.
		if (Config.ALT_GAME_CREATION && _activeMakers.ContainsKey(manufacturer.getObjectId()))
		{
			player.sendPacket(SystemMessageId.PLEASE_CLOSE_THE_SETUP_WINDOW_FOR_YOUR_PRIVATE_WORKSHOP_OR_PRIVATE_STORE_AND_TRY_AGAIN);
			return;
		}
		
		RecipeItemMaker maker = new RecipeItemMaker(manufacturer, recipeList, player);
		if (maker.isValid())
		{
			if (Config.ALT_GAME_CREATION)
			{
				_activeMakers.put(manufacturer.getObjectId(), maker);
				ThreadPool.schedule(maker, 100);
			}
			else
			{
				maker.run();
			}
		}
	}
	
	public void requestMakeItem(Player player, int recipeListId)
	{
		// Check if player is trying to operate a private store or private workshop while engaged in combat.
		if (player.isInCombat() || player.isInDuel())
		{
			player.sendPacket(SystemMessageId.WHILE_YOU_ARE_ENGAGED_IN_COMBAT_YOU_CANNOT_OPERATE_A_PRIVATE_STORE_OR_PRIVATE_WORKSHOP);
			return;
		}
		
		RecipeList recipeList = RecipeData.getInstance().getValidRecipeList(player, recipeListId);
		if (recipeList == null)
		{
			return;
		}
		
		if (!player.getDwarvenRecipeBook().Contains(recipeList) && !player.getCommonRecipeBook().Contains(recipeList))
		{
			Util.handleIllegalPlayerAction(player, "Warning!! Character " + player.getName() + " of account " + player.getAccountName() + " sent a false recipe id.", Config.DEFAULT_PUNISH);
			return;
		}
		
		// Check if player is busy (possible if alt game creation is enabled)
		if (Config.ALT_GAME_CREATION && _activeMakers.ContainsKey(player.getObjectId()))
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S2_S1);
			sm.Params.addItemName(recipeList.getItemId());
			sm.Params.addString("You are busy creating.");
			player.sendPacket(sm);
			return;
		}
		
		RecipeItemMaker maker = new RecipeItemMaker(player, recipeList, player);
		if (maker.isValid())
		{
			if (Config.ALT_GAME_CREATION)
			{
				_activeMakers.put(player.getObjectId(), maker);
				ThreadPool.schedule(maker, 100);
			}
			else
			{
				maker.run();
			}
		}
	}
	
	protected class RecipeItemMaker: Runnable
	{
		private static readonly Logger LOGGER = LogManager.GetLogger(nameof(RecipeItemMaker));
		protected bool _isValid;
		protected List<TempItem> _items = null;
		protected readonly RecipeList _recipeList;
		protected readonly Player _player; // "crafter"
		protected readonly Player _target; // "customer"
		protected readonly Skill _skill;
		protected readonly int _skillId;
		protected readonly int _skillLevel;
		protected int _creationPasses = 1;
		protected int _itemGrab;
		protected int _exp = -1;
		protected int _sp = -1;
		protected long _price;
		protected int _totalItems;
		protected TimeSpan _delay;
		
		public RecipeItemMaker(Player pPlayer, RecipeList pRecipeList, Player pTarget)
		{
			_player = pPlayer;
			_target = pTarget;
			_recipeList = pRecipeList;
			_isValid = false;
			_skillId = _recipeList.isDwarvenRecipe() ? (int)CommonSkill.CREATE_DWARVEN : (int)CommonSkill.CREATE_COMMON;
			_skillLevel = _player.getSkillLevel(_skillId);
			_skill = _player.getKnownSkill(_skillId);
			_player.setCrafting(true);
			
			if (_player.isAlikeDead())
			{
				_player.sendPacket(ActionFailedPacket.STATIC_PACKET);
				abort();
				return;
			}
			
			if (_target.isAlikeDead())
			{
				_target.sendPacket(ActionFailedPacket.STATIC_PACKET);
				abort();
				return;
			}
			
			if (_target.isProcessingTransaction())
			{
				_target.sendPacket(ActionFailedPacket.STATIC_PACKET);
				abort();
				return;
			}
			
			if (_player.isProcessingTransaction())
			{
				_player.sendPacket(ActionFailedPacket.STATIC_PACKET);
				abort();
				return;
			}
			
			// validate recipe list
			if (_recipeList.getRecipes().Count == 0)
			{
				_player.sendPacket(ActionFailedPacket.STATIC_PACKET);
				abort();
				return;
			}
			
			// validate skill level
			if (_recipeList.getLevel() > _skillLevel)
			{
				_player.sendPacket(ActionFailedPacket.STATIC_PACKET);
				abort();
				return;
			}
			
			// check that customer can afford to pay for creation services
			if (_player != _target)
			{
				ManufactureItem item = _player.getManufactureItems().get(_recipeList.getId());
				if (item != null)
				{
					_price = item.getCost();
					if (_target.getAdena() < _price) // check price
					{
						_target.sendPacket(SystemMessageId.NOT_ENOUGH_ADENA);
						abort();
						return;
					}
				}
			}
			
			// make temporary items
			_items = listItems(false);
			if (_items == null)
			{
				abort();
				return;
			}
			
			foreach (TempItem i in _items)
			{
				_totalItems += i.getQuantity();
			}
			
			// initial statUse checks
			if (!calculateStatUse(false, false))
			{
				abort();
				return;
			}
			
			// initial AltStatChange checks
			if (Config.ALT_GAME_CREATION)
			{
				calculateAltStatChange();
			}
			
			updateMakeInfo(true);
			updateCurMp();
			updateCurLoad();
			
			_player.setCrafting(false);
			_isValid = true;
		}

		public bool isValid() => _isValid;
		
		public void run()
		{
			if (!Config.IS_CRAFTING_ENABLED)
			{
				_target.sendMessage("Item creation is currently disabled.");
				abort();
				return;
			}
			
			if (_player == null || _target == null)
			{
				LOGGER.Warn("player or target == null (disconnected?), aborting" + _target + _player);
				abort();
				return;
			}
			
			// if (!_player.isOnline() || !_target.isOnline())
			// {
			// LOGGER.Warn("Player or target is not online, aborting " + _target + _player);
			// abort();
			// return;
			// }
			
			if (Config.ALT_GAME_CREATION && !_activeMakers.ContainsKey(_player.getObjectId()))
			{
				if (_target != _player)
				{
					_target.sendMessage("Manufacture aborted");
					_player.sendMessage("Manufacture aborted");
				}
				else
				{
					_player.sendMessage("Item creation aborted");
				}
				
				abort();
				return;
			}
			
			if (Config.ALT_GAME_CREATION && _items.Count != 0)
			{
				if (!calculateStatUse(true, true))
				{
					return; // check stat use
				}
				updateCurMp(); // update craft window mp bar
				grabSomeItems(); // grab (equip) some more items with a nice msg to player
				
				// if still not empty, schedule another pass
				if (_items.Count != 0)
				{
					_delay = Config.ALT_GAME_CREATION_SPEED * _player.getStat().getReuseTime(_skill) *
					         GameTimeTaskManager.TICKS_PER_SECOND * GameTimeTaskManager.MILLIS_IN_TICK;
					
					// TODO: Fix this packet to show crafting animation?
					MagicSkillUsePacket msk = new MagicSkillUsePacket(_player, _skillId, _skillLevel, _delay, TimeSpan.Zero);
					_player.broadcastPacket(msk);
					
					_player.sendPacket(new SetupGaugePacket(_player.getObjectId(), 0, _delay));
					ThreadPool.schedule(this, TimeSpan.FromMilliseconds(100) + _delay);
				}
				else
				{
					// for alt mode, sleep delay msec before finishing
					_player.sendPacket(new SetupGaugePacket(_player.getObjectId(), 0, _delay));
					
					try
					{
						Thread.Sleep(_delay); // milliseconds // TODO: why?
					}
					catch (Exception e)
					{
						// Ignore.
					}
					finally
					{
						finishCrafting();
					}
				}
			} // for old craft mode just finish
			else
			{
				finishCrafting();
			}
		}
		
		private void finishCrafting()
		{
			if (!Config.ALT_GAME_CREATION)
			{
				calculateStatUse(false, true);
			}
			
			// first take adena for manufacture
			if (_target != _player && _price > 0) // customer must pay for services
			{
				// attempt to pay for item
				Item adenatransfer = _target.transferItem("PayManufacture", _target.getInventory().getAdenaInstance().getObjectId(), _price, _player.getInventory(), _player);
				if (adenatransfer == null)
				{
					_target.sendPacket(SystemMessageId.NOT_ENOUGH_ADENA);
					abort();
					return;
				}
			}
			
			_items = listItems(true); // this line actually takes materials from inventory
			if (_items == null)
			{
				// handle possible cheaters here
				// (they click craft then try to get rid of items in order to get free craft)
			}
			else if (Rnd.get(100) < _recipeList.getSuccessRate() + _player.getStat().getValue(Stat.CRAFT_RATE, 0))
			{
				rewardPlayer(); // and immediately puts created item in its place
				updateMakeInfo(true);
			}
			else
			{
				if (_target != _player)
				{
					SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.YOU_FAILED_TO_CREATE_S2_FOR_C1_AT_THE_PRICE_OF_S3_ADENA);
					msg.Params.addString(_target.getName());
					msg.Params.addItemName(_recipeList.getItemId());
					msg.Params.addLong(_price);
					_player.sendPacket(msg);
					
					msg = new SystemMessagePacket(SystemMessageId.C1_HAS_FAILED_TO_CREATE_S2_AT_THE_PRICE_OF_S3_ADENA);
					msg.Params.addString(_player.getName());
					msg.Params.addItemName(_recipeList.getItemId());
					msg.Params.addLong(_price);
					_target.sendPacket(msg);
				}
				else
				{
					_target.sendPacket(SystemMessageId.YOU_FAILED_AT_MIXING_THE_ITEM);
				}
				updateMakeInfo(false);
			}
			// update load and mana bar of craft window
			updateCurMp();
			_activeMakers.remove(_player.getObjectId());
			_player.setCrafting(false);
			_target.sendItemList();
		}
		
		private void updateMakeInfo(bool success)
		{
			if (_target == _player)
			{
				_target.sendPacket(new RecipeItemMakeInfoPacket(_recipeList.getId(), _target, success));
			}
			else
			{
				_target.sendPacket(new RecipeShopItemInfoPacket(_player, _recipeList.getId()));
			}
		}
		
		private void updateCurLoad()
		{
			_target.sendPacket(new ExUserInfoInventoryWeightPacket(_target));
		}
		
		private void updateCurMp()
		{
			StatusUpdatePacket su = new StatusUpdatePacket(_target);
			su.addUpdate(StatusUpdateType.CUR_MP, (int)_target.getCurrentMp());
			_target.sendPacket(su);
		}
		
		private void grabSomeItems()
		{
			int grabItems = _itemGrab;
			while (grabItems > 0 && _items.Count != 0)
			{
				TempItem item = _items[0];
				int count = item.getQuantity();
				if (count >= grabItems)
				{
					count = grabItems;
				}
				
				item.setQuantity(item.getQuantity() - count);
				if (item.getQuantity() <= 0)
				{
					_items.RemoveAt(0);
				}
				else
				{
					_items[0] = item;
				}
				
				grabItems -= count;
				if (_target == _player)
				{
					SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_S2_EQUIPPED); // you equipped ...
					sm.Params.addLong(count);
					sm.Params.addItemName(item.getItemId());
					_player.sendPacket(sm);
				}
				else
				{
					_target.sendMessage("Manufacturer " + _player.getName() + " used " + count + " " + item.getItemName());
				}
			}
		}
		
		// AltStatChange parameters make their effect here
		private void calculateAltStatChange()
		{
			_itemGrab = _skillLevel;
			foreach (RecipeStatHolder altStatChange in _recipeList.getAltStatChange())
			{
				if (altStatChange.getType() == StatType.XP)
				{
					_exp = altStatChange.getValue();
				}
				else if (altStatChange.getType() == StatType.SP)
				{
					_sp = altStatChange.getValue();
				}
				else if (altStatChange.getType() == StatType.GIM)
				{
					_itemGrab *= altStatChange.getValue();
				}
			}
			// determine number of creation passes needed
			_creationPasses = _totalItems / _itemGrab + (_totalItems % _itemGrab != 0 ? 1 : 0);
			if (_creationPasses < 1)
			{
				_creationPasses = 1;
			}
		}
		
		// StatUse
		private bool calculateStatUse(bool isWait, bool isReduce)
		{
			bool ret = true;
			foreach (RecipeStatHolder statUse in _recipeList.getStatUse())
			{
				double modifiedValue = statUse.getValue() / _creationPasses;
				if (statUse.getType() == StatType.HP)
				{
					// we do not want to kill the player, so its CurrentHP must be greater than the reduce value
					if (_player.getCurrentHp() <= modifiedValue)
					{
						// rest (wait for HP)
						if (Config.ALT_GAME_CREATION && isWait)
						{
							_player.sendPacket(new SetupGaugePacket(_player.getObjectId(), 0, _delay));
							ThreadPool.schedule(this, TimeSpan.FromMilliseconds(100) + _delay);
						}
						else
						{
							_target.sendPacket(SystemMessageId.NOT_ENOUGH_HP);
							abort();
						}
						ret = false;
					}
					else if (isReduce)
					{
						_player.reduceCurrentHp(modifiedValue, _player, _skill);
					}
				}
				else if (statUse.getType() == StatType.MP)
				{
					if (_player.getCurrentMp() < modifiedValue)
					{
						// rest (wait for MP)
						if (Config.ALT_GAME_CREATION && isWait)
						{
							_player.sendPacket(new SetupGaugePacket(_player.getObjectId(), 0, _delay));
							ThreadPool.schedule(this, TimeSpan.FromMilliseconds(100) + _delay);
						}
						else
						{
							_target.sendPacket(SystemMessageId.NOT_ENOUGH_MP);
							abort();
						}
						ret = false;
					}
					else if (isReduce)
					{
						_player.reduceCurrentMp(modifiedValue);
					}
				}
				else
				{
					// there is an unknown StatUse value
					_target.sendMessage("Recipe error!!!, please tell this to your GM.");
					ret = false;
					abort();
				}
			}
			return ret;
		}
		
		private List<TempItem> listItems(bool remove)
		{
			List<RecipeHolder> recipes = _recipeList.getRecipes();
			Inventory inv = _target.getInventory();
			List<TempItem> materials = new();
			SystemMessagePacket sm;
			foreach (RecipeHolder recipe in recipes)
			{
				if (recipe.getQuantity() > 0)
				{
					Item item = inv.getItemByItemId(recipe.getItemId());
					long itemQuantityAmount = item == null ? 0 : item.getCount();
					
					// check materials
					if (itemQuantityAmount < recipe.getQuantity())
					{
						sm = new SystemMessagePacket(SystemMessageId.YOU_NEED_S2_MORE_S1_S);
						sm.Params.addItemName(recipe.getItemId());
						sm.Params.addLong(recipe.getQuantity() - itemQuantityAmount);
						_target.sendPacket(sm);
						
						abort();
						return null;
					}
					
					// make new temporary object, just for counting purposes
					materials.Add(new TempItem(item, recipe.getQuantity()));
				}
			}
			
			if (remove)
			{
				foreach (TempItem tmp in materials)
				{
					inv.destroyItemByItemId("Manufacture", tmp.getItemId(), tmp.getQuantity(), _target, _player);
					if (tmp.getQuantity() > 1)
					{
						sm = new SystemMessagePacket(SystemMessageId.S1_X_S2_DISAPPEARED);
						sm.Params.addItemName(tmp.getItemId());
						sm.Params.addLong(tmp.getQuantity());
						_target.sendPacket(sm);
					}
					else
					{
						sm = new SystemMessagePacket(SystemMessageId.S1_DISAPPEARED);
						sm.Params.addItemName(tmp.getItemId());
						_target.sendPacket(sm);
					}
				}
			}
			return materials;
		}
		
		private void abort()
		{
			updateMakeInfo(false);
			_player.setCrafting(false);
			_activeMakers.remove(_player.getObjectId());
		}
		
		private void rewardPlayer()
		{
			int rareProdId = _recipeList.getRareItemId();
			int itemId = _recipeList.getItemId();
			int itemCount = _recipeList.getCount();
			ItemTemplate template = ItemData.getInstance().getTemplate(itemId);
			
			// check that the current recipe has a rare production or not
			if (rareProdId != -1 && (rareProdId == itemId || Config.CRAFT_MASTERWORK))
			{
				if (Rnd.get(100) < _recipeList.getRarity())
				{
					itemId = rareProdId;
					itemCount = _recipeList.getRareCount();
				}
			}
			
			Item item = _target.getInventory().addItem("Manufacture", itemId, itemCount, _target, _player);
			if (item.isEquipable() && itemCount == 1 && Rnd.get(100) < _player.getStat().getValue(Stat.CRAFTING_CRITICAL))
			{
				_target.getInventory().addItem("Manufacture Critical", itemId, itemCount, _target, _player);
			}
			
			// inform customer of earned item
			SystemMessagePacket sm;
			if (_target != _player)
			{
				// inform manufacturer of earned profit
				if (itemCount == 1)
				{
					sm = new SystemMessagePacket(SystemMessageId.S2_HAS_BEEN_CREATED_FOR_C1_AFTER_THE_PAYMENT_OF_S3_ADENA_WAS_RECEIVED);
					sm.Params.addString(_target.getName());
					sm.Params.addItemName(itemId);
					sm.Params.addLong(_price);
					_player.sendPacket(sm);
					
					sm = new SystemMessagePacket(SystemMessageId.C1_CREATED_S2_AFTER_RECEIVING_S3_ADENA);
					sm.Params.addString(_player.getName());
					sm.Params.addItemName(itemId);
					sm.Params.addLong(_price);
					_target.sendPacket(sm);
				}
				else
				{
					sm = new SystemMessagePacket(SystemMessageId.S3_S2_S_HAVE_BEEN_CREATED_FOR_C1_AT_THE_PRICE_OF_S4_ADENA);
					sm.Params.addString(_target.getName());
					sm.Params.addInt(itemCount);
					sm.Params.addItemName(itemId);
					sm.Params.addLong(_price);
					_player.sendPacket(sm);
					
					sm = new SystemMessagePacket(SystemMessageId.C1_CREATED_S3_S2_S_AT_THE_PRICE_OF_S4_ADENA);
					sm.Params.addString(_player.getName());
					sm.Params.addInt(itemCount);
					sm.Params.addItemName(itemId);
					sm.Params.addLong(_price);
					_target.sendPacket(sm);
				}
			}
			
			if (itemCount > 1)
			{
				sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_OBTAINED_S1_X_S2);
				sm.Params.addItemName(itemId);
				sm.Params.addLong(itemCount);
				_target.sendPacket(sm);
			}
			else
			{
				sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_ACQUIRED_S1);
				sm.Params.addItemName(itemId);
				_target.sendPacket(sm);
			}
			
			if (Config.ALT_GAME_CREATION)
			{
				int recipeLevel = _recipeList.getLevel();
				if (_exp < 0)
				{
					_exp = template.getReferencePrice() * itemCount;
					_exp /= recipeLevel;
				}
				if (_sp < 0)
				{
					_sp = _exp / 10;
				}
				if (itemId == rareProdId)
				{
					_exp = (int)(_exp * Config.ALT_GAME_CREATION_RARE_XPSP_RATE);
					_sp = (int)(_sp * Config.ALT_GAME_CREATION_RARE_XPSP_RATE);
				}
				
				if (_exp < 0)
				{
					_exp = 0;
				}
				if (_sp < 0)
				{
					_sp = 0;
				}
				
				for (int i = _skillLevel; i > recipeLevel; i--)
				{
					_exp /= 4;
					_sp /= 4;
				}
				
				// Added multiplication of Creation speed with XP/SP gain slower crafting => more XP,
				// faster crafting => less XP you can use ALT_GAME_CREATION_XP_RATE/SP to modify XP/SP gained (default = 1)
				_player.addExpAndSp((int) _player.getStat().getValue(Stat.EXPSP_RATE, _exp * Config.ALT_GAME_CREATION_XP_RATE * Config.ALT_GAME_CREATION_SPEED), (int) _player.getStat().getValue(Stat.EXPSP_RATE, _sp * Config.ALT_GAME_CREATION_SP_RATE * Config.ALT_GAME_CREATION_SPEED));
			}
			updateMakeInfo(true); // success
		}
	}
	
	public static RecipeManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly RecipeManager INSTANCE = new RecipeManager();
	}
}