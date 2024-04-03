namespace L2Dn.GameServer.Network.Enums;

public readonly struct CharacterPaperdollSlotInfo(int itemId, int itemVisualId)
{
    public int ItemId { get; } = itemId;
    public int ItemVisualId { get; } = itemVisualId;
}