using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Network.OutgoingPackets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Model.Actor.Instances;

/**
 * This class manages all RaidBoss.<br>
 * In a group mob, there are one master called RaidBoss and several slaves called Minions.
 */
public class RaidBoss: Monster
{
    private bool _useRaidCurse = true;

    /**
     * Constructor of RaidBoss (use Creature and Npc constructor).<br>
     * <br>
     * <b><u>Actions</u>:</b>
     * <ul>
     * <li>Call the Creature constructor to set the _template of the RaidBoss (copy skills from template to object and link _calculators to NPC_STD_CALCULATOR)</li>
     * <li>Set the name of the RaidBoss</li>
     * <li>Create a RandomAnimation Task that will be launched after the calculated delay if the server allow it</li>
     * </ul>
     * @param template to apply to the NPC
     */
    public RaidBoss(NpcTemplate template): base(template)
    {
        InstanceType = InstanceType.RaidBoss;
        setIsRaid(true);
        setLethalable(false);
    }

    public override void onSpawn()
    {
        base.onSpawn();
        setRandomWalking(false);
        broadcastPacket(new PlaySoundPacket(1, getParameters().getString("RaidSpawnMusic", "Rm01_A"), 0, 0, 0, 0, 0));
    }

    public override int getVitalityPoints(int level, double exp, bool isBoss)
    {
        return -base.getVitalityPoints(level, exp, isBoss);
    }

    public override bool useVitalityRate()
    {
        return Config.Character.RAIDBOSS_USE_VITALITY;
    }

    public void setUseRaidCurse(bool value)
    {
        _useRaidCurse = value;
    }

    public override bool giveRaidCurse()
    {
        return _useRaidCurse;
    }
}