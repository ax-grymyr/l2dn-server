using System.Globalization;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * another type of zone where your speed is changed
 * @author kerberos, Pandragon
 */
public class SwampZone : ZoneType
{
	private double _move_bonus;
	private int _castleId;
	private Castle? _castle;
	private int _eventId;

	public SwampZone(int id): base(id)
	{
		// Setup default speed reduce (in %)
		_move_bonus = 0.5;

		// no castle by default
		_castleId = 0;
		_castle = null;

		// no event by default
		_eventId = 0;
	}

	public override void setParameter(string name, string value)
	{
		if (name.equals("move_bonus"))
		{
			_move_bonus = double.Parse(value, CultureInfo.InvariantCulture);
		}
		else if (name.equals("castleId"))
		{
			_castleId = int.Parse(value);
		}
		else if (name.equals("eventId"))
		{
			_eventId = int.Parse(value);
		}
		else
		{
			base.setParameter(name, value);
		}
	}

	private Castle? getCastle()
	{
		if ((_castleId > 0) && (_castle == null))
		{
			_castle = CastleManager.getInstance().getCastleById(_castleId);
		}
		return _castle;
	}

	protected override void onEnter(Creature creature)
	{
		if (getCastle() != null)
		{
			// castle zones active only during siege
			if (!getCastle().getSiege().isInProgress() || !isEnabled())
			{
				return;
			}

			// defenders not affected
			Player player = creature.getActingPlayer();
			if ((player != null) && player.isInSiege() && (player.getSiegeState() == 2))
			{
				return;
			}
		}

		creature.setInsideZone(ZoneId.SWAMP, true);
		if (creature.isPlayer())
		{
			if (_eventId > 0)
			{
				creature.sendPacket(new OnEventTriggerPacket(_eventId, true));
			}

			creature.getActingPlayer().broadcastUserInfo();
		}
	}

	protected override void onExit(Creature creature)
	{
		// don't broadcast info if not needed
		if (creature.isInsideZone(ZoneId.SWAMP))
		{
			creature.setInsideZone(ZoneId.SWAMP, false);
			if (creature.isPlayer())
			{
				if (_eventId > 0)
				{
					creature.sendPacket(new OnEventTriggerPacket(_eventId, false));
				}
				if (!creature.isTeleporting())
				{
					creature.getActingPlayer().broadcastUserInfo();
				}
			}
		}
	}

	public double getMoveBonus()
	{
		return _move_bonus;
	}
}