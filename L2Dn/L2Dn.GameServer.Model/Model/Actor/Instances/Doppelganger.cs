using System.Text;
using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Olympiads;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Templates;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Actor.Instances;

public class Doppelganger : Attackable
{
	private bool _copySummonerEffects = true;
	private ScheduledFuture? _attackTask;
	private Creature? _attackTarget;

	public Doppelganger(NpcTemplate template, Player owner): base(template)
	{
		setSummoner(owner);
		setCloneObjId(owner.ObjectId);
		setClanId(owner.getClanId());
		setInstance(owner.getInstanceWorld()); // set instance to same as owner
		setXYZInvisible(new Location3D(owner.getX() + Rnd.get(-100, 100), owner.getY() + Rnd.get(-100, 100), owner.getZ()));
		((DoppelgangerAI) getAI()).setStartFollowController(true);
		followSummoner(true);
	}

	protected override CreatureAI initAI()
	{
		return new DoppelgangerAI(this);
	}

    public override void onSpawn()
    {
        base.onSpawn();

        Creature? summoner = getSummoner();
        if (_copySummonerEffects && summoner != null)
        {
            foreach (BuffInfo summonerInfo in summoner.getEffectList().getEffects())
            {
                if (summonerInfo.getAbnormalTime() > TimeSpan.Zero)
                {
                    BuffInfo info = new BuffInfo(summoner, this, summonerInfo.getSkill(), false, null, null);
                    info.setAbnormalTime(summonerInfo.getAbnormalTime());
                    getEffectList().add(info);
                }
            }
        }
    }

    public void followSummoner(bool followSummoner)
	{
		if (followSummoner)
		{
			if (getAI().getIntention() == CtrlIntention.AI_INTENTION_IDLE || getAI().getIntention() == CtrlIntention.AI_INTENTION_ACTIVE)
			{
				setRunning();
				getAI().setIntention(CtrlIntention.AI_INTENTION_FOLLOW, getSummoner());
			}
		}
		else if (getAI().getIntention() == CtrlIntention.AI_INTENTION_FOLLOW)
		{
			getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
		}
	}

	public void setCopySummonerEffects(bool copySummonerEffects)
	{
		_copySummonerEffects = copySummonerEffects;
	}

	public void stopAttackTask()
	{
		if (_attackTask != null && !_attackTask.isCancelled() && !_attackTask.isDone())
		{
			_attackTask.cancel(false);
			_attackTask = null;
			_attackTarget = null;
		}
	}

	public void startAttackTask(Creature target)
	{
		stopAttackTask();
		_attackTarget = target;
		_attackTask = ThreadPool.scheduleAtFixedRate(thinkCombat, 1000, 1000);
	}

	private void thinkCombat()
	{
		if (_attackTarget == null)
		{
			stopAttackTask();
			return;
		}

		doAutoAttack(_attackTarget);
		// TODO: Cast skills.
	}

	public override PvpFlagStatus getPvpFlag()
	{
        Creature? summoner = getSummoner();
		return summoner?.getPvpFlag() ?? PvpFlagStatus.None;
	}

	public override Team getTeam()
	{
        Creature? summoner = getSummoner();
		return summoner?.getTeam() ?? Team.NONE;
	}

	public override bool isAutoAttackable(Creature attacker)
	{
        Creature? summoner = getSummoner();
		return summoner?.isAutoAttackable(attacker) ?? base.isAutoAttackable(attacker);
	}

	public override void doAttack(double damage, Creature target, Skill? skill, bool isDOT, bool directlyToHp, bool critical, bool reflect)
	{
		base.doAttack(damage, target, skill, isDOT, directlyToHp, critical, reflect);
		sendDamageMessage(target, skill, (int) damage, 0, critical, false, false);
	}

	public override void sendDamageMessage(Creature target, Skill? skill, int damage, double elementalDamage, bool crit, bool miss, bool elementalCrit)
	{
        Creature? summoner = getSummoner();
        Player? actingPlayer = getActingPlayer();
		if (miss || summoner == null || !summoner.isPlayer() || actingPlayer == null)
			return;

		// Prevents the double spam of system messages, if the target is the owning player.
		if (target.ObjectId != summoner.ObjectId)
        {
            if (actingPlayer.isInOlympiadMode() && target.isPlayer() && ((Player)target).isInOlympiadMode() &&
                ((Player)target).getOlympiadGameId() == actingPlayer.getOlympiadGameId())
            {
                OlympiadGameManager.getInstance().notifyCompetitorDamage(actingPlayer, damage);
            }

            SystemMessagePacket sm;
            if ((target.isHpBlocked() && !target.isNpc()) || (target.isPlayer() &&
                    target.isAffected(EffectFlags.DUELIST_FURY) && !actingPlayer.isAffected(EffectFlags.FACEOFF)))
            {
                sm = new SystemMessagePacket(SystemMessageId.THE_ATTACK_HAS_BEEN_BLOCKED);
            }
            else
			{
				sm = new SystemMessagePacket(SystemMessageId.C1_HAS_DEALT_S3_DAMAGE_TO_C2);
				sm.Params.addNpcName(this);
				sm.Params.addString(target.getName());
				sm.Params.addInt(damage);
				sm.Params.addPopup(target.ObjectId, ObjectId, damage * -1);
			}

			sendPacket(sm);
		}
	}

	public override void reduceCurrentHp(double damage, Creature attacker, Skill? skill)
	{
		base.reduceCurrentHp(damage, attacker, skill);

        Creature? summoner = getSummoner();
		if (summoner != null && summoner.isPlayer() && attacker != null && !isDead() && !isHpBlocked())
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.C1_HAS_RECEIVED_S3_DAMAGE_FROM_C2);
			sm.Params.addNpcName(this);
			sm.Params.addString(attacker.getName());
			sm.Params.addInt((int) damage);
			sm.Params.addPopup(ObjectId, attacker.ObjectId, (int) -damage);
			sendPacket(sm);
		}
	}

	public override Player? getActingPlayer()
	{
        Creature? summoner = getSummoner();
		return summoner != null ? summoner.getActingPlayer() : base.getActingPlayer();
	}

	public override bool deleteMe()
	{
		stopAttackTask();
		return base.deleteMe();
	}

	public override void onTeleported()
	{
		deleteMe(); // In retail, doppelgangers disappear when summoner teleports.
	}

	public override void sendPacket<TPacket>(TPacket packet)
	{
		getSummoner()?.sendPacket(packet);
	}

	public override void sendPacket(SystemMessageId id)
	{
		getSummoner()?.sendPacket(id);
	}

	public override string ToString()
	{
		StringBuilder sb = new();
		sb.Append(base.ToString());
		sb.Append("(");
		sb.Append(Id);
		sb.Append(") Summoner: ");
		sb.Append(getSummoner());
		return sb.ToString();
	}
}