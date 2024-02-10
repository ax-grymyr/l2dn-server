using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Interfaces;

namespace L2Dn.GameServer.Model;

public class Crest: IIdentifiable
{
    private readonly int _id;
    private readonly byte[] _data;
    private readonly CrestType _type;

    public Crest(int id, byte[] data, CrestType type)
    {
        _id = id;
        _data = data;
        _type = type;
    }

    public int getId()
    {
        return _id;
    }

    public byte[] getData()
    {
        return _data;
    }

    public CrestType getType()
    {
        return _type;
    }
}