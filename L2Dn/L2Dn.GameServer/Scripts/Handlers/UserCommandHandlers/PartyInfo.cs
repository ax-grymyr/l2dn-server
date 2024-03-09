using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Handlers.UserCommandHandlers;

/**
 * Party Info user command.
 * @author Tempy
 */
public class PartyInfo: IUserCommandHandler
{
	private static readonly int[] COMMAND_IDS =
	{
		81
	};
	
	public bool useUserCommand(int id, Player player)
	{
		if (id != COMMAND_IDS[0])
		{
			return false;
		}
		
		player.sendPacket(SystemMessageId.PARTY_INFORMATION);
		if (player.isInParty())
		{
			Party party = player.getParty();
			switch (party.getDistributionType())
			{
				case PartyDistributionType.FINDERS_KEEPERS:
				{
					player.sendPacket(SystemMessageId.LOOT_FINDERS_KEEPERS);
					break;
				}
				case PartyDistributionType.RANDOM:
				{
					player.sendPacket(SystemMessageId.LOOT_RANDOM);
					break;
				}
				case PartyDistributionType.RANDOM_INCLUDING_SPOIL:
				{
					player.sendPacket(SystemMessageId.LOOT_RANDOM_INCLUDING_SPOILS);
					break;
				}
				case PartyDistributionType.BY_TURN:
				{
					player.sendPacket(SystemMessageId.LOOTING_METHOD_BY_TURN);
					break;
				}
				case PartyDistributionType.BY_TURN_INCLUDING_SPOIL:
				{
					player.sendPacket(SystemMessageId.LOOTING_METHOD_BY_TURN_INCLUDING_SPOIL);
					break;
				}
			}
			
			// Not used in Infinite Odissey
			// if (!party.isLeader(player))
			// {
			// SystemMessage sm = SystemMessage.getSystemMessage(SystemMessageId.PARTY_LEADER_C1);
			// sm.addPcName(party.getLeader());
			// player.sendPacket(sm);
			// }
		}
		player.sendPacket(SystemMessageId.EMPTY_3);
		return true;
	}
	
	public int[] getUserCommandList()
	{
		return COMMAND_IDS;
	}
}