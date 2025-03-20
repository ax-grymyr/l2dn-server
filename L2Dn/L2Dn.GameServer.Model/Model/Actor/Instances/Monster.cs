using System.Runtime.CompilerServices;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Model.Actor.Instances;

/**
 * This class manages all Monsters.
 * <ul>
 * <li>Minion</li>
 * <li>RaidBoss</li>
 * <li>GrandBoss</li>
 * </ul>
 */
public class Monster: Attackable
{
    protected bool _enableMinions = true;

    private Monster? _master;
    private MinionList? _minionList;

    /**
     * Constructor of Monster (use Creature and Npc constructor).<br>
     * <br>
     * <b><u>Actions</u>:</b>
     * <ul>
     * <li>Call the Creature constructor to set the _template of the Monster (copy skills from template to object and link _calculators to NPC_STD_CALCULATOR)</li>
     * <li>Set the name of the Monster</li>
     * <li>Create a RandomAnimation Task that will be launched after the calculated delay if the server allow it</li>
     * </ul>
     * @param template to apply to the NPC
     */
    public Monster(NpcTemplate template): base(template)
    {
        InstanceType = InstanceType.Monster;
        setAutoAttackable(true);
    }

    /**
     * Return True if the attacker is not another Monster.
     */
    public override bool isAutoAttackable(Creature attacker)
    {
        if (isFakePlayer())
        {
            return Config.FakePlayers.FAKE_PLAYER_AUTO_ATTACKABLE || isInCombat() || attacker.isMonster() || getScriptValue() > 0;
        }

        // Check if the Monster target is aggressive
        if (Config.Npc.GUARD_ATTACK_AGGRO_MOB && getTemplate().isAggressive() && attacker is Guard)
        {
            return true;
        }

        if (attacker.isMonster())
        {
            return attacker.isFakePlayer();
        }

        // Anything considers monsters friendly except Players, Attackables (Guards, Friendly NPC), Traps and EffectPoints.
        if (!attacker.isPlayable() && !attacker.isAttackable() && !(attacker is Trap) && !(attacker is EffectPoint))
        {
            return false;
        }

        return base.isAutoAttackable(attacker);
    }

    /**
     * Return True if the Monster is Aggressive (aggroRange > 0).
     */
    public override bool isAggressive()
    {
        return getTemplate().isAggressive() && !isAffected(EffectFlag.PASSIVE);
    }

    public override void onSpawn()
    {
        if (!isTeleporting() && _master != null)
        {
            setRandomWalking(false);
            setIsRaidMinion(_master.isRaid());
            _master.getMinionList().onMinionSpawn(this);
        }

        // dynamic script-based minions spawned here, after all preparations.
        base.onSpawn();
    }

    public override void onTeleported()
    {
        base.onTeleported();

        if (hasMinions())
        {
            getMinionList().onMasterTeleported();
        }
    }

    public override bool deleteMe()
    {
        if (hasMinions())
        {
            getMinionList().onMasterDie(true);
        }

        if (_master != null)
        {
            _master.getMinionList().onMinionDie(this, 0);
        }

        return base.deleteMe();
    }

    public override Monster? getLeader()
    {
        return _master;
    }

    public void setLeader(Monster? leader)
    {
        _master = leader;
    }

    public void enableMinions(bool value)
    {
        _enableMinions = value;
    }

    public bool hasMinions()
    {
        return _minionList != null;
    }

    public MinionList getMinionList()
    {
        if (_minionList == null)
        {
            lock (this)
            {
                if (_minionList == null)
                {
                    _minionList = new MinionList(this);
                }
            }
        }

        return _minionList;
    }

    public override bool isMonster()
    {
        return true;
    }

    /**
     * @return true if this Monster (or its master) is registered in WalkingManager
     */
    public override bool isWalker()
    {
        return _master == null ? base.isWalker() : _master.isWalker();
    }

    /**
     * @return {@code true} if this Monster is not raid minion, master state otherwise.
     */
    public override bool giveRaidCurse()
    {
        return isRaidMinion() && _master != null ? _master.giveRaidCurse() : base.giveRaidCurse();
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public override void doCast(Skill skill, Item? item, bool ctrlPressed, bool shiftPressed)
    {
        // Might need some exceptions here, but it will prevent the monster buffing player bug.
        Monster? target = getLeader();
        if (!skill.IsBad && target != null && target.isPlayer())
        {
            abortAllSkillCasters();
            return;
        }

        base.doCast(skill, item, ctrlPressed, shiftPressed);
    }
}