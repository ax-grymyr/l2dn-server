namespace L2Dn.GameServer.Enums;

[Flags]
public enum ItemListType
{
    None = 0,
    
    AUGMENT_BONUS = 1,
    ELEMENTAL_ATTRIBUTE = 2,
    VISUAL_ID = 4,
    SOUL_CRYSTAL = 8,
    REUSE_DELAY = 16,
    BLESSED = 128
}