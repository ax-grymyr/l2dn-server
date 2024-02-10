using L2Dn.GameServer.Configuration;

namespace L2Dn.GameServer;

public static class DbUtility
{
    public static async Task PublishGameServerAsync(GameServerConfig gameServerConfig)
    {
        await using L2DbContext ctx = new();
        DbModel.GameServer? gameServer = await ctx.GameServers.FindAsync(gameServerConfig.Id);
        if (gameServer is null)
        {
            gameServer = new DbModel.GameServer();
            gameServer.ServerId = gameServerConfig.Id;
            ctx.GameServers.Add(gameServer);
        }

        gameServer.Address = gameServerConfig.PublishAddress;
        gameServer.Port = gameServerConfig.Port;
        gameServer.PlayerCount = 0;
        gameServer.MaxPlayerCount = gameServerConfig.MaxPlayerCount;
        gameServer.IsPvpServer = gameServerConfig.Pvp;
        gameServer.Attributes = (gameServerConfig.Test ? GameServerAttributes.PublicTest : GameServerAttributes.None) |
                                (gameServerConfig.Clock ? GameServerAttributes.Relax : GameServerAttributes.None);

        gameServer.Brackets = gameServerConfig.Brackets;
        gameServer.IsOnline = false;

        await ctx.SaveChangesAsync();
    }

    public static async Task SetGameServerOnlineAsync(int gameServerId, bool isOnline)
    {
        await using L2DbContext ctx = new();
        DbModel.GameServer? gameServer = await ctx.GameServers.FindAsync(gameServerId);
        if (gameServer is not null)
        {
            gameServer.IsOnline = isOnline;
            await ctx.SaveChangesAsync();
        }
    }

    public static async Task<int?> VerifyAuthDataAsync(string userName, int serverId, int loginKey1, int loginKey2,
        int playKey1, int playKey2)
    {
        await using L2DbContext ctx = new();
        AuthData? authData =
            await ctx.AuthData.SingleOrDefaultAsync(a => a.Account!.Login == userName && a.ServerId == serverId);

        if (authData is null)
            return null;

        bool result = authData.LoginKey1 == loginKey1 && authData.LoginKey2 == loginKey2 &&
                      authData.PlayKey1 == playKey1 && authData.PlayKey2 == playKey2;

        if (!result)
            return null;

        int accountId = authData.AccountId;

        ctx.AuthData.Remove(authData);
        await ctx.SaveChangesAsync();

        return accountId;
    }

    public static async Task<int> GetCharacterCountAsync(int gameServerId, int accountId)
    {
        await using L2DbContext ctx = new();
        return await ctx.Characters.CountAsync(a => a.ServerId == gameServerId && a.AccountId == accountId);
    }

    public static async Task CreateCharacterAsync(Character character)
    {
        await using L2DbContext ctx = new();
        ctx.Characters.Add(character);
        await ctx.SaveChangesAsync();
    }

    public static async Task<List<Character>> GetCharacters(int gameServerId, int accountId)
    {
        await using L2DbContext ctx = new();
        return await ctx.Characters.Where(c => c.ServerId == gameServerId && c.AccountId == accountId).
            OrderBy(c => c.Created).ToListAsync();
    }

    public static async Task<bool> DoesCharacterExistAsync(int gameServerId, string name)
    {
        await using L2DbContext ctx = new();
        return await ctx.Characters.CountAsync(c => c.ServerId == gameServerId && c.Name == name) > 0;
    }
}
