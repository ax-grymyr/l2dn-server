using System.Collections.Frozen;

namespace L2Dn.GameServer.Model.BeautyShop;

public class BeautyItem(int id, int adena, int resetAdena, int beautyShopTicket,
    FrozenDictionary<int, BeautyItem>? colors = null)
{
    public int getId() => id;
    public int getAdena() => adena;
    public int getResetAdena() => resetAdena;
    public int getBeautyShopTicket() => beautyShopTicket;
    public FrozenDictionary<int, BeautyItem> getColors() => colors ?? FrozenDictionary<int, BeautyItem>.Empty;
}