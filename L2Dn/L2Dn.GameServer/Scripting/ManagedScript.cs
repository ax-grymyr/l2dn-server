using NLog;

namespace L2Dn.GameServer.Scripting;

/**
 * Abstract class for classes that are meant to be implemented by scripts.<br>
 * @author KenM
 */
public abstract class ManagedScript
{
    private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ManagedScript));

    private readonly string _scriptFile; // path
    private long _lastLoadTime;
    private bool _isActive;

    public ManagedScript()
    {
        _scriptFile = ScriptEngineManager.getInstance().getCurrentLoadingScript();
        setLastLoadTime(System.currentTimeMillis());
    }

    /**
     * Attempts to reload this script and to refresh the necessary bindings with it ScriptControler.<br>
     * Subclasses of this class should override this method to properly refresh their bindings when necessary.
     * @return true if and only if the script was reloaded, false otherwise.
     */
    public bool reload()
    {
        try
        {
            ScriptEngineManager.getInstance().executeScript(_scriptFile);
            return true;
        }
        catch (Exception e)
        {
            LOGGER.Warn("Failed to reload script!", e);
            return false;
        }
    }

    public abstract bool unload();

    public void setActive(bool status)
    {
        _isActive = status;
    }

    public bool isActive()
    {
        return _isActive;
    }

    /**
     * @return Returns the scriptFile.
     */
    public string getScriptFile()
    {
        return _scriptFile;
    }

    /**
     * @param lastLoadTime The lastLoadTime to set.
     */
    protected void setLastLoadTime(long lastLoadTime)
    {
        _lastLoadTime = lastLoadTime;
    }

    /**
     * @return Returns the lastLoadTime.
     */
    protected long getLastLoadTime()
    {
        return _lastLoadTime;
    }

    public abstract String getScriptName();
}