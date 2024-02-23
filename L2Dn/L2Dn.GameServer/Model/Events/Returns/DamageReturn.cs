namespace L2Dn.GameServer.Model.Events.Returns;

public class DamageReturn: TerminateReturn
{
	private readonly double _damage;

	public DamageReturn(bool terminate, bool @override, bool abort, double damage)
		: base(terminate, @override, abort)
	{
		_damage = damage;
	}

	public double getDamage()
	{
		return _damage;
	}
}