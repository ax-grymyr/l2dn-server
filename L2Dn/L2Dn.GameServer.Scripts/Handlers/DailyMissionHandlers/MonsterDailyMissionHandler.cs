using System.Globalization;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Attackables;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.DailyMissionHandlers;

/**
 * @author Mobius
 */
public class MonsterDailyMissionHandler: AbstractDailyMissionHandler
{
	private readonly int _amount;
	private readonly int _minLevel;
	private readonly int _maxLevel;
	private readonly Set<int> _ids = new();
	private readonly String _startHour;
	private readonly String _endHour;
	
	public MonsterDailyMissionHandler(DailyMissionDataHolder holder): base(holder)
	{
		_amount = holder.getRequiredCompletions();
		_minLevel = holder.getParams().getInt("minLevel", 0);
		_maxLevel = holder.getParams().getInt("maxLevel", int.MaxValue);
		String ids = holder.getParams().getString("ids", "");
		if (!ids.isEmpty())
		{
			foreach (String s in ids.Split(","))
			{
				int id = int.Parse(s);
				if (!_ids.Contains(id))
				{
					_ids.add(id);
				}
			}
		}
		_startHour = holder.getParams().getString("startHour", "");
		_endHour = holder.getParams().getString("endHour", "");
	}
	
	public override void init()
	{
		GlobalEvents.Monsters.Subscribe<OnAttackableKill>(this, onAttackableKill);
	}
	
	public override bool isAvailable(Player player)
	{
		DailyMissionPlayerEntry? entry = player.getDailyMissions().getEntry(getHolder().getId());
		if (entry != null)
		{
			switch (entry.getStatus())
			{
				case DailyMissionStatus.NOT_AVAILABLE: // Initial state
				{
					if (entry.getProgress() >= _amount)
					{
						entry.setStatus(DailyMissionStatus.AVAILABLE);
						player.getDailyMissions().storeEntry(entry);
					}
					break;
				}
				case DailyMissionStatus.AVAILABLE:
				{
					return true;
				}
			}
		}
		
		return false;
	}
	
	private void onAttackableKill(OnAttackableKill @event)
	{
		Attackable monster = @event.getTarget();
		if (!_ids.isEmpty() && !_ids.Contains(monster.getId()))
		{
			return;
		}
		
		Player player = @event.getAttacker();
		int monsterLevel = monster.getLevel();
		if ((_minLevel > 0) && ((monsterLevel < _minLevel) || (monsterLevel > _maxLevel) || ((player.getLevel() - monsterLevel) > 15)))
		{
			return;
		}
		
		if (checkTimeInterval() || (_startHour.equals("") && _endHour.equals("")))
		{
			Party party = player.getParty();
			if (party != null)
			{
				CommandChannel channel = party.getCommandChannel();
				List<Player> members = channel != null ? channel.getMembers() : party.getMembers();
				foreach (Player member in members)
				{
					if ((member.getLevel() >= (monsterLevel - 5)) && (member.calculateDistance3D(monster.getLocation().Location3D) <= Config.ALT_PARTY_RANGE))
					{
						processPlayerProgress(member);
					}
				}
			}
			else
			{
				processPlayerProgress(player);
			}
		}
	}
	
	private void processPlayerProgress(Player player)
	{
		DailyMissionPlayerEntry entry = player.getDailyMissions().getOrCreateEntry(getHolder().getId());
		if (entry.getStatus() == DailyMissionStatus.NOT_AVAILABLE)
		{
			if (entry.increaseProgress() >= _amount)
			{
				entry.setStatus(DailyMissionStatus.AVAILABLE);
			}
			
			player.getDailyMissions().storeEntry(entry);
		}
	}
	
	private bool checkTimeInterval()
	{
		if (!_startHour.equals("") && !_endHour.equals(""))
		{
			return TimeSpan.TryParseExact(_startHour, "HH:mm", CultureInfo.InvariantCulture, out _) &&
			       TimeSpan.TryParseExact(_endHour, "HH:mm", CultureInfo.InvariantCulture, out _);
		}
		
		return false;
	}
}