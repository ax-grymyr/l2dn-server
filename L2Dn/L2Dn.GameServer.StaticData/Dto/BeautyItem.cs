using System.Collections.Frozen;

namespace L2Dn.GameServer.Dto;

public sealed record BeautyItem(int Id, int Adena, int ResetAdena, int BeautyShopTicket,
    FrozenDictionary<int, BeautyItem> Colors);