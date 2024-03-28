using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.BeautyShop;

public class BeautyData
{
    private readonly Map<int, BeautyItem> _hairList = new();
    private readonly Map<int, BeautyItem> _faceList = new();
	
    public void addHair(BeautyItem hair)
    {
        _hairList.put(hair.getId(), hair);
    }
	
    public void addFace(BeautyItem face)
    {
        _faceList.put(face.getId(), face);
    }
	
    public Map<int, BeautyItem> getHairList()
    {
        return _hairList;
    }
	
    public Map<int, BeautyItem> getFaceList()
    {
        return _faceList;
    }
}
