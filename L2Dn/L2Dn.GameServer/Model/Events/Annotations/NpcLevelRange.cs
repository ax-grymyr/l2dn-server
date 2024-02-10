namespace L2Dn.GameServer.Model.Events.Annotations;

public interface NpcLevelRange
{
	int from();
	int to();
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class NpcLevelRangeAttribute: Attribute
{
	private readonly int _from;
	private readonly int _to;

	public NpcLevelRangeAttribute(int from, int to)
	{
		_from = from;
		_to = to;
	}

	public int From => _from;
	public int To => _to;
}