using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HeatMap;

/// <summary>
/// Базовая реализация модели с поддержкой MVVM
/// see: https://habr.com/ru/articles/505036/
/// </summary>
public abstract class AbstractNotifyProperyChanged : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Вызвать событие об изменении свойства
    /// </summary>
    /// <param name="propertyName"> Наименование свойства </param>
    public virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}