using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.BeautyShop;

public class BeautyItem
{
    private readonly int _id;
    private readonly int _adena;
    private readonly int _resetAdena;
    private readonly int _beautyShopTicket;
    private readonly Map<int, BeautyItem> _colors = new();
	
    public BeautyItem(StatSet set)
    {
        _id = set.getInt("id");
        _adena = set.getInt("adena", 0);
        _resetAdena = set.getInt("reset_adena", 0);
        _beautyShopTicket = set.getInt("beauty_shop_ticket", 0);
    }
	
    public int getId()
    {
        return _id;
    }
	
    public int getAdena()
    {
        return _adena;
    }
	
    public int getResetAdena()
    {
        return _resetAdena;
    }
	
    public int getBeautyShopTicket()
    {
        return _beautyShopTicket;
    }
	
    public void addColor(StatSet set)
    {
        BeautyItem color = new BeautyItem(set);
        _colors.put(set.getInt("id"), color);
    }
	
    public Map<int, BeautyItem> getColors()
    {
        return _colors;
    }
}
