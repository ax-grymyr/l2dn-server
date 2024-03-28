using System.Text;

namespace L2Dn.GameServer.Utilities;

public class BypassBuilder
{
    private readonly string _bypass;
    private readonly List<BypassParam> _params = new();
	
    public BypassBuilder(string bypass)
    {
        _bypass = bypass;
    }
	
    public void addParam(BypassParam param)
    {
        Objects.requireNonNull(param, "param cannot be null!");
        _params.add(param);
    }
	
    public void addParam(string name, string? separator, object? value)
    {
        Objects.requireNonNull(name, "name cannot be null!");
        addParam(new BypassParam(name, separator, value));
    }
	
    public void addParam(string name, object value)
    {
        addParam(name, "=", value);
    }
	
    public void addParam(string name)
    {
        addParam(name, null, null);
    }
	
    public StringBuilder toStringBuilder()
    {
        StringBuilder sb = new StringBuilder(_bypass);
        foreach (BypassParam param in _params)
        {
            sb.Append(" ").Append(param.getName().Trim());
            if (param.getSeparator() != null && param.getValue() != null)
            {
                sb.Append(param.getSeparator().Trim());
                object value = param.getValue();
                if (value is string)
                {
                    sb.Append('"');
                }
                sb.Append(value.ToString().Trim());
                if (value is string)
                {
                    sb.Append('"');
                }
            }
        }
        
        return sb;
    }
	
    public string ToString()
    {
        return toStringBuilder().ToString();
    }
	
    public class BypassParam(string name, string? separator, object? value)
    {
        public string getName()
        {
            return name;
        }
		
        public string? getSeparator()
        {
            return separator;
        }
		
        public object? getValue()
        {
            return value;
        }
    }
}