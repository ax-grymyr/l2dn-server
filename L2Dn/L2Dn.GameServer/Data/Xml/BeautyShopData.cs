using System.Runtime.CompilerServices;
using System.Xml.Linq;
using L2Dn.Extensions;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.BeautyShop;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Data.Xml;

/**
 * @author Sdw
 */
public class BeautyShopData: DataReaderBase
{
	private readonly Map<Race, Map<Sex, BeautyData>> _beautyList = new();
	
	protected BeautyShopData()
	{
		load();
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void load()
	{
		_beautyList.clear();

		XDocument document = LoadXmlDocument(DataFileLocation.Data, "BeautyShop.xml");
		document.Elements("list").Elements("race").ForEach(loadElement);
	}
	
	private void loadElement(XElement element)
	{
		Race race = element.Attribute("type").GetEnum<Race>();
		Map<Sex, BeautyData> map = new(); 
		element.Elements("sex").ForEach(el =>
		{
			Sex sex = el.Attribute("type").GetEnum<Sex>();
			BeautyData beautyData = new BeautyData();
			el.Elements("hair").ForEach(he =>
			{
				int id = he.GetAttributeValueAsInt32("id");
				int adena = he.Attribute("adena").GetInt32(0);
				int resetAdena = he.Attribute("reset_adena").GetInt32(0);
				int beautyShopTicket = he.Attribute("beauty_shop_ticket").GetInt32(0);
				BeautyItem hair = new BeautyItem(id, adena, resetAdena, beautyShopTicket);
				
				he.Elements("color").ForEach(ce =>
				{
					int colorId = ce.GetAttributeValueAsInt32("id");
					int colorAdena = ce.Attribute("adena").GetInt32(0);
					int colorResetAdena = ce.Attribute("reset_adena").GetInt32(0);
					int colorBeautyShopTicket = ce.Attribute("beauty_shop_ticket").GetInt32(0);
					BeautyItem color = new BeautyItem(colorId, colorAdena, colorResetAdena, colorBeautyShopTicket);
					hair.addColor(color);
				});
				
				beautyData.addHair(hair);
			});
			
			el.Elements("face").ForEach(fe =>
			{
				int id = fe.GetAttributeValueAsInt32("id");
				int adena = fe.Attribute("adena").GetInt32(0);
				int resetAdena = fe.Attribute("reset_adena").GetInt32(0);
				int beautyShopTicket = fe.Attribute("beauty_shop_ticket").GetInt32(0);
				BeautyItem face = new BeautyItem(id, adena, resetAdena, beautyShopTicket);
				beautyData.addFace(face);
			});

			map.put(sex, beautyData);
		});
						
		_beautyList.put(race, map);
	}
	
	public bool hasBeautyData(Race race, Sex sex)
	{
		return _beautyList.containsKey(race) && _beautyList.get(race).containsKey(sex);
	}
	
	public BeautyData getBeautyData(Race race, Sex sex)
	{
		if (_beautyList.containsKey(race))
		{
			return _beautyList.get(race).get(sex);
		}
		return null;
	}
	
	public static BeautyShopData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly BeautyShopData INSTANCE = new();
	}
}