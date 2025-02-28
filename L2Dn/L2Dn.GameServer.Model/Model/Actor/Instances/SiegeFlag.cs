using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor.Status;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Actor.Instances;

public class SiegeFlag: Npc
{
    private readonly Clan _clan;
    private Siegable _siege;
    private readonly bool _isAdvanced;
    private bool _canTalk;

    public SiegeFlag(Player player, NpcTemplate template, bool advanced): base(template)
    {
        InstanceType = InstanceType.SiegeFlag;

        Clan? clan = player.getClan();
        Siegable? siege = SiegeManager.getInstance().getSiege(player.Location.Location3D) ??
            (Siegable?)FortSiegeManager.getInstance().getSiege(player.Location.Location3D);

        if (clan == null || siege == null)
            throw new ArgumentException("Player is not in clan or siege.");

        SiegeClan? sc = siege.getAttackerClan(clan);
        if (sc == null)
            throw new ArgumentException("Cannot find siege clan.");

        _canTalk = true;
        _clan = clan;
        _siege = siege;

        sc.addFlag(this);
        _isAdvanced = advanced;
        getStatus();
        setInvul(false);
    }

    public override bool canBeAttacked()
    {
        return !isInvul();
    }

    public override bool isAutoAttackable(Creature attacker)
    {
        return !isInvul();
    }

    public override bool doDie(Creature? killer)
    {
        if (!base.doDie(killer))
            return false;

        if (_siege != null && _clan != null)
        {
            SiegeClan? sc = _siege.getAttackerClan(_clan);
            if (sc != null)
            {
                sc.removeFlag(this);
            }
        }

        return true;
    }

    public override void onForcedAttack(Player player)
    {
        onAction(player);
    }

    public override void onAction(Player player, bool interact)
    {
        if (player == null || !canTarget(player))
            return;

        // Check if the Player already target the Npc
        if (this != player.getTarget())
        {
            // Set the target of the Player player
            player.setTarget(this);
        }
        else if (interact)
        {
            if (isAutoAttackable(player) && Math.Abs(player.getZ() - getZ()) < 100)
            {
                player.getAI().setIntention(CtrlIntention.AI_INTENTION_ATTACK, this);
            }
            else
            {
                // Send a Server->Client ActionFailed to the Player in order to avoid that the client wait another packet
                player.sendPacket(ActionFailedPacket.STATIC_PACKET);
            }
        }
    }

    public bool isAdvancedHeadquarter()
    {
        return _isAdvanced;
    }

    public override SiegeFlagStatus getStatus()
    {
        return (SiegeFlagStatus)base.getStatus();
    }

    public override void reduceCurrentHp(double damage, Creature attacker, Skill? skill)
    {
        base.reduceCurrentHp(damage, attacker, skill);
        Castle? castle = getCastle();
        Fort? fort = getFort();
        if (_canTalk &&
            ((castle != null && castle.getSiege().isInProgress()) ||
                (fort != null && fort.getSiege().isInProgress())) && _clan != null)
        {
            // send warning to owners of headquarters that theirs base is under attack
            _clan.broadcastToOnlineMembers(new SystemMessagePacket(SystemMessageId.SIEGE_CAMP_IS_UNDER_ATTACK));
            _canTalk = false;
            ThreadPool.schedule(new ScheduleTalkTask(this), 20000);
        }
    }

    private class ScheduleTalkTask(SiegeFlag siegeFlag): Runnable
    {
        public void run()
        {
            siegeFlag._canTalk = true;
        }
    }

    protected override CreatureStatus CreateStatus() => new SiegeFlagStatus(this);
}