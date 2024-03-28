using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Model.Actor.Instances;

public class Fence : WorldObject
{
	private readonly int _xMin;
	private readonly int _xMax;
	private readonly int _yMin;
	private readonly int _yMax;
	
	private readonly String _name;
	private readonly int _width;
	private readonly int _length;
	
	private FenceState _state;
	private int[] _heightFences;
	
	public Fence(int x, int y, String name, int width, int length, int height, FenceState state): base(IdManager.getInstance().getNextId())
	{
		_xMin = x - (width / 2);
		_xMax = x + (width / 2);
		_yMin = y - (length / 2);
		_yMax = y + (length / 2);
		_name = name;
		_width = width;
		_length = length;
		_state = state;
		if (height > 1)
		{
			_heightFences = new int[height - 1];
			for (int i = 0; i < _heightFences.Length; i++)
			{
				_heightFences[i] = IdManager.getInstance().getNextId();
			}
		}
	}
	
	public override int getId()
	{
		return getObjectId();
	}
	
	public override String getName()
	{
		return _name;
	}
	
	public override bool isAutoAttackable(Creature attacker)
	{
		return false;
	}
	
	public override void sendInfo(Player player)
	{
		player.sendPacket(new ExColosseumFenceInfoPacket(this));
		if (_heightFences != null)
		{
			foreach (int objId in _heightFences)
			{
				player.sendPacket(new ExColosseumFenceInfoPacket(objId, getX(), getY(), getZ(), _width, _length, _state));
			}
		}
	}
	
	public override bool decayMe()
	{
		if (_heightFences != null)
		{
			DeleteObjectPacket[] deleteObjects = new DeleteObjectPacket[_heightFences.Length];
			for (int i = 0; i < _heightFences.Length; i++)
			{
				deleteObjects[i] = new DeleteObjectPacket(_heightFences[i]);
			}
			
			World.getInstance().forEachVisibleObject<Player>(this, player =>
			{
				for (int i = 0; i < _heightFences.Length; i++)
				{
					player.sendPacket(deleteObjects[i]);
				}
			});
		}
		
		return base.decayMe();
	}
	
	public bool deleteMe()
	{
		decayMe();
		
		FenceData.getInstance().removeFence(this);
		return false;
	}
	
	public FenceState getState()
	{
		return _state;
	}
	
	public void setState(FenceState type)
	{
		_state = type;
		broadcastInfo();
	}
	
	public int getWidth()
	{
		return _width;
	}
	
	public int getLength()
	{
		return _length;
	}
	
	public int getXMin()
	{
		return _xMin;
	}
	
	public int getYMin()
	{
		return _yMin;
	}
	
	public int getXMax()
	{
		return _xMax;
	}
	
	public int getYMax()
	{
		return _yMax;
	}
	
	public override bool isFence()
	{
		return true;
	}
}