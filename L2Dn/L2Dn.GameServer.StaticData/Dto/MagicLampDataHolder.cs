using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.Dto;

public sealed record MagicLampDataHolder(LampType Type, long Exp, long Sp, double Chance, int FromLevel, int ToLevel);