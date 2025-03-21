﻿using System.Xml.Serialization;

namespace L2Dn.Model.Xml;

public class XmlAction
{
    [XmlAttribute("id")]
    public int Id { get; set; }

    [XmlAttribute("handler")]
    public string Handler { get; set; } = string.Empty;

    [XmlAttribute("option")]
    public int Option { get; set; }

    [XmlIgnore]
    public bool OptionSpecified { get; set; }
}