namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Geremy
 */
public class PetExtractionHolder
{
	private readonly int _petId;
	private readonly int _petLevel;
	private readonly long _extractExp;
	private readonly int _extractItem;
	private readonly ItemHolder _defaultCost;
	private readonly ItemHolder _extractCost;

	public PetExtractionHolder(int petId, int petLevel, long extractExp, int extractItem, ItemHolder defaultCost,
		ItemHolder extractCost)
	{
		_petId = petId;
		_petLevel = petLevel;
		_extractExp = extractExp;
		_extractItem = extractItem;
		_defaultCost = defaultCost;
		_extractCost = extractCost;
	}

	public int getPetId()
	{
		return _petId;
	}

	public int getPetLevel()
	{
		return _petLevel;
	}

	public long getExtractExp()
	{
		return _extractExp;
	}

	public int getExtractItem()
	{
		return _extractItem;
	}

	public ItemHolder getDefaultCost()
	{
		return _defaultCost;
	}

	public ItemHolder getExtractCost()
	{
		return _extractCost;
	}
}