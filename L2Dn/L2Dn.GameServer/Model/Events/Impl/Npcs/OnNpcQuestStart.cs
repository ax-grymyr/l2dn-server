using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Npcs;

public class OnNpcQuestStart: IBaseEvent
{
    private readonly Npc _npc;
    private readonly Player _player;
	
    public OnNpcQuestStart(Npc npc, Player player)
    {
        _npc = npc;
        _player = player;
    }
	
    public Npc getNpc()
    {
        return _npc;
    }
	
    public Player getActiveChar()
    {
        return _player;
    }
	
    public EventType getType()
    {
        return EventType.ON_NPC_FIRST_TALK;
    }
}