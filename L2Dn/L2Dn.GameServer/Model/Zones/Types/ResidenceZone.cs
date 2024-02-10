using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * @author xban1x
 */
public abstract class ResidenceZone : ZoneRespawn
{
	private int _residenceId;
	
	protected ResidenceZone(int id): base(id)
	{
	}
	
	public void banishForeigners(int owningClanId)
	{
		foreach (Player temp in getPlayersInside())
		{
			if ((owningClanId != 0) && (temp.getClanId() == owningClanId))
			{
				continue;
			}
			temp.teleToLocation(getBanishSpawnLoc(), true);
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