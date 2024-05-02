using L2Dn.GameServer.Model.Actor;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * @author xban1x
 */
public abstract class ResidenceZone(int id): ZoneRespawn(id)
{
	private int _residenceId;

	public void banishForeigners(int owningClanId)
	{
		foreach (Player temp in getPlayersInside())
		{
			if (owningClanId != 0 && temp.getClanId() == owningClanId)
			{
				continue;
			}

			temp.teleToLocation(new LocationHeading(getBanishSpawnLoc(), 0), true);
		}
	}

	protected void setResidenceId(int residenceId)
	{
		_residenceId = residenceId;
	}

	public int getResidenceId()
	{
		return _residenceId;
	}
}