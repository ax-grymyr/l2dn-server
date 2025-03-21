using L2Dn.GameServer.Enums;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Dto;

public sealed record CastleSpawnHolder(int NpcId, CastleSide Side, Location Location);