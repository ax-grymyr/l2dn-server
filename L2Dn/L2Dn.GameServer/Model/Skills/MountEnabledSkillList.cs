namespace L2Dn.GameServer.Model.Skills;

/**
 * @author Mobius
 */
public class MountEnabledSkillList
{
	private const int STRIDER_SIEGE_ASSAULT = 325;
	private const int WYVERN_BREATH = 4289;
	
	public static bool contains(int skillId)
	{
		return (skillId == STRIDER_SIEGE_ASSAULT) || (skillId == WYVERN_BREATH);
	}
}