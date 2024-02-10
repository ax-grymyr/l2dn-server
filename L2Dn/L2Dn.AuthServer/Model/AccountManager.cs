using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using L2Dn.AuthServer.Configuration;
using L2Dn.AuthServer.Db;
using L2Dn.Logging;
using Microsoft.EntityFrameworkCore;

namespace L2Dn.AuthServer.Model;

public sealed class AccountManager: ISingleton<AccountManager>
{
    private readonly ConcurrentDictionary<string, AccountInfo> _accounts = new();
    
    private AccountManager()
    {
    }
    
    public static AccountManager Instance { get; } = new();

    public async Task<AccountInfo?> LoginAsync(string login, string password, string? clientAddress)
    {
        await using AuthServerDbContext ctx = new();

        if (_accounts.TryGetValue(login, out AccountInfo? accountInfo))
        {
            if (accountInfo.Password != password)
                return null;

            // update last login time
            await UpdateLastLoginAsync(ctx, accountInfo.AccountId, clientAddress);
            return accountInfo;
        }

        byte[] passwordHash = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        Account? account = await ctx.Accounts.SingleOrDefaultAsync(a => a.Login == login);
        if (account is not null)
        {
            if (!account.PasswordHash.AsSpan().SequenceEqual(passwordHash))
                return null; // Password doesn't match

            accountInfo = new AccountInfo
            {
                AccountId = account.Id,
                Password = password,
                Login = login
            };

            _accounts.TryAdd(login, accountInfo);
            await UpdateLastLoginAsync(ctx, accountInfo.AccountId, clientAddress);
            return accountInfo; // Login ok
        }

        if (!Config.Instance.Settings.AutoCreateAccounts)
            return null;

        DateTime now = DateTime.UtcNow;
        account = new Account
        {
            Login = login,
            PasswordHash = passwordHash,
            LastLogin = now,
            Created = now,
            LastIpAddress = clientAddress,
        };

        ctx.Accounts.Add(account);

        try
        {
            await ctx.SaveChangesAsync();
        }
        catch (Exception exception)
        {
            Logger.Error("Could not create account: " + exception);
            return null;
        }

        accountInfo = new AccountInfo
        {
            AccountId = account.Id,
            Password = password,
            Login = login
        };

        _accounts.TryAdd(login, accountInfo);
        return accountInfo;
    }

    public async Task UpdateSelectedGameServerAsync(int accountId, byte serverId)
    {
        await using AuthServerDbContext ctx = new();
        await ctx.Accounts.Where(a => a.Id == accountId)
            .ExecuteUpdateAsync(b =>
                b.SetProperty(a => a.LastSelectedServerId, serverId));
    }

    private async Task UpdateLastLoginAsync(AuthServerDbContext ctx, int accountId, string? ipAddress)
    {
        await ctx.Accounts.Where(a => a.Id == accountId)
            .ExecuteUpdateAsync(b =>
                b.SetProperty(a => a.LastLogin, DateTime.UtcNow).SetProperty(a => a.LastIpAddress, ipAddress));
    }
}