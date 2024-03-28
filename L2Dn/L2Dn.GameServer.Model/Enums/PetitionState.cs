namespace L2Dn.GameServer.Enums;

/**
 * Enum with petition states.
 * @author xban1x
 */
public enum PetitionState
{
    PENDING,
    RESPONDER_CANCEL,
    RESPONDER_MISSING,
    RESPONDER_REJECT,
    RESPONDER_COMPLETE,
    PETITIONER_CANCEL,
    PETITIONER_MISSING,
    IN_PROCESS,
    COMPLETED
}