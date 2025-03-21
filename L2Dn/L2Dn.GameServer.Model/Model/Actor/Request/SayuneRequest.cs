﻿using System.Runtime.CompilerServices;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Network.OutgoingPackets.Sayune;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Actor.Request;

public class SayuneRequest : AbstractRequest
{
	private readonly int _mapId;
	private bool _isSelecting;
	private readonly Queue<SayuneEntry> _possibleEntries = new();

	public SayuneRequest(Player player, int mapId): base(player)
	{
		SayuneEntry? map = SayuneData.getInstance().getMap(mapId);
		if (map is null)
			throw new ArgumentException("Invalid mapId", nameof(mapId));

		_mapId = mapId;
		foreach (SayuneEntry entry in map.getInnerEntries())
			_possibleEntries.Enqueue(entry);
	}

	public override bool isUsing(int objectId)
	{
		return false;
	}

	private SayuneEntry? findEntry(int pos)
	{
		if (_possibleEntries.Count == 0)
		{
			return null;
		}

        if (_isSelecting)
        {
            foreach (SayuneEntry entry in _possibleEntries)
            {
                if (entry.getId() == pos)
                {
                    return entry;
                }
            }
            return null;
        }

        if (_possibleEntries.TryDequeue(out SayuneEntry? sayuneEntry))
			return sayuneEntry;

		return null;
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	public void move(Player player, int pos)
	{
		SayuneEntry? map = SayuneData.getInstance().getMap(_mapId);
		if (map == null || map.getInnerEntries().Count == 0)
		{
			player.sendMessage("MapId: " + _mapId + " was not found in the map!");
			return;
		}

		SayuneEntry? nextEntry = findEntry(pos);
		if (nextEntry == null)
		{
			player.removeRequest<SayuneRequest>();
			return;
		}

		// If player was selecting unset and set his possible path
		if (_isSelecting)
		{
			_isSelecting = false;

			// Set next possible path
			if (!nextEntry.isSelector())
			{
				_possibleEntries.Clear();

				foreach (SayuneEntry entry in nextEntry.getInnerEntries())
					_possibleEntries.Enqueue(entry);
			}
		}

		SayuneType type = pos == 0 && nextEntry.isSelector() ? SayuneType.START_LOC : nextEntry.isSelector() ? SayuneType.MULTI_WAY_LOC : SayuneType.ONE_WAY_LOC;
		List<SayuneEntry> locations = nextEntry.isSelector() ? nextEntry.getInnerEntries() : [nextEntry];
		if (nextEntry.isSelector())
		{
			_possibleEntries.Clear();

			foreach (SayuneEntry entry in locations)
				_possibleEntries.Enqueue(entry);

			_isSelecting = true;
		}

		player.sendPacket(new ExFlyMovePacket(player, type, _mapId, locations));

		SayuneEntry activeEntry = locations[0];
		Broadcast.toKnownPlayersInRadius(player, new ExFlyMoveBroadcastPacket(player, type, map.getId(), activeEntry.Location), 1000);
		player.setXYZ(activeEntry.Location.X, activeEntry.Location.Y, activeEntry.Location.Z);
	}

	public void onLogout()
	{
		SayuneEntry? map = SayuneData.getInstance().getMap(_mapId);
		if (map != null && map.getInnerEntries().Count != 0)
		{
			SayuneEntry? nextEntry = findEntry(0);
			if (_isSelecting || (nextEntry != null && nextEntry.isSelector()))
			{
				// If player is on selector or next entry is selector go back to first entry
				getActiveChar().setXYZ(map.Location.X, map.Location.Y, map.Location.Z);
			}
			else
			{
				// Try to find last entry to set player, if not set him to first entry
				SayuneEntry lastEntry = map.getInnerEntries()[^1];
				if (lastEntry != null)
				{
					getActiveChar().setXYZ(lastEntry.Location.X, lastEntry.Location.Y, lastEntry.Location.Z);
				}
				else
				{
					getActiveChar().setXYZ(map.Location.X, map.Location.Y, map.Location.Z);
				}
			}
		}
	}
}