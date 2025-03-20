using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerWeight.
 * @author Kerberos
 */
public class ConditionPlayerWeight: Condition
{
    private readonly int _weight;

    /**
     * Instantiates a new condition player weight.
     * @param weight the weight
     */
    public ConditionPlayerWeight(int weight)
    {
        _weight = weight;
    }

    protected override bool TestImpl(Creature effector, Creature? effected, Skill? skill, ItemTemplate? item)
    {
        Player? player = effector.getActingPlayer();
        if (player != null && player.getMaxLoad() > 0)
        {
            int weightProc = (player.getCurrentLoad() - player.getBonusWeightPenalty()) * 100 / player.getMaxLoad();
            return weightProc < _weight || player.getDietMode();
        }

        return true;
    }
}