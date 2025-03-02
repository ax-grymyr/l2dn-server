using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlBeautyShopItem
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("adena")]
    public int Adena { get; set; }

    [XmlAttribute("reset_adena")]
    public int ResetAdena { get; set; }

    [XmlIgnore]
    public bool ResetAdenaSpecified { get; set; }

    [XmlAttribute("beauty_shop_ticket")]
    public int BeautyShopTicket { get; set; }

    [XmlIgnore]
    public bool BeautyShopTicketSpecified { get; set; }
}