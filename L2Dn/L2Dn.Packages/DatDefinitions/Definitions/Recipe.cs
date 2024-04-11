﻿using L2Dn.Packages.DatDefinitions.Annotations;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Interlude, Chronicles.HighFive - 1)]
public sealed class Recipe
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public RecipeRecord[] Records { get; set; } = [];

    public sealed class RecipeRecord
    {
        public string Name { get; set; } = string.Empty;
        public uint Id { get; set; }
        public uint RecipeId { get; set; }
        public uint Level { get; set; }
        public uint ProductId { get; set; }
        public uint ProductCount { get; set; }
        public uint MpConsume { get; set; }
        public uint SuccessRate { get; set; }

        [ArrayLengthType(ArrayLengthType.Int32)]
        public RecipeMaterial[] Materials { get; set; } = [];
    }

    public class RecipeMaterial
    {
        public int ItemId { get; set; }
        public int Count { get; set; }
    }
}