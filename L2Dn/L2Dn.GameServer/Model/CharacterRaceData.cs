namespace L2Dn.GameServer.Model;

public sealed class CharacterRaceData(CharacterSpecData fighterData, CharacterSpecData? mageData)
{
    public CharacterSpecData this[CharacterSpec spec] => spec switch
    {
        CharacterSpec.Fighter => fighterData,
        CharacterSpec.Mage when mageData is not null => mageData,
        _ => throw new ArgumentOutOfRangeException(nameof(spec))
    };
}
