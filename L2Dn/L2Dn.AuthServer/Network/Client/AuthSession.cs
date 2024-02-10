using System.Security.Cryptography;
using System.Text;
using L2Dn.AuthServer.Configuration;
using L2Dn.AuthServer.Cryptography;
using L2Dn.AuthServer.Model;
using L2Dn.Cryptography;
using L2Dn.DbModel;
using L2Dn.Network;

namespace L2Dn.AuthServer.Network.Client;

internal sealed class AuthSession(RsaKeyPair rsaKeyPair, byte[] blowfishKey, OldBlowfishEngine blowfishEngine)
    : Session, ISession<AuthSessionState>
{
    public AuthSessionState State { get; set; } = AuthSessionState.Authorization;
    public byte[] BlowfishKey => blowfishKey;
    public OldBlowfishEngine BlowfishEngine => blowfishEngine;
    public RsaKeyPair RsaKeyPair => rsaKeyPair;
    public int LoginKey1 { get; } = RandomGenerator.GetInt32();
    public int LoginKey2 { get; } = RandomGenerator.GetInt32();
    public int PlayKey1 { get; } = RandomGenerator.GetInt32();
    public int PlayKey2 { get; } = RandomGenerator.GetInt32();

    public int? AccountId { get; set; }
    public int? SelectedGameServerId { get; set; }
    public AuthServerConfig Config => ServerConfig.Instance.AuthServer;
    public GameServerList GameServers => GameServerList.Instance;
    
    public async Task UpdateGameServerListAsync()
    {
        GameServers.UpdateFrom(await DbUtility.GetGameServerListAsync());
    }

    public async Task<Account?> GetAccountAsync(string username, string password)
    {
        byte[] passwordHash = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return await DbUtility.GetAccountAsync(username, passwordHash, Config.AutoCreateAccounts);
    }

    public async Task UpdateSelectedGameServerAsync()
    {
        if (AccountId is not null)
            await DbUtility.UpdateSelectedGameServerAsync(AccountId.Value, SelectedGameServerId);
    }

    public async Task InsertOrUpdateAuthDataAsync()
    {
        if (SelectedGameServerId is not null && AccountId is not null)
            await DbUtility.InsertOrUpdateAuthDataAsync(SelectedGameServerId.Value, AccountId.Value, LoginKey1,
                LoginKey2, PlayKey1, PlayKey2);
    }
}
