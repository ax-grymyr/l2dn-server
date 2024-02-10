using L2Dn.DbModel;
using Microsoft.EntityFrameworkCore;

namespace L2Dn.AuthServer;

public static class DbUtility
{
    public static async Task<List<GameServer>> GetGameServerListAsync()
    {
        await using L2DbContext ctx = new();
        return await ctx.GameServers.AsNoTracking().ToListAsync();
    }

    public static async Task<Account?> GetAccountAsync(string username, byte[] passwordHash, bool autoCreateAccounts)
    {
        await using L2DbContext ctx = new();
        Account? account = await ctx.Accounts.SingleOrDefaultAsync(a => a.Login == username);
        if (account is not null)
        {
            if (account.PasswordHash.AsSpan().SequenceEqual(passwordHash))
                return account;
            
            return null;
        }
        
        if (!autoCreateAccounts) 
            return null;
        
        account = new Account
        {
            Login = username,
            PasswordHash = passwordHash
        };

        ctx.Accounts.Add(account);
        await ctx.SaveChangesAsync();

        return account;
    }

    public static async Task UpdateSelectedGameServerAsync(int accountId, int? gameServerId)
    {
        await using L2DbContext ctx = new();
        
        Account? account = await ctx.Accounts.SingleOrDefaultAsync(a => a.Id == accountId);
        if (account is not null)
        {
            account.LastSelectedServerId = gameServerId;
            await ctx.SaveChangesAsync();
        }
    }

    public static async Task InsertOrUpdateAuthDataAsync(int serverId, int accountId, int loginKey1, int loginKey2, int playKey1, int playKey2)
    {
        await using L2DbContext ctx = new();
        
        AuthData? authData = await ctx.AuthData.SingleOrDefaultAsync(a => a.ServerId == serverId && a.AccountId == accountId);
        if (authData is null)
        {
            authData = new AuthData
            {
                ServerId = serverId,
                AccountId = accountId
            };

            ctx.AuthData.Add(authData);
        }

        authData.LoginKey1 = loginKey1;
        authData.LoginKey2 = loginKey2;
        authData.PlayKey1 = playKey1;
        authData.PlayKey2 = playKey2;
        
        await ctx.SaveChangesAsync();
    }
}
