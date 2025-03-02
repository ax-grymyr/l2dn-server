﻿using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlEnchantScrollStep
{
    [XmlAttribute("level")]
    public int Level { get; set; }

    [XmlAttribute("successRate")]
    public double SuccessRate { get; set; }
}