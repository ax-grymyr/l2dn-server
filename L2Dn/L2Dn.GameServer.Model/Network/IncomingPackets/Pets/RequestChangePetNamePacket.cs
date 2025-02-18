using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Pets;

public struct RequestChangePetNamePacket: IIncomingPacket<GameSession>
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

        Summon? pet = player.getPet();
        if (pet == null)
            return ValueTask.CompletedTask;

        if (!pet.isPet())
        {
            player.sendPacket(SystemMessageId.YOU_DON_T_HAVE_A_PET);
            return ValueTask.CompletedTask;
        }

        if (!string.IsNullOrEmpty(pet.getName()))
        {
            player.sendPacket(SystemMessageId.YOU_CANNOT_SET_THE_NAME_OF_THE_PET);
            return ValueTask.CompletedTask;
        }

        if (PetNameTable.getInstance().doesPetNameExist(_name, pet.getTemplate().getId()))
        {
            player.sendPacket(SystemMessageId.THIS_IS_ALREADY_IN_USE_BY_ANOTHER_PET);
            return ValueTask.CompletedTask;
        }

        if (_name.Length < 3 || _name.Length > 16)
        {
            // player.sendPacket(SystemMessageId.YOUR_PET_S_NAME_CAN_BE_UP_TO_8_CHARACTERS_IN_LENGTH);
            player.sendMessage("Your pet's name can be up to 16 characters in length.");
            return ValueTask.CompletedTask;
        }

        if (!PetNameTable.getInstance().isValidPetName(_name))
        {
            player.sendPacket(SystemMessageId.AN_INVALID_CHARACTER_IS_INCLUDED_IN_THE_PET_S_NAME);
            return ValueTask.CompletedTask;
        }

        pet.setName(_name);
        pet.updateAndBroadcastStatus(1);

        return ValueTask.CompletedTask;
    }
}