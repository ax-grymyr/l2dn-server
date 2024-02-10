using L2Dn.GameServer.TaskManagers.Tasks;
using L2Dn.GameServer.Utilities;
using NLog;
using ThreadPool = System.Threading.ThreadPool;

namespace L2Dn.GameServer.TaskManagers;

/**
 * @author Layane
 */
public class TaskManager
{
	static readonly Logger LOGGER = LogManager.GetLogger(nameof(TaskManager));
	
	private readonly Map<int, Task> _tasks = new();
	readonly Set<ExecutedTask> _currentTasks = new();
	
	static readonly String[] SQL_STATEMENTS =
	{
		"SELECT id,task,type,last_activation,param1,param2,param3 FROM global_tasks",
		"UPDATE global_tasks SET last_activation=? WHERE id=?",
		"SELECT id FROM global_tasks WHERE task=?",
		"INSERT INTO global_tasks (task,type,last_activation,param1,param2,param3) VALUES(?,?,?,?,?,?)"
	};
	
	protected TaskManager()
	{
		initializate();
		startAllTasks();
		LOGGER.Info(GetType().Name + ": Loaded " + _tasks.size() + " Tasks.");
	}
	
	public class ExecutedTask: Runnable
	{
		int id;
		private DateTime lastActivation;
		private readonly Task task;
		private readonly TaskTypes type;
		private readonly String[] pars;
		ScheduledFuture scheduled;
		
		public ExecutedTask(Task ptask, TaskTypes ptype, ResultSet rset)
		{
			task = ptask;
			type = ptype;
			id = rset.getInt("id");
			lastActivation = rset.getLong("last_activation");
			pars = new String[]
			{
				rset.getString("param1"),
				rset.getString("param2"),
				rset.getString("param3")
			};
		}
		
		public void run()
		{
			task.onTimeElapsed(this);
			lastActivation = DateTime.Now;
			
			try
			{
				Connection con = DatabaseFactory.getConnection();
				PreparedStatement statement = con.prepareStatement(SQL_STATEMENTS[1]);
				statement.setLong(1, lastActivation);
				statement.setInt(2, id);
				statement.executeUpdate();
			}
			catch (Exception e)
			{
				LOGGER.Warn(GetType().Name + ": Cannot updated the Global Task " + id + ": " + e);
			}
			
			if ((type == TaskTypes.TYPE_SHEDULED) || (type == TaskTypes.TYPE_TIME))
			{
				stopTask();
			}
		}
		
		public override bool Equals(Object? obj)
		{
			return (this == obj) || ((obj is ExecutedTask) && (id == ((ExecutedTask) obj).id));
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
		
		public String[] getParams()
		{
			return params;
		}
		
		public DateTime getLastActivation()
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
			
			_currentTasks.remove(this);
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
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement statement = con.prepareStatement(SQL_STATEMENTS[0]);
			ResultSet rset = statement.executeQuery();
			while (rset.next())
			{
				Task task = _tasks.get(rset.getString("task").trim().toLowerCase().hashCode());
				if (task == null)
				{
					continue;
				}
				
				TaskTypes type = TaskTypes.valueOf(rset.getString("type"));
				if (type != TaskTypes.TYPE_NONE)
				{
					ExecutedTask current = new ExecutedTask(task, type, rset);
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
		long delay;
		long interval;
		switch (type)
		{
			case TaskTypes.TYPE_STARTUP:
			{
				task.run();
				return false;
			}
			case TaskTypes.TYPE_SHEDULED:
			{
				delay = Long.parseLong(task.getParams()[0]);
				task.scheduled = ThreadPool.schedule(task, delay);
				return true;
			}
			case TaskTypes.TYPE_FIXED_SHEDULED:
			{
				delay = Long.parseLong(task.getParams()[0]);
				interval = Long.parseLong(task.getParams()[1]);
				task.scheduled = ThreadPool.scheduleAtFixedRate(task, delay, interval);
				return true;
			}
			case TaskTypes.TYPE_TIME:
			{
				try
				{
					Date desired = DateFormat.getInstance().parse(task.getParams()[0]);
					long diff = desired.getTime() - System.currentTimeMillis();
					if (diff >= 0)
					{
						task.scheduled = ThreadPool.schedule(task, diff);
						return true;
					}
					LOGGER.info(getClass().getSimpleName() + ": Task " + task.getId() + " is obsoleted.");
				}
				catch (Exception e)
				{
					// Ignore.
				}
				break;
			}
			case TaskTypes.TYPE_SPECIAL:
			{
				ScheduledFuture<?> result = task.getTask().launchSpecial(task);
				if (result != null)
				{
					task.scheduled = result;
					return true;
				}
				break;
			}
			case TaskTypes.TYPE_GLOBAL_TASK:
			{
				interval = long.Parse(task.getParams()[0]) * 86400000;
				String[] hour = task.getParams()[1].Split(":");
				
				if (hour.Length != 3)
				{
					LOGGER.Warn(GetType().Name + ": Task " + task.getId() + " has incorrect parameters");
					return false;
				}
				
				Calendar check = Calendar.getInstance();
				check.setTimeInMillis(task.getLastActivation() + interval);
				
				Calendar min = Calendar.getInstance();
				try
				{
					min.set(Calendar.HOUR_OF_DAY, int.Parse(hour[0]));
					min.set(Calendar.MINUTE, int.Parse(hour[1]));
					min.set(Calendar.SECOND, int.Parse(hour[2]));
				}
				catch (Exception e)
				{
					LOGGER.Warn(GetType().Name + ": Bad parameter on task " + task.getId() + ": " + e);
					return false;
				}
				
				delay = min.getTimeInMillis() - System.currentTimeMillis();
				
				if (check.after(min) || (delay < 0))
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
	
	public static bool addUniqueTask(String task, TaskTypes type, String param1, String param2, String param3)
	{
		return addUniqueTask(task, type, param1, param2, param3, 0);
	}
	
	private static bool addUniqueTask(String task, TaskTypes type, String param1, String param2, String param3, long lastActivation)
	{
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement ps1 = con.prepareStatement(SQL_STATEMENTS[2]);
			ps1.setString(1, task);

			{
				ResultSet rs = ps1.executeQuery();
				if (!rs.next())
				{
					PreparedStatement ps2 = con.prepareStatement(SQL_STATEMENTS[3]);
					{
						ps2.setString(1, task);
						ps2.setString(2, type.ToString());
						ps2.setLong(3, lastActivation);
						ps2.setString(4, param1);
						ps2.setString(5, param2);
						ps2.setString(6, param3);
						ps2.execute();
					}
				}
			}
			return true;
		}
		catch (Exception e)
		{
			LOGGER.Warn(nameof(TaskManager) + ": Cannot add the unique task: " + e);
		}
		return false;
	}
	
	public static bool addTask(String task, TaskTypes type, String param1, String param2, String param3)
	{
		return addTask(task, type, param1, param2, param3, 0);
	}
	
	private static bool addTask(String task, TaskTypes type, String param1, String param2, String param3, long lastActivation)
	{
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement statement = con.prepareStatement(SQL_STATEMENTS[3]);
			statement.setString(1, task);
			statement.setString(2, type.toString());
			statement.setLong(3, lastActivation);
			statement.setString(4, param1);
			statement.setString(5, param2);
			statement.setString(6, param3);
			statement.execute();
			return true;
		}
		catch (Exception e)
		{
			LOGGER.Warn(nameof(TaskManager) + ": Cannot add the task: " + e);
		}
		return false;
	}
	
	public static TaskManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly TaskManager INSTANCE = new TaskManager();
	}
}