using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace HeatMap;

public class LinearPositionsContext : AbstractNotifyProperyChanged
{
    private readonly ConcurrentQueue<IEnumerable<LinearPosition>> _queue = new ConcurrentQueue<IEnumerable<LinearPosition>>();

    private int Count { get; init; } = 200;

    private int HoldCount { get; init; } = 10;

    public IEnumerable<LinearPosition> Points
    {
        get
        {
            var _ = _queue.TryPeek(out var positions);
            return positions ?? new List<LinearPosition>();
        }
        set
        {
            _queue.TryPeek(out var source);
            if (!Equals(value, source) && (value?.Any() ?? false))
            {
                if(Count < _queue.Count)
                    _ = _queue.TryDequeue(out var positions);

                _queue.Enqueue(value);
                OnPropertyChanged();
            }
        }
    }

    public List<List<LinearPosition>> ToList()
    {
      var result = new List<List<LinearPosition>>();
      using var enumerator = _queue.GetEnumerator();
      while (enumerator.MoveNext())
        result.Add(enumerator.Current.ToList());

      return result.ToList();
    }      
        
    #region Конструкторы

    public LinearPositionsContext(IEnumerable<LinearPosition>? points) => Points = points ?? new List<LinearPosition>();
    
    public LinearPositionsContext(int count)
    {
        if(count <= 0) throw new ArgumentOutOfRangeException(nameof(count));
        HoldCount = count;
        Points = new List<LinearPosition>();
    }

    #endregion
}