namespace L2Dn.GameServer.Model.Holders;

/**
 * @author JoeAlisson
 */
public class ElementalSpiritAbsorbItemHolder
{
	private readonly int _id;
	private readonly int _experience;
	
	public ElementalSpiritAbsorbItemHolder(int itemId, int experience)
	{
		_id = itemId;
		_experience = experience;
	}
	
	public int getId()
	{
		return _id;
	}
	
	public int getExperience()
	{
		return _experience;
	}
}