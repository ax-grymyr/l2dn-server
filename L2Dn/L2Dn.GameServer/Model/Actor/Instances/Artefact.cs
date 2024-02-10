using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Model.Actor.Instances;

/**
 * This class manages all Castle Siege Artefacts.<br>
 * <br>
 * @version $Revision: 1.11.2.1.2.7 $ $Date: 2005/04/06 16:13:40 $
 */
public class Artefact: Npc
{
    /**
     * Constructor of Artefact (use Creature and Npc constructor).<br>
     * <br>
     * <b><u>Actions</u>:</b><br>
     * <li>Call the Creature constructor to set the _template of the Artefact (copy skills from template to object and link _calculators to NPC_STD_CALCULATOR)</li>
     * <li>Set the name of the Artefact</li>
     * <li>Create a RandomAnimation Task that will be launched after the calculated delay if the server allow it</li><br>
     * @param template to apply to the NPC
     */
    public Artefact(NpcTemplate template): base(template)
    {
        setInstanceType(InstanceType.Artefact);
    }

    public override void onSpawn()
    {
        base.onSpawn();
        getCastle().registerArtefact(this);
    }

    public override bool isArtefact()
    {
        return true;
    }

    public override bool isAutoAttackable(Creature attacker)
    {
        return false;
    }

    public override bool canBeAttacked()
    {
        return false;
    }

    public override void onForcedAttack(Player player)
    {
        // Send a Server->Client ActionFailed to the Player in order to avoid that the client wait another packet
        player.sendPacket(ActionFailedPacket.STATIC_PACKET);
    }

    public override void reduceCurrentHp(double damage, Creature attacker, Skill skill)
    {
    }

    public override void reduceCurrentHp(double value, Creature attacker, Skill skill, bool isDOT, bool directlyToHp,
        bool critical, bool reflect)
    {
    }
}
