namespace L2Dn.GameServer.Model.Options;

/**
 * @author UnAfraid
 */
public class EnchantOptions
{
	private readonly int _level;
	private readonly int[] _options;

	public EnchantOptions(int level)
	{
		_level = level;
		_options = new int[3];
	}

	public int getLevel()
	{
		return _level;
	}

	public int[] getOptions()
	{
		return _options;
	}

	public void setOption(byte index, int option)
	{
		if (_options.Length > index)
		{
			_options[index] = option;
		}
	}
}