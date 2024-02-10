using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model;

public class Radar
{
	private readonly Player _player;
	private readonly Set<RadarMarker> _markers = new();
	
	public Radar(Player player)
	{
		_player = player;
	}
	
	// Add a marker to player's radar
	public void addMarker(int x, int y, int z)
	{
		RadarMarker newMarker = new RadarMarker(x, y, z);
		_markers.add(newMarker);
		_player.sendPacket(new RadarControl(2, 2, x, y, z));
		_player.sendPacket(new RadarControl(0, 1, x, y, z));
	}
	
	// Remove a marker from player's radar
	public void removeMarker(int x, int y, int z)
	{
		foreach (RadarMarker rm in _markers)
		{
			if ((rm._x == x) && (rm._y == y) && (rm._z == z))
			{
				_markers.remove(rm);
			}
		}
		_player.sendPacket(new RadarControl(1, 1, x, y, z));
	}
	
	public void removeAllMarkers()
	{
		foreach (RadarMarker tempMarker in _markers)
		{
			_player.sendPacket(new RadarControl(2, 2, tempMarker._x, tempMarker._y, tempMarker._z));
		}
		
		_markers.clear();
	}
	
	public void loadMarkers()
	{
		_player.sendPacket(new RadarControl(2, 2, _player.getX(), _player.getY(), _player.getZ()));
		foreach (RadarMarker tempMarker in _markers)
		{
			_player.sendPacket(new RadarControl(0, 1, tempMarker._x, tempMarker._y, tempMarker._z));
		}
	}
	
	public class RadarMarker
	{
		// Simple class to model radar points.
		public int _type;
		public int _x;
		public int _y;
		public int _z;
		
		public RadarMarker(int type, int x, int y, int z)
		{
			_type = type;
			_x = x;
			_y = y;
			_z = z;
		}
		
		public RadarMarker(int x, int y, int z)
		{
			_type = 1;
			_x = x;
			_y = y;
			_z = z;
		}
		
		public override int GetHashCode()
		{
			int prime = 31;
			int result = 1;
			result = (prime * result) + _type;
			result = (prime * result) + _x;
			result = (prime * result) + _y;
			result = (prime * result) + _z;
			return result;
		}
		
		public override bool Equals(object? obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (!(obj is RadarMarker))
			{
				return false;
			}
			
			RadarMarker other = (RadarMarker) obj;
			return (_type == other._type) && (_x == other._x) && (_y == other._y) && (_z == other._z);
		}
	}
}