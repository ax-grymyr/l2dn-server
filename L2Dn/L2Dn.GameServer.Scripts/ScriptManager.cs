using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Scripts;

public static class ScriptManager
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(ScriptManager));
    private static readonly Map<string, AbstractScript> _scripts = new();

    public static void AddScript(AbstractScript script)
    {
        _scripts.AddOrUpdate(script.Name, _ => script, (_, oldScript) =>
        {
            if (!ReferenceEquals(oldScript, script))
                oldScript.Unload();

            return script;
        });

        if (Config.ALT_DEV_SHOW_SCRIPTS_LOAD_IN_LOGS)
        {
            _logger.Info("Loaded script " + script.GetType().FullName + ".");
        }
    }

    public static ICollection<AbstractScript> GetScripts() => _scripts.Values;
    public static AbstractScript? GetScript(string scriptName) => _scripts.GetValueOrDefault(scriptName);
}