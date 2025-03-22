using L2Dn.GameServer.Model.Residences;

namespace L2Dn.GameServer.Dto;

public sealed record ResidenceFunctionTemplate(int Id, int Level, ResidenceFunctionType Type, ItemHolder Cost,
    TimeSpan Duration, double Value)
{
    public long DurationAsDays => (long)Duration.TotalDays;
}