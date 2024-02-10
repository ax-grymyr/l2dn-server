namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Mobius
 */
public class AutoPlaySettingsHolder
{
	private int _options;
	private bool _pickup;
	private int _nextTargetMode;
	private bool _shortRange;
	private bool _respectfulHunting;
	private int _autoPotionPercent;
	private int _autoPetPotionPercent;
	private int _macroIndex;

	public AutoPlaySettingsHolder()
	{
	}

	public int getOptions()
	{
		return _options;
	}

	public void setOptions(int options)
	{
		_options = options;
	}

	public bool doPickup()
	{
		return _pickup;
	}

	public void setPickup(bool value)
	{
		_pickup= value;
	}

	public int getNextTargetMode()
	{
		return _nextTargetMode;
	}

	public void setNextTargetMode(int nextTargetMode)
	{
		_nextTargetMode= nextTargetMode;
	}

	public bool isShortRange()
	{
		return _shortRange;
	}

	public void setShortRange(bool value)
	{
		_shortRange= value;
	}

	public bool isRespectfulHunting()
	{
		return _respectfulHunting;
	}

	public void setRespectfulHunting(bool value)
	{
		_respectfulHunting= value;
	}

	public int getAutoPotionPercent()
	{
		return _autoPotionPercent;
	}

	public void setAutoPotionPercent(int value)
	{
		_autoPotionPercent= value;
	}

	public int getAutoPetPotionPercent()
	{
		return _autoPetPotionPercent;
	}

	public void setAutoPetPotionPercent(int value)
	{
		_autoPetPotionPercent= value;
	}

	public void setMacroIndex(int value)
	{
		_macroIndex= value;
	}

	public int getMacroIndex()
	{
		return _macroIndex;
	}
}