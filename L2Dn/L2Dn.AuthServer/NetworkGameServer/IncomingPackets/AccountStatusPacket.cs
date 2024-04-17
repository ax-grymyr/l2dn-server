using L2Dn.AuthServer.Model;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.AuthServer.NetworkGameServer.IncomingPackets;

internal struct AccountStatusPacket: IIncomingPacket<GameServerSession>
{
    private int _accountId;
    private byte _characterCount;

    public void ReadContent(PacketBitReader reader)
    {
        _accountId = reader.ReadInt32();
        _characterCount = reader.ReadByte();
    }

    public async ValueTask ProcessAsync(Connection connection, GameServerSession session)
    {
        GameServerInfo? serverInfo = session.ServerInfo;
        if (serverInfo is null)
        {
            connection.Close();
            return;
        }

        await AccountManager.Instance.UpdateCharacterCountAsync(_accountId, serverInfo.ServerId, _characterCount);
    }
}