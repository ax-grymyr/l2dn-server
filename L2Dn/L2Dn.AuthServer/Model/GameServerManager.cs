using System.Collections.Immutable;
using L2Dn.AuthServer.Configuration;
using L2Dn.AuthServer.Db;
using L2Dn.AuthServer.Network.GameServer.OutgoingPacket;
using L2Dn.Logging;
using L2Dn.Utilities;
using Microsoft.EntityFrameworkCore;

namespace L2Dn.AuthServer.Model;

internal sealed class GameServerManager: ISingleton<GameServerManager>
{
    private ImmutableSortedSet<GameServerInfo> _servers = ImmutableSortedSet<GameServerInfo>.Empty;

    private GameServerManager()
    {
        LoadServers();
    }

    public static GameServerManager Instance { get; } = new();

    public ImmutableSortedSet<GameServerInfo> Servers => _servers;

    public GameServerInfo? GetServerInfo(byte serverId)
    {
        ImmutableSortedSet<GameServerInfo> servers = _servers;
        foreach (GameServerInfo server in servers)
        {
            if (server.ServerId == serverId)
                return server;
        }

        return null;
    }

    public RegistrationResult RegisterServer(GameServerInfo serverInfo)
    {
        GameServerListenerConfig gameServerListenerConfig = Config.Instance.GameServerListener;
        string configAccessKey = gameServerListenerConfig.AccessKey;
        
        if (!_servers.TryGetValue(serverInfo, out GameServerInfo? actualValue))
        {
            if (!gameServerListenerConfig.AcceptNewGameServer)
                return RegistrationResult.NotListedInDb; // new servers not accepted

            if (!string.IsNullOrEmpty(configAccessKey) && serverInfo.AccessKey != configAccessKey)
                return RegistrationResult.InvalidAccessKey;

            // Atomically assign the new set
            if (!ImmutableInterlocked.Update(ref _servers, (set, info) => set.Add(info), serverInfo))
            {
                // another server registered with the same id
                return RegistrationResult.AnotherServerRegistered;
            }
            
            // set server online
            serverInfo.IsOnline = true;
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
        serverInfo.IsOnline = true;
        return RegistrationResult.Success;
    }

    private void LoadServers()
    {
        // Load servers synchronously and test db connection at the same time.
        using AuthServerDbContext context = new();

        _servers = context.GameServers.AsNoTracking()
            .Select(server => new GameServerInfo
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
            }).ToImmutableSortedSet(new GameServerInfoComparer());
    }

    private static int ConvertAddress(string address, byte serverId)
    {
        try
        {
            return IPAddressUtil.ConvertIP4AddressToInt(address);
        }
        catch (ArgumentException exception)
        {
            Logger.Error($"Invalid IPv4 address '{address}' in {nameof(GameServer)}s table, server id '{serverId}'");
            return 0x0100007F; // 127.0.0.1
        }
    }
}