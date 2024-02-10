using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using ThreadPool = System.Threading.ThreadPool;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * Teleport residence zone for clan hall sieges
 * @author BiggBoss
 */
public class ResidenceHallTeleportZone : ResidenceTeleportZone
{
	private int _id;
	private ScheduledFuture<?> _teleTask;
	
	/**
	 * @param id
	 */
	public ResidenceHallTeleportZone(int id):base(id)
	{
		base(id);
	}
	
	public void setParameter(String name, String value)
	{
		if (name.equals("residenceZoneId"))
		{
			_id = int.Parse(value);
		}
		else
		{
			base.setParameter(name, value);
		}
	}
	
	public int getResidenceZoneId()
	{
		return _id;
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void checkTeleportTask()
	{
		if ((_teleTask == null) || _teleTask.isDone())
		{
			_teleTask = ThreadPool.schedule(new TeleportTask(), 30000);
		}
	}
	
	protected class TeleportTask: Runnable
	{
		public void run()
		{
			int index = getSpawns().size() > 1 ? Rnd.get(getSpawns().size()) : 0;
			Location loc = getSpawns().get(index);
			if (loc == null)
			{
				throw new NullPointerException();
			}
			
			foreach (Player pc in getPlayersInside())
			{
				if (pc != null)
				{
					pc.teleToLocation(loc, false);
				}
			}
		}
	}
}