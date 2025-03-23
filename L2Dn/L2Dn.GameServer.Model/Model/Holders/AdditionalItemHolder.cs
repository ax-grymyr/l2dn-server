using L2Dn.GameServer.Dto;

namespace L2Dn.GameServer.Model.Holders;

public sealed record AdditionalItemHolder(int Id, bool AllowedToUse): ItemHolder(Id, 0);