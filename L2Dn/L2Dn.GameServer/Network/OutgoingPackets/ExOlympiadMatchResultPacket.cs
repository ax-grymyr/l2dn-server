using L2Dn.GameServer.Model.Olympiads;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExOlympiadMatchResultPacket: IOutgoingPacket
{
    private readonly bool _tie;
    private readonly int _winTeam; // 1,2
    private readonly int _loseTeam = 2;
    private readonly  List<OlympiadInfo> _winnerList;
    private readonly  List<OlympiadInfo> _loserList;
	
    public ExOlympiadMatchResultPacket(bool tie, int winTeam, List<OlympiadInfo> winnerList, List<OlympiadInfo> loserList)
    {
        _tie = tie;
        _winTeam = winTeam;
        _winnerList = winnerList;
        _loserList = loserList;
        if (_winTeam == 2)
        {
            _loseTeam = 1;
        }
        else if (_winTeam == 0)
        {
            _winTeam = 1;
        }
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_RECEIVE_OLYMPIAD);
        
        writer.WriteInt32(1); // Type 0 = Match List, 1 = Match Result
        writer.WriteInt32(_tie); // 0 - win, 1 - tie
        writer.WriteString(_winnerList[0].getName());
        writer.WriteInt32(_winTeam);
        writer.WriteInt32(_winnerList.Count);
        foreach (OlympiadInfo info in _winnerList)
        {
            writer.WriteString(info.getName());
            writer.WriteString(info.getClanName());
            writer.WriteInt32(info.getClanId() ?? 0);
            writer.WriteInt32((int)info.getClassId());
            writer.WriteInt32(info.getDamage());
            writer.WriteInt32(info.getCurrentPoints());
            writer.WriteInt32(info.getDiffPoints());
            writer.WriteInt32(0); // Helios
        }
        
        writer.WriteInt32(_loseTeam);
        writer.WriteInt32(_loserList.Count);
        foreach (OlympiadInfo info in _loserList)
        {
            writer.WriteString(info.getName());
            writer.WriteString(info.getClanName());
            writer.WriteInt32(info.getClanId() ?? 0);
            writer.WriteInt32((int)info.getClassId());
            writer.WriteInt32(info.getDamage());
            writer.WriteInt32(info.getCurrentPoints());
            writer.WriteInt32(info.getDiffPoints());
            writer.WriteInt32(0); // Helios
        }
    }
}