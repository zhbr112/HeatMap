namespace HeatMap;

/// <summary>
/// Модель представления линейного графика
/// </summary>
public class LinearPosition
{
    /// <summary>
    /// Частота
    /// </summary>
    public double Frequency { get; set; }

    /// <summary>
    /// Мощность 
    /// </summary>
    public double Power { get; set; }

    public LinearPosition(double frequency, double power)
    {
        Frequency = frequency;
        Power = power;
    }
}