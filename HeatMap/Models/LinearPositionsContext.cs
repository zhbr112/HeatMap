using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace HeatMap;

/// <summary>
/// Контекст с данными для построения линейного графика
/// </summary>
public class LinearPositionsContext : AbstractNotifyProperyChanged
{
    // Очередь
    private readonly ConcurrentQueue<IEnumerable<LinearPosition>> _queue = new ConcurrentQueue<IEnumerable<LinearPosition>>();

    /// <summary>
    /// Количество измерений хранить
    /// </summary>
    private int Count { get; init; } = 200;
        
    /// <summary>
    /// Получить текущий набор записей
    /// </summary>
    public IEnumerable<LinearPosition> Points
    {
        get
        {
            var _ = _queue.TryPeek(out var positions);
            return positions ?? new List<LinearPosition>();
        }
        set
        {
            // Автоматически старый набор данных выталкиваем
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

    /// <summary>
    /// Сформировать списочную копию текущей очереди
    /// </summary>
    public List<List<LinearPosition>> ToList()
    {
      var result = new List<List<LinearPosition>>();
      using var enumerator = _queue.GetEnumerator();
      while (enumerator.MoveNext())
        result.Add(enumerator.Current.ToList());

      return result.ToList();
    }

    /// <summary>
    /// Получить набор с минимальными значениями
    /// </summary>
    /// <returns></returns>
    public IEnumerable<LinearPosition?> Minimun
    {
        get
        {
            var list = this.ToList();
            var result = Enumerable.Range(0, Points.Count())
                .Select(number => list.Select(y => y[number]).MinBy(x => x.Power));
            return result;
        }
    }
    
    
    /// <summary>
    /// Получить набор с максимальными значениями
    /// </summary>
    /// <returns></returns>
    public IEnumerable<LinearPosition?> Maximum
    {
        get {
            var list = this.ToList();
            var result = Enumerable.Range(0, Points.Count())
                .Select(number => list.Select(y => y[number]).MaxBy(x => x.Power));
            return result;
        }
    }

    /// <summary>
    /// Получить набор с усредненными значениями
    /// </summary>
    /// <returns></returns>
    public IEnumerable<LinearPosition?> Average
    {
        get
        {
            var list = this.ToList();
            var result = Enumerable.Range(0, Points.Count())
                .Select(number => list.Select(y => y[number]).Average(x => x.Power))
                .Zip(Points.Select(x => x.Frequency),  (p, f) => new LinearPosition(f, p));
            return result;
        }
    }
        
        
    #region Конструкторы

    public LinearPositionsContext(IEnumerable<LinearPosition>? points) => Points = points ?? new List<LinearPosition>();
    
    public LinearPositionsContext(int count)
    {
        if(count <= 0) throw new ArgumentOutOfRangeException(nameof(count));
        Count = count;
        Points = new List<LinearPosition>();
    }

    #endregion

}