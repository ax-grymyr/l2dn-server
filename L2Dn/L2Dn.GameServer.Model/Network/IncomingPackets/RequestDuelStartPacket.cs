using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Geometry;
using L2Dn.Network;
using L2Dn.Packets;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestDuelStartPacket: IIncomingPacket<GameSession>
{
    private string _player;
    private bool _partyDuel;

    public void ReadContent(PacketBitReader reader)
    {
        _player = reader.ReadString();
        _partyDuel = reader.ReadInt32() != 0;
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
	{
		Player? player = session.Player;
		if (player == null)
			return ValueTask.CompletedTask;

		if (FakePlayerData.getInstance().isTalkable(_player))
		{
			SystemMessagePacket sm;
			string name = FakePlayerData.getInstance().getProperName(_player);
			if (player.isInsideZone(ZoneId.PVP) || player.isInsideZone(ZoneId.PEACE) || player.isInsideZone(ZoneId.SIEGE))
			{
				sm = new SystemMessagePacket(SystemMessageId.C1_IS_IN_AN_AREA_WHERE_DUEL_IS_NOT_ALLOWED_AND_YOU_CANNOT_APPLY_FOR_A_DUEL);
				sm.Params.addString(name);
				player.sendPacket(sm);
				return ValueTask.CompletedTask;
			}

			bool npcInRange = false;
			foreach (Npc npc in World.getInstance().getVisibleObjectsInRange<Npc>(player, 250))
			{
				if (string.Equals(npc.getName(), name))
				{
					npcInRange = true;
				}
			}

			if (!npcInRange)
			{
				sm = new SystemMessagePacket(SystemMessageId.C1_IS_TOO_FAR_AWAY_TO_RECEIVE_A_DUEL_CHALLENGE);
				sm.Params.addString(name);
				player.sendPacket(sm);
				return ValueTask.CompletedTask;
			}

			if (player.isProcessingRequest())
			{
				sm = new SystemMessagePacket(SystemMessageId.C1_IS_ON_ANOTHER_TASK_PLEASE_TRY_AGAIN_LATER);
				sm.Params.addString(name);
				player.sendPacket(sm);
				return ValueTask.CompletedTask;
			}

			sm = new SystemMessagePacket(SystemMessageId.C1_HAS_BEEN_CHALLENGED_TO_A_DUEL);
			sm.Params.addString(name);
			player.sendPacket(sm);
			ThreadPool.schedule(() => scheduleDeny(player, name), 10000);
			player.blockRequest();
			return ValueTask.CompletedTask;
		}

		Player targetChar = World.getInstance().getPlayer(_player);
		if (targetChar == null)
		{
			player.sendPacket(SystemMessageId.THERE_IS_NO_OPPONENT_TO_RECEIVE_YOUR_CHALLENGE_FOR_A_DUEL);
			return ValueTask.CompletedTask;
		}

		if (player == targetChar)
		{
			player.sendPacket(SystemMessageId.THERE_IS_NO_OPPONENT_TO_RECEIVE_YOUR_CHALLENGE_FOR_A_DUEL);
			return ValueTask.CompletedTask;
		}

		// Check if duel is possible
		if (!player.canDuel())
		{
			player.sendPacket(SystemMessageId.YOU_ARE_UNABLE_TO_REQUEST_A_DUEL_AT_THIS_TIME);
			return ValueTask.CompletedTask;
		}

		if (!targetChar.canDuel())
		{
			player.sendPacket(targetChar.getNoDuelReason());
			return ValueTask.CompletedTask;
		}

		// Players may not be too far apart
		if (!player.IsInsideRadius2D(targetChar, 250))
		{
			SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.C1_IS_TOO_FAR_AWAY_TO_RECEIVE_A_DUEL_CHALLENGE);
			msg.Params.addString(targetChar.getName());
			player.sendPacket(msg);
			return ValueTask.CompletedTask;
		}

		// Duel is a party duel
		if (_partyDuel)
		{
			// Player must be in a party & the party leader
			Party? party = player.getParty();
			if ((party == null) || !party.isLeader(player))
			{
				player.sendMessage("You have to be the leader of a party in order to request a party duel.");
				return ValueTask.CompletedTask;
			}

			// Target must be in a party
			if (!targetChar.isInParty())
			{
				player.sendPacket(SystemMessageId.SINCE_THE_PERSON_YOU_CHALLENGED_IS_NOT_CURRENTLY_IN_A_PARTY_THEY_CANNOT_DUEL_AGAINST_YOUR_PARTY);
				return ValueTask.CompletedTask;
			}

			// Target may not be of the same party
			if (party.containsPlayer(targetChar))
			{
				player.sendMessage("This player is a member of your own party.");
				return ValueTask.CompletedTask;
			}

			// Check if every player is ready for a duel
			foreach (Player temp in party.getMembers())
			{
				if (!temp.canDuel())
				{
					player.sendMessage("Not all the members of your party are ready for a duel.");
					return ValueTask.CompletedTask;
				}
			}

			Player? partyLeader = null; // snatch party leader of targetChar's party
			foreach (Player temp in targetChar.getParty().getMembers())
			{
				if (partyLeader == null)
				{
					partyLeader = temp;
				}

				if (!temp.canDuel())
				{
					player.sendPacket(SystemMessageId.THE_OPPOSING_PARTY_IS_CURRENTLY_UNABLE_TO_ACCEPT_A_CHALLENGE_TO_A_DUEL);
					return ValueTask.CompletedTask;
				}
			}

			// Send request to targetChar's party leader
			if (partyLeader != null)
			{
				if (!partyLeader.isProcessingRequest())
				{
					player.onTransactionRequest(partyLeader);
					partyLeader.sendPacket(new ExDuelAskStartPacket(player.getName(), _partyDuel));
					SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.C1_S_PARTY_HAS_BEEN_CHALLENGED_TO_A_DUEL);
					msg.Params.addString(partyLeader.getName());
					player.sendPacket(msg);

					msg = new SystemMessagePacket(SystemMessageId.C1_S_PARTY_HAS_CHALLENGED_YOUR_PARTY_TO_A_DUEL);
					msg.Params.addString(player.getName());
					targetChar.sendPacket(msg);
				}
				else
				{
					SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.C1_IS_ON_ANOTHER_TASK_PLEASE_TRY_AGAIN_LATER);
					msg.Params.addString(partyLeader.getName());
					player.sendPacket(msg);
				}
			}
		}
		else
		// 1vs1 duel
		{
			if (!targetChar.isProcessingRequest())
			{
				player.onTransactionRequest(targetChar);
				targetChar.sendPacket(new ExDuelAskStartPacket(player.getName(), _partyDuel));
				SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.C1_HAS_BEEN_CHALLENGED_TO_A_DUEL);
				msg.Params.addString(targetChar.getName());
				player.sendPacket(msg);

				msg = new SystemMessagePacket(SystemMessageId.C1_HAS_CHALLENGED_YOU_TO_A_DUEL);
				msg.Params.addString(player.getName());
				targetChar.sendPacket(msg);
			}
			else
			{
				SystemMessagePacket msg = new SystemMessagePacket(SystemMessageId.C1_IS_ON_ANOTHER_TASK_PLEASE_TRY_AGAIN_LATER);
				msg.Params.addString(targetChar.getName());
				player.sendPacket(msg);
			}
		}

		return ValueTask.CompletedTask;
    }

    private static void scheduleDeny(Player player, string name)
	{
		if (player != null)
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.C1_HAS_DECLINED_YOUR_CHALLENGE_TO_A_DUEL);
			sm.Params.addString(name);
			player.sendPacket(sm);
			player.onTransactionResponse();
		}
	}
}