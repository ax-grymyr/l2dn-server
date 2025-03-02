﻿using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlDoorEvent
{
    [XmlAttribute("masterClose")]
    public string MasterClose { get; set; } = string.Empty;

    [XmlAttribute("masterOpen")]
    public string MasterOpen { get; set; } = string.Empty;
}