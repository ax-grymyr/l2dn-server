namespace L2Dn.GameServer.Enums;

/// <summary>
/// Skill target type.
/// </summary>
public enum TargetType
{
    /// <summary>
    /// Advance Headquarters (Outposts).
    /// </summary>
    ADVANCE_BASE,

    /// <summary>
    /// Enemies in high terrain or protected by castle walls and doors.
    /// </summary>
    ARTILLERY,

    /// <summary>
    /// Doors or treasure chests.
    /// </summary>
    DOOR_TREASURE,

    /// <summary>
    /// Any enemies (included allies).
    /// </summary>
    ENEMY,

    /// <summary>
    /// Friendly.
    /// </summary>
    ENEMY_NOT,

    /// <summary>
    /// Only enemies (not included allies).
    /// </summary>
    ENEMY_ONLY,

    /// <summary>
    /// Fortress's Flagpole.
    /// </summary>
    FORTRESS_FLAGPOLE,

    /// <summary>
    /// Ground.
    /// </summary>
    GROUND,

    /// <summary>
    /// Holy Artifacts from sieges.
    /// </summary>
    HOLYTHING,

    /// <summary>
    /// Items.
    /// </summary>
    ITEM,

    /// <summary>
    /// Nothing.
    /// </summary>
    NONE,

    /// <summary>
    /// NPC corpses.
    /// </summary>
    NPC_BODY,

    /// <summary>
    /// Others, except caster.
    /// </summary>
    OTHERS,

    /// <summary>
    /// Player corpses.
    /// </summary>
    PC_BODY,

    /// <summary>
    /// Self.
    /// </summary>
    SELF,

    /// <summary>
    /// Servitor or pet.
    /// </summary>
    SUMMON,

    /// <summary>
    /// Anything targetable.
    /// </summary>
    TARGET,

    /// <summary>
    /// Wyverns.
    /// </summary>
    WYVERN_TARGET,

    /// <summary>
    /// Mentee's Mentor.
    /// </summary>
    MY_MENTOR,

    /// <summary>
    /// Me or my party (if any). Seen in aura skills.
    /// </summary>
    MY_PARTY,

    /// <summary>
    /// Pet's owner.
    /// </summary>
    OWNER_PET,

    /// <summary>
    /// Pet Target
    /// </summary>
    PET,
}