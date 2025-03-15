using System.Net;
using System.Net.Sockets;
using L2Dn.Cryptography;

namespace L2Dn;

public class NetworkTests
{
    //[Fact]
    public void Test1()
    {
        for (int i = 0; i < 100; i++)
        {
            using FileStream s = new FileStream(@$"E:\Temp\init\init_{i}.packet", FileMode.Create, FileAccess.Write);
            using TcpClient client = new TcpClient();
            client.Connect(IPAddress.Loopback, 2106);
            byte[] data = new byte[172];
            int size = 0;
            while (size < 172)
            {
                int len = client.Client.Receive(data, 0, 172 - size, SocketFlags.None);
                s.Write(data, 0, len);
                s.Flush();
                size += len;
            }
        }
    }

    //[Fact]
    public void Decrypt()
    {
        byte[] staticKey = new byte[]
            { 0x6b, 0x60, 0xcb, 0x5b, 0x82, 0xce, 0x90, 0xb1, 0xcc, 0x2b, 0x6c, 0x55, 0x6c, 0x6c, 0x6c, 0x6c };

        OldBlowfishEngine engine = new OldBlowfishEngine(staticKey);
        for (int i = 0; i < 100; i++)
        {
            byte[] data = File.ReadAllBytes(@$"E:\Temp\init\init_{i}.packet");
            byte[] decrypted = new byte[data.Length - 2];
            engine.Decode(decrypted, data.AsSpan(2));
            File.WriteAllBytes(@$"E:\Temp\init\init_{i}.packet.decoded", decrypted);
        }
    }
}
