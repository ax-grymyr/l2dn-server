using System.Runtime.CompilerServices;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Olympiads;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Model.Enums;
using L2Dn.Utilities;
using Config = L2Dn.GameServer.Configuration.Config;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Cubics;

public class Cubic: Creature
{
    private readonly Player _owner;
    private readonly Player _caster;
    private readonly CubicTemplate _template;
    private ScheduledFuture? _skillUseTask;
    private ScheduledFuture? _expireTask;

    public Cubic(Player owner, Player? caster, CubicTemplate template): base(template)
    {
        InstanceType = InstanceType.Cubic;
        _owner = owner;
        _caster = caster ?? owner;
        _template = template;
        activate();
    }

    private void activate()
    {
        _skillUseTask = ThreadPool.scheduleAtFixedRate(readyToUseSkill, 0, _template.getDelay() * 1000);
        _expireTask = ThreadPool.schedule(deactivate, _template.getDuration() * 1000);
    }

    public void deactivate()
    {
        if (_skillUseTask != null && !_skillUseTask.isDone())
        {
            _skillUseTask.cancel(true);
        }

        _skillUseTask = null;
        if (_expireTask != null && !_expireTask.isDone())
        {
            _expireTask.cancel(true);
        }

        _expireTask = null;
        _owner.getCubics().remove(_template.getId());
        _owner.sendPacket(new ExUserInfoCubicPacket(_owner));
        _owner.broadcastCharInfo();
    }

    private void readyToUseSkill()
    {
        switch (_template.getTargetType())
        {
            case CubicTargetType.TARGET:
            {
                actionToCurrentTarget();
                break;
            }
            case CubicTargetType.BY_SKILL:
            {
                actionToTargetBySkill();
                break;
            }
            case CubicTargetType.HEAL:
            {
                actionHeal();
                break;
            }
            case CubicTargetType.MASTER:
            {
                actionToMaster();
                break;
            }
        }
    }

    private CubicSkill? chooseSkill()
    {
        double random = Rnd.nextDouble() * 100;
        double commulativeChance = 0;
        foreach (CubicSkill cubicSkill in _template.getCubicSkills())
        {
            if ((commulativeChance += cubicSkill.getTriggerRate()) > random)
                return cubicSkill;
        }

        return null;
    }

    private void actionToCurrentTarget()
    {
        CubicSkill? skill = chooseSkill();
        WorldObject? target = _owner.getTarget();
        if (skill != null && target != null)
        {
            tryToUseSkill(target, skill);
        }
    }

    private void actionToTargetBySkill()
    {
        CubicSkill? skill = chooseSkill();
        if (skill != null)
        {
            switch (skill.getTargetType())
            {
                case CubicTargetType.TARGET:
                {
                    WorldObject? target = _owner.getTarget();
                    if (target != null)
                    {
                        tryToUseSkill(target, skill);
                    }

                    break;
                }
                case CubicTargetType.HEAL:
                {
                    actionHeal();
                    break;
                }
                case CubicTargetType.MASTER:
                {
                    tryToUseSkill(_owner, skill);
                    break;
                }
            }
        }
    }

    private void actionHeal()
    {
        double random = Rnd.nextDouble() * 100;
        double commulativeChance = 0;
        foreach (CubicSkill cubicSkill in _template.getCubicSkills())
        {
            if ((commulativeChance += cubicSkill.getTriggerRate()) > random)
            {
                Skill skill = cubicSkill.getSkill();
                if (skill != null && Rnd.get(100) < cubicSkill.getSuccessRate())
                {
                    Party? party = _owner.getParty();
                    IEnumerable<Creature> stream;
                    if (party != null)
                    {
                        stream = World.getInstance().getVisibleObjectsInRange<Creature>(_owner, Config.Character.ALT_PARTY_RANGE,
                            c => c.getParty() == party && _template.validateConditions(this, _owner, c) &&
                                cubicSkill.validateConditions(this, _owner, c));
                    }
                    else
                    {
                        stream = _owner.getServitorsAndPets().Where(summon =>
                            _template.validateConditions(this, _owner, summon) &&
                            cubicSkill.validateConditions(this, _owner, summon));
                    }

                    if (_template.validateConditions(this, _owner, _owner) &&
                        cubicSkill.validateConditions(this, _owner, _owner))
                    {
                        stream = stream.Concat([_owner]);
                    }

                    Creature? target = stream.MinBy(c => c.getCurrentHpPercent());
                    if (target != null && !target.isDead()) // Life Cubic should not try to heal dead targets.
                    {
                        if (Rnd.nextDouble() > target.getCurrentHp() / target.getMaxHp())
                        {
                            activateCubicSkill(skill, target);
                        }

                        break;
                    }
                }
            }
        }
    }

    private void actionToMaster()
    {
        CubicSkill? skill = chooseSkill();
        if (skill != null)
        {
            tryToUseSkill(_owner, skill);
        }
    }

    private void tryToUseSkill(WorldObject worldObject, CubicSkill cubicSkill)
    {
        WorldObject? target = worldObject;
        Skill skill = cubicSkill.getSkill();
        if (_template.getTargetType() != CubicTargetType.MASTER &&
            !(_template.getTargetType() == CubicTargetType.BY_SKILL &&
                cubicSkill.getTargetType() == CubicTargetType.MASTER))
        {
            target = skill.GetTarget(_owner, target, false, false, false);
        }

        if (target != null)
        {
            if (target.isDoor() && !cubicSkill.canUseOnStaticObjects())
            {
                return;
            }

            if (_template.validateConditions(this, _owner, target) &&
                cubicSkill.validateConditions(this, _owner, target) && Rnd.get(100) < cubicSkill.getSuccessRate())
            {
                activateCubicSkill(skill, target);
            }
        }
    }

    private void activateCubicSkill(Skill skill, WorldObject target)
    {
        if (!_owner.hasSkillReuse(skill.ReuseHashCode))
        {
            _caster.broadcastPacket(new MagicSkillUsePacket(_owner, target, skill.DisplayId,
                skill.DisplayLevel, skill.HitTime, skill.ReuseDelay));

            skill.ActivateSkill(this, [target]);
            _owner.addTimeStamp(skill, skill.ReuseDelay);
        }
    }

    public override void sendDamageMessage(Creature target, Skill? skill, int damage, double elementalDamage, bool crit,
        bool miss, bool elementalCrit)
    {
        if (miss || _owner == null)
        {
            return;
        }

        if (_owner.isInOlympiadMode() && target.isPlayer() && ((Player)target).isInOlympiadMode() &&
            ((Player)target).getOlympiadGameId() == _owner.getOlympiadGameId())
        {
            OlympiadGameManager.getInstance().notifyCompetitorDamage(_owner, damage);
        }

        if (target.isHpBlocked() && !target.isNpc())
        {
            _owner.sendPacket(SystemMessageId.THE_ATTACK_HAS_BEEN_BLOCKED);
        }
        else
        {
            SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.C1_HAS_DEALT_S3_DAMAGE_TO_C2);
            sm.Params.addString(getName());
            sm.Params.addString(target.getName());
            sm.Params.addInt(damage);
            sm.Params.addPopup(target.ObjectId, _owner.ObjectId, damage * -1);
            _owner.sendPacket(sm);
        }
    }

    public override void sendPacket<TPacket>(TPacket packet)
    {
        if (_owner != null)
        {
            _owner.sendPacket(packet);
        }
    }

    public override Player getActingPlayer()
    {
        return _owner;
    }

    /**
     * @return the {@link Creature} that casted this cubic
     */
    public Creature getCaster()
    {
        return _caster;
    }

    /**
     * @return {@code true} if cubic is casted from someone else but the owner, {@code false}
     */
    public bool isGivenByOther()
    {
        return _caster != _owner;
    }

    /**
     * @return the owner's name.
     */
    public override string getName()
    {
        return _owner.getName();
    }

    /**
     * @return the owner's level.
     */
    public override int getLevel()
    {
        return _owner.getLevel();
    }

    public override int getX()
    {
        return _owner.getX();
    }

    public override int getY()
    {
        return _owner.getY();
    }

    public override int getZ()
    {
        return _owner.getZ();
    }

    public override int getHeading()
    {
        return _owner.getHeading();
    }

    public override int getInstanceId()
    {
        return _owner.getInstanceId();
    }

    public override bool isInInstance()
    {
        return _owner.isInInstance();
    }

    public override Instance? getInstanceWorld()
    {
        return _owner.getInstanceWorld();
    }

    public override Location Location => _owner.Location;

    public override double getRandomDamageMultiplier()
    {
        int random = (int)_owner.getStat().getValue(Stat.RANDOM_DAMAGE);
        return 1 + (double)Rnd.get(-random, random) / 100;
    }

    public override int getMagicAccuracy()
    {
        return _owner.getMagicAccuracy();
    }

    /**
     * @return the {@link CubicTemplate} of this cubic
     */
    public override CubicTemplate getTemplate()
    {
        return _template;
    }

    public override int Id => _template.getId();

    public override int getPAtk()
    {
        return _template.getBasePAtk();
    }

    public override int getMAtk()
    {
        return _template.getBaseMAtk();
    }

    public override Item? getActiveWeaponInstance()
    {
        return null;
    }

    public override Weapon? getActiveWeaponItem()
    {
        return null;
    }

    public override Item? getSecondaryWeaponInstance()
    {
        return null;
    }

    public override ItemTemplate? getSecondaryWeaponItem()
    {
        return null;
    }

    public override bool isAutoAttackable(Creature attacker)
    {
        return false;
    }

    public override bool spawnMe()
    {
        return true;
    }

    public override void onSpawn()
    {
    }

    public override bool deleteMe()
    {
        return true;
    }

    public override bool decayMe()
    {
        return true;
    }

    public override void onDecay()
    {
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    public override void onTeleported()
    {
    }

    public override void sendInfo(Player player)
    {
    }

    public override bool isInvul()
    {
        return _owner.isInvul();
    }

    public override bool isTargetable()
    {
        return false;
    }

    public override bool isUndying()
    {
        return true;
    }

    /**
     * Considered a player in order to send messages, calculate magic fail formula etc...
     */
    public override bool isPlayer()
    {
        return true;
    }

    public override bool isCubic()
    {
        return true;
    }
}