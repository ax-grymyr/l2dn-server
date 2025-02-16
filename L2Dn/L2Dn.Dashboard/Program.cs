using System.Net.Sockets;

var builder = DistributedApplication.CreateBuilder(args);

var authServer = builder.AddProject<Projects.L2Dn_AuthServer>("AuthServer")
    .WithEndpoint("AuthServerEndpoint1", ea =>
    {
        ea.Protocol = ProtocolType.Tcp;
        ea.Port = 2106;
        ea.TargetPort = 2106;
        ea.IsProxied = false;
    })
    .WithEndpoint("AuthServerEndpoint2", ea =>
    {
        ea.Protocol = ProtocolType.Tcp;
        ea.Port = 2107;
        ea.TargetPort = 2107;
        ea.IsProxied = false;
    });

builder.AddProject<Projects.L2Dn_GameServer>("GameServer")
    .WithEndpoint("GameServerEndpoint", ea =>
    {
        ea.Protocol = ProtocolType.Tcp;
        ea.Port = 7777;
        ea.TargetPort = 7777;
        ea.IsProxied = false;
    })
    .WithReference(authServer)
    .WaitFor(authServer);

builder.Build().Run();
