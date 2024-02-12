namespace L2Dn.GameServer.Model.Clans;

/**
 * This enum is used for clan privileges.<br>
 * @author HorridoJoho
 */
[Flags]
public enum ClanPrivilege // TODO: rename to ClanPrivileges
{
    /** dummy entry */
    None = 0,

    /** Privilege to join clan */
    CL_JOIN_CLAN = 1 << 0,

    /** Privilege to give a title */
    CL_GIVE_TITLE = 1 << 1,

    /** Privilege to view warehouse content */
    CL_VIEW_WAREHOUSE = 1 << 2,

    /** Privilege to manage clan ranks */
    CL_MANAGE_RANKS = 1 << 3,
    CL_PLEDGE_WAR = 1 << 4,
    CL_DISMISS = 1 << 5,

    /** Privilege to register clan crest */
    CL_REGISTER_CREST = 1 << 6,
    CL_APPRENTICE = 1 << 7,
    CL_TROOPS_FAME = 1 << 8,
    CL_SUMMON_AIRSHIP = 1 << 9,

    /** Privilege to open a door */
    CH_OPEN_DOOR = 1 << 10,
    CH_OTHER_RIGHTS = 1 << 11,
    CH_AUCTION = 1 << 12,
    CH_DISMISS = 1 << 13,
    CH_SET_FUNCTIONS = 1 << 14,
    CS_OPEN_DOOR = 1 << 15,
    CS_MANOR_ADMIN = 1 << 16,
    CS_MANAGE_SIEGE = 1 << 17,
    CS_USE_FUNCTIONS = 1 << 18,
    CS_DISMISS = 1 << 19,
    CS_TAXES = 1 << 20,
    CS_MERCENARIES = 1 << 21,
    CS_SET_FUNCTIONS = 1 << 22,
    
    All = -1 // all bits set
}