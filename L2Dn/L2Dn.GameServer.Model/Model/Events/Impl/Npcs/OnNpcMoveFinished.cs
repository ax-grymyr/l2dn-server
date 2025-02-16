using L2Dn.Events;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Npcs;

public sealed class OnNpcMoveFinished(Npc npc): EventBase
{
	public Npc Npc => npc;
}