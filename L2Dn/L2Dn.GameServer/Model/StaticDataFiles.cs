namespace L2Dn.GameServer.Model;

public static class StaticDataFiles
{
    public const string DataFolderName = "Data";
    public const string CharacterLevelExpFileName = "CharacterLevelExp.json";
    public const string CharacterCollisionDimensionsFileName = "CharacterCollisionDimensions.json";
    public const string CharacterSpawnLocationFileName = "CharacterSpawnLocation.json";
    public const string CharacterSpawnItemsFileName = "CharacterSpawnItems.json";
    public const string CharacterBaseStatsFileName = "CharacterBaseStats.json";
    public const string CharacterClassTreeFileName = "CharacterClassTree.json";

    public static string CharacterLevelExpFilePath => Path.Combine(DataFolderName, CharacterLevelExpFileName);

    public static string CharacterCollisionDimensionsFilePath =>
        Path.Combine(DataFolderName, CharacterCollisionDimensionsFileName);

    public static string CharacterSpawnLocationFilePath => Path.Combine(DataFolderName, CharacterSpawnLocationFileName);
    public static string CharacterSpawnItemsFilePath => Path.Combine(DataFolderName, CharacterSpawnItemsFileName);
    public static string CharacterBaseStatsFilePath => Path.Combine(DataFolderName, CharacterBaseStatsFileName);
    public static string CharacterClassTreeFilePath => Path.Combine(DataFolderName, CharacterClassTreeFileName);
}
