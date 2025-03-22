using L2Dn.Geometry;

namespace L2Dn.GameServer.Dto;

public sealed record TeleportListHolder(int TeleportId, Location3D Location, int Price, bool IsSpecial);