using L2Dn.GameServer.Db;
using L2Dn.GameServer.TaskManagers.Tasks;
using L2Dn.GameServer.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.TaskManagers;

/**
 * @author Layane
 */
public class TaskManager // TODO: needs to be completely rewritten
{
	static readonly Logger LOGGER = LogManager.GetLogger(nameof(TaskManager));

	private readonly Map<int, Task> _tasks = new();
	readonly Set<ExecutedTask> _currentTasks = new();

	protected TaskManager()
	{
		initializate();
		startAllTasks();
		LOGGER.Info(GetType().Name + ": Loaded " + _tasks.Count + " Tasks.");
	}

	public class ExecutedTask: Runnable
	{
		int id;
		private DateTime? lastActivation;
		private readonly TaskManager _taskManager;
		private readonly Task task;
		private readonly TaskTypes type;
		private readonly string[] pars;
		public ScheduledFuture? scheduled;

		public ExecutedTask(TaskManager taskManager, Task ptask, TaskTypes ptype, DbGlobalTask rset)
		{
			_taskManager = taskManager;
			task = ptask;
			type = ptype;
			id = rset.Id;
			lastActivation = rset.LastRun;
            pars = [rset.TaskParam1 ?? string.Empty, rset.TaskParam2 ?? string.Empty, rset.TaskParam3 ?? string.Empty];
        }

		public void run()
		{
			task.onTimeElapsed(this);
			lastActivation = DateTime.UtcNow;

			try
			{
				using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
				ctx.GlobalTasks.Where(r => r.Id == id)
					.ExecuteUpdate(s => s.SetProperty(r => r.LastRun, lastActivation));
			}
			catch (Exception e)
			{
				LOGGER.Error(GetType().Name + ": Cannot updated the Global Task " + id + ": " + e);
			}

			if (type == TaskTypes.TYPE_SHEDULED || type == TaskTypes.TYPE_TIME)
			{
				stopTask();
			}
		}

		public override bool Equals(object? obj)
		{
			return this == obj || (obj is ExecutedTask && id == ((ExecutedTask) obj).id);
		}

		public override int GetHashCode()
		{
			return id;
		}

		public Task getTask()
		{
			return task;
		}

		public TaskTypes getType()
		{
			return type;
		}

		public int getId()
		{
			return id;
		}

		public string[] getParams()
		{
			return pars;
		}

		public DateTime? getLastActivation()
		{
			return lastActivation;
		}

		private void stopTask()
		{
			task.onDestroy();

			if (scheduled != null)
			{
				scheduled.cancel(true);
			}

			_taskManager._currentTasks.remove(this);
		}
	}

	private void initializate()
	{
		registerTask(new TaskBirthday());
		registerTask(new TaskCleanUp());
		registerTask(new TaskRecom());
		registerTask(new TaskRestart());
		registerTask(new TaskShutdown());
	}

	private void registerTask(Task task)
	{
		_tasks.computeIfAbsent(task.getName().GetHashCode(), k =>
		{
			task.initializate();
			return task;
		});
	}

	private void startAllTasks()
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			foreach (var record in ctx.GlobalTasks)
			{
				Task? task = _tasks.get(record.TaskName.Trim().toLowerCase().GetHashCode());
				if (task == null)
				{
					continue;
				}

				TaskTypes type = (TaskTypes)record.TaskType;
				if (type != TaskTypes.TYPE_NONE)
				{
					ExecutedTask current = new ExecutedTask(this, task, type, record);
					if (launchTask(current))
					{
						_currentTasks.add(current);
					}
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Error(GetType().Name + ": Error while loading Global Task table: " + e);
		}
	}

	private bool launchTask(ExecutedTask task)
	{
		TaskTypes type = task.getType();
		TimeSpan delay;
		TimeSpan interval;
		switch (type)
		{
			case TaskTypes.TYPE_STARTUP:
			{
				task.run();
				return false;
			}
			case TaskTypes.TYPE_SHEDULED:
			{
				delay = TimeSpan.FromMilliseconds(long.Parse(task.getParams()[0]));
				task.scheduled = ThreadPool.schedule(task, delay);
				return true;
			}
			case TaskTypes.TYPE_FIXED_SHEDULED:
			{
				delay = TimeSpan.FromMilliseconds(long.Parse(task.getParams()[0]));
				interval = TimeSpan.FromMilliseconds(long.Parse(task.getParams()[1]));
				task.scheduled = ThreadPool.scheduleAtFixedRate(task, delay, interval);
				return true;
			}
			case TaskTypes.TYPE_TIME:
			{
				try
				{
					DateTime desired = DateTime.Parse(task.getParams()[0]);
					TimeSpan diff = desired - DateTime.UtcNow;
					if (diff >= TimeSpan.Zero)
					{
						task.scheduled = ThreadPool.schedule(task, diff);
						return true;
					}

					LOGGER.Info(GetType().Name + ": Task " + task.getId() + " is obsolete.");
				}
				catch (Exception e)
				{
                    LOGGER.Trace(e);
					// Ignore.
				}
				break;
			}
			case TaskTypes.TYPE_SPECIAL:
			{
				ScheduledFuture? result = task.getTask().launchSpecial(task);
				if (result != null)
				{
					task.scheduled = result;
					return true;
				}
				break;
			}
			case TaskTypes.TYPE_GLOBAL_TASK:
			{
				interval = TimeSpan.FromMilliseconds(long.Parse(task.getParams()[0]) * 86400000);
				string[] hour = task.getParams()[1].Split(":");

				if (hour.Length != 3)
				{
					LOGGER.Warn(GetType().Name + ": Task " + task.getId() + " has incorrect parameters");
					return false;
				}

				DateTime check = task.getLastActivation() ?? DateTime.UtcNow + interval;

				DateTime min = DateTime.Now;
				try
				{
					min = new DateTime(min.Year, min.Month, min.Day, int.Parse(hour[0]), int.Parse(hour[1]),
						int.Parse(hour[2]));
				}
				catch (Exception e)
				{
					LOGGER.Warn(GetType().Name + ": Bad parameter on task " + task.getId() + ": " + e);
					return false;
				}

				delay = min - DateTime.Now;

				if (check > min || delay < TimeSpan.Zero)
				{
					delay += interval;
				}

				task.scheduled = ThreadPool.scheduleAtFixedRate(task, delay, interval);
				return true;
			}
			default:
			{
				return false;
			}
		}
		return false;
	}

	public static bool addUniqueTask(string task, TaskTypes type, string param1, string param2, string param3)
	{
		return addUniqueTask(task, type, param1, param2, param3, null);
	}

	private static bool addUniqueTask(string task, TaskTypes type, string param1, string param2, string param3, DateTime? lastActivation)
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			if (!ctx.GlobalTasks.Any(r => r.TaskName == task))
			{
				ctx.GlobalTasks.Add(new DbGlobalTask()
				{
					TaskName = task,
					TaskType = (int)type,
					TaskParam1 = param1,
					TaskParam2 = param2,
					TaskParam3 = param3,
					LastRun = lastActivation
				});

				ctx.SaveChanges();
			}

			return true;
		}
		catch (Exception e)
		{
			LOGGER.Error(nameof(TaskManager) + ": Cannot add the unique task: " + e);
		}
		return false;
	}

	public static bool addTask(string task, TaskTypes type, string param1, string param2, string param3)
	{
		return addTask(task, type, param1, param2, param3, null);
	}

	private static bool addTask(string task, TaskTypes type, string param1, string param2, string param3, DateTime? lastActivation)
	{
		try
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.GlobalTasks.Add(new DbGlobalTask()
			{
				TaskName = task,
				TaskType = (int)type,
				TaskParam1 = param1,
				TaskParam2 = param2,
				TaskParam3 = param3,
				LastRun = lastActivation
			});

			ctx.SaveChanges();
			return true;
		}
		catch (Exception e)
		{
			LOGGER.Error(nameof(TaskManager) + ": Cannot add the task: " + e);
		}
		return false;
	}

	public static TaskManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly TaskManager INSTANCE = new();
	}
}