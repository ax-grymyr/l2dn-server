namespace L2Dn.GameServer.Model.Skills;

/**
 * @author NosBit
 */
public enum SkillConditionScope
{
	GENERAL,
	TARGET,
	PASSIVE
}

public static class SkillConditionScopeUtil
{
	public static string GetName(this SkillConditionScope skillConditionScope)
	{
		return skillConditionScope switch
		{
			SkillConditionScope.GENERAL => "conditions",
			SkillConditionScope.TARGET => "targetConditions",
			SkillConditionScope.PASSIVE => "passiveConditions",
			_ => throw new ArgumentOutOfRangeException()
		};
	}
}