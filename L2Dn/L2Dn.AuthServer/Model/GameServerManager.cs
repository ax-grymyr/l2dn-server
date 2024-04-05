using System.Collections.Concurrent;
using L2Dn.AuthServer.Configuration;
using L2Dn.AuthServer.Db;
using L2Dn.AuthServer.NetworkGameServer.OutgoingPacket;
using L2Dn.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace L2Dn.AuthServer.Model;

internal sealed class GameServerManager: ISingleton<GameServerManager>
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(GameServerManager));

    // Array with one fake server in case no game servers registered yet.
    // Client needs at least one game server to display that it is offline. 
    private static readonly GameServerInfo[] _emptyList = [new GameServerInfo() { ServerId = 1 }];
    
    private readonly ConcurrentDictionary<int, GameServerInfo> _servers = new();
    private GameServerInfo[] _serverList = _emptyList; // server list for packets

    private GameServerManager()
    {
    }

    public static GameServerManager Instance { get; } = new();

    public ReadOnlyMemory<GameServerInfo> Servers => _serverList;
    public GameServerInfo? GetServerInfo(byte serverId) => _servers.GetValueOrDefault(serverId);

    public RegistrationResult RegisterServer(GameServerInfo serverInfo)
    {
        GameServerListenerConfig gameServerListenerConfig = Config.Instance.GameServerListener;
        string configAccessKey = gameServerListenerConfig.AccessKey;
        
        if (!_servers.TryGetValue(serverInfo.ServerId, out GameServerInfo? actualValue))
        {
            if (!gameServerListenerConfig.AcceptNewGameServer)
                return RegistrationResult.NotListedInDb; // new servers not accepted

            if (!string.IsNullOrEmpty(configAccessKey) && serverInfo.AccessKey != configAccessKey)
                return RegistrationResult.InvalidAccessKey;

            if (!_servers.TryAdd(serverInfo.ServerId, serverInfo))
            {
                // another server registered with the same id
                return RegistrationResult.AnotherServerRegistered;
            }

            UpdateServerList();
            
            // set server online
            serverInfo.IsOnline = true;
            _logger.Info($"Game server {serverInfo.ServerId} is ONLINE now.");
            return RegistrationResult.Success;
        }

        // Check access key
        string expectedAccessKey =
            string.IsNullOrEmpty(actualValue.AccessKey) ? configAccessKey : actualValue.AccessKey;
        
        if (!string.IsNullOrEmpty(expectedAccessKey) && serverInfo.AccessKey != expectedAccessKey)
            return RegistrationResult.InvalidAccessKey;
        
        if (actualValue.IsOnline)
        {
            // another server registered with the same id
            return RegistrationResult.AnotherServerRegistered;
        }
        
        // If the server parameters are set from the database,
        // don't update the server address, port and other settings
        // for security reasons.
        if (!actualValue.FromDatabase)
        {
            actualValue.Address = serverInfo.Address;
            actualValue.Port = serverInfo.Port;
        }

        actualValue.AgeLimit = serverInfo.AgeLimit;
        actualValue.IsPvpServer = serverInfo.IsPvpServer;
        actualValue.Attributes = serverInfo.Attributes;
        actualValue.Brackets = serverInfo.Brackets;
        actualValue.PlayerCount = serverInfo.PlayerCount;
        actualValue.MaxPlayerCount = serverInfo.MaxPlayerCount;
            
        // set server online
        if (!actualValue.IsOnline)
        {
            actualValue.IsOnline = true;
            _logger.Info($"Game server {actualValue.ServerId} is ONLINE now.");
        }

        return RegistrationResult.Success;
    }

    public void LoadServers()
    {
        // Load servers synchronously and test db connection at the same time.
        using AuthServerDbContext context = DbFactory.Instance.CreateDbContext();

        IQueryable<GameServer> query = context.GameServers.AsNoTracking();
        foreach (GameServer server in query)
        {
            _servers.TryAdd(server.ServerId, new GameServerInfo
            {
                ServerId = server.ServerId,
                Address = ConvertAddress(server.IPAddress, server.ServerId),
                Port = server.Port,
                AgeLimit = server.AgeLimit,
                IsPvpServer = server.IsPvpServer,
                Brackets = server.Brackets,
                Attributes = server.Attributes,
                MaxPlayerCount = server.MaxPlayerCount,
                AccessKey = server.AccessKey,
                FromDatabase = true
            });
        }
        
        UpdateServerList();
        
        _logger.Info($"Loaded {_servers.Count} game servers from db");
    }

    private void UpdateServerList()
    {
        _serverList = _servers.Count == 0 ? _emptyList : _servers.Values.OrderBy(x => x.ServerId).ToArray();
    }

    private static int ConvertAddress(string address, byte serverId)
    {
        try
        {
            return IPAddressUtil.ConvertIP4AddressToInt(address);
        }
        catch (ArgumentException)
        {
            _logger.Error($"Invalid IPv4 address '{address}' in {nameof(GameServer)}s table, server id '{serverId}'");
            return 0x0100007F; // 127.0.0.1
        }
    }
}