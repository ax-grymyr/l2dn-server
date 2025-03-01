using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Variables;

/**
 * NPC Variables implementation.
 * @author GKR
 */
public class NpcVariables: AbstractVariables<DbNpcVariable>
{
    private readonly int _npcId;

    public NpcVariables(int npcId)
    {
        _npcId = npcId;
    }

    public override int getInt(string key)
    {
        return base.getInt(key, 0);
    }

    protected override IQueryable<DbNpcVariable> GetQuery(GameServerDbContext ctx)
    {
        return ctx.NpcVariables.Where(r => r.NpcId == _npcId);
    }

    protected override DbNpcVariable CreateVar()
    {
        return new DbNpcVariable()
        {
            NpcId = _npcId
        };
    }

    /**
     * Gets the stored player.
     * @param name the name of the variable
     * @return the stored player or {@code null}
     */
    public Player? getPlayer(string name)
    {
        return getObject<Player>(name);
    }

    /**
     * Gets the stored summon.
     * @param name the name of the variable
     * @return the stored summon or {@code null}
     */
    public Summon? getSummon(string name)
    {
        return getObject<Summon>(name);
    }
}