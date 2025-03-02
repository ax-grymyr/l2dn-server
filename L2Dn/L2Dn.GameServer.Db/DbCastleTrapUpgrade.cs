﻿using Microsoft.EntityFrameworkCore;

namespace L2Dn.GameServer.Db;

[PrimaryKey(nameof(TowerIndex), nameof(CastleId))]
public class DbCastleTrapUpgrade
{
    public short CastleId { get; set; }
    public short TowerIndex { get; set; }
    public short Level { get; set; }
}