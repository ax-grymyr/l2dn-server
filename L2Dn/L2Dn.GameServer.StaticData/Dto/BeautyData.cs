using System.Collections.Frozen;
using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.Dto;

public sealed record BeautyData(Race Race, Sex Sex, FrozenDictionary<int, BeautyItem> HairList,
    FrozenDictionary<int, BeautyItem> FaceList);