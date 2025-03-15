﻿using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.StaticData;

namespace L2Dn.GameServer;

public static class StaticDataLoader
{
    public static void Load()
    {
        // Config files
        Config.Load();

        // Xml files
        MapRegionData.Instance.Load();
    }
}