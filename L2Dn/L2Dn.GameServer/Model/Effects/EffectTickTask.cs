using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Effects;

/**
 * Effect tick task.
 * @author Zoey76
 */
public class EffectTickTask: Runnable
{
    private readonly BuffInfo _info;
    private readonly AbstractEffect _effect;

    /**
     * EffectTickTask constructor.
     * @param info the buff info
     * @param effect the effect
     */
    public EffectTickTask(BuffInfo info, AbstractEffect effect)
    {
        _info = info;
        _effect = effect;
    }

    /**
     * Gets the buff info.
     * @return the buff info
     */
    public BuffInfo getBuffInfo()
    {
        return _info;
    }

    /**
     * Gets the effect.
     * @return the effect
     */
    public AbstractEffect getEffect()
    {
        return _effect;
    }

    public override void run()
    {
        _info.onTick(_effect);
    }
}
