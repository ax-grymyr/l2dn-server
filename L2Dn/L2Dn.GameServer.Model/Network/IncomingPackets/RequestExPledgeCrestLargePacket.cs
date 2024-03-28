using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestExPledgeCrestLargePacket: IIncomingPacket<GameSession>
{
    private int _crestId;
    private int _clanId;

    public void ReadContent(PacketBitReader reader)
    {
        _crestId = reader.ReadInt32();
        _clanId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
        Crest crest = CrestTable.getInstance().getCrest(_crestId);
        byte[] data = crest != null ? crest.getData() : null;
        if (data != null)
        {
            for (int i = 0; i <= 4; i++)
            {
                int size = Math.Max(Math.Min(14336, data.Length - 14336 * i), 0);
                if (size == 0)
                {
                    continue;
                }
                
                byte[] chunk = new byte[size];
                Array.Copy(data, 14336 * i, chunk, 0, size);
                player.sendPacket(new ExPledgeEmblemPacket(_crestId, chunk, _clanId, i));
            }
        }

        return ValueTask.CompletedTask;
    }
}