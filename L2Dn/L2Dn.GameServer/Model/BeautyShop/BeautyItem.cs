using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.BeautyShop;

public class BeautyItem
{
    private readonly int _id;
    private readonly int _adena;
    private readonly int _resetAdena;
    private readonly int _beautyShopTicket;
    private readonly Map<int, BeautyItem> _colors = new();
	
    public BeautyItem(int id, int adena, int resetAdena, int beautyShopTicket)
    {
        _id = id;
        _adena = adena;
        _resetAdena = resetAdena;
        _beautyShopTicket = beautyShopTicket;
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
	
    public void addColor(BeautyItem color)
    {
        _colors.put(color.getId(), color);
    }
	
    public Map<int, BeautyItem> getColors()
    {
        return _colors;
    }
}
