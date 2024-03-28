using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestCharacterNameCreatablePacket: IIncomingPacket<GameSession>
{
    private string? _name;

    public void ReadContent(PacketBitReader reader)
    {
        _name = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        CharacterNameValidationResult result = CharacterNameValidationResult.Ok;
        if (string.IsNullOrEmpty(_name) || _name.Length > 16)
            result = CharacterNameValidationResult.InvalidLength;
        else if (!IsValidName(_name))
            result = CharacterNameValidationResult.InvalidName;
        else
        {
            int charId = CharInfoTable.getInstance().getIdByName(_name);
            if (charId > 0)
                result = CharacterNameValidationResult.NameAlreadyExists;
            else if (FakePlayerData.getInstance().getProperName(_name) != null)
                result = CharacterNameValidationResult.NameAlreadyExists;
        }

        ExIsCharNameCreatablePacket resultPacket = new ExIsCharNameCreatablePacket(result);
        connection.Send(ref resultPacket);
        return ValueTask.CompletedTask;
    }

    private static bool IsValidName(string name)
    {
        if (name.Length is < 1 or > 16)
            return false;

        foreach (char c in name)
        {
            // TODO: make config parameter to allow/disallow russian nicknames
            if (!char.IsAsciiLetterOrDigit(c) &&
                !char.IsBetween(c, 'А', 'Я') &&
                !char.IsBetween(c, 'а', 'я'))
                return false;
        }

        return true;
    }
}