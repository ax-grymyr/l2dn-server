﻿using L2Dn.GameServer.Configuration;
using L2Dn.Network;

namespace L2Dn.GameServer.NetworkAuthServer;

internal sealed class AuthServerSession: Session
{
    public ServerConfig Config => ServerConfig.Instance;
}