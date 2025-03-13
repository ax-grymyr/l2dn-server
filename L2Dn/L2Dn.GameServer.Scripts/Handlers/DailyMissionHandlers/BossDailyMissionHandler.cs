using L2Dn.GameServer.Db;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Attackables;
using L2Dn.Geometry;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Scripts.Handlers.DailyMissionHandlers;

/**
 * @author UnAfraid
 */
public class BossDailyMissionHandler: AbstractDailyMissionHandler
{
	private readonly int _amount;

	public BossDailyMissionHandler(DailyMissionDataHolder holder): base(holder)
	{
		_amount = holder.getRequiredCompletions();
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
		Player? player = @event.getAttacker();
		if (monster.isRaid() && monster.getInstanceId() > 0 && player != null)
		{
			Party? party = player.getParty();
			if (party != null)
			{
				CommandChannel? channel = party.getCommandChannel();
				List<Player> members = channel != null ? channel.getMembers() : party.getMembers();
				foreach (Player member in members)
				{
					if (member.Distance3D(monster) <= Config.Character.ALT_PARTY_RANGE)
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
}