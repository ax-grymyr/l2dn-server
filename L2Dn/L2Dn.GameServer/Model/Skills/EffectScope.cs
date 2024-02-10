namespace L2Dn.GameServer.Model.Skills;

/**
 * @author NosBit
 */
public enum EffectScope
{
	GENERAL,
	START,
	SELF,
	CHANNELING,
	PVP,
	PVE,
	END
}

public static class EffectScopeUtil
{
	public static string GetName(this EffectScope effectScope)
	{
		// TODO: make attribute
		return effectScope switch
		{
			EffectScope.GENERAL => "effects",
			EffectScope.START => "startEffects",
			EffectScope.SELF => "selfEffects",
			EffectScope.CHANNELING => "channelingEffects",
			EffectScope.PVP => "pvpEffects",
			EffectScope.PVE => "pveEffects",
			EffectScope.END => "endEffects",
			_ => throw new ArgumentOutOfRangeException()
		};
	}
}