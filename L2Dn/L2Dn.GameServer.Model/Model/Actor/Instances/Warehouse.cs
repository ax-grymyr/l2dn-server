using System.Globalization;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;

namespace L2Dn.GameServer.Model.Actor.Instances;

public class Warehouse: Folk
{
    public Warehouse(NpcTemplate template): base(template)
    {
        InstanceType = InstanceType.Warehouse;
    }

    public override bool isAutoAttackable(Creature attacker)
    {
        if (attacker.isMonster())
        {
            return true;
        }

        return base.isAutoAttackable(attacker);
    }

    public override bool isWarehouse()
    {
        return true;
    }

    public override string getHtmlPath(int npcId, int value, Player? player)
    {
        string pom;
        if (value == 0)
        {
            pom = npcId.ToString(CultureInfo.InvariantCulture);
        }
        else
        {
            pom = npcId + "-" + value;
        }

        return "html/warehouse/" + pom + ".htm";
    }
}