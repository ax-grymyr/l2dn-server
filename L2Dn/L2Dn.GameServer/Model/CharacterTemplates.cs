using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace L2Dn.GameServer.Model;

public sealed class CharacterTemplates
{
    private readonly CharacterRaceData[] _data;
    private readonly CharacterClassInfo?[] _classes;

    public CharacterTemplates()
    {
        int raceCount = (int)EnumUtility.GetMaxValue<Race>() + 1;
        _data = new CharacterRaceData[raceCount];
        
        int classCount = (int)EnumUtility.GetMaxValue<CharacterClass>() + 1;
        _classes = new CharacterClassInfo?[classCount];
    }

    public CharacterClassInfo this[CharacterClass @class] => (int)@class >= _classes.Length
        ? throw new ArgumentOutOfRangeException(nameof(@class))
        : _classes[(int)@class] ?? throw new ArgumentException($"No data for class '{@class}'", nameof(@class));

    public CharacterRaceData this[Race race] => (int)race >= _data.Length
        ? throw new ArgumentOutOfRangeException(nameof(race))
        : _data[(int)race];

    public void Reload()
    {
        var collisionDimensions = Deserialize<Dictionary<Sex, Dimensions>>(
            StaticDataFiles.CharacterCollisionDimensionsFilePath, StaticDataFiles.CharacterCollisionDimensionsFileName,
            v => v.ContainsKey(Sex.Male) && v.ContainsKey(Sex.Female) && v[Sex.Male] is { Radius: > 0, Height: > 0 } &&
                 v[Sex.Female] is { Radius: > 0, Height: > 0 });

        var spawnLocations = Deserialize<SpawnLocation>(
            StaticDataFiles.CharacterSpawnLocationFilePath, StaticDataFiles.CharacterSpawnLocationFileName);

        var spawnItems = Deserialize<List<SpawnItem>>(
            StaticDataFiles.CharacterSpawnItemsFilePath, StaticDataFiles.CharacterSpawnItemsFileName,
            v => v.All(x => x is { ItemId: > 0, Count: > 0 }));

        var baseStats = Deserialize<BaseStats>(
            StaticDataFiles.CharacterBaseStatsFilePath, StaticDataFiles.CharacterBaseStatsFileName);

        var classTree = JsonUtility.DeserializeFile<ClassTree>(StaticDataFiles.CharacterClassTreeFilePath);

        Dictionary<CharacterClass, (Race, CharacterSpec, CharacterClass?)> classDict = new(); 
        foreach (var pair in classTree.BaseClasses)
        {
            if (pair.Value != null)
            {
                foreach (var pair2 in pair.Value)
                    classDict[pair2.Value] = (pair.Key, pair2.Key, null);
            }
        }
        
        foreach (var pair in classTree.Transitions)
        {
            var (race, spec, parentClass) = classDict[pair.Key];
            if (parentClass is null)
            {
                _classes[(int)pair.Key] = new CharacterClassInfo(_classes, pair.Key, race, spec,
                    pair.Value ?? Enumerable.Empty<CharacterClass>());
            }
            else
            {
                var parent = _classes[(int)parentClass.Value] ??
                             throw new InvalidOperationException(
                                 $"'{StaticDataFiles.CharacterClassTreeFileName}' has invalid data for '{parentClass.Value}'");

                _classes[(int)pair.Key] = new CharacterClassInfo(_classes, pair.Key, parent,
                    pair.Value ?? Enumerable.Empty<CharacterClass>());
            }

            foreach (var characterClass in pair.Value ?? Enumerable.Empty<CharacterClass>())
                classDict[characterClass] = (race, spec, pair.Key);
        }
        
        foreach (Race race in Enum.GetValues<Race>())
        {
            SpawnLocation fighterLoc = spawnLocations[race][CharacterSpec.Fighter]; 
            List<SpawnItem> fighterItems = spawnItems[race][CharacterSpec.Fighter];
            BaseStats fighterStats = baseStats[race][CharacterSpec.Fighter];
            Dimensions fighterMaleDims = collisionDimensions[race][CharacterSpec.Fighter][Sex.Male];
            Dimensions fighterFemaleDims = collisionDimensions[race][CharacterSpec.Fighter][Sex.Female];
            CharacterClassInfo fighterBaseClass =
                _classes.FirstOrDefault(c => c is not null && c.Race == race && c.Spec == CharacterSpec.Fighter) ??
                throw new InvalidOperationException(
                    $"'{StaticDataFiles.CharacterClassTreeFileName}' has no data for '{race}' fighter");

            CharacterSpecData fighterSpecData =
                new CharacterSpecData(fighterLoc.ToLocation(),
                    fighterItems.Select(v => v.ToCharacterSpawnItem()).ToImmutableArray(),
                    fighterStats.ToCharacterBaseStats(), fighterMaleDims.ToCollisionDimensions(),
                    fighterFemaleDims.ToCollisionDimensions(), fighterBaseClass);

            spawnLocations[race].TryGetValue(CharacterSpec.Mage, out SpawnLocation? mageLoc);
            spawnItems[race].TryGetValue(CharacterSpec.Mage, out List<SpawnItem>? mageItems);
            baseStats[race].TryGetValue(CharacterSpec.Mage, out BaseStats? mageStats);
            CharacterSpecData? mageSpawnData = null;
            if (mageLoc is not null && mageStats is not null)
            {
                Dimensions mageMaleDims = collisionDimensions[race][CharacterSpec.Mage][Sex.Male];
                Dimensions mageFemaleDims = collisionDimensions[race][CharacterSpec.Mage][Sex.Female];

                CharacterClassInfo mageBaseClass =
                    _classes.FirstOrDefault(c => c is not null && c.Race == race && c.Spec == CharacterSpec.Mage) ??
                    throw new InvalidOperationException(
                        $"'{StaticDataFiles.CharacterClassTreeFileName}' has no data for '{race}' mage");

                mageSpawnData = new CharacterSpecData(mageLoc.ToLocation(),
                    mageItems?.Select(v => v.ToCharacterSpawnItem()).ToImmutableArray() ??
                    ImmutableArray<CharacterSpawnItem>.Empty,
                    mageStats.ToCharacterBaseStats(), mageMaleDims.ToCollisionDimensions(),
                    mageFemaleDims.ToCollisionDimensions(), mageBaseClass);

            }

            _data[(int)race] = new CharacterRaceData(fighterSpecData, mageSpawnData);
        }
    }

    private static Dictionary<Race, Dictionary<CharacterSpec, T>> Deserialize<T>(string filePath, string fileName,
        Func<T, bool>? validate = null)
    {
        Dictionary<Race, Dictionary<CharacterSpec, T>> result =
            JsonUtility.DeserializeFile<Dictionary<Race, Dictionary<CharacterSpec, T>>>(filePath);

        // Validation
        if (result.Count == 0)
            throw new InvalidOperationException($"'{fileName}' file is invalid or empty");

        foreach (Race race in Enum.GetValues<Race>())
        {
            if (!result.TryGetValue(race, out var value1) || value1 is null)
                throw new InvalidOperationException($"'{fileName}' has no data for '{race}'");

            foreach (CharacterSpec key in new[] { CharacterSpec.Fighter, CharacterSpec.Mage })
            {
                if (race == Race.Dwarf && key is CharacterSpec.Mage)
                {
                    // exception: all dwarfs are fighters
                    if (value1.ContainsKey(key))
                        throw new InvalidOperationException($"'{fileName}' has invalid data for '{race}' {key}");

                    continue;
                }

                if (!value1.TryGetValue(key, out T? value) || value is null ||
                    (validate is not null && !validate(value)))
                    throw new InvalidOperationException($"'{fileName}' has no data for '{race}' {key}");
            }
        }

        return result;
    }
}

file class Dimensions
{
    [JsonRequired]
    public decimal Radius { get; set; }

    [JsonRequired]
    public decimal Height { get; set; }

    public CollisionDimensions ToCollisionDimensions() => new(Radius, Height);
}

file class SpawnLocation
{
    [JsonRequired]
    public int X { get; set; }

    [JsonRequired]
    public int Y { get; set; }
    
    [JsonRequired]
    public int Z { get; set; }

    public Location ToLocation() => new(X, Y, Z);
}

file class SpawnItem
{
    [JsonRequired]
    public int ItemId { get; set; }

    [JsonRequired]
    public int Count { get; set; }

    public CharacterSpawnItem ToCharacterSpawnItem() => new(ItemId, Count);
}

file class BaseStats
{
    [JsonRequired]
    public int STR { get; set; }
    
    [JsonRequired]
    public int CON { get; set; }
    
    [JsonRequired]
    public int DEX { get; set; }
    
    [JsonRequired]
    public int INT { get; set; }
    
    [JsonRequired]
    public int WIT { get; set; }
    
    [JsonRequired]
    public int MEN { get; set; }
    
    [JsonRequired]
    public int PAtk { get; set; }
    
    [JsonRequired]
    public int PDef { get; set; }
    
    [JsonRequired]
    public int MAtk { get; set; }
    
    [JsonRequired]
    public int MDef { get; set; }
    
    [JsonRequired]
    public int PAtkSpd { get; set; }
    
    [JsonRequired]
    public int MAtkSpd { get; set; }
    
    [JsonRequired]
    public int CritRate { get; set; }
    
    [JsonRequired]
    public int RunSpd { get; set; }

    public CharacterBaseStats ToCharacterBaseStats() => new(STR, CON, DEX, INT, WIT, MEN, PAtk, PDef, MAtk, MDef,
        PAtkSpd, MAtkSpd, CritRate, RunSpd);
}

file class ClassTree
{
    [JsonRequired]
    public Dictionary<Race, Dictionary<CharacterSpec, CharacterClass>?> BaseClasses { get; set; } = new();

    [JsonRequired]
    public Dictionary<CharacterClass, List<CharacterClass>?> Transitions { get; set; } = new();
}
