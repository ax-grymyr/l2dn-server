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
            if ((id >= 18265) && (id <= 18286))
            {
                id += 3536;
            }
            else if ((id == 18287) || (id == 18288))
            {
                id = 21671;
            }
            else if ((id == 18289) || (id == 18290))
            {
                id = 21694;
            }
            else if ((id == 18291) || (id == 18292))
            {
                id = 21717;
            }
            else if ((id == 18293) || (id == 18294))
            {
                id = 21740;
            }
            else if ((id == 18295) || (id == 18296))
            {
                id = 21763;
            }
            else if ((id == 18297) || (id == 18298))
            {
                id = 21786;
            }
        }

        base.doItemDrop(NpcData.getInstance().getTemplate(id), lastAttacker);
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
