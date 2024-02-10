namespace L2Dn.GameServer.Model.Events.Returns;

public abstract class AbstractEventReturn
{
    private readonly bool _override;
    private readonly bool _abort;

    public AbstractEventReturn(bool @override, bool abort)
    {
        _override = @override;
        _abort = abort;
    }

    /**
     * @return {@code true} if return back object must be overridden by this object, {@code false} otherwise.
     */
    public bool @override()
    {
        return _override;
    }

    /**
     * @return {@code true} if notification has to be terminated, {@code false} otherwise.
     */
    public bool abort()
    {
        return _abort;
    }
}