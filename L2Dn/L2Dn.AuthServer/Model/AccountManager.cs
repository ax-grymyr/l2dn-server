using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using L2Dn.AuthServer.Configuration;
using L2Dn.AuthServer.Db;
using L2Dn.AuthServer.NetworkGameServer.OutgoingPacket;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace L2Dn.AuthServer.Model;

public sealed class AccountManager: ISingleton<AccountManager>
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(AccountManager));
    private readonly ConcurrentDictionary<string, AccountInfo> _accounts = new();
    private readonly ConcurrentDictionary<int, AccountInfo> _accountById = new();
    
    private AccountManager()
    {
    }
    
    public static AccountManager Instance { get; } = new();

    public async Task<AccountInfo?> LoginAsync(string login, string password, string? clientAddress)
    {
        await using AuthServerDbContext ctx = await DbFactory.Instance.CreateDbContextAsync().ConfigureAwait(false);

        if (_accounts.TryGetValue(login, out AccountInfo? accountInfo))
        {
            if (accountInfo.Password != password)
                return null;

            // update last login time
            await UpdateLastLoginAsync(ctx, accountInfo.AccountId, clientAddress).ConfigureAwait(false);
            return accountInfo;
        }

        byte[] passwordHash = GetPasswordHash(password);
        DbAccount? account = await ctx.Accounts.SingleOrDefaultAsync(a => a.Login == login).ConfigureAwait(false);
        if (account is not null)
        {
            if (!account.PasswordHash.AsSpan().SequenceEqual(passwordHash))
                return null; // Password doesn't match

            int accountId = account.Id;
            accountInfo = new AccountInfo
            {
                AccountId = accountId,
                Password = password,
                Login = login,
            };

            var accountChars = await ctx.AccountCharacterData
                .Where(a => a.AccountId == accountId)
                .Select(a => new { a.ServerId, a.CharacterCount })
                .ToListAsync().ConfigureAwait(false);

            foreach (var tuple in accountChars)
                accountInfo.CharacterCount[tuple.ServerId] = tuple.CharacterCount;

            accountInfo = _accounts.GetOrAdd(login, accountInfo);
            _accountById.TryAdd(accountId, accountInfo);
            await UpdateLastLoginAsync(ctx, accountInfo.AccountId, clientAddress).ConfigureAwait(false);
            return accountInfo; // Login ok
        }

        if (!Config.Instance.Settings.AutoCreateAccounts)
            return null;

        DateTime now = DateTime.UtcNow;
        account = new DbAccount
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
            await ctx.SaveChangesAsync().ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            _logger.Error("Could not create account: " + exception);
            return null;
        }

        accountInfo = new AccountInfo
        {
            AccountId = account.Id,
            Password = password,
            Login = login
        };

        _accounts.TryAdd(login, accountInfo);
        _accountById.TryAdd(account.Id, accountInfo);
        return accountInfo;
    }

    public async Task UpdateSelectedGameServerAsync(int accountId, byte serverId)
    {
        await using AuthServerDbContext ctx = await DbFactory.Instance.CreateDbContextAsync();
        await ctx.Accounts.Where(a => a.Id == accountId)
            .ExecuteUpdateAsync(b =>
                b.SetProperty(a => a.LastSelectedServerId, serverId)).ConfigureAwait(false);
    }

    private async Task UpdateLastLoginAsync(AuthServerDbContext ctx, int accountId, string? ipAddress)
    {
        await ctx.Accounts.Where(a => a.Id == accountId)
            .ExecuteUpdateAsync(b =>
                b.SetProperty(a => a.LastLogin, DateTime.UtcNow).SetProperty(a => a.LastIpAddress, ipAddress))
            .ConfigureAwait(false);
    }

    public async Task UpdateCharacterCountAsync(int accountId, byte serverId, byte characterCount)
    {
        // Update AccountInfo in memory
        if (_accountById.TryGetValue(accountId, out AccountInfo? accountInfo))
            accountInfo.CharacterCount[serverId] = characterCount;

        await using AuthServerDbContext ctx = await DbFactory.Instance.CreateDbContextAsync();
        DbAccountCharacterData? record = await ctx.AccountCharacterData.SingleOrDefaultAsync(r
            => r.AccountId == accountId && r.ServerId == serverId).ConfigureAwait(false);

        int recordCharacterCount = record?.CharacterCount ?? 0;
        if (recordCharacterCount == characterCount)
            return;
        
        if (record is null)
        {
            record = new DbAccountCharacterData
            {
                AccountId = accountId,
                ServerId = serverId,
            };

            ctx.AccountCharacterData.Add(record);
        }

        record.CharacterCount = characterCount;
        await ctx.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task<ChangePasswordResult> ChangePasswordAsync(int accountId, string currentPassword,
        string newPassword)
    {
        try
        {
            await using AuthServerDbContext ctx = await DbFactory.Instance.CreateDbContextAsync().ConfigureAwait(false);
            byte[] currentPasswordHash = GetPasswordHash(currentPassword);
            byte[] newPasswordHash = GetPasswordHash(newPassword);

            int updatedCount = await ctx.Accounts.Where(r => r.Id == accountId && r.PasswordHash == currentPasswordHash)
                .ExecuteUpdateAsync(s => s.SetProperty(r => r.PasswordHash, newPasswordHash));

            return updatedCount == 1 ? ChangePasswordResult.Ok : ChangePasswordResult.InvalidPassword;
        }
        catch (Exception exception)
        {
            _logger.Error("Error updating password: " + exception);
            return ChangePasswordResult.UnknownError;
        }
    }

    private static byte[] GetPasswordHash(string password) => SHA256.HashData(Encoding.UTF8.GetBytes(password));
}