using System.Runtime.CompilerServices;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Actor.Request;

public class SayuneRequest : AbstractRequest
{
	private readonly int _mapId;
	private bool _isSelecting;
	private readonly Deque<SayuneEntry> _possibleEntries = new();

	public SayuneRequest(Player player, int mapId): base(player)
	{
		_mapId = mapId;
		
		SayuneEntry map = SayuneData.getInstance().getMap(_mapId);
		Objects.requireNonNull(map);
		
		_possibleEntries.addAll(map.getInnerEntries());
	}
	
	public override bool isUsing(int objectId)
	{
		return false;
	}
	
	private SayuneEntry findEntry(int pos)
	{
		if (_possibleEntries.isEmpty())
		{
			return null;
		}
		else if (_isSelecting)
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
		return _possibleEntries.removeFirst();
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void move(Player player, int pos)
	{
		SayuneEntry map = SayuneData.getInstance().getMap(_mapId);
		if ((map == null) || map.getInnerEntries().isEmpty())
		{
			player.sendMessage("MapId: " + _mapId + " was not found in the map!");
			return;
		}
		
		SayuneEntry nextEntry = findEntry(pos);
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
				_possibleEntries.clear();
				_possibleEntries.addAll(nextEntry.getInnerEntries());
			}
		}
		
		SayuneType type = (pos == 0) && nextEntry.isSelector() ? SayuneType.START_LOC : nextEntry.isSelector() ? SayuneType.MULTI_WAY_LOC : SayuneType.ONE_WAY_LOC;
		List<SayuneEntry> locations = nextEntry.isSelector() ? nextEntry.getInnerEntries() : [nextEntry];
		if (nextEntry.isSelector())
		{
			_possibleEntries.clear();
			_possibleEntries.addAll(locations);
			_isSelecting = true;
		}
		
		player.sendPacket(new ExFlyMove(player, type, _mapId, locations));
		
		SayuneEntry activeEntry = locations.get(0);
		Broadcast.toKnownPlayersInRadius(player, new ExFlyMoveBroadcast(player, type, map.getId(), activeEntry), 1000);
		player.setXYZ(activeEntry);
	}
	
	public void onLogout()
	{
		SayuneEntry map = SayuneData.getInstance().getMap(_mapId);
		if ((map != null) && !map.getInnerEntries().isEmpty())
		{
			SayuneEntry nextEntry = findEntry(0);
			if (_isSelecting || ((nextEntry != null) && nextEntry.isSelector()))
			{
				// If player is on selector or next entry is selector go back to first entry
				getActiveChar().setXYZ(map);
			}
			else
			{
				// Try to find last entry to set player, if not set him to first entry
				SayuneEntry lastEntry = map.getInnerEntries().get(map.getInnerEntries().size() - 1);
				if (lastEntry != null)
				{
					getActiveChar().setXYZ(lastEntry);
				}
				else
				{
					getActiveChar().setXYZ(map);
				}
			}
		}
	}
}
