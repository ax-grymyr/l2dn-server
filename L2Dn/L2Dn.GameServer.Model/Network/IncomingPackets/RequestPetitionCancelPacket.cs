using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestPetitionCancelPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (PetitionManager.getInstance().isPlayerInConsultation(player))
        {
            if (player.isGM())
            {
                PetitionManager.getInstance().endActivePetition(player);
            }
            else
            {
                player.sendPacket(SystemMessageId.YOUR_GLOBAL_SUPPORT_REQUEST_IS_BEING_PROCESSED);
            }
        }
        else if (PetitionManager.getInstance().isPlayerPetitionPending(player))
        {
            if (PetitionManager.getInstance().cancelActivePetition(player))
            {
                int numRemaining = Config.MAX_PETITIONS_PER_PLAYER -
                                   PetitionManager.getInstance().getPlayerTotalPetitionCount(player);
                
                SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId
                    .YOUR_GLOBAL_SUPPORT_REQUEST_HAS_BEEN_REVOKED_NUMBER_OR_REQUESTS_YOU_CAN_SEND_S1);
                
                sm.Params.addString(numRemaining.ToString());
                player.sendPacket(sm);

                // Notify all GMs that the player's pending petition has been cancelled.
                string msgContent = player.getName() + " has canceled a pending petition.";
                AdminData.getInstance()
                    .broadcastToGMs(new CreatureSayPacket(player, ChatType.HERO_VOICE, "Petition System", msgContent));
            }
            else
            {
                player.sendPacket(SystemMessageId.FAILED_TO_CANCEL_YOUR_GLOBAL_SUPPORT_REQUEST_PLEASE_TRY_AGAIN_LATER);
            }
        }
        else
        {
            player.sendPacket(SystemMessageId.GLOBAL_SUPPORT_DOES_NOT_ACCEPT_REQUESTS_AT_THE_MOMENT);
        }

        return ValueTask.CompletedTask;
    }
}