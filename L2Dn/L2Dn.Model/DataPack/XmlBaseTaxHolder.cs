using System.Xml.Serialization;

namespace L2Dn.Model.DataPack;

public abstract class XmlBaseTaxHolder
{
    [XmlAttribute(AttributeName = "baseTax")]
    public int BaseTax { get; set; }
    
    [XmlIgnore]
    public bool BaseTaxSpecified { get; set; }
}