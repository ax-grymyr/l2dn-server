using System.Net;
using System.Text;
using L2Dn.AuthServer.Configuration;
using L2Dn.AuthServer.Cryptography;
using L2Dn.AuthServer.Model;
using L2Dn.AuthServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;
using Org.BouncyCastle.Crypto.Engines;

namespace L2Dn.AuthServer.Network.IncomingPackets;

internal struct RequestAuthLoginPacket: IIncomingPacket<AuthSession>
{
    private byte[]? _raw;

    public void ReadContent(PacketBitReader reader)
    {
        if (reader.Length >= 256)
            _raw = reader.ReadBytes(256).ToArray(); // new auth method
        else if (reader.Length >= 128)
            _raw = reader.ReadBytes(128).ToArray();
    }

    public async ValueTask ProcessAsync(Connection<AuthSession> connection)
    {
        if (_raw is null)
        {
            LoginFailPacket loginFailPacket = new(LoginFailReason.AccessDenied);
            connection.Send(ref loginFailPacket, SendPacketOptions.CloseAfterSending);
            return;
        }
        
        AuthSession session = connection.Session;

        (string username, string password) = DecryptUsernameAndPassword(session.RsaKeyPair, _raw);
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            LoginFailPacket loginFailPacket = new(LoginFailReason.InvalidLoginOrPassword);
            connection.Send(ref loginFailPacket, SendPacketOptions.CloseAfterSending);
            return;
        }

        //Logger.Info($"S({session.Id})  RequestAuthLogin, user: {username}, password: {password}");

        IPAddress? address = connection.GetRemoteAddress();
        AccountInfo? account = await AccountManager.Instance.LoginAsync(username, password, address?.ToString());
        if (account is null)
        {
            LoginFailPacket loginFailPacket = new(LoginFailReason.InvalidLoginOrPassword);
            connection.Send(ref loginFailPacket, SendPacketOptions.CloseAfterSending);
            return;
        }

        session.AccountInfo = account;
        session.State = AuthSessionState.GameServerLogin;

        if (Config.Instance.Settings.ShowLicense)
        {
            LoginOkPacket loginOkPacket = new(session.LoginKey1, session.LoginKey2);
            connection.Send(ref loginOkPacket);
        }
        else
        {
            ServerListPacket serverListPacket = new(GameServerManager.Instance.Servers, account.LastServerId);
            connection.Send(ref serverListPacket);
        }
    }

    private static (string Username, string Password) DecryptUsernameAndPassword(RsaKeyPair keyPair, byte[] data)
    {
        Encoding encoding = Encoding.ASCII;
        RsaEngine rsa = new RsaEngine();
        rsa.Init(false, keyPair.PrivateKey);
        byte[] decrypted = DecryptBlock(rsa, data, 0);

        string username;
        string password;

        if (data.Length == 256) // new auth method
        {
            byte[] decrypted2 = DecryptBlock(rsa, data, 128);

            username = encoding.GetString(decrypted.AsSpan(0x4E, 50)).TrimEnd('\0') +
                       encoding.GetString(decrypted2.AsSpan(0x4E, 14)).TrimEnd('\0');

            password = encoding.GetString(decrypted2.AsSpan(0x5C, 16)).TrimEnd('\0');
        }
        else
        {
            // C4 ???
            username = encoding.GetString(decrypted.AsSpan(0x03, 14)).TrimEnd('\0');
            password = encoding.GetString(decrypted.AsSpan(0x11, 16)).TrimEnd('\0');

            //username = encoding.GetString(decrypted.AsSpan(0x5E, 14)).Trim();
            //password = encoding.GetString(decrypted.AsSpan(0x6C, 16)).Trim();
        }

        return (username, password);
    }

    private static byte[] DecryptBlock(RsaEngine rsa, byte[] block, int offset)
    {
        const int size = 128;
        byte[] decrypted = rsa.ProcessBlock(block, offset, size);
        if (decrypted.Length < size)
        {
            byte[] temp = new byte[size];
            decrypted.AsSpan().CopyTo(temp);
            decrypted = temp;
        }

        return decrypted;
    }
}