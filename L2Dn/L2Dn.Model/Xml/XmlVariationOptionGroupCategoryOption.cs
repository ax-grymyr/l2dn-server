﻿using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlVariationOptionGroupCategoryOption: XmlId
{
    [XmlAttribute("chance")]
    public double Chance { get; set; }

    [XmlIgnore]
    public bool ChanceSpecified { get; set; }
}