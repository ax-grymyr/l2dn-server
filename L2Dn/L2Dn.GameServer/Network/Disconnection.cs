using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.TaskManagers;
using L2Dn.Network;
using L2Dn.Packets;
using NLog;

namespace L2Dn.GameServer.Network;

internal class Disconnection
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(Disconnection));
	
	public static Connection<GameSession>? getClient(Connection<GameSession>? client, Player? player)
	{
		if (client != null)
		{
			return client;
		}
		
		if (player != null)
		{
			return player.getClient();
		}
		
		return null;
	}
	
	public static Player getActiveChar(Connection<GameSession>? client, Player? player)
	{
		if (player != null)
		{
			return player;
		}
		
		if (client != null)
		{
			return client.Session.Player;
		}
		
		return null;
	}
	
	private readonly Connection<GameSession> _client;
	private readonly Player _player;
	
	private Disconnection(Connection<GameSession> client): this(client, null)
	{
	}
	
	public static Disconnection of(Connection<GameSession> client)
	{
		return new Disconnection(client);
	}
	
	private Disconnection(Player player): this(null, player)
	{
	}
	
	public static Disconnection of(Player player)
	{
		return new Disconnection(player);
	}
	
	private Disconnection(Connection<GameSession> client, Player player)
	{
		_client = getClient(client, player);
		_player = getActiveChar(client, player);
		
		// Stop player tasks.
		if (_player != null)
		{
			_player.stopAllTasks();
		}
		
		// Anti Feed
		AntiFeedManager.getInstance().onDisconnect(_client);
		
		if (_client != null)
		{
			_client.Session.Player = null;
		}
		
		if (_player != null)
		{
			_player.setClient(null);
		}
	}
	
	public static Disconnection of(Connection<GameSession> client, Player player)
	{
		return new Disconnection(client, player);
	}
	
	public Disconnection storeMe()
	{
		try
		{
			if (_player != null)
			{
				_player.storeMe();
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(e);
		}
		
		return this;
	}
	
	public Disconnection deleteMe()
	{
		try
		{
			if ((_player != null) && _player.isOnline())
			{
				_player.deleteMe();
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(e);
		}
		
		return this;
	}
	
	public Disconnection close<TPacket>(TPacket packet)
		where TPacket: struct, IOutgoingPacket
	{
		if (_client != null)
		{
			_client.Send(ref packet, SendPacketOptions.CloseAfterSending);
		}
		
		return this;
	}
	
	public void defaultSequence<TPacket>(TPacket packet)
		where TPacket: struct, IOutgoingPacket
	{
		defaultSequence();
		close(packet);
	}
	
	private void defaultSequence()
	{
		storeMe();
		deleteMe();
	}
	
	public void onDisconnection()
	{
		if (_player != null)
		{
			Utilities.ThreadPool.schedule(defaultSequence,
				_player.canLogout() ? TimeSpan.Zero : AttackStanceTaskManager.COMBAT_TIME);
		}
	}
}