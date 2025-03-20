namespace L2Dn.GameServer.Enums;

/// <summary>
/// Basic property type of skills.
/// Before Goddess of Destruction, BaseStats was used. CON for physical, MEN for magical, and others for special cases.
/// After, only 3 types are used: physical, magic and none.
/// <para>
/// Physical: Stun, Paralyze, Knockback, Knock Down, Hold, Disarm, Petrify.
/// Mental: Sleep, Mutate, Fear, Aerial Yoke, Silence.
/// All others are considered with no basic property aka NONE.
/// </para>
/// </summary>
public enum BasicProperty
{
    NONE,
    PHYSICAL,
    MAGIC
}