using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct EndScenePlayerPacket: IIncomingPacket<GameSession>
{
    private Movie _movieId;

    public void ReadContent(PacketBitReader reader)
    {
        _movieId = (Movie)reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null || _movieId == 0)
            return ValueTask.CompletedTask;

        MovieHolder? holder = player.getMovieHolder();
        if (holder == null || holder.getMovie() != _movieId)
        {
            // PacketLogger.warning("Player " + client + " sent EndScenePlayer with wrong movie id: " + _movieId);
            return ValueTask.CompletedTask;
        }

        player.stopMovie();

        return ValueTask.CompletedTask;
    }
}