using System.Xml.Serialization;
using L2Dn.Model.Enums;

namespace L2Dn.Model.DataPack;

public class XmlInstanceReenter 
{
    [XmlElement("reset")]
    public List<XmlInstanceReenterReset> Resets { get; set; } = [];

    [XmlAttribute("apply")]
    public InstanceReenterType Apply { get; set; }
}