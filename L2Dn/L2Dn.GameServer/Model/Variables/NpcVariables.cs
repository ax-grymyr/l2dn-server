using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Variables;

/**
 * NPC Variables implementation.
 * @author GKR
 */
public class NpcVariables: AbstractVariables
{
    public override int getInt(String key)
    {
        return base.getInt(key, 0);
    }

    public override bool restoreMe()
    {
        return true;
    }

    public override bool storeMe()
    {
        return true;
    }

    public override bool deleteMe()
    {
        return true;
    }

    /**
     * Gets the stored player.
     * @param name the name of the variable
     * @return the stored player or {@code null}
     */
    public Player getPlayer(String name)
    {
        return getObject<Player>(name);
    }

    /**
     * Gets the stored summon.
     * @param name the name of the variable
     * @return the stored summon or {@code null}
     */
    public Summon getSummon(String name)
    {
        return getObject<Summon>(name);
    }
}