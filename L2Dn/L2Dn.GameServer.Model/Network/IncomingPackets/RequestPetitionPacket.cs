using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestPetitionPacket: IIncomingPacket<GameSession>
{
    private string _content;
    private int _type; // 1 = on : 0 = off

    public void ReadContent(PacketBitReader reader)
    {
        _content = reader.ReadString();
        _type = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
	    Player? player = session.Player;
	    if (player == null)
		    return ValueTask.CompletedTask;

		if (_type <= 0 || _type >= 10)
			return ValueTask.CompletedTask;

		if (!GmManager.getInstance().IsGMOnline(false))
		{
			player.sendPacket(SystemMessageId.THERE_ARE_NO_GMS_CURRENTLY_VISIBLE_IN_THE_PUBLIC_LIST_AS_THEY_MAY_BE_PERFORMING_OTHER_FUNCTIONS_AT_THE_MOMENT);
			return ValueTask.CompletedTask;
		}

		if (!PetitionManager.getInstance().isPetitioningAllowed())
		{
			player.sendPacket(SystemMessageId.UNABLE_TO_CONNECT_TO_THE_GLOBAL_SUPPORT_SERVER);
			return ValueTask.CompletedTask;
		}

		if (PetitionManager.getInstance().isPlayerPetitionPending(player))
		{
			player.sendPacket(SystemMessageId.YOUR_GLOBAL_SUPPORT_REQUEST_WAS_RECEIVED);
			return ValueTask.CompletedTask;
		}

		if (PetitionManager.getInstance().getPendingPetitionCount() == Config.Character.MAX_PETITIONS_PENDING)
		{
			player.sendPacket(SystemMessageId.UNABLE_TO_SEND_YOUR_REQUEST_TO_THE_GLOBAL_SUPPORT_PLEASE_TRY_AGAIN_LATER);
			return ValueTask.CompletedTask;
		}

		SystemMessagePacket sm;
		int totalPetitions = PetitionManager.getInstance().getPlayerTotalPetitionCount(player) + 1;
		if (totalPetitions > Config.Character.MAX_PETITIONS_PER_PLAYER)
		{
			sm = new SystemMessagePacket(SystemMessageId.YOU_HAVE_SUBMITTED_MAXIMUM_NUMBER_OF_S1_GLOBAL_SUPPORT_REQUESTS_TODAY_YOU_CANNOT_SUBMIT_MORE_REQUESTS);
			sm.Params.addInt(totalPetitions);
			player.sendPacket(sm);
			return ValueTask.CompletedTask;
		}

		if (_content.Length > 255)
		{
			player.sendPacket(SystemMessageId.YOUR_GLOBAL_SUPPORT_REQUEST_CAN_CONTAIN_UP_TO_800_CHARACTERS);
			return ValueTask.CompletedTask;
		}

		int petitionId = PetitionManager.getInstance().submitPetition(player, _content, _type);
		sm = new SystemMessagePacket(SystemMessageId.YOUR_GLOBAL_SUPPORT_REQUEST_WAS_RECEIVED_REQUEST_NO_S1);
		sm.Params.addInt(petitionId);
		player.sendPacket(sm);

		sm = new SystemMessagePacket(SystemMessageId.SUPPORT_RECEIVED_S1_TIME_S_GLOBAL_SUPPORT_REQUESTS_LEFT_FOR_TODAY_S2);
		sm.Params.addInt(totalPetitions);
		sm.Params.addInt(Config.Character.MAX_PETITIONS_PER_PLAYER - totalPetitions);
		player.sendPacket(sm);

		sm = new SystemMessagePacket(SystemMessageId.S1_USERS_ARE_IN_LINE_TO_GET_THE_GLOBAL_SUPPORT);
		sm.Params.addInt(PetitionManager.getInstance().getPendingPetitionCount());
		player.sendPacket(sm);
		return ValueTask.CompletedTask;
    }
}