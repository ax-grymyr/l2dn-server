using System.Text;
using System.Xml.Schema;
using System.Xml.Serialization;
using L2Dn.Model.Xml.Skills;

XmlSchemas schemas = new();
XmlSchemaExporter exporter = new(schemas);
XmlTypeMapping mapping = new XmlReflectionImporter().ImportTypeMapping(typeof(XmlSkillList));
exporter.ExportTypeMapping(mapping);

using FileStream fileStream = File.Open(@"..\..\..\..\..\L2Dn.GameServer\DataPack\xsd\skills.xsd", FileMode.Create,
    FileAccess.ReadWrite, FileShare.None);

using StreamWriter writer = new StreamWriter(fileStream, Encoding.UTF8);
foreach (XmlSchema schema in schemas)
    schema.Write(writer);