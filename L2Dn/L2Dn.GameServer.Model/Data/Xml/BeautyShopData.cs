using System.Collections.Frozen;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.BeautyShop;
using L2Dn.Model.Enums;
using L2Dn.Model.Xml;

namespace L2Dn.GameServer.Data.Xml;

public sealed class BeautyShopData: DataReaderBase
{
	private FrozenDictionary<(Race, Sex), BeautyData> _beautyData = FrozenDictionary<(Race, Sex), BeautyData>.Empty;

	private BeautyShopData()
	{
		Load();
	}

	private void Load()
	{
		static BeautyItem ConvertToBeautyItem(XmlBeautyShopItem item, FrozenDictionary<int, BeautyItem>? colors = null)
			=> new(item.Id, item.Adena, item.ResetAdena, item.BeautyShopTicket, colors);

		XmlBeautyShopData data = LoadXmlDocument<XmlBeautyShopData>(DataFileLocation.Data, "BeautyShop.xml");
		_beautyData =
			(from raceData in data.Races
				let race = Enum.Parse<Race>(raceData.Type, true)
				from sexData in raceData.SexData
				let sex = Enum.Parse<Sex>(sexData.Type, true)
				let hairList =
					(from hairData in sexData.Hairs
						let colors =
							(from colorData in hairData.Colors
								select ConvertToBeautyItem(colorData))
							.ToFrozenDictionary(item => item.getId())
						select ConvertToBeautyItem(hairData, colors))
					.ToFrozenDictionary(item => item.getId())
				let faceList =
					(from faceData in sexData.Faces
						select ConvertToBeautyItem(faceData))
					.ToFrozenDictionary(item => item.getId())
				select new BeautyData(race, sex, hairList, faceList))
			.ToFrozenDictionary(x => (x.Race, x.Sex));
	}

	public bool hasBeautyData(Race race, Sex sex)
	{
		return _beautyData.ContainsKey((race, sex));
	}

	public BeautyData? getBeautyData(Race race, Sex sex)
	{
		return _beautyData.GetValueOrDefault((race, sex));
	}

	public static BeautyShopData getInstance()
	{
		return SingletonHolder.Instance;
	}

	private static class SingletonHolder
	{
		public static readonly BeautyShopData Instance = new();
	}
}