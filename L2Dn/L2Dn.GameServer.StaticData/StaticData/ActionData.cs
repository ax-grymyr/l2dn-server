using System.Collections.Frozen;
using System.Collections.Immutable;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.StaticData.Xml.Actions;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.StaticData;

public sealed class ActionData
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(ActionData));
    private FrozenDictionary<int, ActionDataHolder> _actionData = FrozenDictionary<int, ActionDataHolder>.Empty;
    private FrozenDictionary<int, int> _actionSkillsData = FrozenDictionary<int, int>.Empty; // skillId, actionId

    private ActionData()
    {
    }

    public static ActionData Instance { get; } = new();

    public void Load()
    {
        XmlActionList document = XmlLoader.LoadXmlDocument<XmlActionList>("ActionData.xml");

        _actionData = document.Actions.Select(action => new ActionDataHolder(action.Id, action.Handler, action.Option)).
            ToFrozenDictionary(action => action.Id);

        Dictionary<int, int> actionSkillsData = new Dictionary<int, int>();
        foreach (XmlAction action in document.Actions)
        {
            if (action.Handler is "PetSkillUse" or "ServitorSkillUse" && action.Option > 0)
                actionSkillsData[action.Option] = action.Id;
        }

        _actionSkillsData = actionSkillsData.ToFrozenDictionary();

        _logger.Info($"{nameof(ActionData)}: Loaded {_actionData.Count} player actions.");
    }

    public ActionDataHolder? GetActionData(int id) => _actionData.GetValueOrDefault(id);

    /// <summary>
    /// Returns the actionId corresponding to the skillId or -1 if no actionId is found for the specified skill.
    /// </summary>
    public int GetSkillActionId(int skillId) => _actionSkillsData.GetValueOrDefault(skillId, -1);

    public ImmutableArray<int> GetActionIdList() => _actionData.Keys;
}