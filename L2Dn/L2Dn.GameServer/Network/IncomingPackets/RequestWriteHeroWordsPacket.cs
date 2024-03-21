using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Olympiads;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestWriteHeroWordsPacket: IIncomingPacket<GameSession>
{
    private string _heroWords;

    public void ReadContent(PacketBitReader reader)
    {
        _heroWords = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null || !player.isHero())
            return ValueTask.CompletedTask;
		
        if (_heroWords == null || _heroWords.Length > 300)
            return ValueTask.CompletedTask;
		
        Hero.getInstance().setHeroMessage(player, _heroWords);

        return ValueTask.CompletedTask;
    }
}