using L2Dn.GameServer.Model;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

internal struct RequestCharacterCreatePacket: IIncomingPacket<GameSession>
{
    private string? _name;
    private Sex _sex;
    private Race _race;
    private CharacterClass _class;
    private int _hairStyle;
    private int _hairColor;
    private int _face;

    public void ReadContent(PacketBitReader reader)
    {
        _name = reader.ReadString();
        _race = (Race)reader.ReadInt32();
        _sex = reader.ReadInt32() == 0 ? Sex.Male : Sex.Female;
        _class = (CharacterClass)reader.ReadInt32();

        reader.Skip(24); // int, str, con, men, dex, wit
        _hairStyle = reader.ReadInt32();
        _hairColor = reader.ReadInt32();
        _face = reader.ReadInt32();
    }

    public async ValueTask ProcessAsync(Connection<GameSession> connection)
    {
        // Check name
        if (string.IsNullOrEmpty(_name) || !IsValidName(_name))
        {
            CharacterCreateFailPacket characterCreateFailPacket = new(CharacterCreateFailReason.IncorrectName);
            connection.Send(ref characterCreateFailPacket);
            return;
        }

        // Check parameters
        if (_face is < 0 or > 4 || _hairColor is < 0 or > 3 || _hairStyle < 0 ||
            (_sex == Sex.Male && _hairStyle > 8) || (_sex == Sex.Female && _hairStyle > 11) ||
            !Enum.IsDefined(_class))
        {
            CharacterCreateFailPacket characterCreateFailPacket = new(CharacterCreateFailReason.CreationFailed);
            connection.Send(ref characterCreateFailPacket);
            return;
        }

        CharacterClassInfo classTemplate = StaticData.Templates[_class];
        if (classTemplate.Level > 1 || classTemplate.Race != _race)
        {
            CharacterCreateFailPacket characterCreateFailPacket = new(CharacterCreateFailReason.CreationFailed);
            connection.Send(ref characterCreateFailPacket);
            return;
        }

        // Check character count
        GameSession session = connection.Session;
        int serverId = session.Config.GameServer.Id;
        int characterCount = await DbUtility.GetCharacterCountAsync(serverId, session.AccountId);
        if (characterCount >= 7)
        {
            CharacterCreateFailPacket characterCreateFailPacket = new(CharacterCreateFailReason.TooManyCharacters);
            connection.Send(ref characterCreateFailPacket);
            return;
        }

        CharacterSpecData template = StaticData.Templates[classTemplate.Race][classTemplate.Spec];
        Character character = new Character
        {
            ServerId = serverId,
            AccountId = session.AccountId,
            Name = _name,

            // Appearance
            Face = (byte)_face,
            HairColor = (byte)_hairColor,
            HairStyle = (byte)_hairStyle,
            Sex = _sex,
            Class = _class,

            // TODO: HP, MP, CP
            MaxHp = 300,
            MaxMp = 200,
            MaxCp = 150,
            CurrentHp = 300,
            CurrentMp =200,
            CurrentCp = 0,

            // Location
            LocationX = template.SpawnLocation.X,
            LocationY = template.SpawnLocation.Y,
            LocationZ = template.SpawnLocation.Z,
            Heading = 0,

            Created = DateTime.UtcNow,
            LastAccess = DateTime.UtcNow,
        };

        try
        {
            await DbUtility.CreateCharacterAsync(character);
        }
        catch
        {
            CharacterCreateFailPacket characterCreateFailPacket = new(CharacterCreateFailReason.NameAlreadyExists);
            connection.Send(ref characterCreateFailPacket);
            return;
        }

        session.Characters.Add(character);
        session.SelectedCharacter = character;

        CharacterCreateSuccessPacket characterCreateSuccessPacket = new();
        connection.Send(ref characterCreateSuccessPacket);
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
