using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets.Collections;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Collections;

public struct RequestCollectionUpdateFavoritePacket: IIncomingPacket<GameSession>
{
    private int _isAdd;
    private int _collectionId;

    public void ReadContent(PacketBitReader reader)
    {
        _isAdd = reader.ReadByte();
        _collectionId = reader.ReadInt16();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (_isAdd == 1)
        {
            player.addCollectionFavorite(_collectionId);
        }
        else
        {
            player.removeCollectionFavorite(_collectionId);
        }

        player.sendPacket(new ExCollectionUpdateFavoritePacket(_isAdd, _collectionId));
        
        return ValueTask.CompletedTask;
    }
}