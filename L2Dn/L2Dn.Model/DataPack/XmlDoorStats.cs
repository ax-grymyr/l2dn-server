using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public class XmlDoorStats 
{
    [XmlAttribute("basePDef")]
    public int BasePDef { get; set; }


    [XmlAttribute("baseMDef")]
    public int BaseMDef { get; set; }

    [XmlAttribute("baseHpMax")]
    public int BaseHpMax { get; set; }
}