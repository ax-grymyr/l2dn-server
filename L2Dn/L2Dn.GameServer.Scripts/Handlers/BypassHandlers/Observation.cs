using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Geometry;
using NLog;

namespace L2Dn.GameServer.Scripts.Handlers.BypassHandlers;

public class Observation: IBypassHandler
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(Observation));
	
	private static readonly string[] COMMANDS =
	{
		"observesiege",
		"observeoracle",
		"observe"
	};
	
	private static readonly int[][] LOCATIONS = [
		//@formatter:off
		// Gludio
		[-18347, 114000, -2360, 500],
		[-18347, 113255, -2447, 500],
		// Dion
		[22321, 155785, -2604, 500],
		[22321, 156492, -2627, 500],
		// Giran
		[112000, 144864, -2445, 500],
		[112657, 144864, -2525, 500],
		// Innadril
		[116260, 244600, -775, 500],
		[116260, 245264, -721, 500],
		// Oren
		[78100, 36950, -2242, 500],
		[78744, 36950, -2244, 500],
		// Aden
		[147457, 9601, -233, 500],
		[147457, 8720, -252, 500],
		// Goddard
		[147542, -43543, -1328, 500],
		[147465, -45259, -1328, 500],
		// Rune
		[20598, -49113, -300, 500],
		[18702, -49150, -600, 500],
		// Schuttgart
		[77541, -147447, 353, 500],
		[77541, -149245, 353, 500],
		// Coliseum
		[148416, 46724, -3000, 80],
		[149500, 46724, -3000, 80],
		[150511, 46724, -3000, 80],
		// Dusk
		[-77200, 88500, -4800, 500],
		[-75320, 87135, -4800, 500],
		[-76840, 85770, -4800, 500],
		[-76840, 85770, -4800, 500],
		[-79950, 85165, -4800, 500],
		// Dawn
		[-79185, 112725, -4300, 500],
		[-76175, 113330, -4300, 500],
		[-74305, 111965, -4300, 500],
		[-75915, 110600, -4300, 500],
		[-78930, 110005, -4300, 500]
		//@formatter:on
	];
	
	public bool useBypass(String command, Player player, Creature target)
	{
		if (!(target is BroadcastingTower))
		{
			return false;
		}
		
		if (player.hasSummon())
		{
			player.sendPacket(SystemMessageId.YOU_MAY_NOT_OBSERVE_A_SIEGE_WITH_A_SERVITOR_SUMMONED);
			return false;
		}
		if (player.isOnEvent())
		{
			player.sendMessage("Cannot use while current Event");
			return false;
		}
		
		String _command = command.Split(" ")[0].ToLower();
		int param;
		try
		{
			param = int.Parse(command.Split(" ")[1]);
		}
		catch (FormatException nfe)
		{
			_logger.Warn("Exception in " + GetType().Name, nfe);
			return false;
		}
		
		if ((param < 0) || (param > (LOCATIONS.Length - 1)))
		{
			return false;
		}
		int[] locCost = LOCATIONS[param];
		LocationHeading loc = new LocationHeading(locCost[0], locCost[1], locCost[2], 0);
		long cost = locCost[3];
		
		switch (_command)
		{
			case "observesiege":
			{
				if (SiegeManager.getInstance().getSiege(loc.Location) != null)
				{
					doObserve(player, (Npc) target, loc, cost);
				}
				else
				{
					player.sendPacket(SystemMessageId.SPECTATOR_MODE_IS_ONLY_AVAILABLE_DURING_A_SIEGE);
				}
				return true;
			}
			case "observeoracle": // Oracle Dusk/Dawn
			{
				doObserve(player, (Npc) target, loc, cost);
				return true;
			}
			case "observe": // Observe
			{
				doObserve(player, (Npc) target, loc, cost);
				return true;
			}
		}
		return false;
	}
	
	private void doObserve(Player player, Npc npc, LocationHeading pos, long cost)
	{
		if (player.reduceAdena("Broadcast", cost, npc, true))
		{
			// enter mode
			player.enterObserverMode(pos);
			player.sendItemList();
		}
		player.sendPacket(ActionFailedPacket.STATIC_PACKET);
	}
	
	public String[] getBypassList()
	{
		return COMMANDS;
	}
}