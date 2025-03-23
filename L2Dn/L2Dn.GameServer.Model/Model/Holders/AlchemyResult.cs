using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.Model.Holders;

public sealed record AlchemyResult(int Id, long Count, TryMixCubeResultType Type): ItemHolder(Id, Count);