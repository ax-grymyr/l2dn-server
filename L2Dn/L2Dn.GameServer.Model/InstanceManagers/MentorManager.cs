using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Variables;
using L2Dn.GameServer.Utilities;
using Microsoft.EntityFrameworkCore;
using NLog;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * @author UnAfraid
 */
public class MentorManager
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(MentorManager));
	
	private readonly Map<int, Map<int, Mentee>> _menteeData = new();
	private readonly Map<int, Mentee> _mentors = new();
	
	protected MentorManager()
	{
		load();
	}
	
	private void load()
	{
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			foreach (CharacterMentee mentee in ctx.CharacterMentees)
			{
				addMentor(mentee.MentorId, mentee.CharacterId);
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(e);
		}
	}
	
	/**
	 * Removes mentee for current Player
	 * @param mentorId
	 * @param menteeId
	 */
	public void deleteMentee(int mentorId, int menteeId)
	{
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.CharacterMentees.Where(r => r.MentorId == mentorId && r.CharacterId == menteeId).ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Warn(e);
		}
	}
	
	/**
	 * @param mentorId
	 * @param menteeId
	 */
	public void deleteMentor(int mentorId, int menteeId)
	{
		try 
		{
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			ctx.CharacterMentees.Where(r => r.MentorId == mentorId && r.CharacterId == menteeId).ExecuteDelete();
		}
		catch (Exception e)
		{
			LOGGER.Warn(e);
		}
		finally
		{
			removeMentor(mentorId, menteeId);
		}
	}
	
	public bool isMentor(int objectId)
	{
		return _menteeData.ContainsKey(objectId);
	}
	
	public bool isMentee(int objectId)
	{
		foreach (Map<int, Mentee> map in _menteeData.Values)
		{
			if (map.ContainsKey(objectId))
			{
				return true;
			}
		}
		return false;
	}
	
	public Map<int, Map<int, Mentee>> getMentorData()
	{
		return _menteeData;
	}
	
	public void cancelAllMentoringBuffs(Player player)
	{
		if (player == null)
		{
			return;
		}
		
		foreach (BuffInfo info in player.getEffectList().getEffects())
		{
			if (info.getSkill().isMentoring())
			{
				player.stopSkillEffects(info.getSkill());
			}
		}
	}
	
	public void setPenalty(int mentorId, TimeSpan penalty)
	{
		Player player = World.getInstance().getPlayer(mentorId);
		PlayerVariables vars = player != null ? player.getVariables() : new PlayerVariables(mentorId);
		vars.set("Mentor-Penalty-" + mentorId, DateTime.UtcNow + penalty);
	}
	
	public DateTime getMentorPenalty(int mentorId)
	{
		Player player = World.getInstance().getPlayer(mentorId);
		PlayerVariables vars = player != null ? player.getVariables() : new PlayerVariables(mentorId);
		return vars.getDateTime("Mentor-Penalty-" + mentorId, DateTime.MinValue);
	}
	
	/**
	 * @param mentorId
	 * @param menteeId
	 */
	public void addMentor(int mentorId, int menteeId)
	{
		Map<int, Mentee> mentees = _menteeData.computeIfAbsent(mentorId, map => new());
		if (mentees.TryGetValue(menteeId, out Mentee? mentee))
		{
			mentee.load(); // Just reloading data if is already there
		}
		else
		{
			mentees.put(menteeId, new Mentee(menteeId));
		}
	}
	
	/**
	 * @param mentorId
	 * @param menteeId
	 */
	public void removeMentor(int mentorId, int menteeId)
	{
		if (_menteeData.TryGetValue(mentorId, out Map<int, Mentee>? mentees))
		{
			mentees.remove(menteeId);
			if (mentees.Count == 0)
			{
				_menteeData.remove(mentorId);
				_mentors.remove(mentorId);
			}
		}
	}
	
	/**
	 * @param menteeId
	 * @return
	 */
	public Mentee getMentor(int menteeId)
	{
		foreach (var map in _menteeData)
		{
			if (map.Value.ContainsKey(menteeId))
			{
				if (!_mentors.TryGetValue(map.Key, out Mentee? mentee))
				{
					_mentors.put(map.Key, mentee = new Mentee(map.Key));
				}

				return mentee;
			}
		}
		return null;
	}
	
	public ICollection<Mentee> getMentees(int mentorId)
	{
		return _menteeData.TryGetValue(mentorId, out Map<int, Mentee>? mentees) ? mentees.Values : [];
	}
	
	/**
	 * @param mentorId
	 * @param menteeId
	 * @return
	 */
	public Mentee? getMentee(int mentorId, int menteeId)
	{
		return _menteeData.GetValueOrDefault(mentorId)?.GetValueOrDefault(menteeId);
	}
	
	public bool isAllMenteesOffline(int mentorId, int menteeId)
	{
		bool isAllMenteesOffline = true;
		foreach (Mentee men in getMentees(mentorId))
		{
			if (men.isOnline() && (men.getObjectId() != menteeId))
			{
				isAllMenteesOffline = false;
				break;
			}
		}
		return isAllMenteesOffline;
	}
	
	public bool hasOnlineMentees(int menteorId)
	{
		foreach (Mentee mentee in getMentees(menteorId))
		{
			if ((mentee != null) && mentee.isOnline())
			{
				return true;
			}
		}
		return false;
	}
	
	public static MentorManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly MentorManager INSTANCE = new MentorManager();
	}
}