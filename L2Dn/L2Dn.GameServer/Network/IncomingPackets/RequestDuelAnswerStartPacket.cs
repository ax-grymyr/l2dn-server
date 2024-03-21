using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestDuelAnswerStartPacket: IIncomingPacket<GameSession>
{
    private bool _partyDuel;
    private int _response;

    public void ReadContent(PacketBitReader reader)
    {
        _partyDuel = reader.ReadInt32() != 0;
        reader.ReadInt32(); // unknown
        _response = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
	    Player? player = session.Player;
	    if (player == null)
		    return ValueTask.CompletedTask;
		
		Player requestor = player.getActiveRequester();
		if (requestor == null)
			return ValueTask.CompletedTask;
		
		if (_response == 1)
		{
			SystemMessagePacket msg1;
			SystemMessagePacket msg2;
			if (requestor.isInDuel())
			{
				msg1 = new SystemMessagePacket(SystemMessageId.C1_IS_ALREADY_IN_A_DUEL);
				msg1.Params.addString(requestor.getName());
				player.sendPacket(msg1);
				return ValueTask.CompletedTask;
			}
			
			if (player.isInDuel())
			{
				player.sendPacket(SystemMessageId.YOU_ARE_UNABLE_TO_REQUEST_A_DUEL_AT_THIS_TIME);
				return ValueTask.CompletedTask;
			}
			
			if (_partyDuel)
			{
				msg1 = new SystemMessagePacket(SystemMessageId.YOU_HAVE_ACCEPTED_C1_S_CHALLENGE_TO_A_PARTY_DUEL_THE_DUEL_WILL_BEGIN_IN_A_FEW_MOMENTS);
				msg1.Params.addString(requestor.getName());
				
				msg2 = new SystemMessagePacket(SystemMessageId.C1_HAS_ACCEPTED_YOUR_CHALLENGE_TO_DUEL_AGAINST_THEIR_PARTY_THE_DUEL_WILL_BEGIN_IN_A_FEW_MOMENTS);
				msg2.Params.addString(player.getName());
			}
			else
			{
				msg1 = new SystemMessagePacket(SystemMessageId.YOU_HAVE_ACCEPTED_C1_S_CHALLENGE_A_DUEL_THE_DUEL_WILL_BEGIN_IN_A_FEW_MOMENTS);
				msg1.Params.addString(requestor.getName());
				
				msg2 = new SystemMessagePacket(SystemMessageId.C1_HAS_ACCEPTED_YOUR_CHALLENGE_TO_A_DUEL_THE_DUEL_WILL_BEGIN_IN_A_FEW_MOMENTS);
				msg2.Params.addString(player.getName());
			}
			
			player.sendPacket(msg1);
			requestor.sendPacket(msg2);
			
			DuelManager.getInstance().addDuel(requestor, player, _partyDuel);
		}
		else if (_response == -1)
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.C1_IS_SET_TO_REFUSE_DUEL_REQUESTS_AND_CANNOT_RECEIVE_A_DUEL_REQUEST);
			sm.Params.addPcName(player);
			requestor.sendPacket(sm);
		}
		else
		{
			SystemMessagePacket msg;
			if (_partyDuel)
			{
				msg = new SystemMessagePacket(SystemMessageId.THE_OPPOSING_PARTY_HAS_DECLINED_YOUR_CHALLENGE_TO_A_DUEL);
			}
			else
			{
				msg = new SystemMessagePacket(SystemMessageId.C1_HAS_DECLINED_YOUR_CHALLENGE_TO_A_DUEL);
				msg.Params.addPcName(player);
			}
			
			requestor.sendPacket(msg);
		}
		
		player.setActiveRequester(null);
		requestor.onTransactionResponse();

		return ValueTask.CompletedTask;
	}
}