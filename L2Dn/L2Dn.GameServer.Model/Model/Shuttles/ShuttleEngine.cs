using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Utilities;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Shuttles;

public class ShuttleEngine: Runnable
{
    private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ShuttleEngine));

    private const int DELAY = 15 * 1000;

    private readonly Shuttle _shuttle;
    private int _cycle;
    private readonly Door _door1;
    private readonly Door _door2;

    public ShuttleEngine(ShuttleDataHolder data, Shuttle shuttle)
    {
        _shuttle = shuttle;
        _door1 = DoorData.getInstance().getDoor(data.getDoors()[0]);
        _door2 = DoorData.getInstance().getDoor(data.getDoors()[1]);
    }

// TODO: Rework me..
    public void run()
    {
        try
        {
            if (!_shuttle.isSpawned())
            {
                return;
            }

            switch (_cycle)
            {
                case 0:
                {
                    _door1.openMe();
                    _door2.closeMe();
                    _shuttle.openDoor(0);
                    _shuttle.closeDoor(1);
                    _shuttle.broadcastShuttleInfo();
                    ThreadPool.schedule(this, DELAY);
                    break;
                }
                case 1:
                {
                    _door1.closeMe();
                    _door2.closeMe();
                    _shuttle.closeDoor(0);
                    _shuttle.closeDoor(1);
                    _shuttle.broadcastShuttleInfo();
                    ThreadPool.schedule(this, 1000);
                    break;
                }
                case 2:
                {
                    _shuttle.executePath(_shuttle.getShuttleData().getRoutes()[0]);
                    break;
                }
                case 3:
                {
                    _door1.closeMe();
                    _door2.openMe();
                    _shuttle.openDoor(1);
                    _shuttle.closeDoor(0);
                    _shuttle.broadcastShuttleInfo();
                    ThreadPool.schedule(this, DELAY);
                    break;
                }
                case 4:
                {
                    _door1.closeMe();
                    _door2.closeMe();
                    _shuttle.closeDoor(0);
                    _shuttle.closeDoor(1);
                    _shuttle.broadcastShuttleInfo();
                    ThreadPool.schedule(this, 1000);
                    break;
                }
                case 5:
                {
                    _shuttle.executePath(_shuttle.getShuttleData().getRoutes()[1]);
                    break;
                }
            }

            _cycle++;
            if (_cycle > 5)
            {
                _cycle = 0;
            }
        }
        catch (Exception e)
        {
            LOGGER.Info(e);
        }
    }
}
