namespace L2Dn.GameServer.Network.OutgoingPackets;

public enum CharacterCreateFailReason
{
    /// <summary>
    /// Your character creation has failed.
    /// </summary>
    CreationFailed = 0x00,

    /// <summary>
    /// You cannot create another character. Please delete the existing character and try again.
    /// Removes all settings that were selected (race, class, etc).
    /// </summary>
    TooManyCharacters = 0x01,

    /// <summary>
    /// This name already exists.
    /// </summary>
    NameAlreadyExists = 0x02,

    /// <summary>
    /// Your title cannot exceed 16 characters in length. Please try again.
    /// </summary>
    LengthExceeded = 0x03,

    /// <summary>
    /// Incorrect name. Please try again.
    /// </summary>
    IncorrectName = 0x04,

    /// <summary>
    /// Characters cannot be created from this server.
    /// </summary>
    CreationNotAllowed = 0x05,

    /// <summary>
    /// Unable to create character. You are unable to create a new character on the selected server.
    /// A restriction is in place which restricts users from creating characters on different servers
    /// where no previous character exists. Please choose another server.
    /// </summary>
    ChooseAnotherServer = 0x06,
}