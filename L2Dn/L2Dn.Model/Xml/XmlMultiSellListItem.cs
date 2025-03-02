﻿using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlMultiSellListItem
{
    [XmlElement("ingredient")]
    public List<XmlMultiSellListIngredient> Ingredients { get; set; } = [];

    [XmlElement("production", Type = typeof(XmlMultiSellListProduct))]
    public List<XmlMultiSellListProduct> Products { get; set; } = [];
}