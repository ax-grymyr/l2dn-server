using System.Collections.Immutable;
using System.Reflection;
using L2Dn.CustomAttributes;
using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.Network.Enums;

public static class NpcStringIdUtil
{
    private static readonly ImmutableDictionary<NpcStringId, NpcStringIdInfo> _parameterCount =
        ParseNpcStringIdStrings().ToImmutableDictionary(info => info.Id);

    public static int GetParamCount(this NpcStringId npcStringId) =>
        _parameterCount.TryGetValue(npcStringId, out NpcStringIdInfo info) ? info.ParamCount : 0;

    private static IEnumerable<NpcStringIdInfo> ParseNpcStringIdStrings() =>
        typeof(NpcStringId).GetFields().Where(f =>
            f.Attributes == (FieldAttributes.Literal | FieldAttributes.Public | FieldAttributes.HasDefault |
                             FieldAttributes.Static) && f.FieldType == typeof(NpcStringId)).Select(f =>
            ((NpcStringId)f.GetValue(null)!, f.GetCustomAttribute<TextAttribute>()?.Text ?? string.Empty)).Select(t =>
            new NpcStringIdInfo
            {
                Id = t.Item1,
                Annotation = t.Item2,
                ParamCount = ParseParamCount(t.Item2)
            });

    private static int ParseParamCount(string text)
    {
        int paramCount = 0;
        for (int i = 0; i < text.Length - 1; i++)
        {
            if (text[i] is 'C' or 'S')
            {
                char d = text[i + 1];
                if (char.IsDigit(d))
                {
                    paramCount = Math.Max(paramCount, d - '0');
                    i++;
                }
            }
        }

        return paramCount;
    }

    private record struct NpcStringIdInfo(NpcStringId Id, string Annotation, int ParamCount);
}