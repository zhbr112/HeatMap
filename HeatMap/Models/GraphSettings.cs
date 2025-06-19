using System.ComponentModel;

namespace HeatMap;

/// <summary>
/// Настройки графиков с поддержкой уведомлений об изменениях
/// </summary>
public class GraphSettings
{
    /// <summary>
    /// Использовать интерполяцию
    /// </summary>
    [Description("Использовать интерполяцию")]
    public bool Extrapolation { get; set; }

    /// <summary>
    /// Количество отображаемых точек на графике
    /// </summary>
    [Description("Количество точек")]
    public int DisplayPoints { get; set; }

    /// <summary>
    /// Режим реального времени обновления графика
    /// </summary>
    [Description("Режим реального времени")]
    [Browsable(false)]
    public bool RealTime { get; set; }


    /// <summary>
    /// Включение режима отображения максимальных значений (удержание максимумов)
    /// </summary>
    [Description("Удержание максимумов")]
    public bool MaxHold { get; set; }

    /// <summary>
    /// Включение режима отображения минимальных значений (удержание минимумов)
    /// </summary>
    [Description("Удержание минимумов")]
    public bool MinHold { get; set; }

    /// <summary>
    /// Количество измерений которые удерживаются в режимах Hold (Количество измерений)
    /// </summary>
    [Description("Количество измерений")]
    public int TimeToHold { get; set; }

    /// <summary>
    /// Включение режима усреднения значений на графике
    /// </summary>
    [Description("Режима усреднения")]
    public bool Average { get; set; }

    /// <summary>
    /// Минимальный уровень градиента
    /// </summary>
    [Description("Минимальный уровень градиента")]
    public int GradientLevelMin { get; set; }

    /// <summary>
    /// Максимальный уровень градиента
    /// </summary>
    [Description("Максимальный уровень градиента")]
    public int GradientLevelMax { get; set; }

    /// <summary>
    /// Минимальный уровень графика по оси Y
    /// </summary>
    [Description("Минимальный уровень графика по оси Y")]
    public int ChartYLevelMin { get; set; }

    /// <summary>
    /// Максимальный уровень графика по оси Y
    /// </summary>
    [Description("Максимальный уровень графика по оси Y")]
    public int ChartYLevelMax { get; set; }

    /// <summary>
    /// Минимальный уровень графика по оси X
    /// </summary>
    [Description("Минимальный уровень графика по оси X")]
    public int ChartXLevelMin { get; set; }

    /// <summary>
    /// Максимальный уровень графика по оси X
    /// </summary>
    [Description("Максимальный уровень графика по оси X")]
    public int ChartXLevelMax { get; set; }
}
