using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Olympiads;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Handlers.UserCommandHandlers;

/**
 * Olympiad Stat user command.
 * @author kamy, Zoey76
 */
public class OlympiadStat: IUserCommandHandler
{
	private static readonly int[] COMMAND_IDS =
	{
		109
	};
	
	public bool useUserCommand(int id, Player player)
	{
		if (!Config.OLYMPIAD_ENABLED)
		{
			player.sendPacket(SystemMessageId.THE_OLYMPIAD_IS_NOT_HELD_RIGHT_NOW);
			return false;
		}
		
		if (id != COMMAND_IDS[0])
		{
			return false;
		}
		
		int nobleObjId = player.getObjectId();
		WorldObject target = player.getTarget();
		if ((target == null) || !target.isPlayer() || (target.getActingPlayer().getClassId().GetLevel() < 2))
		{
			player.sendPacket(SystemMessageId.COMMAND_AVAILABLE_FOR_THOSE_WHO_HAVE_COMPLETED_2ND_CLASS_TRANSFER);
			return false;
		}
		
		SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.FOR_THE_CURRENT_OLYMPIAD_YOU_HAVE_PARTICIPATED_IN_S1_MATCH_ES_AND_HAD_S2_WIN_S_AND_S3_DEFEAT_S_YOU_CURRENTLY_HAVE_S4_OLYMPIAD_POINT_S);
		sm.Params.addInt(Olympiad.getInstance().getCompetitionDone(nobleObjId));
		sm.Params.addInt(Olympiad.getInstance().getCompetitionWon(nobleObjId));
		sm.Params.addInt(Olympiad.getInstance().getCompetitionLost(nobleObjId));
		sm.Params.addInt(Olympiad.getInstance().getNoblePoints((Player) target));
		player.sendPacket(sm);
		
		SystemMessagePacket sm2 = new SystemMessagePacket(SystemMessageId.THIS_WEEK_YOU_CAN_PARTICIPATE_IN_A_TOTAL_OF_S1_MATCHES);
		sm2.Params.addInt(Olympiad.getInstance().getRemainingWeeklyMatches(nobleObjId));
		player.sendPacket(sm2);
		return true;
	}
	
	public int[] getUserCommandList()
	{
		return COMMAND_IDS;
	}
}