namespace L2Dn.Packets;

public struct PacketRegistration
{
    private readonly PacketHandlerHelper _helper;

    internal PacketRegistration(PacketHandlerHelper helper)
    {
        _helper = helper;
    }

    public void WithAllowedStates<TAllowedStates>(TAllowedStates states)
        where TAllowedStates: unmanaged, Enum
    {
        if (_helper is not null)
            _helper.AllowedStates = states.ToInt64();
    }
}