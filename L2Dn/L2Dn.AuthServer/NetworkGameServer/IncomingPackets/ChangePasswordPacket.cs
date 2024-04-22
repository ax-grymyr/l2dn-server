using L2Dn.AuthServer.Model;
using L2Dn.AuthServer.NetworkGameServer.OutgoingPacket;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.AuthServer.NetworkGameServer.IncomingPackets;

internal struct ChangePasswordPacket: IIncomingPacket<GameServerSession>
{
    private int _accountId;
    private int _activePlayerId;
    private string _currentPassword;
    private string _newPassword;
    private int _signature;

    public void ReadContent(PacketBitReader reader)
    {
        _accountId = reader.ReadInt32();
        _activePlayerId = reader.ReadInt32();
        _currentPassword = reader.ReadString();
        _newPassword = reader.ReadString();
        _signature = reader.ReadInt32();
    }

    public async ValueTask ProcessAsync(Connection connection, GameServerSession session)
    {
        ChangePasswordResult result;
        
        const int signature = 0x7ABD_9123; // prime
        if (_signature != signature * _newPassword.GetHashCode())
            result = ChangePasswordResult.InvalidPassword;
        else
            result = await AccountManager.Instance.ChangePasswordAsync(_accountId, _currentPassword, _newPassword);

        ChangePasswordResponsePacket changePasswordResponsePacket = new(result, _activePlayerId);
        connection.Send(ref changePasswordResponsePacket);
    }
}