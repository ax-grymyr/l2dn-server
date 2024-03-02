using L2Dn.GameServer.Db;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace L2Dn.GameServer.Model.Variables;

public class ClanVariables: AbstractVariables<DbClanVariable>
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ClanVariables));
	
	// Public variable names.
	public const String CONTRIBUTION = "CONTRIBUTION_";
	public const String CONTRIBUTION_WEEKLY = "CONTRIBUTION_WEEKLY_";
	
	private readonly int _objectId;
	
	public ClanVariables(int objectId)
	{
		_objectId = objectId;
		restoreMe();
	}

	protected override IQueryable<DbClanVariable> GetQuery(GameServerDbContext ctx)
	{
		return ctx.ClanVariables.Where(r => r.ClanId == _objectId);
	}

	protected override DbClanVariable CreateVar()
	{
		return new DbClanVariable()
		{
			ClanId = _objectId
		};
	}

	public bool deleteWeeklyContribution()
	{
		try
		{
			using GameServerDbContext ctx = new();
			
			// Clear previous entries.
			GetQuery(ctx).Where(r => EF.Functions.Like(r.Name, "CONTRIBUTION_WEEKLY_%")).ExecuteDelete();

			// Clear all entries
			List<string> names = getSet().Select(x => x.Key).Where(x => x.StartsWith("CONTRIBUTION_WEEKLY_")).ToList();
			foreach (string name in names)
			{
				getSet().remove(name);
			}
		}
		catch (Exception e)
		{
			LOGGER.Error(GetType().Name + ": Couldn't delete variables for: " + _objectId + ": " + e);
			return false;
		}

		return true;
	}
}
