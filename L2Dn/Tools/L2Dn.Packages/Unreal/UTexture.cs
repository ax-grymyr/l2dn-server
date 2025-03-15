using System.Diagnostics;
using L2Dn.Packages.Textures;

namespace L2Dn.Packages.Unreal;

public class UTexture: UMaterial
{
    internal UTexture(UExport export): base(export)
    {
    }

    public int Width { get; set; }
    public int Height { get; set; }
    public UTextureFormat Format { get; set; } = UTextureFormat.Rgba8;
    public int UClamp { get; set; }
    public int VClamp { get; set; }
    public string TextureName { get; set; } = string.Empty;
    public string Lineage2Name { get; set; } = string.Empty;
    public List<UBitmap> Bitmaps { get; set; } = new();

    static UTexture()
    {
        RegisterProperty<UTexture>("Format", (obj, value) => obj.Format = (UTextureFormat)(byte)value);
        RegisterProperty<UTexture>("USize", (obj, value) => obj.Width = (int)value);
        RegisterProperty<UTexture>("VSize", (obj, value) => obj.Height = (int)value);
        RegisterProperty<UTexture>("UClamp", (obj, value) => obj.UClamp = (int)value);
        RegisterProperty<UTexture>("VClamp", (obj, value) => obj.VClamp = (int)value);
        //RegisterProperty<UTexture>("Palette", (obj, value) => obj.Palette = (int)value);
        // bAlphaTexture
        // bMasked
    }

    public override void Read(UBinaryReader reader)
    {
        base.Read(reader);
        Lineage2Name = ReadGarbage(reader);
        
        switch (Format)
        {
            case UTextureFormat.G16:
                Bitmaps = ReadBitmaps<UBitmapG16>(reader);
                break;

            case UTextureFormat.Dxt1:
                Bitmaps = ReadBitmaps<UBitmapDxt1>(reader);
                break;

            case UTextureFormat.Dxt3:
                Bitmaps = ReadBitmaps<UBitmapDxt3>(reader);
                break;

            case UTextureFormat.Dxt5:
                Bitmaps = ReadBitmaps<UBitmapDxt5>(reader);
                break;

            case UTextureFormat.Rgba8:
                Bitmaps = ReadBitmaps<UBitmapRgba8>(reader);
                break;

            default:
                throw new NotSupportedException($"Texture format '{Format}' is not supported");
        }
    }

    private static List<UBitmap> ReadBitmaps<T>(UBinaryReader reader)
        where T: UBitmap, new()
    {
        int count = reader.ReadIndex();
        return reader.ReadObjects<T>(count).Cast<UBitmap>().ToList();
    }

    private static string ReadGarbage(UBinaryReader reader)
    {
        string lineage2Name = string.Empty;
        
        if (reader is { PackageVersion: >= 123, LicenseeVersion: >= 16 and < 37 })
            reader.ReadInt32(); // Unknown int in Lineage 2

        if (reader is { PackageVersion: >= 123, LicenseeVersion: >= 30 and < 37 })
        {
            int i;
            // some function
            byte materialInfo, TextureTranform, MAX_SAMPLER_NUM, MAX_TEXMAT_NUM, MAX_PASS_NUM, TwoPassRenderState, AlphaRef;
            if (reader.LicenseeVersion >= 33 && reader.LicenseeVersion < 36)
                materialInfo = reader.ReadByte();

            TextureTranform = reader.ReadByte();
            MAX_SAMPLER_NUM = reader.ReadByte();
            MAX_TEXMAT_NUM = reader.ReadByte();
            MAX_PASS_NUM = reader.ReadByte();
            TwoPassRenderState = reader.ReadByte();
            AlphaRef = reader.ReadByte();

            int SrcBlend, DestBlend, OverriddenFogColor;
            SrcBlend = reader.ReadInt32();
            DestBlend = reader.ReadInt32();
            OverriddenFogColor = reader.ReadInt32();

            // serialize matTexMatrix[16] (strange code)
            for (i = 0; i < 8; i++)
            {
                byte b1, b2;
                b1 = reader.ReadByte();
                if (reader.LicenseeVersion < 36) 
                    b2 = reader.ReadByte();
                for (int j = 0; j < 126; j++)
                {
                    // really, 1Kb of floats and ints ...
                    byte b3 = reader.ReadByte();
                }
            }

            // another nested function - serialize FC_* variables
            // union with "int FC_Color1, FC_Color2" (strange byte order)
            //Ar << c[2] << c[1] << c[0] << c[3] << c[6] << c[5] << c[4] << c[7];
            reader.ReadInt64();
         
            int FC_FadePeriod, FC_FadePhase, FC_ColorFadeType; // really, floats?
            FC_FadePeriod = reader.ReadInt32();
            FC_FadePhase = reader.ReadInt32();
            FC_ColorFadeType = reader.ReadInt32();

            // end of nested function
            for (i = 0; i < 16; i++)
            {
                string strTex = reader.ReadUString(); // strTex[16]
            }
            // end of function
            
            string ShaderCode = reader.ReadUString();
        }

        if (reader.PackageVersion >= 123 && reader.LicenseeVersion >= 37)
        {
            // ShaderProperty + ShaderCode
            FLineageShaderProperty ShaderProp = default;
            ShaderProp.Read(reader);
            string ShaderCode = reader.ReadUString();

            if (ShaderProp.Stages != null && ShaderProp.Stages.Length > 0)
            {
                if (ShaderProp.Stages.Length > 1 || ShaderProp.Stages[0].Unknown2.Length > 0)
                    Debugger.Break();
                    
                lineage2Name = ShaderProp.Stages[0].Unknown1;
            }
        }
        
        if (reader.PackageVersion >= 123 && reader.LicenseeVersion >= 31)
        {
            ushort ver1, ver2; // 'int MaterialCodeVersion' serialized as 2 words
            ver1 = reader.ReadUInt16();
            ver2 = reader.ReadUInt16();
        }

        return lineage2Name;
    }
    
    private struct FLineageMaterialStageProperty: ISerializableObject
    {
        public string Unknown1;
        public string[] Unknown2;
        
        public void Read(UBinaryReader reader)
        {
            Unknown1 = reader.ReadUString();

            int length = reader.ReadIndex();
            if (length == 0)
                Unknown2 = Array.Empty<string>();
            else
            {
                Unknown2 = new string[length];
                for (int i = 0; i < length; i++)
                    Unknown2[i] = reader.ReadUString();
            }
        }
    }

    private struct FLineageShaderProperty: ISerializableObject
    {
        // possibly, MaterialInfo, TextureTranform, TwoPassRenderState, AlphaRef
        public byte b1; 
        public byte b2;
        public byte b3_0;
        public byte b3_1;
        public byte b3_2;
        public byte b3_3;
        public byte b3_4;
        public byte b4_0;
        public byte b4_1;
        public byte b4_2;
        public byte b4_3;
        public byte b4_4;
        public byte b5_0;
        public byte b5_1;
        public byte b5_2;
        public byte b5_3;
        public byte b5_4;
        public byte b6_0;
        public byte b6_1;
        public byte b6_2;
        public byte b6_3;
        public byte b6_4;
        public byte b7_0;
        public byte b7_1;
        public byte b7_2;
        public byte b7_3;
        public byte b7_4;
        public byte b8_0;
        public byte b8_1;
        public byte b8_2;
        public byte b8_3;
        public byte b8_4;
        
        // possibly, SrcBlend, DestBlend, OverriddenFogColor
        public int i1_0;
        public int i1_1;
        public int i1_2;
        public int i1_3;
        public int i1_4;
        public int i2_0;
        public int i2_1;
        public int i2_2;
        public int i2_3;
        public int i2_4;
        public int i3_0;
        public int i3_1;
        public int i3_2;
        public int i3_3;
        public int i3_4;

        // nested structure
        // possibly, int FC_Color1, FC_Color2 (strange byte order)
        public long be; // 8 bytes
        // possibly, float FC_FadePeriod, FC_FadePhase, FC_ColorFadeType
        public int ie_0;
        public int ie_1;
        public int ie_2;

        // stages
        public FLineageMaterialStageProperty[] Stages;

        public void Read(UBinaryReader reader)
        {
            b1 = reader.ReadByte();
            b2 = reader.ReadByte();

            switch (reader.PackageVersion)
            {
                case < 129:
                    b3_0 = reader.ReadByte();
                    b4_0 = reader.ReadByte();
                    i1_0 = reader.ReadInt32();
                    i2_0 = reader.ReadInt32();
                    i3_0 = reader.ReadInt32();
                    break;
                
                case 129:
                    b3_0 = reader.ReadByte();
                    b4_0 = reader.ReadByte();
                    i1_0 = reader.ReadInt32();
                    i2_0 = reader.ReadInt32();
                    i3_0 = reader.ReadInt32();

                    b3_1 = reader.ReadByte();
                    b4_1 = reader.ReadByte();
                    i1_1 = reader.ReadInt32();
                    i2_1 = reader.ReadInt32();
                    i3_1 = reader.ReadInt32();

                    b3_2 = reader.ReadByte();
                    b4_2 = reader.ReadByte();
                    i1_2 = reader.ReadInt32();
                    i2_2 = reader.ReadInt32();
                    i3_2 = reader.ReadInt32();

                    b3_3 = reader.ReadByte();
                    b4_3 = reader.ReadByte();
                    i1_3 = reader.ReadInt32();
                    i2_3 = reader.ReadInt32();
                    i3_3 = reader.ReadInt32();

                    b3_4 = reader.ReadByte();
                    b4_4 = reader.ReadByte();
                    i1_4 = reader.ReadInt32();
                    i2_4 = reader.ReadInt32();
                    i3_4 = reader.ReadInt32();
                    break;
                
                default: // PackageVersion >= 130
                    b3_0 = reader.ReadByte();
                    b4_0 = reader.ReadByte();
                    b5_0 = reader.ReadByte();
                    b6_0 = reader.ReadByte();
                    b7_0 = reader.ReadByte();
                    b8_0 = reader.ReadByte();
                    i2_0 = reader.ReadInt32();
                    i3_0 = reader.ReadInt32();

                    b3_1 = reader.ReadByte();
                    b4_1 = reader.ReadByte();
                    b5_1 = reader.ReadByte();
                    b6_1 = reader.ReadByte();
                    b7_1 = reader.ReadByte();
                    b8_1 = reader.ReadByte();
                    i2_1 = reader.ReadInt32();
                    i3_1 = reader.ReadInt32();

                    b3_2 = reader.ReadByte();
                    b4_2 = reader.ReadByte();
                    b5_2 = reader.ReadByte();
                    b6_2 = reader.ReadByte();
                    b7_2 = reader.ReadByte();
                    b8_2 = reader.ReadByte();
                    i2_2 = reader.ReadInt32();
                    i3_2 = reader.ReadInt32();

                    b3_3 = reader.ReadByte();
                    b4_3 = reader.ReadByte();
                    b5_3 = reader.ReadByte();
                    b6_3 = reader.ReadByte();
                    b7_3 = reader.ReadByte();
                    b8_3 = reader.ReadByte();
                    i2_3 = reader.ReadInt32();
                    i3_3 = reader.ReadInt32();

                    b3_4 = reader.ReadByte();
                    b4_4 = reader.ReadByte();
                    b5_4 = reader.ReadByte();
                    b6_4 = reader.ReadByte();
                    b7_4 = reader.ReadByte();
                    b8_4 = reader.ReadByte();
                    i2_4 = reader.ReadInt32();
                    i3_4 = reader.ReadInt32();
                    break;
            }

            be = reader.ReadInt64();
            ie_0 = reader.ReadInt32();
            ie_1 = reader.ReadInt32();
            ie_2 = reader.ReadInt32();

            int length = reader.ReadIndex();
            if (length == 0)
                Stages = Array.Empty<FLineageMaterialStageProperty>();
            else
            {
                Stages = new FLineageMaterialStageProperty[length];
                for (int i = 0; i < length; i++)
                    Stages[i].Read(reader);
            }
        }
    }
}