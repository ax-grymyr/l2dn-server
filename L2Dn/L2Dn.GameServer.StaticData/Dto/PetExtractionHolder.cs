namespace L2Dn.GameServer.Dto;

public sealed record PetExtractionHolder(int PetId, int PetLevel, long ExtractExp, int ExtractItem,
    ItemHolder DefaultCost, ItemHolder ExtractCost);