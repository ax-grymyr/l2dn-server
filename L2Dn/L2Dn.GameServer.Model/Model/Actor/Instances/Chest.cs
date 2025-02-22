using System.Runtime.CompilerServices;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;

namespace L2Dn.GameServer.Model.Actor.Instances;

/**
 * This class manages all chest.
 * @author Julian
 */
public class Chest: Monster
{
    private volatile bool _specialDrop;

    public Chest(NpcTemplate template): base(template)
    {
        InstanceType = InstanceType.Chest;
        setRandomWalking(false);
        _specialDrop = false;
    }

    public override void onSpawn()
    {
        base.onSpawn();
        _specialDrop = false;
        setMustRewardExpSp(true);
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public void setSpecialDrop()
    {
        _specialDrop = true;
    }

    public override void doItemDrop(NpcTemplate npcTemplate, Creature lastAttacker)
    {
        int id = getTemplate().getId();
        if (!_specialDrop)
        {
            switch (id) // TODO: unhardcode the values
            {
                case >= 18265 and <= 18286:
                    id += 3536;
                    break;
                case 18287:
                case 18288:
                    id = 21671;
                    break;
                case 18289:
                case 18290:
                    id = 21694;
                    break;
                case 18291:
                case 18292:
                    id = 21717;
                    break;
                case 18293:
                case 18294:
                    id = 21740;
                    break;
                case 18295:
                case 18296:
                    id = 21763;
                    break;
                case 18297:
                case 18298:
                    id = 21786;
                    break;
            }
        }

        NpcTemplate template = NpcData.getInstance().getTemplate(id) ??
            throw new InvalidOperationException("No npc template for artifact id: " + id);

        base.doItemDrop(template, lastAttacker);
    }

    public override bool isMovementDisabled()
    {
        return true;
    }

    public override bool hasRandomAnimation()
    {
        return false;
    }
}