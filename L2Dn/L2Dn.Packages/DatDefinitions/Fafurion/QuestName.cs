using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Fafurion;

public class QuestName
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public Quest[] Quests { get; set; } = Array.Empty<Quest>();
}