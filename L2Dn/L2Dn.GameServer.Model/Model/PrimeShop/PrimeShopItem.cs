using L2Dn.GameServer.Dto;

namespace L2Dn.GameServer.Model.PrimeShop;

public sealed record PrimeShopItem(int Id, long Count, int Weight, int IsTradable): ItemHolder(Id, Count);