using System.Runtime.CompilerServices;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Utilities;
using NLog;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Model.Html;

public class HtmlActionValidator
{
	private const int CacheArraySize = (int)HtmlActionScope.MaxValue + 1;
	private const char ParamStartChar = '$';
    private static readonly Logger _logger = LogManager.GetLogger(nameof(HtmlActionValidator));
    private CacheArray _array;
    private int? _originObjId;

    /// <summary>
    /// Origin of the last incoming html action request.
    /// This can be used for html continuing the conversation with an npc.
    /// </summary>
    public int? OriginObjectId => _originObjId;

    /// <summary>
    /// Check if the HTML action was sent in a HTML packet previously.
    /// If the HTML action was not sent for whatever reason, false is returned.
    /// Otherwise, true and the origin object ID is returned.
    /// </summary>
    /// <param name="action">The action id.</param>
    /// <param name="originObjectId">NPC object ID or null if the HTML action was not bound to an NPC
    /// or action not found.</param>
    /// <returns>True, if the HTML action was sent in a HTML packet previously.</returns>
    public bool IsValidAction(string action, out int? originObjectId)
    {
        for (int i = 0; i < CacheArraySize; ++i)
        {
            ref CacheArrayItem item = ref _array[i];
            if (ValidateHtmlAction(ref item.Actions, action))
            {
                _originObjId = originObjectId = item.OriginObjectId;
                return true;
            }
        }

        originObjectId = null;
        return false;
    }

    public void BuildActions(HtmlActionScope scope, string html, int? originObjectId = null)
    {
        if ((int)scope < 0 || (int)scope >= CacheArraySize)
            throw new ArgumentOutOfRangeException(nameof(scope), scope, null);

        if (string.IsNullOrEmpty(html))
            throw new ArgumentException("Html cannot be null or empty", nameof(html));

        if (originObjectId < 0)
            throw new ArgumentOutOfRangeException(nameof(originObjectId), originObjectId, null);

        if (Config.HTML_ACTION_CACHE_DEBUG)
            _logger.Info("Set html action npc(" + scope + "): " + originObjectId);

        _array[(int)scope].OriginObjectId = originObjectId;

        ref List<ActionItem>? actions = ref _array[(int)scope].Actions;
        if (actions is null)
	        actions = new List<ActionItem>();
        else
            actions.Clear();

        BuildHtmlBypassCache(scope, html, actions);
        BuildHtmlLinkCache(scope, html, actions);
    }

	private static void BuildHtmlBypassCache(HtmlActionScope scope, string html, List<ActionItem> actions)
	{
		// TODO: use spans
		int bypassEnd = 0;
		int bypassStart = html.IndexOf("=\"bypass ", bypassEnd, StringComparison.OrdinalIgnoreCase);
		while (bypassStart >= 0)
		{
			int bypassStartEnd = bypassStart + 9;
			bypassEnd = html.IndexOf('"', bypassStartEnd);
			if (bypassEnd < 0)
				break;

			int hParamPos = html.IndexOf("-h ", bypassStartEnd, StringComparison.OrdinalIgnoreCase);
			string bypass;
			if (hParamPos >= 0 && hParamPos < bypassEnd)
				bypass = html.Substring(hParamPos + 3, bypassEnd - hParamPos - 3).Trim();
			else
				bypass = html.Substring(bypassStartEnd, bypassEnd - bypassStartEnd).Trim();

			int firstParameterStart = bypass.IndexOf(ParamStartChar);
			bool hasParameters = firstParameterStart >= 0;
			if (hasParameters)
				bypass = bypass.Substring(0, firstParameterStart).Trim();

			if (Config.HTML_ACTION_CACHE_DEBUG)
				_logger.Info("Cached html bypass(" + scope + "): '" + bypass + "'");

			actions.Add(new ActionItem { Action = bypass, Prefix = hasParameters });

			bypassStart = html.IndexOf("=\"bypass ", bypassEnd, StringComparison.OrdinalIgnoreCase);
		}
	}

	private static void BuildHtmlLinkCache(HtmlActionScope scope, string html, List<ActionItem> actions)
	{
		// TODO: use spans
		int linkEnd = 0;
		int linkStart = html.IndexOf("=\"link ", linkEnd, StringComparison.OrdinalIgnoreCase);
		while (linkStart != -1)
		{
			int linkStartEnd = linkStart + 7;
			linkEnd = html.IndexOf('\"', linkStartEnd);
			if (linkEnd == -1)
			{
				break;
			}

			string htmlLink = html.Substring(linkStartEnd, linkEnd - linkStartEnd).Trim();
			if (string.IsNullOrEmpty(htmlLink))
			{
				_logger.Warn("Html link path is empty!");
				continue;
			}

			if (htmlLink.Contains(".."))
			{
				_logger.Warn("Html link path is invalid: " + htmlLink);
				continue;
			}

			if (Config.HTML_ACTION_CACHE_DEBUG)
				_logger.Info("Cached html link(" + scope + "): '" + htmlLink + "'");

			// let's keep an action cache with "link " lowercase literal kept
			actions.Add(new ActionItem { Action = "link " + htmlLink });

			linkStart = html.IndexOf("=\"link ", linkEnd, StringComparison.OrdinalIgnoreCase);
		}
	}

    public void ClearActions(HtmlActionScope scope)
    {
        if ((int)scope < 0 || (int)scope >= CacheArraySize)
            return;

        ref CacheArrayItem item = ref _array[(int)scope];
        item.Actions?.Clear();
        item.OriginObjectId = null;
    }

    private static bool ValidateHtmlAction(ref List<ActionItem>? actions, string action)
    {
        if (actions is null)
            return false;

        foreach (ActionItem cachedAction in actions)
        {
            if (cachedAction.Prefix)
            {
                if (action.StartsWith(cachedAction.Action))
                    return true;
            }
            else if (string.Equals(cachedAction.Action, action))
                return true;
        }

        return false;
    }

    private struct ActionItem
    {
	    public string Action;
	    public bool Prefix;
    }

    private struct CacheArrayItem
    {
        /// <summary>
        /// Last Html Npcs, null = last html was not bound to an npc.
        /// </summary>
        public int? OriginObjectId;

        public List<ActionItem>? Actions;
    }

    [InlineArray(CacheArraySize)]
    private struct CacheArray
    {
        public CacheArrayItem Items;
    }
}