using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestPetitionFeedbackPacket: IIncomingPacket<GameSession>
{
    private int _rate; // 4=VeryGood, 3=Good, 2=Fair, 1=Poor, 0=VeryPoor
    private string _message;

    public void ReadContent(PacketBitReader reader)
    {
        reader.ReadInt32(); // unknown
        _rate = reader.ReadInt32();
        _message = reader.ReadString();
    }

    public async ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null || player.getLastPetitionGmName() == null)
            return;
		
        if (_rate > 4 || _rate < 0)
            return;
		
        try
        {
            await using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
            ctx.PetitionFeedbacks.Add(new DbPetitionFeedback()
            {
                CharacterName = player.getName(),
                GmName = player.getLastPetitionGmName(),
                Rate = (byte)_rate,
                Message = _message,
                Time = DateTime.UtcNow
            });

            await ctx.SaveChangesAsync();
        }
        catch (Exception e)
        {
            PacketLogger.Instance.Warn("Error while saving petition feedback: " + e);
        }
    }
}