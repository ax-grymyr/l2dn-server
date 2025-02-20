using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * Teleport residence zone for clan hall sieges
 * @author BiggBoss
 */
public class ResidenceHallTeleportZone : ResidenceTeleportZone
{
	private int _id;
	private ScheduledFuture? _teleTask;
	
	/**
	 * @param id
	 */
	public ResidenceHallTeleportZone(int id): base(id)
	{
	}

	public override void setParameter(string name, string value)
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
		if (_teleTask == null || _teleTask.isDone())
		{
			_teleTask = ThreadPool.schedule(new TeleportTask(this), 30000);
		}
	}

	private sealed class TeleportTask: Runnable
	{
		private readonly ResidenceHallTeleportZone _zone;

		public TeleportTask(ResidenceHallTeleportZone zone)
		{
			_zone = zone;
		}

		public void run()
		{
			int index = _zone.getSpawns().Length > 1 ? Rnd.get(_zone.getSpawns().Length) : 0;
			Location3D loc = _zone.getSpawns()[index];
			foreach (Player pc in _zone.getPlayersInside())
			{
				if (pc != null)
				{
					pc.teleToLocation(new Location(loc, 0), false);
				}
			}
		}
	}
}