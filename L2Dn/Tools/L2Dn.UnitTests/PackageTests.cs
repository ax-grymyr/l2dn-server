using System.Diagnostics;
using System.Text;
using L2Dn.Packages;
using L2Dn.Packages.Textures;
using L2Dn.Packages.Unreal;
using SixLabors.ImageSharp;

namespace L2Dn;

public class PackageTests
{
    [Fact]
    public void Test1()
    {
        UPackageManager packageManager = new();
        UPackage package = UPackage.LoadFrom(packageManager, @"E:\L2\L2C4\textures\t_18_20.utx.decrypted");

        IEnumerable<UTexture> list = package.Exports.Where(x => x.Class?.Name == "Texture")
            .Select(x => (UTexture)x.Object);

        foreach (UTexture texture in list)
        {
            for (int index = 0; index < texture.Bitmaps.Count; index++)
            {
                UBitmap bitmap = texture.Bitmaps[index];
                string fileName = $@"E:\L2\L2C4\textures\t_18_20.utx.{texture.Name}.{index + 1}.png";
                bitmap.Image?.SaveAsPng(fileName);
            }
        }
    }

    [Fact]
    public void Test2()
    {
        UPackageManager packageManager = new();
        UPackage package = UPackage.LoadFrom(packageManager , @"E:\L2\L2C4\maps\18_20.unr.decrypted");

        IEnumerable<UTexture> list = package.Exports.Where(x => x.Class?.Name == "Texture")
            .Select(x => (UTexture)x.Object);

        foreach (UTexture texture in list)
        {
            for (int index = 0; index < texture.Bitmaps.Count; index++)
            {
                UBitmap bitmap = texture.Bitmaps[index];
                string fileName = $@"E:\L2\L2C4\maps\18_20.unr.{texture.Name}.{index + 1}.png";
                bitmap.Image?.SaveAsPng(fileName);
            }
        }
    }

    [Fact]
    public void Test3()
    {
        const string clientPath = @"D:\L2\L2EU-P447-D20240313-P-230809-240318-1";
        const string savePath = @"D:\L2";

        string[] files =
        [
            "Icon.utx", "BranchIcon.utx", "branchSys.utx", "BranchSys2.utx", "BranchSys3.utx",
            "br_L2Icon.utx"
        ];

        foreach (string file in files)
        {
            ExportIcons(Path.Combine(clientPath, "SysTextures", file), savePath);
        }
    }

    private static void ExportIcons(string filePath, string savePath)
    {
        UPackageManager packageManager = new();
        UPackage package = UPackage.LoadFrom(packageManager, filePath);

        IEnumerable<UTexture> list = package.Exports.Where(x => x.Class?.Name == "Texture")
            .Select(x => (UTexture)x.Object);

        StringBuilder sb = new StringBuilder();
        string packageName = Path.GetFileNameWithoutExtension(filePath);
        sb.AppendLine($"Package: {packageName}");
        sb.AppendLine();
        foreach (UTexture texture in list)
        {
            sb.AppendLine($"Texture: {texture.Name}, {texture.Lineage2Name}");
            for (int index = 0; index < texture.Bitmaps.Count; index++)
            {
                UBitmap bitmap = texture.Bitmaps[index];
                sb.AppendLine($"    Bitmap {index}: {bitmap.Width}x{bitmap.Height}");
            }

            sb.AppendLine();
        }

        string iconPath = Path.Combine(savePath, packageName);
        Directory.CreateDirectory(iconPath);
        File.WriteAllText(Path.Combine(savePath, $"{packageName}.txt"), sb.ToString(), Encoding.UTF8);
        
        foreach (UTexture texture in list)
        {
            for (int index = 0; index < texture.Bitmaps.Count; index++)
            {
                UBitmap bitmap = texture.Bitmaps[index];
                string fileName = texture.Bitmaps.Count == 1
                    ? $"{texture.Lineage2Name}.png"
                    : $"{texture.Lineage2Name}_{bitmap.Width}x{bitmap.Height}_{index + 1}.png";

                string saveFilePath = Path.Combine(iconPath, fileName);
                if (File.Exists(saveFilePath))
                {
                    Directory.CreateDirectory(Path.Combine(iconPath, "DuplicatedNames"));
                    saveFilePath = Path.Combine(iconPath, "DuplicatedNames", fileName);
                }
                
                bitmap.Image?.SaveAsPng(saveFilePath);
            }
        }
    }
}