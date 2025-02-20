using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.OutgoingPackets.Pets;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Pets;

public struct RequestGetItemFromPetPacket: IIncomingPacket<GameSession>
{
    private int _objectId;
    private long _amount;
    private int _unknown;

    public void ReadContent(PacketBitReader reader)
    {
        _objectId = reader.ReadInt32();
        _amount = reader.ReadInt64();
        _unknown = reader.ReadInt32(); // = 0 for most trades
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (_amount <= 0 || player == null || !player.hasPet())
            return ValueTask.CompletedTask;

        // TODO: flood protection
        // if (!client.getFloodProtectors().canPerformTransaction())
        // {
        //     player.sendMessage("You get items from pet too fast.");
        //     return;
        // }

        if (player.hasItemRequest())
            return ValueTask.CompletedTask;

        Pet? pet = player.getPet();
        Item? item = pet?.getInventory().getItemByObjectId(_objectId);
        if (pet == null || item == null)
            return ValueTask.CompletedTask;

        if (_amount > item.getCount())
        {
            Util.handleIllegalPlayerAction(player,
                GetType().Name + ": Character " + player.getName() + " of account " + player.getAccountName() +
                " tried to get item with oid " + _objectId + " from pet but has invalid count " + _amount +
                " item count: " + item.getCount(), Config.DEFAULT_PUNISH);

            return ValueTask.CompletedTask;
        }

        Item transferedItem = pet.transferItem("Transfer", _objectId, _amount, player.getInventory(), player, pet);
        if (transferedItem != null)
        {
            player.sendPacket(new PetItemListPacket(pet.getInventory().getItems()));
        }
        else
        {
            PacketLogger.Instance.Warn("Invalid item transfer request: " + pet.getName() + "(pet) --> " +
                                       player.getName());
        }

        return ValueTask.CompletedTask;
    }
}