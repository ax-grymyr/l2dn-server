using L2Dn.Packages.DatDefinitions.Annotations;
using L2Dn.Packages.DatDefinitions.Definitions.Shared;

namespace L2Dn.Packages.DatDefinitions.Definitions;

[ChronicleRange(Chronicles.Salvation, Chronicles.Latest)]
public sealed class AgathionData
{
    [ArrayLengthType(ArrayLengthType.Int32)]
    public AgathionDataRecord[] Records { get; set; } = Array.Empty<AgathionDataRecord>();

    public sealed class AgathionDataRecord
    {
        public uint ItemId { get; set; }
        public uint Enchant { get; set; }
        
        [ArrayLengthType(ArrayLengthType.Int32)]
        public AgathionDataSkill[] MainSkills { get; set; } = Array.Empty<AgathionDataSkill>();

        [ArrayLengthType(ArrayLengthType.Int32)]
        public AgathionDataSkill[] SubSkills { get; set; } = Array.Empty<AgathionDataSkill>();
    }

    public sealed class AgathionDataSkill
    {
        public uint SkillId { get; set; }
        public uint SkillLevel { get; set; }
    }
}