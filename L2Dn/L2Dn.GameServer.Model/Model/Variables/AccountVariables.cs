using L2Dn.GameServer.Db;

namespace L2Dn.GameServer.Model.Variables;

public class AccountVariables: AbstractVariables<AccountVariable>
{
    // Public variable names
    public const string HWID = "HWID";
    public const string HWIDSLIT_VAR = "	";
    public const string PRIME_SHOP_PRODUCT_COUNT = "PSPCount";
    public const string PRIME_SHOP_PRODUCT_DAILY_COUNT = "PSPDailyCount";
    public const string LCOIN_SHOP_PRODUCT_COUNT = "LCSCount";
    public const string LCOIN_SHOP_PRODUCT_DAILY_COUNT = "LCSDailyCount";
    public const string LCOIN_SHOP_PRODUCT_MONTLY_COUNT = "LCSMontlyCount";
    public const string VIP_POINTS = "VipPoints";
    public const string VIP_TIER = "VipTier";
    public const string VIP_EXPIRATION = "VipExpiration";
    public const string VIP_ITEM_BOUGHT = "Vip_Item_Bought";

    private readonly int _accountId;

    public AccountVariables(int accountId)
    {
        _accountId = accountId;
        Restore();
    }

    protected override IQueryable<AccountVariable> GetQuery(GameServerDbContext ctx) =>
        ctx.AccountVariables.Where(r => r.AccountId == _accountId);

    protected override AccountVariable CreateVar() => new() { AccountId = _accountId };
}