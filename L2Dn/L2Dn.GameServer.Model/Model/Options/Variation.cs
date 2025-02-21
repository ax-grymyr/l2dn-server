using NLog;

namespace L2Dn.GameServer.Model.Options;

/**
 * @author Pere, Mobius
 */
public class Variation
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(Variation));

	private readonly int _mineralId;
	private readonly OptionDataGroup[] _effects = new OptionDataGroup[2];
	private readonly int _itemGroup = -1;

	public Variation(int mineralId, int itemGroup)
	{
		_mineralId = mineralId;
		_itemGroup = itemGroup;
	}

	public int getMineralId()
	{
		return _mineralId;
	}

	public int getItemGroup()
	{
		return _itemGroup;
	}

	public void setEffectGroup(int order, OptionDataGroup group)
	{
		_effects[order] = group;
	}

	public Options? getRandomEffect(int order, int targetItemId)
	{
		if (_effects == null)
		{
			LOGGER.Warn("Null effect: for mineral " + _mineralId + ", order " + order);
			return null;
		}

		if (_effects[order] == null)
		{
			return null;
		}

		return _effects[order].getRandomEffect(targetItemId);
	}
}