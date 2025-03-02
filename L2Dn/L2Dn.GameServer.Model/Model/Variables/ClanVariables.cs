using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model.Zones.Types;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace L2Dn.GameServer.Model.Variables;

public class ClanVariables: AbstractVariables<DbClanVariable>
{
    // Public variable names.
    public const string CONTRIBUTION = "CONTRIBUTION_";
    public const string CONTRIBUTION_WEEKLY = "CONTRIBUTION_WEEKLY_";

    private readonly int _objectId;

    public ClanVariables(int objectId)
    {
        _objectId = objectId;
        Restore();
    }

    protected override IQueryable<DbClanVariable> GetQuery(GameServerDbContext ctx) =>
        ctx.ClanVariables.Where(r => r.ClanId == _objectId);

    protected override DbClanVariable CreateVar() => new() { ClanId = _objectId };

    public void DeleteWeeklyContribution()
    {
        RemoveAll("CONTRIBUTION_WEEKLY_");
    }
}