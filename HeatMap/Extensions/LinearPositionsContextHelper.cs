using System;
using System.Linq;

namespace HeatMap;

public static class LinearPositionsContextHelper
{
    private static readonly Random Rnd = new Random();
 

    public static LinearPositionsContext CreateSinContext(int countPositions = 500)
    {
        var positions = Enumerable.Range(0, countPositions)
            .Select(x => new LinearPosition((double)x, (Math.Sin((double)x / 10)-1)*50-10));
        var result = new LinearPositionsContext(positions);
        return result;
    }

    public static LinearPositionsContext CreateRandomContext(int countPositions = 500, int maxPower = -100)
    {
        var positions = Enumerable.Range(0, countPositions)
            .Select(x => new LinearPosition((double)x * 10, Rnd.NextDouble() * maxPower - 10));
        var result = new LinearPositionsContext(positions);
        return result;
    }
}

