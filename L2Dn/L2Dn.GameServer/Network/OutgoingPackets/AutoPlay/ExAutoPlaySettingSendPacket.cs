using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.AutoPlay;

public readonly struct ExAutoPlaySettingSendPacket: IOutgoingPacket
{
    private readonly int _options;
    private readonly bool _active;
    private readonly bool _pickUp;
    private readonly int _nextTargetMode;
    private readonly bool _shortRange;
    private readonly int _potionPercent;
    private readonly bool _respectfulHunting;
    private readonly int _petPotionPercent;

    public ExAutoPlaySettingSendPacket(int options, bool active, bool pickUp, int nextTargetMode, bool shortRange,
        int potionPercent, bool respectfulHunting, int petPotionPercent)
    {
        _options = options;
        _active = active;
        _pickUp = pickUp;
        _nextTargetMode = nextTargetMode;
        _shortRange = shortRange;
        _potionPercent = potionPercent;
        _respectfulHunting = respectfulHunting;
        _petPotionPercent = petPotionPercent;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_AUTOPLAY_SETTING);
        
        writer.WriteInt16((short)_options);
        writer.WriteByte(_active);
        writer.WriteByte(_pickUp);
        writer.WriteInt16((short)_nextTargetMode);
        writer.WriteByte(_shortRange);
        writer.WriteInt32(_potionPercent);
        writer.WriteInt32(_petPotionPercent); // 272
        writer.WriteByte(_respectfulHunting);
    }
}