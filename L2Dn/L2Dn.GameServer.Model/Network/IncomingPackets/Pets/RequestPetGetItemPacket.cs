using L2Dn.GameServer.AI;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Pets;

public struct RequestPetGetItemPacket: IIncomingPacket<GameSession>
{
    private int _objectId;

    public void ReadContent(PacketBitReader reader)
    {
        _objectId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null || !player.hasPet())
        {
            connection.Send(ActionFailedPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        World world = World.getInstance();
        Item? item = world.findObject(_objectId) as Item;
        if (item == null)
        {
            connection.Send(ActionFailedPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        Castle? castle = CastleManager.getInstance().getCastle(item);
        if (castle != null && SiegeGuardManager.getInstance().getSiegeGuardByItem(castle.getResidenceId(), item.getId()) != null)
        {
            connection.Send(ActionFailedPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        if (FortSiegeManager.getInstance().isCombat(item.getId()))
        {
            connection.Send(ActionFailedPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        Pet? pet = player.getPet();
        if (pet == null || pet.isDead() || pet.isControlBlocked())
        {
            connection.Send(ActionFailedPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }

        if (pet.isUncontrollable())
        {
            connection.Send(SystemMessageId.WHEN_YOUR_PET_S_SATIETY_REACHES_0_YOU_CANNOT_CONTROL_IT);
            return ValueTask.CompletedTask;
        }

        pet.getAI().setIntention(CtrlIntention.AI_INTENTION_PICK_UP, item);

        return ValueTask.CompletedTask;
    }
}