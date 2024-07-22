using System.Text;

namespace L2Dn.GameServer.Utilities;

public class BypassBuilder(string bypass)
{
    private readonly List<BypassParam> _params = [];

    public void AddParam(string name, object value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(value);

        string val = value is string s ? '"' + s + '"' : value.ToString() ?? string.Empty;
        _params.Add(new BypassParam(name.Trim(), "=", val.Trim()));
    }

    public StringBuilder ToStringBuilder()
    {
        StringBuilder sb = new(bypass);
        foreach (BypassParam param in _params)
        {
            sb.Append(' ');
            sb.Append(param.Name);
            sb.Append(param.Separator);
            sb.Append(param.Value);
        }

        return sb;
    }

    public override string ToString()
    {
        return ToStringBuilder().ToString();
    }

    private readonly struct BypassParam(string name, string separator, string value)
    {
        public string Name => name;
        public string Separator => separator;
        public string Value => value;
    }
}