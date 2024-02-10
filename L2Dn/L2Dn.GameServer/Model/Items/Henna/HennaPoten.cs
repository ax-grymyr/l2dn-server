namespace L2Dn.GameServer.Model.Items.Henna;

/**
 * @author Serenitty
 */
public class HennaPoten
{
	private Henna _henna;
	private int _potenId;
	private int _enchantLevel = 1;
	private int _enchantExp;
	private int _slotPosition;

	public HennaPoten()
	{
	}

	public void setHenna(Henna henna)
	{
		_henna = henna;
	}

	public Henna getHenna()
	{
		return _henna;
	}

	public void setPotenId(int val)
	{
		_potenId = val;
	}

	public int getSlotPosition()
	{
		return _slotPosition;
	}

	public void setSlotPosition(int val)
	{
		_slotPosition = val;
	}

	public int getPotenId()
	{
		return _potenId;
	}

	public void setEnchantLevel(int val)
	{
		_enchantLevel = val;
	}

	public int getEnchantLevel()
	{
		return _enchantLevel;
	}

	public void setEnchantExp(int val)
	{
		_enchantExp = val;
	}

	public int getEnchantExp()
	{
		if (_enchantExp > HennaPatternPotentialData.getInstance().getMaxPotenExp())
		{
			_enchantExp = HennaPatternPotentialData.getInstance().getMaxPotenExp();
			return _enchantExp;
		}

		return _enchantExp;
	}

	public bool isPotentialAvailable()
	{
		return (_henna != null) && (_enchantLevel > 1);
	}

	public int getActiveStep()
	{
		if (!isPotentialAvailable())
		{
			return 0;
		}

		if (_enchantExp == HennaPatternPotentialData.getInstance().getMaxPotenExp())
		{
			return Math.Min(_enchantLevel, _henna.getPatternLevel());
		}

		return Math.Min(_enchantLevel - 1, _henna.getPatternLevel());
	}
}