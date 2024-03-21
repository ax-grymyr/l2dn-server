using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestExAskJoinMPCCPacket: IIncomingPacket<GameSession>
{
    private string _name;

    public void ReadContent(PacketBitReader reader)
    {
        _name = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
	    Player? player = session.Player;
	    if (player == null)
		    return ValueTask.CompletedTask;
		
		Player target = World.getInstance().getPlayer(_name);
		if (target == null)
			return ValueTask.CompletedTask;

		// invite yourself? ;)
		if (player.isInParty() && target.isInParty() && player.getParty() == target.getParty())
			return ValueTask.CompletedTask;
		
		SystemMessagePacket sm;
		// activeChar is in a Party?
		if (player.isInParty())
		{
			Party activeParty = player.getParty();
			// activeChar is PartyLeader? && activeChars Party is already in a CommandChannel?
			if (activeParty.getLeader() == player)
			{
				// if activeChars Party is in CC, is activeChar CCLeader?
				if (activeParty.isInCommandChannel() && activeParty.getCommandChannel().getLeader().Equals(player))
				{
					// in CC and the CCLeader
					// target in a party?
					if (target.isInParty())
					{
						// targets party already in a CChannel?
						if (target.getParty().isInCommandChannel())
						{
							sm = new SystemMessagePacket(SystemMessageId.C1_S_PARTY_IS_ALREADY_A_MEMBER_OF_THE_COMMAND_CHANNEL);
							sm.Params.addString(target.getName());
							player.sendPacket(sm);
						}
						else
						{
							// ready to open a new CC
							// send request to targets Party's PartyLeader
							askJoinMPCC(player, target);
						}
					}
					else
					{
						player.sendMessage(target.getName() + " doesn't have party and cannot be invited to Command Channel.");
					}
				}
				else if (activeParty.isInCommandChannel() && !activeParty.getCommandChannel().getLeader().Equals(player))
				{
					// in CC, but not the CCLeader
					sm = new SystemMessagePacket(SystemMessageId.YOU_DO_NOT_HAVE_AUTHORITY_TO_INVITE_SOMEONE_TO_THE_COMMAND_CHANNEL);
					player.sendPacket(sm);
				}
				else
				{
					// target in a party?
					if (target.isInParty())
					{
						// targets party already in a CChannel?
						if (target.getParty().isInCommandChannel())
						{
							sm = new SystemMessagePacket(SystemMessageId.C1_S_PARTY_IS_ALREADY_A_MEMBER_OF_THE_COMMAND_CHANNEL);
							sm.Params.addString(target.getName());
							player.sendPacket(sm);
						}
						else
						{
							// ready to open a new CC
							// send request to targets Party's PartyLeader
							askJoinMPCC(player, target);
						}
					}
					else
					{
						player.sendMessage(target.getName() + " doesn't have party and cannot be invited to Command Channel.");
					}
				}
			}
			else
			{
				player.sendPacket(SystemMessageId.YOU_DO_NOT_HAVE_AUTHORITY_TO_INVITE_SOMEONE_TO_THE_COMMAND_CHANNEL);
			}
		}
		
		return ValueTask.CompletedTask;
	}
	
	private void askJoinMPCC(Player requestor, Player target)
	{
		bool hasRight = false;
		if (requestor.isClanLeader() && (requestor.getClan().getLevel() >= 5))
		{
			// Clan leader of level5 Clan or higher.
			hasRight = true;
		}
		else if (requestor.getInventory().getItemByItemId(8871) != null)
		{
			// 8871 Strategy Guide.
			// TODO: Should destroyed after successful invite?
			hasRight = true;
		}
		else if ((requestor.getPledgeClass() >= SocialClass.ELDER) && (requestor.getKnownSkill(391) != null))
		{
			// At least Baron or higher and the skill Clan Imperium
			hasRight = true;
		}
		
		if (!hasRight)
		{
			requestor.sendPacket(SystemMessageId.ONLY_A_PARTY_LEADER_WHO_IS_ALSO_A_LV_5_CLAN_LEADER_CAN_CREATE_A_COMMAND_CHANNEL);
			return;
		}
		
		// Get the target's party leader, and do whole actions on him.
		Player targetLeader = target.getParty().getLeader();
		SystemMessagePacket sm;
		if (!targetLeader.isProcessingRequest())
		{
			requestor.onTransactionRequest(targetLeader);
			sm = new SystemMessagePacket(SystemMessageId.C1_IS_INVITING_YOU_TO_A_COMMAND_CHANNEL_DO_YOU_ACCEPT);
			sm.Params.addString(requestor.getName());
			targetLeader.sendPacket(sm);
			targetLeader.sendPacket(new ExAskJoinMPCCPacket(requestor.getName()));
			requestor.sendMessage("You invited " + targetLeader.getName() + " to your Command Channel.");
		}
		else
		{
			sm = new SystemMessagePacket(SystemMessageId.C1_IS_ON_ANOTHER_TASK_PLEASE_TRY_AGAIN_LATER);
			sm.Params.addString(targetLeader.getName());
			requestor.sendPacket(sm);
		}
	}
}