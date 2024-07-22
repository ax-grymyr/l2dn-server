using System.Text.RegularExpressions;
using L2Dn.GameServer.Model;

namespace L2Dn.GameServer.Utilities;

public class BypassParser: StatSet
{
    private const string AllowedChars = @"a-zA-Z0-9-_`!@#%^&*()\[\]|\\/";
    private const string RegexPattern = $"([{AllowedChars}]*)=('([{AllowedChars} ]*)'|[{AllowedChars}]*)";
    private static readonly Regex _regex = new(RegexPattern);

    public BypassParser(string bypass): base(new Map<string, object>())
    {
        Process(bypass);
    }

    private void Process(string bypass)
    {
        MatchCollection matches = _regex.Matches(bypass);
        foreach (Match match in matches)
        {
            string name = match.Groups[1].Value;
            string escapedValue = match.Groups[2].Value.Trim();
            string unescapedValue = match.Groups[3].Value;
            set(name, unescapedValue != null ? unescapedValue.Trim() : escapedValue);
        }
    }
}