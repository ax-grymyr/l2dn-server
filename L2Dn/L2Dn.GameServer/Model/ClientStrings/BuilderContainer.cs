using System.Text;

namespace L2Dn.GameServer.Model.ClientStrings;

public class BuilderContainer: Builder
{
    private readonly Builder[] _builders;

    public BuilderContainer(Builder[] builders)
    {
        _builders = builders;
    }

    public override String toString(Object param)
    {
        return toString(new Object[]
        {
            param
        });
    }

    public override String toString(params Object[] @params)
    {
        int buildersLength = _builders.Length;
        int paramsLength = @params.Length;
        String[] builds = new String[buildersLength];
        Builder builder;
        String build;
        int i;
        int paramIndex;
        int buildTextLen = 0;
        if (paramsLength != 0)
        {
            for (i = buildersLength; i-- > 0;)
            {
                builder = _builders[i];
                paramIndex = builder.getIndex();
                build = (paramIndex != -1) && (paramIndex < paramsLength)
                    ? builder.toString(@params[paramIndex])
                    : builder.toString();
                buildTextLen += build.Length;
                builds[i] = build;
            }
        }
        else
        {
            for (i = buildersLength; i-- > 0;)
            {
                build = _builders[i].toString();
                buildTextLen += build.Length;
                builds[i] = build;
            }
        }

        StringBuilder sb = new StringBuilder(buildTextLen);
        for (i = 0; i < buildersLength; i++)
        {
            sb.Append(builds[i]);
        }

        return sb.ToString();
    }

    public override int getIndex()
    {
        return -1;
    }
}