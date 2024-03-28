namespace L2Dn.GameServer.Model.Actor.Status;

public class PlayableStatus: CreatureStatus
{
    public PlayableStatus(Playable activeChar): base(activeChar)
    {
    }

    public override Playable getActiveChar()
    {
        return (Playable)base.getActiveChar();
    }
}
