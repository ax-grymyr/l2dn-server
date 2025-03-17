using L2Dn.GameServer.Dto.ZoneForms;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.StaticData.Xml.Zones;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * Tax zone type.
 * @author malyelfik
 */
public class TaxZone(int id, ZoneForm form): Zone(id, form)
{
	private int _domainId;
	private Castle? _castle;

    public override void setParameter(XmlZoneStatName name, string value)
	{
		if (name == XmlZoneStatName.domainId)
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
	public Castle? getCastle()
	{
		// Lazy loading is used because zone is loaded before residence
		if (_castle == null)
		{
			_castle = CastleManager.getInstance().getCastleById(_domainId);
		}
		return _castle;
	}
}