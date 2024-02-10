namespace L2Dn.GameServer.Model;

public static class StaticData
{
    private static readonly CharacterTemplates _templates = new();
    private static readonly LevelList _levels = new();

    public static CharacterTemplates Templates => _templates;
    public static LevelList Levels => _levels;

    public static void Reload()
    {
        _levels.Reload();
        _templates.Reload();
    }
}
