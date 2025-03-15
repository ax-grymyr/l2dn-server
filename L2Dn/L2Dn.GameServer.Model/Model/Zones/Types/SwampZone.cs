using System.Globalization;
using L2Dn.GameServer.Dto.ZoneForms;
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
public class SwampZone(int id, ZoneForm form): ZoneType(id, form)
{
    private double _moveBonus = 0.5; // Setup default speed reduce (in %)
    private int _castleId; // no castle by default
    private Castle? _castle;
    private int _eventId; // no event by default

    public override void setParameter(string name, string value)
    {
        if (name.equals("move_bonus"))
        {
            _moveBonus = double.Parse(value, CultureInfo.InvariantCulture);
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
        if (_castleId > 0 && _castle == null)
        {
            _castle = CastleManager.getInstance().getCastleById(_castleId);
        }

        return _castle;
    }

    protected override void onEnter(Creature creature)
    {
        Player? player = creature.getActingPlayer();
        if (getCastle() is { } castle)
        {
            // castle zones active only during siege
            if (!castle.getSiege().isInProgress() || !isEnabled())
            {
                return;
            }

            // defenders not affected
            if (player != null && player.isInSiege() && player.getSiegeState() == 2)
            {
                return;
            }
        }

        creature.setInsideZone(ZoneId.SWAMP, true);
        if (creature.isPlayer() && player != null)
        {
            if (_eventId > 0)
            {
                creature.sendPacket(new OnEventTriggerPacket(_eventId, true));
            }

            player.broadcastUserInfo();
        }
    }

    protected override void onExit(Creature creature)
    {
        // don't broadcast info if not needed
        if (creature.isInsideZone(ZoneId.SWAMP))
        {
            creature.setInsideZone(ZoneId.SWAMP, false);
            Player? player = creature.getActingPlayer();
            if (creature.isPlayer() && player != null)
            {
                if (_eventId > 0)
                {
                    creature.sendPacket(new OnEventTriggerPacket(_eventId, false));
                }

                if (!creature.isTeleporting())
                {
                    player.broadcastUserInfo();
                }
            }
        }
    }

    public double getMoveBonus()
    {
        return _moveBonus;
    }
}