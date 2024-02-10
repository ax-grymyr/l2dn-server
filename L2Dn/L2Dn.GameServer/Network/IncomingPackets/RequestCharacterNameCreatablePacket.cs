using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

internal struct RequestCharacterNameCreatablePacket: IIncomingPacket<GameSession>
{
    private string? _name;

    public void ReadContent(PacketBitReader reader)
    {
        _name = reader.ReadString();
    }

    public async ValueTask ProcessAsync(Connection<GameSession> connection)
    {
        CharacterNameValidationResult result = CharacterNameValidationResult.Ok;
        if (string.IsNullOrEmpty(_name) || _name.Length > 16)
            result = CharacterNameValidationResult.InvalidLength;
        else if (!IsValidName(_name))
            result = CharacterNameValidationResult.InvalidName;
        else
        {
            GameSession session = connection.Session;
            bool exists = await DbUtility.DoesCharacterExistAsync(session.Config.GameServer.Id, _name);
            if (exists)
                result = CharacterNameValidationResult.NameAlreadyExists;
        }

        CharacterNameCreatablePacket characterNameCreatablePacket = new(result);
        connection.Send(ref characterNameCreatablePacket);
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
