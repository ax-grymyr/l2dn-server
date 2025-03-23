using L2Dn.GameServer.Dto;

namespace L2Dn.GameServer.Model.Items;

public sealed record PlayerItemTemplate(int Id, long Count, bool Equipped): ItemHolder(Id, Count);