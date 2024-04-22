using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.VoicedCommandHandlers;

/**
 * @author Zoey76
 */
public class CastleVCmd: IVoicedCommandHandler
{
	private static readonly String[] VOICED_COMMANDS =
	{
		"opendoors",
		"closedoors",
		"ridewyvern"
	};
	
	public bool useVoicedCommand(String command, Player activeChar, String @params)
	{
		switch (command)
		{
			case "opendoors":
			{
				if (!@params.equals("castle"))
				{
					activeChar.sendMessage("Only Castle doors can be open.");
					return false;
				}
				
				if (!activeChar.isClanLeader())
				{
					activeChar.sendPacket(SystemMessageId.ONLY_THE_CLAN_LEADER_MAY_ISSUE_COMMANDS);
					return false;
				}
				
				Door door = (Door) activeChar.getTarget();
				if (door == null)
				{
					activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
					return false;
				}

                int? castleId = activeChar.getClan()?.getCastleId();
                if (castleId is null)
                {
                    activeChar.sendMessage("Your clan does not own a castle.");
                    return false;
                }

                Castle castle = CastleManager.getInstance().getCastleById(castleId.Value);
				if (castle == null)
				{
					activeChar.sendMessage("Your clan does not own a castle.");
					return false;
				}
				
				if (castle.getSiege().isInProgress())
				{
					activeChar.sendPacket(SystemMessageId.THE_CASTLE_GATES_CANNOT_BE_OPENED_AND_CLOSED_DURING_A_SIEGE);
					return false;
				}
				
				if (castle.checkIfInZone(door.getX(), door.getY(), door.getZ()))
				{
					activeChar.sendPacket(SystemMessageId.THE_GATE_IS_BEING_OPENED);
					door.openMe();
				}
				break;
			}
			case "closedoors":
			{
				if (!@params.equals("castle"))
				{
					activeChar.sendMessage("Only Castle doors can be closed.");
					return false;
				}
				if (!activeChar.isClanLeader())
				{
					activeChar.sendPacket(SystemMessageId.ONLY_THE_CLAN_LEADER_MAY_ISSUE_COMMANDS);
					return false;
				}
                
				Door door2 = (Door) activeChar.getTarget();
				if (door2 == null)
				{
					activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
					return false;
				}
                
                int? castleId = activeChar.getClan()?.getCastleId();
                if (castleId is null)
                {
                    activeChar.sendMessage("Your clan does not own a castle.");
                    return false;
                }

                Castle castle2 = CastleManager.getInstance().getCastleById(castleId.Value);
				if (castle2 == null)
				{
					activeChar.sendMessage("Your clan does not own a castle.");
					return false;
				}
				
				if (castle2.getSiege().isInProgress())
				{
					activeChar.sendPacket(SystemMessageId.THE_CASTLE_GATES_CANNOT_BE_OPENED_AND_CLOSED_DURING_A_SIEGE);
					return false;
				}
				
				if (castle2.checkIfInZone(door2.getX(), door2.getY(), door2.getZ()))
				{
					activeChar.sendMessage("The gate is being closed.");
					door2.closeMe();
				}
				break;
			}
			case "ridewyvern":
			{
				if (activeChar.isClanLeader() && (activeChar.getClan().getCastleId() > 0))
				{
					activeChar.mount(15955, 0, true);
				}
				break;
			}
		}
		return true;
	}
	
	public String[] getVoicedCommandList()
	{
		return VOICED_COMMANDS;
	}
}