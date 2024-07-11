using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * Tax zone type.
 * @author malyelfik
 */
public class TaxZone : ZoneType
{
	private int _domainId;
	private Castle _castle;
	
	public TaxZone(int id): base(id)
	{
	}
	
	public override void setParameter(string name, string value)
	{
		if (name.equalsIgnoreCase("domainId"))
		{
			_domainId = int.Parse(value);
		}
		else
		{
			base.setParameter(name, value);
		}
	}
	
	protected override void onEnter(Creature creature)
	{
		creature.setInsideZone(ZoneId.TAX, true);
		if (creature.isNpc())
		{
			((Npc) creature).setTaxZone(this);
		}
	}
	
	protected override void onExit(Creature creature)
	{
		creature.setInsideZone(ZoneId.TAX, false);
		if (creature.isNpc())
		{
			((Npc) creature).setTaxZone(null);
		}
	}
	
	/**
	 * Gets castle associated with tax zone.
	 * @return instance of {@link Castle} if found otherwise {@code null}
	 */
	public Castle getCastle()
	{
		// Lazy loading is used because zone is loaded before residence
		if (_castle == null)
		{
			_castle = CastleManager.getInstance().getCastleById(_domainId);
		}
		return _castle;
	}
}