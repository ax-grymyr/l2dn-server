using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Olympiads;
using L2Dn.GameServer.Model.Variables;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets.HuntingZones;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.HuntingZones;

public struct ExTimedHuntingZoneEnterPacket: IIncomingPacket<GameSession>
{
	private int _zoneId;

	public void ReadContent(PacketBitReader reader)
	{
		_zoneId = reader.ReadInt32();
	}

	public ValueTask ProcessAsync(Connection connection, GameSession session)
	{
		Player? player = session.Player;
		if (player == null)
			return ValueTask.CompletedTask;

		if (!player.isInsideZone(ZoneId.PEACE))
		{
			player.sendMessage("Can only enter to a peace zone.");
			return ValueTask.CompletedTask;
		}

		if (player.isInCombat())
		{
			player.sendMessage("You can only enter in time-limited hunting zones while not in combat.");
			return ValueTask.CompletedTask;
		}

		if (player.getReputation() < 0)
		{
			player.sendMessage("You can only enter in time-limited hunting zones when you have positive reputation.");
			return ValueTask.CompletedTask;
		}

		if (player.isMounted())
		{
			player.sendMessage("Cannot use time-limited hunting zones while mounted.");
			return ValueTask.CompletedTask;
		}

		if (player.isInDuel())
		{
			player.sendMessage("Cannot use time-limited hunting zones during a duel.");
			return ValueTask.CompletedTask;
		}

		if (player.isInOlympiadMode() || OlympiadManager.getInstance().isRegistered(player))
		{
			player.sendMessage("Cannot use time-limited hunting zones while waiting for the Olympiad.");
			return ValueTask.CompletedTask;
		}

		if (player.isRegisteredOnEvent())
		{
			player.sendMessage("Cannot use time-limited hunting zones while registered on an event.");
			return ValueTask.CompletedTask;
		}

		if (player.isInInstance() || player.isInTimedHuntingZone())
		{
			player.sendMessage("Cannot use time-limited hunting zones while in an instance.");
			return ValueTask.CompletedTask;
		}

		TimedHuntingZoneHolder holder = TimedHuntingZoneData.getInstance().getHuntingZone(_zoneId);
		if (holder == null)
			return ValueTask.CompletedTask;

		if ((player.getLevel() < holder.getMinLevel()) || (player.getLevel() > holder.getMaxLevel()))
		{
			player.sendMessage("Your level does not correspond the zone equivalent.");
			return ValueTask.CompletedTask;
		}

		// TODO: Move shared instance cooldown to XML.
		DateTime currentTime = DateTime.UtcNow;
		int instanceId = holder.getInstanceId();
		if ((instanceId > 0) && holder.isSoloInstance())
		{
			if (instanceId == 228) // Cooldown for Training Zone instance.
			{
				if (InstanceManager.getInstance().getInstanceTime(player, instanceId) > currentTime)
				{
					player.sendMessage("The training zone has not reset yet.");
					return ValueTask.CompletedTask;
				}
			}
			else // Shared cooldown for all Transcendent instances.
			{
				for (int instId = 208; instId <= 213; instId++)
				{
					if (InstanceManager.getInstance().getInstanceTime(player, instId) > currentTime)
					{
						player.sendMessage("The transcendent instance has not reset yet.");
						return ValueTask.CompletedTask;
					}
				}
			}
		}

		// TODO verify time calculations
		DateTime endTime = currentTime + TimeSpan.FromMilliseconds(player.getTimedHuntingZoneRemainingTime(_zoneId));
		DateTime lastEntryTime = new DateTime(player.getVariables().getLong(PlayerVariables.HUNTING_ZONE_ENTRY + _zoneId, 0));
		if ((lastEntryTime + holder.getResetDelay()) < currentTime)
		{
			if (endTime == currentTime)
			{
				endTime += TimeSpan.FromMilliseconds(holder.getInitialTime());
				player.getVariables().set(PlayerVariables.HUNTING_ZONE_ENTRY + _zoneId, currentTime);
			}
		}

		if (endTime > currentTime)
		{
			if (holder.getEntryItemId() == Inventory.ADENA_ID)
			{
				if (player.getAdena() > holder.getEntryFee())
				{
					player.reduceAdena("TimedHuntingZone", holder.getEntryFee(), player, true);
				}
				else
				{
					player.sendMessage("Not enough adena.");
					return ValueTask.CompletedTask;
				}
			}
			else if (!player.destroyItemByItemId("TimedHuntingZone", holder.getEntryItemId(), holder.getEntryFee(),
				         player, true))
			{
				player.sendPacket(SystemMessageId.YOU_DO_NOT_HAVE_ENOUGH_REQUIRED_ITEMS);
				return ValueTask.CompletedTask;
			}

			player.getVariables().set(PlayerVariables.HUNTING_ZONE_TIME + _zoneId, endTime - currentTime);

			if (instanceId == 0)
			{
				player.teleToLocation(holder.getEnterLocation().ToLocationHeading());

				// Send time icon.
				connection.Send(new TimedHuntingZoneEnterPacket(player, _zoneId));
			}
			else // Instanced zones.
			{
				QuestManager.getInstance().getQuest("TimedHunting").notifyEvent("ENTER " + _zoneId, null, player);
			}
		}
		else
		{
			player.sendMessage("You don't have enough time available to enter the hunting zone.");
		}

		return ValueTask.CompletedTask;
	}
}