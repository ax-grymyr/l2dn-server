using L2Dn.GameServer.Model.Olympiads;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExOlympiadMatchListPacket: IOutgoingPacket
{
    private readonly List<OlympiadGameTask> _games;
	
    public ExOlympiadMatchListPacket()
    {
        _games = new List<OlympiadGameTask>();
        for (int i = 0; i < OlympiadGameManager.getInstance().getNumberOfStadiums(); i++)
        {
            OlympiadGameTask? task = OlympiadGameManager.getInstance().getOlympiadTask(i);
            if (task != null)
            {
                if (!task.isGameStarted() || task.isBattleFinished())
                {
                    continue; // initial or finished state not shown
                }
                
                _games.Add(task);
            }
        }
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_RECEIVE_OLYMPIAD);
        
        writer.WriteInt32(0); // Type 0 = Match List, 1 = Match Result
        writer.WriteInt32(_games.Count);
        writer.WriteInt32(0);
        foreach (OlympiadGameTask curGame in _games)
        {
            AbstractOlympiadGame game = curGame.getGame();
            if (game != null)
            {
                writer.WriteInt32(game.getStadiumId()); // Stadium Id (Arena 1 = 0)
                if (game is OlympiadGameNonClassed)
                {
                    writer.WriteInt32(1);
                }
                else if (game is OlympiadGameClassed)
                {
                    writer.WriteInt32(2);
                }
                else
                {
                    writer.WriteInt32(0);
                }
                
                writer.WriteInt32(curGame.isRunning() ? 2 : 1); // (1 = Standby, 2 = Playing)
                writer.WriteString(game.getPlayerNames()[0]); // Player 1 Name
                writer.WriteString(game.getPlayerNames()[1]); // Player 2 Name
            }
        }
    }
}