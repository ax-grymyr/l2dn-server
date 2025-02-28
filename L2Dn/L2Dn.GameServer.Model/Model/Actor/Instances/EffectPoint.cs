using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Actor.Instances;

public class EffectPoint : Npc
{
	private readonly Player _owner;
	private ScheduledFuture? _skillTask;

	public EffectPoint(NpcTemplate template, Player owner): base(template)
	{
		InstanceType = InstanceType.EffectPoint;
		setInvul(false);

		_owner = owner;
		setInstance(owner.getInstanceWorld());

		SkillHolder? skill = template.getParameters().getSkillHolder("union_skill");
		if (skill != null)
		{
			TimeSpan castTime = TimeSpan.FromMilliseconds(template.getParameters().getFloat("cast_time", 0.1f) * 1000);
			TimeSpan skillDelay = TimeSpan.FromMilliseconds(template.getParameters().getFloat("skill_delay", 2) * 1000);
			_skillTask = ThreadPool.scheduleAtFixedRate(() =>
			{
				if ((isDead() || !isSpawned()) && _skillTask != null)
				{
					_skillTask.cancel(false);
					_skillTask = null;
					return;
				}

				doCast(skill.getSkill());
			}, castTime, skillDelay);
		}
	}

	public override bool deleteMe()
	{
		if (_skillTask != null)
		{
			_skillTask.cancel(false);
			_skillTask = null;
		}
		return base.deleteMe();
	}

	public override Player getActingPlayer()
	{
		return _owner;
	}

	/**
	 * this is called when a player interacts with this NPC
	 * @param player
	 */
	public override void onAction(Player player, bool interact)
	{
		// Send a Server->Client ActionFailed to the Player in order to avoid that the client wait another packet
		player.sendPacket(ActionFailedPacket.STATIC_PACKET);
	}

	public override void onActionShift(Player player)
	{
		if (player == null)
		{
			return;
		}

		player.sendPacket(ActionFailedPacket.STATIC_PACKET);
	}

	/**
	 * Return the Party object of its Player owner or null.
	 */
	public override Party? getParty()
	{
		return _owner.getParty();
	}

	/**
	 * Return True if the Creature has a Party in progress.
	 */
	public override bool isInParty()
	{
		return _owner != null && _owner.isInParty();
	}

	public override int? getClanId()
	{
		return _owner != null ? _owner.getClanId() : null;
	}

	public override int? getAllyId()
	{
		return _owner != null ? _owner.getAllyId() : null;
	}

	public override PvpFlagStatus getPvpFlag()
	{
		return _owner != null ? _owner.getPvpFlag() : PvpFlagStatus.None;
	}

	public override Team getTeam()
	{
		return _owner != null ? _owner.getTeam() : Team.NONE;
	}
}