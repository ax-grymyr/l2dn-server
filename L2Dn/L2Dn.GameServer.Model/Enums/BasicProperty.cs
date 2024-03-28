namespace L2Dn.GameServer.Enums;

/**
 * Basic property type of skills.<br>
 * Before Goddess of Destruction, BaseStats was used. CON for physical, MEN for magical, and others for special cases.<br>
 * After, only 3 types are used: physical, magic and none.<br>
 * <br>
 * Quote from Juji:<br>
 * ----------------------------------------------------------------------<br>
 * Physical: Stun, Paralyze, Knockback, Knock Down, Hold, Disarm, Petrify<br>
 * Mental: Sleep, Mutate, Fear, Aerial Yoke, Silence<br>
 * ----------------------------------------------------------------------<br>
 * All other are considered with no basic property aka NONE.<br>
 * <br>
 * @author Nik
 */
public enum BasicProperty
{
    NONE,
    PHYSICAL,
    MAGIC
}