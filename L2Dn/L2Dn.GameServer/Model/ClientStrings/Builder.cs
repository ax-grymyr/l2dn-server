namespace L2Dn.GameServer.Model.ClientStrings;

public abstract class Builder
{
    public abstract String toString(Object param);
	
    public abstract String toString(params Object[] @params);
	
    public abstract int getIndex();
	
    public static Builder newBuilder(String text)
    {
        List<Builder> builders = new();
		
        int index1 = 0;
        int index2 = 0;
        int paramId;
        int subTextLen;
		
        char[] array = text.ToCharArray();
        int arrayLength = array.Length;
		
        char c;
        char c2;
        char c3;
        for (; index1 < arrayLength; index1++)
        {
            c = array[index1];
            if ((c == '$') && (index1 < (arrayLength - 2)))
            {
                c2 = array[index1 + 1];
                if ((c2 == 'c') || (c2 == 's') || (c2 == 'p') || (c2 == 'C') || (c2 == 'S') || (c2 == 'P'))
                {
                    c3 = array[index1 + 2];
                    if (char.IsDigit(c3))
                    {
                        paramId = c3 - '0';
                        subTextLen = index1 - index2;
                        if (subTextLen != 0)
                        {
                            builders.Add(new BuilderText(new String(array, index2, subTextLen)));
                        }
						
                        builders.Add(new BuilderObject(paramId));
                        index1 += 2;
                        index2 = index1 + 1;
                    }
                }
            }
        }
		
        if (arrayLength >= index1)
        {
            subTextLen = index1 - index2;
            if (subTextLen != 0)
            {
                builders.Add(new BuilderText(new String(array, index2, subTextLen)));
            }
        }
		
        if (builders.Count == 1)
        {
            return builders[0];
        }
        return new BuilderContainer(builders.ToArray());
    }
}