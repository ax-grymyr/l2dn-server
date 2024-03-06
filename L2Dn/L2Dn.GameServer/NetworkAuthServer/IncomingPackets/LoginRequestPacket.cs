using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.NetworkAuthServer.IncomingPackets;

internal struct LoginRequestPacket: IIncomingPacket<AuthServerSession>
{
    private int _accountId;
    private string _accountName;
    private int _loginKey1;
    private int _loginKey2;
    private int _playKey1;
    private int _playKey2;

    public void ReadContent(PacketBitReader reader)
    {
        _accountId = reader.ReadInt32();
        _accountName = reader.ReadString();
        _loginKey1 = reader.ReadInt32();
        _loginKey2 = reader.ReadInt32();
        _playKey1 = reader.ReadInt32();
        _playKey2 = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, AuthServerSession session)
    {
        if (string.IsNullOrEmpty(_accountName))
            return ValueTask.CompletedTask;

        session.Logins[_accountName] = new AuthServerLoginData(_accountId, _accountName, DateTime.UtcNow, _loginKey1,
            _loginKey2, _playKey1, _playKey2);

        return ValueTask.CompletedTask;
    }
}