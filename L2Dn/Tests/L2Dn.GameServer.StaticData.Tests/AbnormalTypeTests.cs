using FluentAssertions;
using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.StaticData.Tests;

public sealed class AbnormalTypeTests
{
    [Fact]
    public void CheckDuplicateValues()
    {
        List<(AbnormalType Value, List<string> Names)> duplicateGroups = Enum.GetNames<AbnormalType>().
            Where(n => n != AbnormalType.NONE.ToString()).
            GroupBy(Enum.Parse<AbnormalType>).
            Where(g => g.Count() > 1).
            Select(g => (g.Key, g.ToList())).ToList();

        foreach ((AbnormalType value, List<string> names) in duplicateGroups)
        {
            Assert.Fail(string.Join(", ", names.Select(n => "AbnormalType." + n)) +
                $" have the same value {(int)value}");
        }

        duplicateGroups.Count.Should().Be(0);

        // TestEnum has duplicate values
        Enum.GetValues<TestEnum>().GroupBy(t => t).Any(g => g.Count() > 1).Should().BeTrue();
    }

    private enum TestEnum
    {
        A = 0,
        B = 1,
        C = 2,
        D = 2,
    }
}