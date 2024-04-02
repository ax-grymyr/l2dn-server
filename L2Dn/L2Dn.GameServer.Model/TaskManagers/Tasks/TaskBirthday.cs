using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.ItemContainers;

namespace L2Dn.GameServer.TaskManagers.Tasks;

/**
 * @author Nyaran
 */
public class TaskBirthday: Task
{
	private const string NAME = "birthday";
	private int _count = 0;
	
	public override String getName()
	{
		return NAME;
	}
	
	public override void onTimeElapsed(TaskManager.ExecutedTask task)
	{
		DateTime today = DateTime.Today;
		DateTime lastExecDate = today;
		DateTime? lastActivation = task.getLastActivation();
		if (lastActivation != null)
		{
			lastExecDate = lastActivation.Value;
		}

		
		string rangeDate = $"[{lastExecDate:MMM-dd}] - [{today:MMM-dd}]";
		while (lastExecDate <= today)
		{
			checkBirthday(lastExecDate.Month * 100 + lastExecDate.Day);
			lastExecDate = lastExecDate.AddDays(1);
		}
		
		LOGGER.Info("BirthdayManager: " + _count + " gifts sent. " + rangeDate);
	}
	
	private void checkBirthday(int day)
	{
		try
		{
			// If character birthday is 29-Feb and year isn't leap, send gift on 28-feb
			bool include29February = day == 228 && DateTime.IsLeapYear(DateTime.Today.Year);
			
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			var query = ctx.Characters.Where(r => r.BirthDay == day || (include29February && r.BirthDay == 229)).Select(
				r => new
				{
					r.Id,
					r.Created,
					r.Name
				});

			foreach (var record in query)
			{
				int playerId = record.Id;
				DateTime createDate = record.Created;

				int age = record.Created.Year - createDate.Year;
				if (age <= 0)
				{
					continue;
				}

				String text = Config.ALT_BIRTHDAY_MAIL_TEXT;
				if (text.Contains("$c1"))
				{
					text = text.Replace("$c1", record.Name);
				}

				if (text.Contains("$s1"))
				{
					text = text.Replace("$s1", age.ToString());
				}

				Message msg = new Message(playerId, Config.ALT_BIRTHDAY_MAIL_SUBJECT, text, MailType.BIRTHDAY);
				Mail attachments = msg.createAttachments();
				attachments.addItem("Birthday", Config.ALT_BIRTHDAY_GIFT, 1, null, null);
				MailManager.getInstance().sendMessage(msg);
				_count++;
			}
		}
		catch (Exception e)
		{
			LOGGER.Error("Error checking birthdays: " + e);
		}
	}
	
	/**
	 * @param num the number to format.
	 * @return the formatted number starting with a 0 if it is lower or equal than 10.
	 */
	private String getNum(int num)
	{
		return (num <= 9) ? "0" + num : num.ToString();
	}
	
	public override void initializate()
	{
		TaskManager.addUniqueTask(NAME, TaskTypes.TYPE_GLOBAL_TASK, "1", "06:30:00", "");
	}
}