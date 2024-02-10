namespace L2Dn.GameServer.Model;

public sealed class LevelList
{
    private long[] _levelExp = Array.Empty<long>();

    public LevelList()
    {
        Reload();
    }
    
    public long GetExpForLevel(int level) => _levelExp[level - 1];
    public int GetMaxLevel() => _levelExp.Length;
    public long GetMaxExp() => _levelExp[^1];
    
    public (int Level, decimal Percents) GetLevelForExp(long exp)
    {
        if (exp < 0)
            throw new ArgumentOutOfRangeException(nameof(exp));
        
        int pos = Array.BinarySearch(_levelExp, exp);
        if (pos >= 0)
            return (pos + 1, 0);

        int index = ~pos; // 0..Count
        if (index == _levelExp.Length)
            return (_levelExp.Length - 1, 100);

        long levelExp = _levelExp[index - 1];
        return (index, 100M * (exp - levelExp) / (_levelExp[index] - levelExp));
    }

    public void Reload()
    {
        List<long>? levels = JsonUtility.DeserializeFile<List<long>>(StaticDataFiles.CharacterLevelExpFilePath);
        if (levels is null || levels.Count == 0)
            throw new InvalidOperationException($"'{StaticDataFiles.CharacterLevelExpFileName}' is invalid or empty");
        
        _levelExp = levels.ToArray();
    }
}
