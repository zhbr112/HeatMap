using System;
using System.Collections.Generic;
using System.Linq;

namespace HeatMap;

public static class LinearPositionsContextExtension
{
    private static readonly Random _rnd = new Random();

    public static LinearPositionsContext UpdateRandomPosition(this LinearPositionsContext sourse)
    {
        var result = sourse.Points!.ToList();
        var index = _rnd.Next(result.Count);
        result![index].Power = _rnd.NextDouble() * 999;
        sourse.Points = result;
        return sourse;
    }

    private static IEnumerable<LinearPosition> GetSortByFrequency(this IEnumerable<LinearPosition> sourse) => sourse.OrderBy(x => x.Frequency);

    public static IEnumerable<LinearPosition> GetExtrapolation(this IEnumerable<LinearPosition> sourse, GraphSettings settings)
    {
        var sortedPositions = sourse.GetSortByFrequency().ToList();

        if (!sortedPositions.Any())
            yield break;

        var devider = Math.Ceiling(Convert.ToDouble(settings.DisplayPoints) / sortedPositions.Count);
        if (devider == 0) yield break;

        for (int i = 0; i < sortedPositions.Count - 1; i++)
        {
            var current = sortedPositions[i];
            var next = sortedPositions[i + 1];

            yield return current;

            for (double n = 1; n < devider; n++)
            {
                var frequency = current.Frequency + (next.Frequency - current.Frequency) * n / devider;
                var power = current.Power + (next.Power - current.Power) * n / devider;

                yield return new LinearPosition(frequency, power);
            }
        }

        yield return sortedPositions.Last();
    }
}