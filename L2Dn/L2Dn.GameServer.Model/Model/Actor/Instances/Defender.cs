using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Geo;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Templates;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Model.Actor.Instances;

public class Defender : Attackable
{
	private Castle? _castle; // the castle which the instance should defend
	private Fort? _fort; // the fortress which the instance should defend

	public Defender(NpcTemplate template): base(template)
	{
		InstanceType = InstanceType.Defender;
	}

	public override void addDamage(Creature attacker, int damage, Skill? skill)
	{
		base.addDamage(attacker, damage, skill);
        World.getInstance().forEachVisibleObjectInRange<Defender>(this, 500,
            defender => defender.addDamageHate(attacker, 0, 10));
    }

	/**
	 * Return True if a siege is in progress and the Creature attacker isn't a Defender.
	 * @param attacker The Creature that the SiegeGuard try to attack
	 */
	public override bool isAutoAttackable(Creature attacker)
	{
		// Attackable during siege by all except defenders
		if (!attacker.isPlayable())
		{
			return false;
		}

		Player? player = attacker.getActingPlayer();

		// Check if siege is in progress
		if ((_fort != null && _fort.getZone().isActive()) || (_castle != null && _castle.getZone().isActive()))
        {
            // TODO: null checking hack: both fort and castle cannot be null at the same time here
            int activeSiegeId = _fort?.getResidenceId() ?? (_castle?.getResidenceId() ?? 0);

			// Check if player is an enemy of this defender npc
			if (player != null && ((player.getSiegeState() == 2 && !player.isRegisteredOnThisSiegeField(activeSiegeId)) || player.getSiegeState() == 1 || player.getSiegeState() == 0))
			{
				return true;
			}
		}
		return false;
	}

	public override bool hasRandomAnimation()
	{
		return false;
	}

	/**
	 * This method forces guard to return to home location previously set
	 */
	public override void returnHome()
	{
		if (getWalkSpeed() <= 0)
		{
			return;
		}

        Spawn? spawn = getSpawn();
		if (spawn == null)
			return;

        if (!this.IsInsideRadius2D(spawn, 40))
		{
			clearAggroList();

			if (hasAI())
			{
				getAI().setIntention(CtrlIntention.AI_INTENTION_MOVE_TO, spawn.Location.Location3D);
			}
		}
	}

	public override void onSpawn()
	{
		base.onSpawn();

		_fort = InstanceManagers.FortManager.getInstance().getFort(Location.Location3D);
		_castle = CastleManager.getInstance().getCastle(Location.Location3D);
		if (_fort == null && _castle == null)
		{
			LOGGER.Warn("Defender spawned outside of Fortress or Castle zone!" + this);
		}
	}

	/**
	 * Custom onAction behaviour. Note that super() is not called because guards need extra check to see if a player should interact or ATTACK them when clicked.
	 */
	public override void onAction(Player player, bool interact)
	{
		if (!canTarget(player))
		{
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			return;
		}

		// Check if the Player already target the Npc
		if (this != player.getTarget())
		{
			// Set the target of the Player player
			player.setTarget(this);
		}
		else if (interact)
		{
			// this max heigth difference might need some tweaking
			if (isAutoAttackable(player) && !isAlikeDead() && Math.Abs(player.getZ() - getZ()) < 600)
			{
				player.getAI().setIntention(CtrlIntention.AI_INTENTION_ATTACK, this);
			}
			// Notify the Player AI with AI_INTENTION_INTERACT
			if (!isAutoAttackable(player) && !canInteract(player))
			{
				player.getAI().setIntention(CtrlIntention.AI_INTENTION_INTERACT, this);
			}
		}
		// Send a Server->Client ActionFailed to the Player in order to avoid that the client wait another packet
		player.sendPacket(ActionFailedPacket.STATIC_PACKET);
	}

	public override void useMagic(Skill skill)
	{
		if (!skill.IsBad)
		{
			Creature target = this;
			double lowestHpValue = double.MaxValue;
			foreach (Creature nearby in World.getInstance().getVisibleObjectsInRange<Creature>(this, skill.CastRange))
			{
				if (nearby == null || nearby.isDead() || !GeoEngine.getInstance().canSeeTarget(this, nearby))
				{
					continue;
				}
				if (nearby is Defender)
				{
					double targetHp = nearby.getCurrentHp();
					if (lowestHpValue > targetHp)
					{
						target = nearby;
						lowestHpValue = targetHp;
					}
				}
				else if (nearby.isPlayer())
				{
					Player player = (Player) nearby;
					if (player.getSiegeState() == 2 && !player.isRegisteredOnThisSiegeField(getScriptValue()))
					{
						double targetHp = nearby.getCurrentHp();
						if (lowestHpValue > targetHp)
						{
							target = nearby;
							lowestHpValue = targetHp;
						}
					}
				}
			}
			setTarget(target);
		}
		base.useMagic(skill);
	}

	public override void addDamageHate(Creature? attacker, long damage, long aggro)
	{
		if (attacker == null)
			return;

		if (attacker is not Defender)
		{
            Player? player = attacker.getActingPlayer();
			if (damage == 0 && aggro <= 1 && attacker.isPlayable() && player != null)
			{
				// Check if siege is in progress
				if ((_fort != null && _fort.getZone().isActive()) || (_castle != null && _castle.getZone().isActive()))
				{
					int activeSiegeId = _fort?.getResidenceId() ?? _castle?.getResidenceId() ?? 0;

					// Do not add hate on defenders.
					if (player.getSiegeState() == 2 && player.isRegisteredOnThisSiegeField(activeSiegeId))
					{
						return;
					}
				}
			}

			base.addDamageHate(attacker, damage, aggro);
		}
	}
}