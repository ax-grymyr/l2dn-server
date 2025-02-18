using System.Text;
using System.Xml.Linq;
using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn;

public class GenerateDatDefinitionsFromXml
{
    private const string BasePath = @"D:\Temp\000";

    //[Fact]
    public void GenerateDefs()
    {
        StringBuilder sb = new StringBuilder();

        var files = Directory.EnumerateFiles(BasePath, "*.xml");
        foreach (string file in files)
        {
            string name = Path.GetFileNameWithoutExtension(file).Substring(3);
            sb.AppendLine($"        dictionary[Chronicles.{name}] =");
            sb.AppendLine("        [");

            XDocument document = XDocument.Load(file);
            var elements = document.Elements("list").Elements("link");
            foreach (XElement element in elements)
            {
                string pattern = element.Attribute("pattern")?.Value ??
                    throw new InvalidOperationException("Missing pattern attribute");

                string className = element.Attribute("file")?.Value ??
                    throw new InvalidOperationException("Missing file attribute");

                string version = element.Attribute("version")?.Value ??
                    throw new InvalidOperationException("Missing version attribute");

                className = className.Replace("-", "");

                sb.AppendLine($"            new DatFileDefinition(@\"{pattern}\", typeof({version}.{className})),");
            }

            sb.AppendLine("        ];");
            sb.AppendLine();
        }

        File.WriteAllText(Path.Combine(BasePath, "get.txt"), sb.ToString(), Encoding.UTF8);
    }

    //[Fact]
    public void GenerateClasses()
    {
        Dictionary<string, SortedSet<string>> classes = new Dictionary<string, SortedSet<string>>();
        var files = Directory.EnumerateFiles(Path.Combine(BasePath, "dats"), "*.xml");
        foreach (string file in files)
        {
            string name = Path.GetFileNameWithoutExtension(file);
            if (char.IsDigit(name[0]))
                continue;

            XDocument document = XDocument.Load(file);
            var elements = document.Elements("list").Elements("file");
            foreach (XElement element in elements)
            {
                string version = element.Attribute("pattern")?.Value ?? // TODO: Check if this is correct
                    throw new InvalidOperationException("Missing pattern attribute");

                string className = name.Replace("-", "");

                GenerateClass(version, className, element, classes);
            }
        }
    }

    private static ClassDef GetClassDef(string version, string className, XElement element,
        Dictionary<string, SortedSet<string>> classes)
    {
        ClassDef classDef = new ClassDef(version, className, false);

        XElement classElem = element;
        XElement[] subElems = element.Elements().ToArray();
        if (subElems.Length == 2 && subElems[0].Name.LocalName == "node" &&
            subElems[1].Name.LocalName == "for" &&
            subElems[1].Attribute("size")?.Value == "#" + subElems[0].Attribute("name")?.Value)
        {
            classElem = subElems[1];
            classDef = classDef with { IsArray = true };
        }

        XElement[] propElems = classElem.Elements().ToArray();
        for (int i = 0; i < propElems.Length; i++)
        {
            XElement propElem = propElems[i];
            string propName = propElem.Attribute("name")?.Value ??
                throw new InvalidOperationException("Missing name attribute");

            if (propElem.Name.LocalName == "node")
            {
                string propType = propElem.Attribute("reader")?.Value ??
                    throw new InvalidOperationException("Missing reader attribute");

                classDef.Properties.Add(new PropertyDef(propName, new PropertyTypeDef(propType, false)));
            }
            else if (propElem.Name.LocalName == "for")
            {
                string size = propElem.Attribute("size")?.Value ??
                    throw new InvalidOperationException("Missing size attribute");

                if (size.StartsWith("#"))
                {
                    string sizePropName = size.Substring(1);
                    var lastProp = classDef.Properties[^1];
                    if (lastProp.Name != sizePropName)
                        throw new NotSupportedException();

                    classDef.Properties.RemoveAt(classDef.Properties.Count - 1);

                    PropertyTypeDef arrayElemType = GetArrayElemType(version, className, propElem, classes);

                    ArrayLengthType arrayLengthType;
                    if (lastProp.Type.TypeName == "UINT")
                        arrayLengthType = ArrayLengthType.Int32;
                    else if (lastProp.Type.TypeName == "UCHAR")
                        arrayLengthType = ArrayLengthType.Int16;
                    else if (lastProp.Type.TypeName == "CNTR")
                        arrayLengthType = ArrayLengthType.CompactInt;
                    else if (lastProp.Type.TypeName == "UBYTE")
                        arrayLengthType = ArrayLengthType.Byte;
                    else
                        throw new NotSupportedException();

                    classDef.Properties.Add(new PropertyDef(propName,
                        arrayElemType with { IsArray = true, LengthType = arrayLengthType }));
                }
                else if (int.TryParse(size, out int sz))
                {
                    PropertyTypeDef arrayElemType = GetArrayElemType(version, className, propElem, classes);
                    classDef.Properties.Add(new PropertyDef(propName,
                        arrayElemType with { IsArray = true, LengthType = ArrayLengthType.Fixed, Size = sz }));
                }
                else
                    throw new NotSupportedException();
            }
            else if (propElem.Name.LocalName == "wrapper")
            {
                string propType = className + "_" + propName;
                GenerateClass(version, propType, propElem, classes);
                classDef.Properties.Add(new PropertyDef(propName, new PropertyTypeDef(propType, true)));
            }
        }

        return classDef;
    }

    private static PropertyTypeDef GetArrayElemType(string version, string className, XElement propElem,
        Dictionary<string, SortedSet<string>> classes)
    {
        XElement[] elems = propElem.Elements().ToArray();
        if (elems.Length == 1 && elems[0].Name.LocalName == "node")
        {
            string elemType = elems[0].Attribute("reader")?.Value ??
                throw new InvalidOperationException("Missing reader attribute");

            return new PropertyTypeDef(elemType, false);
        }

        string propName = propElem.Attribute("name")?.Value ??
            throw new InvalidOperationException("Missing name attribute");

        GenerateClass(version, className + "_" + propName, propElem, classes);
        return new PropertyTypeDef(className + "_" + propName, true);
    }

    private static void GenerateClass(string version, string className, XElement element, Dictionary<string, SortedSet<string>> classes)
    {
        ClassDef classDef = GetClassDef(version, className, element, classes);

        StringBuilder sb = new StringBuilder();

        sb.AppendLine($"namespace L2Dn.Packages.DatDefinitions.{classDef.Version};");
        sb.AppendLine();

        sb.AppendLine($"public class {classDef.Name}");
        sb.AppendLine("{");

        foreach (PropertyDef propertyDef in classDef.Properties)
        {
            PropertyTypeDef propertyType = propertyDef.Type;
            string propName = propertyDef.Name;
            bool isArray = propertyType.IsArray;
            if (isArray)
            {
                switch (propertyType.LengthType)
                {
                    case ArrayLengthType.CompactInt:
                        break;

                    case ArrayLengthType.Fixed:
                        sb.AppendLine($"    [ArrayLengthType(ArrayLengthType.Fixed, {propertyType.Size})]");
                        break;

                    default:
                        sb.AppendLine($"    [ArrayLengthType(ArrayLengthType.{propertyType.LengthType})]");
                        break;
                }
            }

            if (propertyType.TypeName == "UNICODE")
                sb.AppendLine("    [StringType(StringType.Unicode)]");

            string type = propertyType.IsClass
                ? propertyType.TypeName
                : propertyType.TypeName switch
                {
                    "UNICODE" => "string",
                    "ASCF" => "string",
                    "UINT" => "uint",
                    "MAP_INT" => "int",
                    "INT" => "int",
                    "UBYTE" => "byte",
                    "USHORT" => "ushort",
                    "SHORT" => "short",
                    "UCHAR" => "char",
                    "FLOAT" => "float",
                    "DOUBLE" => "double",
                    "RGBA_TEST" => "RgbaTest",
                    "RGB" => "Rgb",
                    "RGBA" => "Rgba",
                    "MTX" => "Mtx",
                    "MTX_NEW2" => "MtxNew2",
                    "MTX3" => "Mtx3",
                    "MTX3_NEW2" => "Mtx3New2",
                    _ => throw new NotSupportedException()
                };

            if (isArray)
                sb.AppendLine($"    public {type}[] {propName} {{ get; set; }} = Array.Empty<{type}>();");
            else if (type == "string")
                sb.AppendLine($"    public {type} {propName} {{ get; set; }} = string.Empty;");
            else
                sb.AppendLine($"    public {type} {propName} {{ get; set; }}");
        }

        sb.AppendLine("}");

        sb.AppendLine();

        Directory.CreateDirectory(Path.Combine(BasePath, "!Classes", version));
        File.WriteAllText(Path.Combine(BasePath, "!Classes", version, className + ".cs"), sb.ToString(), Encoding.UTF8);
    }

    private record ClassDef(string Version, string Name, bool IsArray)
    {
        public List<PropertyDef> Properties { get; } = new();
    }

    private record PropertyDef(string Name, PropertyTypeDef Type);

    private record PropertyTypeDef(
        string TypeName,
        bool IsClass,
        bool IsArray = false,
        ArrayLengthType LengthType = ArrayLengthType.CompactInt,
        int Size = -1);
}