using System.Collections;
using System.Collections.Frozen;
using System.Globalization;
using L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

namespace L2Dn.GameServer.StaticData.Tests;

public static class ValueComparer
{
    public static void CompareValue<T>(string owner, string propertyName, FrozenSet<T> oldValue, FrozenSet<T> newValue)
    {
        bool equal = oldValue.Order().SequenceEqual(newValue.Order());
        if (!equal)
        {
            Assert.Fail($"{owner}: property '{propertyName}' old values '{string.Join(", ", oldValue.Order())}" +
                $"', new values '{string.Join(", ", newValue.Order())}'");
        }
    }

    public static void CompareValue<T>(string owner, string propertyName, ReadOnlySpan<T> oldValue,
        ReadOnlySpan<T> newValue)
    {
        CompareValue(owner, propertyName + ".Count", oldValue.Length, newValue.Length);
        if (oldValue.Length == newValue.Length)
        {
            for (int i = 0; i < oldValue.Length; i++)
                CompareValue(owner, $"{propertyName}[{i}]", oldValue[i], newValue[i]);
        }
    }

    public static void CompareValue<T>(string owner, string propertyName, T oldValue, T newValue)
    {
        string index = string.Empty;
        bool equal;
        if (oldValue is RestorationRandom oldRestorationRandom &&
            newValue is RestorationRandom newRestorationRandom)
        {
            // Allow old values to be approximate because of double rounding
            equal = oldRestorationRandom.EqualsApproximately(newRestorationRandom);
        }
        else if (oldValue is IEnumerable oldEnumerable && newValue is IEnumerable newEnumerable)
        {
            List<object?> oldEnum = oldEnumerable.Cast<object?>().ToList();
            List<object?> newEnum = newEnumerable.Cast<object?>().ToList();
            equal = oldEnum.SequenceEqual(newEnum);
            if (!equal && oldEnum.Count == newEnum.Count)
            {
                for (int i = 0; i < oldEnum.Count; i++)
                {
                    object? oldV = oldEnum[i];
                    object? newV = newEnum[i];
                    if (oldV is null)
                    {
                        if (newV is not null)
                        {
                            index = $"[{i}]";
                            break;
                        }
                    }
                    else if (newV is null)
                    {
                        index = $"[{i}]";
                        break;
                    }
                    else if (!oldV.Equals(newV))
                    {
                        index = $"[{i}]";
                        break;
                    }
                }
            }
        }
        else
            equal = EqualityComparer<T>.Default.Equals(oldValue, newValue);

        if (!equal)
        {
            if (typeof(T) == typeof(TimeSpan))
            {
                // Allow old value to be approximate, ending with 9999's for example (double rounding).
                TimeSpan oldTimeSpan = (TimeSpan)(object)oldValue!;
                TimeSpan newTimeSpan = (TimeSpan)(object)newValue!;

                if (newTimeSpan.Ticks.ToString(CultureInfo.InvariantCulture).EndsWith("0000"))
                {
                    equal = oldTimeSpan - TimeSpan.FromMilliseconds(1) <= newTimeSpan &&
                        newTimeSpan <= oldTimeSpan + TimeSpan.FromMilliseconds(1);
                }
            }
        }

        if (!equal)
        {
            string? oldValueString;
            if (oldValue is IEnumerable enumerable)
                oldValueString = string.Join(", ", enumerable.Cast<object?>());
            else
                oldValueString = oldValue?.ToString();

            string? newValueString;
            if (newValue is IEnumerable enumerable2)
                newValueString = string.Join(", ", enumerable2.Cast<object?>());
            else
                newValueString = newValue?.ToString();

            Assert.Fail($"{owner}: property '{propertyName}{index}' old value '{oldValueString}', " +
                $"new value '{newValueString}'");
        }
    }
}