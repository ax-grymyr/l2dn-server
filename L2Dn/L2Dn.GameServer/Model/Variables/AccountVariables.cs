using L2Dn.GameServer.Db;

namespace L2Dn.GameServer.Model.Variables;

public class AccountVariables: AbstractVariables<AccountVariable>
{
	// Public variable names
	public const String HWID = "HWID";
	public const String HWIDSLIT_VAR = "	";
	public const String PRIME_SHOP_PRODUCT_COUNT = "PSPCount";
	public const String PRIME_SHOP_PRODUCT_DAILY_COUNT = "PSPDailyCount";
	public const String LCOIN_SHOP_PRODUCT_COUNT = "LCSCount";
	public const String LCOIN_SHOP_PRODUCT_DAILY_COUNT = "LCSDailyCount";
	public const String LCOIN_SHOP_PRODUCT_MONTLY_COUNT = "LCSMontlyCount";
	public const String VIP_POINTS = "VipPoints";
	public const String VIP_TIER = "VipTier";
	public const String VIP_EXPIRATION = "VipExpiration";
	public const String VIP_ITEM_BOUGHT = "Vip_Item_Bought";

	private readonly int _accountId;
	
	public AccountVariables(int accountId)
	{
		_accountId = accountId;
		restoreMe();
	}

	protected override IQueryable<AccountVariable> GetQuery(GameServerDbContext ctx)
	{
		return ctx.AccountVariables.Where(r => r.AccountId == _accountId);
	}

	protected override AccountVariable CreateVar()
	{
		return new AccountVariable()
		{
			AccountId = _accountId
		};
	}
}