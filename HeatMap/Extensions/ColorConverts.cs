using Avalonia.Media;
using System;

namespace HeatMap;

public class ColorConverts
{
    /// <summary>
    /// Получение цвета в зависимости от мощности
    /// </summary>
    public static Color GetColorForPower(double power, double min, double max, Color minPowerColor, Color maxPowerColor)
    {
        double range = max - min;

        double normalizedPower;
        if (range <= 0)
            normalizedPower = power <= min ? 0.0 : 1.0;
        else if (power <= min)
            return minPowerColor;
        else if (power >= max)
            return maxPowerColor;
        else
            normalizedPower = Math.Clamp((power - min) / range, 0.0, 1.0);
        

        var startHsl = RgbToHsl(minPowerColor);
        var endHsl = RgbToHsl(maxPowerColor);

        double h = startHsl.H + (endHsl.H - startHsl.H) * normalizedPower;
        double s = startHsl.S + (endHsl.S - startHsl.S) * normalizedPower;
        double l = startHsl.L + (endHsl.L - startHsl.L) * normalizedPower;

        return HslToColor(h, s, l);
    }

    /// <summary>
    /// Конвертация rgb в hsl
    /// </summary>
    public static (double H, double S, double L) RgbToHsl(Color color)
    {
        double r = color.R / 255.0;
        double g = color.G / 255.0;
        double b = color.B / 255.0;

        double max = Math.Max(r, Math.Max(g, b));
        double min = Math.Min(r, Math.Min(g, b));

        double h = 0, s = 0, l = (max + min) / 2;

        if (Math.Abs(max - min) < 0.0001)
        {
            h = s = 0;
        }
        else
        {
            double d = max - min;
            s = l > 0.5 ? d / (2 - max - min) : d / (max + min);
            if (max == r) h = (g - b) / d + (g < b ? 6 : 0);
            else if (max == g) h = (b - r) / d + 2;
            else if (max == b) h = (r - g) / d + 4;
            h /= 6;
        }

        return (h, s, l);
    }

    /// <summary>
    /// Конвертация hsl в rgb
    /// </summary>
    public static Color HslToColor(double h, double s, double l, byte alpha = 255)
    {
        double r, g, b;
        if (s == 0) { r = g = b = l; }
        else
        {
            double q = l < 0.5 ? l * (1 + s) : l + s - l * s;
            double p = 2 * l - q;
            r = HueToRgb(p, q, h + 1.0 / 3.0);
            g = HueToRgb(p, q, h);
            b = HueToRgb(p, q, h - 1.0 / 3.0);
        }
        return Color.FromArgb(alpha, (byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
    }

    /// <summary>
    /// Конвертация hsl в rgb для одного канала
    /// </summary>
    public static double HueToRgb(double p, double q, double t)
    {
        if (t < 0) t += 1;
        if (t > 1) t -= 1;
        if (t < 1.0 / 6.0) return p + (q - p) * 6 * t;
        if (t < 1.0 / 2.0) return q;
        if (t < 2.0 / 3.0) return p + (q - p) * (2.0 / 3.0 - t) * 6;
        return p;
    }

    /// <summary>
    /// Конвертация hex в Color
    /// </summary>
    public static Color HexToColor(string hexColor, byte alpha = 255)
    {
        hexColor = hexColor.TrimStart('#');

        byte r = (byte)Convert.ToInt32(hexColor.Substring(0, 2), 16);
        byte g = (byte)Convert.ToInt32(hexColor.Substring(2, 2), 16);
        byte b = (byte)Convert.ToInt32(hexColor.Substring(4, 2), 16);

        return Color.FromArgb(alpha, r, g, b);
    }
}
