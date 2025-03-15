using L2Dn.IO;

namespace L2Dn;

public class FileDecryptionTest
{
    [Fact]
    public void Test1()
    {
        const string path = @"E:\L2\L2C4\textures\t_18_20.utx";
        using EncryptedStream input = EncryptedStream.OpenRead(path);
        using FileStream output = new FileStream(path + ".decrypted", FileMode.Create, FileAccess.Write, FileShare.None);
        input.CopyTo(output);
    }

    [Fact]
    public void Test2()
    {
        const string path = @"E:\L2\L2C4\maps\18_20.unr";
        using EncryptedStream input = EncryptedStream.OpenRead(path);
        using FileStream output = new FileStream(path + ".decrypted", FileMode.Create, FileAccess.Write, FileShare.None);
        input.CopyTo(output);
    }
}
