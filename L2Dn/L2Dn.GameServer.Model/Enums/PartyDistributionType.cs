namespace L2Dn.GameServer.Enums;

public enum PartyDistributionType
{
    FINDERS_KEEPERS,
    RANDOM,
    RANDOM_INCLUDING_SPOIL,
    BY_TURN,
    BY_TURN_INCLUDING_SPOIL
}

public static class PartyDistributionTypeUtil
{
    public static int getSysStringId(this PartyDistributionType type)
    {
        return type switch
        {
            PartyDistributionType.FINDERS_KEEPERS => 487,
            PartyDistributionType.RANDOM => 488,
            PartyDistributionType.RANDOM_INCLUDING_SPOIL => 798,
            PartyDistributionType.BY_TURN => 799,
            PartyDistributionType.BY_TURN_INCLUDING_SPOIL => 800,
            _ => 0
        };
    }
}