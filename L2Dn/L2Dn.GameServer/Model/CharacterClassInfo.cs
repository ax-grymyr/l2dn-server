// using System.Collections.Immutable;
// using L2Dn.GameServer.Db;
// using L2Dn.GameServer.Enums;
//
// namespace L2Dn.GameServer.Model;
//
// public sealed class CharacterClassInfo
// {
//     private readonly CharacterClassInfo?[] _classes;
//     private readonly CharacterClass _class;
//     private readonly Race _race;
//     private readonly CharacterSpec _spec;
//     private readonly int _level;
//     private readonly CharacterClassInfo? _parentClass;
//     private readonly CharacterClass[] _childClasses;
//     private ImmutableList<CharacterClassInfo>? _childClassArray;
//
//     public CharacterClassInfo(CharacterClassInfo?[] classes, CharacterClass @class, Race race, CharacterSpec spec,
//         IEnumerable<CharacterClass> childClasses)
//     {
//         _classes = classes;
//         _class = @class;
//         _race = race;
//         _level = 1;
//         _spec = spec;
//         _childClasses = childClasses.ToArray();
//     }
//
//     public CharacterClassInfo(CharacterClassInfo?[] classes, CharacterClass @class, CharacterClassInfo parentClass,
//         IEnumerable<CharacterClass> childClasses)
//     {
//         _classes = classes;
//         _class = @class;
//         _race = parentClass.Race;
//         _spec = parentClass.Spec;
//         _parentClass = parentClass;
//         _level = _parentClass._level + 1;
//         _childClasses = childClasses.ToArray();
//     }
//
//     public CharacterClass Class => _class;
//     public Race Race => _race;
//     public CharacterSpec Spec => _spec;
//
//     public int Level => _level;
//     public CharacterClassInfo? ParentClass => _parentClass;
//     public CharacterClassInfo BaseClass => _parentClass?.BaseClass ?? this;
//
//     public ImmutableList<CharacterClassInfo> ChildClasses => _childClassArray ??=
//         _childClasses.Select(c => _classes[(int)c] ?? throw new InvalidOperationException("Invalid class data"))
//             .ToImmutableList();
// }
