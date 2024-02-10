using L2Dn.Packages;
using L2Dn.Packages.Textures;
using SixLabors.ImageSharp;

namespace L2Dn;

public class PackageTests
{
    [Fact]
    public void Test1()
    {
        UPackageManager packageManager = new();
        UPackage package = UPackage.LoadFrom(packageManager, @"E:\L2\L2C4\textures\t_18_20.utx.decrypted");

        var list = package.Exports.Where(x => x.Class?.Name == "Texture")
            .Select(x => (x.Name, (UTexture)package.LoadObject(x.Name))).ToList();

        foreach (var (name, texture) in list)
        {
            for (int index = 0; index < texture.Bitmaps.Count; index++)
            {
                UBitmap bitmap = texture.Bitmaps[index];
                string fileName = $@"E:\L2\L2C4\textures\t_18_20.utx.{name}.{index + 1}.png";
                bitmap.Image?.SaveAsPng(fileName);
            }
        }
    }

    [Fact]
    public void Test2()
    {
        UPackageManager packageManager = new();
        UPackage package = UPackage.LoadFrom(packageManager , @"E:\L2\L2C4\maps\18_20.unr.decrypted");

        var classes = package.Exports.GroupBy(x => x.Class?.Name ?? string.Empty).ToList();

        var list = package.Exports.Where(x => x.Class?.Name == "Texture")
            .Select(x => (x.Name, (UTexture)package.LoadObject(x.Name))).ToList();

        foreach (var (name, texture) in list)
        {
            for (int index = 0; index < texture.Bitmaps.Count; index++)
            {
                UBitmap bitmap = texture.Bitmaps[index];
                string fileName = $@"E:\L2\L2C4\maps\18_20.unr.{name}.{index + 1}.png";
                bitmap.Image?.SaveAsPng(fileName);
            }
        }
    }
}
