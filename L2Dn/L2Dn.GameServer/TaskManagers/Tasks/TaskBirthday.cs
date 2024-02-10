using L2Dn.GameServer.Data.Sql;
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
	private const string QUERY = "SELECT charId, createDate FROM characters WHERE createDate LIKE ?";
	private int _count = 0;
	
	public override String getName()
	{
		return NAME;
	}
	
	public override void onTimeElapsed(ExecutedTask task)
	{
		Calendar lastExecDate = Calendar.getInstance();
		long lastActivation = task.getLastActivation();
		if (lastActivation > 0)
		{
			lastExecDate.setTimeInMillis(lastActivation);
		}
		
		String rangeDate = "[" + Util.getDateString(lastExecDate.getTime()) + "] - [" + Util.getDateString(TODAY.getTime()) + "]";
		for (; !TODAY.before(lastExecDate); lastExecDate.add(Calendar.DATE, 1))
		{
			checkBirthday(lastExecDate.get(Calendar.YEAR), lastExecDate.get(Calendar.MONTH), lastExecDate.get(Calendar.DATE));
		}
		
		LOGGER.Info("BirthdayManager: " + _count + " gifts sent. " + rangeDate);
	}
	
	private void checkBirthday(int year, int month, int day)
	{
		try 
		{
			Connection con = DatabaseFactory.getConnection();
			PreparedStatement statement = con.prepareStatement(QUERY);
			statement.setString(1, "%-" + getNum(month + 1) + "-" + getNum(day));

			{
				ResultSet rset = statement.executeQuery();
				while (rset.next())
				{
					int playerId = rset.getInt("charId");
					Calendar createDate = Calendar.getInstance();
					createDate.setTime(rset.getDate("createDate"));
					
					int age = year - createDate.get(Calendar.YEAR);
					if (age <= 0)
					{
						continue;
					}
					
					String text = Config.ALT_BIRTHDAY_MAIL_TEXT;
					if (text.Contains("$c1"))
					{
						text = text.Replace("$c1", CharInfoTable.getInstance().getNameById(playerId));
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
		}
		catch (Exception e)
		{
			LOGGER.Warn("Error checking birthdays: " + e);
		}
		
		// If character birthday is 29-Feb and year isn't leap, send gift on 28-feb
		GregorianCalendar calendar = new GregorianCalendar();
		if ((month == Calendar.FEBRUARY) && (day == 28) && !calendar.isLeapYear(TODAY.get(Calendar.YEAR)))
		{
			checkBirthday(year, Calendar.FEBRUARY, 29);
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
