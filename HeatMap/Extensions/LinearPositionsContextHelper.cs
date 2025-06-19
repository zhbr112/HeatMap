using System;
using System.Linq;

namespace HeatMap;

/// <summary>
/// Набор статических дополнительных методов для типа <see cref="LinearPositionsContext"/>
/// </summary>
public static class LinearPositionsContextHelper
{
    private static readonly Random Rnd = new Random();
 

    /// <summary>
    /// Сформировать синусоидальный набор данных
    /// </summary>
    /// <returns></returns>
    public static LinearPositionsContext CreateSinContext(int countPositions = 2000)
    {
        var positions = Enumerable.Range(0, countPositions)
            .Select(x => new LinearPosition((double)x / 100, Math.Sin((double)x / 100)));
        var result = new LinearPositionsContext(positions);
        return result;
    }

    /// <summary>
    /// Сформировать произвольный набор данных
    /// </summary>
    /// <returns></returns>
    public static LinearPositionsContext CreateRandomContext(int countPositions = 101, int maxPower = -10)
    {
        var positions = Enumerable.Range(500, countPositions)
            .Select(x => new LinearPosition((double)x * 10, Rnd.NextDouble() * maxPower - 100));
        var result = new LinearPositionsContext(positions);
        return result;
    }
}

