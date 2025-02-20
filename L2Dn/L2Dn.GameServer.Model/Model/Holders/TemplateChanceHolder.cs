namespace L2Dn.GameServer.Model.Holders;

/**
 * An object for holding template id and chance
 * @author Nik
 */
public class TemplateChanceHolder
{
	private readonly int _templateId;
	private readonly int _minChance;
	private readonly int _maxChance;
	
	public TemplateChanceHolder(int templateId, int minChance, int maxChance)
	{
		_templateId = templateId;
		_minChance = minChance;
		_maxChance = maxChance;
	}
	
	public int getTemplateId()
	{
		return _templateId;
	}
	
	public bool calcChance(int chance)
	{
		return _maxChance > chance && chance >= _minChance;
	}
	
	public override string ToString()
	{
		return "[TemplateId: " + _templateId + " minChance: " + _minChance + " maxChance: " + _minChance + "]";
	}
}