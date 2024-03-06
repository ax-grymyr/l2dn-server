namespace L2Dn.GameServer.Network.Enums;

public enum CharacterNameValidationResult
{
    Ok = -1,
    CharacterCreateFailed = 1,
    NameAlreadyExists = 2,
    InvalidLength = 3,
    InvalidName = 4,
    CannotCreateOnServer = 5,
}