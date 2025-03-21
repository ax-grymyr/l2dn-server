using System.Collections.Frozen;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.StaticData.Xml.BeautyShop;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.StaticData;

public sealed class BeautyShopData
{
    private FrozenDictionary<(Race, Sex), BeautyData> _beautyData = FrozenDictionary<(Race, Sex), BeautyData>.Empty;

    private BeautyShopData()
    {
    }

    public static BeautyShopData Instance { get; } = new();

    public void Load()
    {
        XmlBeautyShopList list = XmlLoader.LoadXmlDocument<XmlBeautyShopList>("BeautyShop.xml");
        _beautyData =
            (from raceData in list.Races
             from sexData in raceData.SexData
             let hairList =
                 (from hairData in sexData.Hairs
                  let colors =
                      (from colorData in hairData.Colors
                       select ConvertToBeautyItem(colorData)).ToFrozenDictionary(item => item.Id)
                  select ConvertToBeautyItem(hairData, colors)).ToFrozenDictionary(item => item.Id)
             let faceList =
                 (from faceData in sexData.Faces
                  select ConvertToBeautyItem(faceData)).ToFrozenDictionary(item => item.Id)
             select new BeautyData(raceData.Race, sexData.Sex, hairList, faceList)).
            ToFrozenDictionary(x => (x.Race, x.Sex));
    }

    public bool HasBeautyData(Race race, Sex sex) => _beautyData.ContainsKey((race, sex));
    public BeautyData? GetBeautyData(Race race, Sex sex) => _beautyData.GetValueOrDefault((race, sex));


    private static BeautyItem ConvertToBeautyItem(XmlBeautyShopItem item,
        FrozenDictionary<int, BeautyItem>? colors = null) =>
        new(item.Id, item.Adena, item.ResetAdena, item.BeautyShopTicket,
            colors ?? FrozenDictionary<int, BeautyItem>.Empty);
}