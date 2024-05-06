using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Players;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.DailyMissionHandlers;

/**
 * @author JoeAlisson
 */
public class SpiritDailyMissionHandler: AbstractDailyMissionHandler
{
	private readonly int _amount;
	private readonly ElementalType _type;
	
	public SpiritDailyMissionHandler(DailyMissionDataHolder holder): base(holder)
	{
		_type = getHolder().getParams().getEnum("element", ElementalType.NONE);
		_amount = holder.getRequiredCompletions();
	}
	
	public override void init()
	{
		MissionKind kind = getHolder().getParams().getEnum("kind", MissionKind.UNKNOWN);
		if (MissionKind.EVOLVE == kind)
		{
			GlobalEvents.Players.Subscribe<OnPlayerElementalSpiritUpgrade>(this, onElementalSpiritUpgrade);
		}
		else if (MissionKind.LEARN == kind)
		{
			GlobalEvents.Players.Subscribe<OnPlayerElementalSpiritLearn>(this, onElementalSpiritLearn);
		}
	}
	
	public override bool isAvailable(Player player)
	{
		DailyMissionPlayerEntry? entry = player.getDailyMissions().getEntry(getHolder().getId());
		return (entry != null) && (entry.getStatus() == DailyMissionStatus.AVAILABLE);
	}
	
	private void onElementalSpiritLearn(OnPlayerElementalSpiritLearn @event)
	{
		Player player = @event.getPlayer();
		DailyMissionPlayerEntry missionData = player.getDailyMissions().getOrCreateEntry(getHolder().getId());
		missionData.setProgress(1);
		missionData.setStatus(DailyMissionStatus.AVAILABLE);
		player.getDailyMissions().storeEntry(missionData);
	}
	
	private void onElementalSpiritUpgrade(OnPlayerElementalSpiritUpgrade @event)
	{
		ElementalSpirit spirit = @event.getSpirit();
		if (spirit.getType() != _type)
		{
			return;
		}
		
		Player player = @event.getPlayer();
		DailyMissionPlayerEntry missionData = player.getDailyMissions().getOrCreateEntry(getHolder().getId());
		missionData.setProgress(spirit.getStage());
		if (missionData.getProgress() >= _amount)
		{
			missionData.setStatus(DailyMissionStatus.AVAILABLE);
		}

		player.getDailyMissions().storeEntry(missionData);
	}
	
	private enum MissionKind
	{
		LEARN,
		EVOLVE,
		
		UNKNOWN
	}
}