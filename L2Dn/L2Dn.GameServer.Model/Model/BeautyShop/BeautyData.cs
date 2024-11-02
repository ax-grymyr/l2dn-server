using System.Collections.Frozen;
using L2Dn.GameServer.Db;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model.BeautyShop;

public sealed class BeautyData(
    Race race,
    Sex sex,
    FrozenDictionary<int, BeautyItem> hairList,
    FrozenDictionary<int, BeautyItem> faceList)
{
    public Race Race => race;
    public Sex Sex => sex;
    public FrozenDictionary<int, BeautyItem> getHairList() => hairList;
    public FrozenDictionary<int, BeautyItem> getFaceList() => faceList;
}