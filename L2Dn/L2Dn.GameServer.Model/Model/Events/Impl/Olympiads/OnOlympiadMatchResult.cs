using L2Dn.Events;
using L2Dn.GameServer.Model.Olympiads;

namespace L2Dn.GameServer.Model.Events.Impl.Olympiads;

public class OnOlympiadMatchResult: EventBase
{
    private readonly Participant? _winner;
    private readonly Participant _loser;
    private readonly CompetitionType _type;

    public OnOlympiadMatchResult(Participant? winner, Participant looser, CompetitionType type)
    {
        _winner = winner;
        _loser = looser;
        _type = type;
    }

    public Participant? getWinner()
    {
        return _winner;
    }

    public Participant getLoser()
    {
        return _loser;
    }

    public CompetitionType getCompetitionType()
    {
        return _type;
    }
}