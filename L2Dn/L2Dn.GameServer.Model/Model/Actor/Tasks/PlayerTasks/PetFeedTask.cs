using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Model.Actor.Tasks.PlayerTasks;

/**
 * Task dedicated for feeding player's pet.
 * @author UnAfraid
 */
public class PetFeedTask: Runnable
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(PetFeedTask));

	private readonly Player _player;

	public PetFeedTask(Player player)
	{
		_player = player;
	}

	public void run()
	{
		if (_player != null)
		{
			try
			{
				if (!_player.isMounted() || _player.getMountNpcId() == 0 || _player.getPetData(_player.getMountNpcId()) == null)
				{
					_player.stopFeed();
					return;
				}

				if (_player.getCurrentFeed() > _player.getFeedConsume())
				{
					// eat
					_player.setCurrentFeed(_player.getCurrentFeed() - _player.getFeedConsume());
				}
				else
				{
					// go back to pet control item, or simply said, unsummon it
					_player.setCurrentFeed(0);
					_player.stopFeed();
					_player.dismount();
					_player.sendPacket(SystemMessageId.YOU_ARE_OUT_OF_FEED_MOUNT_STATUS_CANCELED);
				}

				Set<int> foodIds = _player.getPetData(_player.getMountNpcId()).getFood();
				if (foodIds.isEmpty())
				{
					return;
				}

				Item? food = null;
				foreach (int id in foodIds)
				{
					// TODO: possibly pet inv?
					food = _player.getInventory().getItemByItemId(id);
					if (food != null)
					{
						break;
					}
				}

				if (food != null && _player.isHungry())
				{
					IItemHandler? handler = ItemHandler.getInstance().getHandler(food.getEtcItem());
					if (handler != null)
					{
						handler.useItem(_player, food, false);
						SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOUR_PET_WAS_HUNGRY_SO_IT_ATE_S1);
						sm.Params.addItemName(food.getId());
						_player.sendPacket(sm);
					}
				}
			}
			catch (Exception e)
			{
				LOGGER.Error(
					"Mounted Pet [NpcId: " + _player.getMountNpcId() + "] a feed task error has occurred: " + e);
			}
		}
	}
}