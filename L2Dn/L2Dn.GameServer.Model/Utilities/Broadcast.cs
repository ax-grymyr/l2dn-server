using L2Dn.Extensions;
using L2Dn.GameServer.Cache;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Packets;
using NLog;

namespace L2Dn.GameServer.Utilities;

public static class Broadcast
{
    // TODO: refactor this later
    
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(Broadcast));
	
	/**
	 * Send a packet to all Player in the _KnownPlayers of the Creature that have the Character targeted.<br>
	 * <br>
	 * <b><u>Concept</u>:</b><br>
	 * <br>
	 * Player in the detection area of the Creature are identified in <b>_knownPlayers</b>.<br>
	 * In order to inform other players of state modification on the Creature, server just need to go through _knownPlayers to send Server=>Client Packet<br>
	 * <font color=#FF0000><b><u>Caution</u>: This method DOESN'T SEND Server=>Client packet to this Creature (to do this use method toSelfAndKnownPlayers)</b></font>
	 * @param creature
	 * @param packet
	 */
	public static void toPlayersTargettingMyself<TPacket>(Creature creature, TPacket packet)
		where TPacket: struct, IOutgoingPacket
	{
		World.getInstance().forEachVisibleObject<Player>(creature, player =>
		{
			if (player.getTarget() == creature)
			{
				player.sendPacket(packet);
			}
		});
	}
	
	/**
	 * Send a packet to all Player in the _KnownPlayers of the Creature.<br>
	 * <br>
	 * <b><u>Concept</u>:</b><br>
	 * <br>
	 * Player in the detection area of the Creature are identified in <b>_knownPlayers</b>.<br>
	 * In order to inform other players of state modification on the Creature, server just need to go through _knownPlayers to send Server=>Client Packet<br>
	 * <font color=#FF0000><b><u>Caution</u>: This method DOESN'T SEND Server=>Client packet to this Creature (to do this use method toSelfAndKnownPlayers)</b></font>
	 * @param creature
	 * @param packet
	 */
	public static void toKnownPlayers<TPacket>(Creature creature, TPacket packet)
		where TPacket: struct, IOutgoingPacket
	{
		World.getInstance().forEachVisibleObject<Player>(creature, player =>
		{
			try
			{
				player.sendPacket(packet);
				if ((packet is CharacterInfoPacket) && (creature.isPlayer()))
				{
					long relation = ((Player) creature).getRelation(player);
					bool isAutoAttackable = creature.isAutoAttackable(player);
					RelationCache oldrelation = creature.getKnownRelations().get(player.getObjectId());
					if ((oldrelation == null) || (oldrelation.getRelation() != relation) || (oldrelation.isAutoAttackable() != isAutoAttackable))
					{
						RelationChangedPacket rc = new RelationChangedPacket();
						rc.addRelation((Player) creature, relation, isAutoAttackable);
						if (creature.hasSummon())
						{
							Summon pet = creature.getPet();
							if (pet != null)
							{
								rc.addRelation(pet, relation, isAutoAttackable);
							}
							if (creature.hasServitors())
							{
								creature.getServitors().Values.ForEach(s => rc.addRelation(s, relation, isAutoAttackable));
							}
						}
						player.sendPacket(rc);
						creature.getKnownRelations().put(player.getObjectId(), new RelationCache(relation, isAutoAttackable));
					}
				}
			}
			catch (NullReferenceException e)
			{
				LOGGER.Error(e);
			}
		});
	}
	
	/**
	 * Send a packet to all Player in the _KnownPlayers (in the specified radius) of the Creature.<br>
	 * <br>
	 * <b><u>Concept</u>:</b><br>
	 * <br>
	 * Player in the detection area of the Creature are identified in <b>_knownPlayers</b>.<br>
	 * In order to inform other players of state modification on the Creature, server just needs to go through _knownPlayers to send Server=>Client Packet and check the distance between the targets.<br>
	 * <font color=#FF0000><b><u>Caution</u>: This method DOESN'T SEND Server=>Client packet to this Creature (to do this use method toSelfAndKnownPlayers)</b></font>
	 * @param creature
	 * @param packet
	 * @param radiusValue
	 */
	public static void toKnownPlayersInRadius<TPacket>(Creature creature, TPacket packet, int radiusValue)
		where TPacket: struct, IOutgoingPacket
	{
		int radius = radiusValue;
		if (radius < 0)
		{
			radius = 1500;
		}
		
		World.getInstance().forEachVisibleObjectInRange<Player>(creature, radius, player => player.sendPacket(packet));
	}
	
	/**
	 * Send a packet to all Player in the _KnownPlayers of the Creature and to the specified character.<br>
	 * <br>
	 * <b><u>Concept</u>:</b><br>
	 * <br>
	 * Player in the detection area of the Creature are identified in <b>_knownPlayers</b>.<br>
	 * In order to inform other players of state modification on the Creature, server just need to go through _knownPlayers to send Server=>Client Packet
	 * @param creature
	 * @param packet
	 */
	public static void toSelfAndKnownPlayers<TPacket>(Creature creature, TPacket packet)
		where TPacket: struct, IOutgoingPacket
	{
		if (creature.isPlayer())
		{
			creature.sendPacket(packet);
		}
		
		toKnownPlayers(creature, packet);
	}
	
	// To improve performance we are comparing values of radius^2 instead of calculating sqrt all the time
	public static void toSelfAndKnownPlayersInRadius<TPacket>(Creature creature, TPacket packet, int radiusValue)
		where TPacket: struct, IOutgoingPacket
	{
		int radius = radiusValue;
		if (radius < 0)
		{
			radius = 600;
		}
		
		if (creature.isPlayer())
		{
			creature.sendPacket(packet);
		}
		
		World.getInstance().forEachVisibleObjectInRange<Player>(creature, radius, player => player.sendPacket(packet));
	}
	
	/**
	 * Send a packet to all Player present in the world.<br>
	 * <br>
	 * <b><u>Concept</u>:</b><br>
	 * <br>
	 * In order to inform other players of state modification on the Creature, server just need to go through _allPlayers to send Server=>Client Packet<br>
	 * <font color=#FF0000><b><u>Caution</u>: This method DOESN'T SEND Server=>Client packet to this Creature (to do this use method toSelfAndKnownPlayers)</b></font>
	 * @param packet
	 */
	public static void toAllOnlinePlayers<TPacket>(TPacket packet)
		where TPacket: struct, IOutgoingPacket
	{
		foreach (Player player in World.getInstance().getPlayers())
		{
			if (player.isOnline())
			{
				player.sendPacket(packet);
			}
		}
	}
	
	public static void toAllOnlinePlayers(string text)
	{
		toAllOnlinePlayers(text, false);
	}
	
	public static void toAllOnlinePlayers(string text, bool isCritical)
	{
		toAllOnlinePlayers(new CreatureSayPacket(null, isCritical ? ChatType.CRITICAL_ANNOUNCE : ChatType.ANNOUNCEMENT, "", text));
	}
	
	public static void toAllOnlinePlayersOnScreen(string text)
	{
		toAllOnlinePlayers(new ExShowScreenMessagePacket(text, 10000));
	}
	
	/**
	 * Send a packet to all players in a specific zone type.
	 * @param <T> ZoneType.
	 * @param zoneType : The zone type to send packets.
	 * @param packets : The packets to send.
	 */
	public static PacketSendUtil toAllPlayersInZoneType<TZone>()
		where TZone: ZoneType
	{
		IEnumerable<Creature> creatures =
			ZoneManager.getInstance().getAllZones<TZone>().SelectMany(z => z.getCharactersInside());

		return new PacketSendUtil(creatures);
	}
}

public struct PacketSendUtil // TODO: refactor this later
{
	private readonly IEnumerable<Creature?> _creatures;

	public PacketSendUtil(IEnumerable<Creature?> creatures)
	{
		_creatures = creatures.Where(c => c is not null);
	}

	public void SendPackets<TPacket>(TPacket packet)
		where TPacket: struct, IOutgoingPacket
	{
		foreach (Creature? creature in _creatures)
		{
			if (creature is null)
				continue;

			creature.sendPacket(packet);
		}
	}

	public void SendPackets<TPacket1, TPacket2>(TPacket1 packet1, TPacket2 packet2)
		where TPacket1: struct, IOutgoingPacket
		where TPacket2: struct, IOutgoingPacket
	{
		foreach (Creature? creature in _creatures)
		{
			if (creature is null)
				continue;

			creature.sendPacket(packet1);
			creature.sendPacket(packet2);
		}
	}

	public void SendPackets<TPacket1, TPacket2, TPacket3>(TPacket1 packet1, TPacket2 packet2, TPacket3 packet3)
		where TPacket1: struct, IOutgoingPacket
		where TPacket2: struct, IOutgoingPacket
		where TPacket3: struct, IOutgoingPacket
	{
		foreach (Creature? creature in _creatures)
		{
			if (creature is null)
				continue;

			creature.sendPacket(packet1);
			creature.sendPacket(packet2);
			creature.sendPacket(packet3);
		}
	}

	public void SendPackets<TPacket1, TPacket2, TPacket3, TPacket4>(TPacket1 packet1, TPacket2 packet2,
		TPacket3 packet3, TPacket4 packet4)
		where TPacket1: struct, IOutgoingPacket
		where TPacket2: struct, IOutgoingPacket
		where TPacket3: struct, IOutgoingPacket
		where TPacket4: struct, IOutgoingPacket
	{
		foreach (Creature? creature in _creatures)
		{
			if (creature is null)
				continue;

			creature.sendPacket(packet1);
			creature.sendPacket(packet2);
			creature.sendPacket(packet3);
			creature.sendPacket(packet4);
		}
	}

	public void SendPackets<TPacket1, TPacket2, TPacket3, TPacket4, TPacket5, TPacket6, TPacket7, TPacket8>(TPacket1 packet1, TPacket2 packet2,
		TPacket3 packet3, TPacket4 packet4, TPacket5 packet5, TPacket6 packet6, TPacket7 packet7, TPacket8 packet8)
		where TPacket1: struct, IOutgoingPacket
		where TPacket2: struct, IOutgoingPacket
		where TPacket3: struct, IOutgoingPacket
		where TPacket4: struct, IOutgoingPacket
		where TPacket5: struct, IOutgoingPacket
		where TPacket6: struct, IOutgoingPacket
		where TPacket7: struct, IOutgoingPacket
		where TPacket8: struct, IOutgoingPacket
	{
		foreach (Creature? creature in _creatures)
		{
			if (creature is null)
				continue;

			creature.sendPacket(packet1);
			creature.sendPacket(packet2);
			creature.sendPacket(packet3);
			creature.sendPacket(packet4);
			creature.sendPacket(packet5);
			creature.sendPacket(packet6);
			creature.sendPacket(packet7);
			creature.sendPacket(packet8);
		}
	}
}