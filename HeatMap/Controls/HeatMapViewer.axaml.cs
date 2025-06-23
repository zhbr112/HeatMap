using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace HeatMap;

public partial class HeatMapViewer : Control
{
    public Color MinPowerColor { get; set; } = Colors.DarkBlue;
    public Color MaxPowerColor { get; set; } = Colors.Red;
    public int DisplayedRowCount { get; set; } = 200;

    private readonly ApplicationSettings _settings;
    private readonly LinearPositionsContext _context;

    private WriteableBitmap? _heatmapBitmap;

    public HeatMapViewer()
    {
        InitializeComponent();

        _context = App.GetRequiredService<LinearPositionsContext>();
        _settings = App.GetRequiredService<ApplicationSettings>();

        _context.PropertyChanged += (obj, args) => InvalidateVisual();
        _settings.PropertyChanged += (obj, args) => InvalidateVisual();
    }

    private void GenerateBitmap()
    {
        var allStripsData = _context.ToList();

        if (Bounds.Width < 1 || Bounds.Height < 1 || DisplayedRowCount <= 0 || allStripsData.Count == 0)
        {
            return;
        }

        var pixelWidth = (int)Bounds.Width;
        var pixelHeight = (int)Bounds.Height;

        _heatmapBitmap = new WriteableBitmap(
            new PixelSize(pixelWidth, pixelHeight),
            new Vector(96, 96),
            PixelFormat.Bgra8888,
            AlphaFormat.Premul);

        var stripsToDisplay = allStripsData.Count > DisplayedRowCount
            ? allStripsData.Skip(allStripsData.Count - DisplayedRowCount).ToList()
            : allStripsData;

        double stripHeight = Bounds.Height / DisplayedRowCount;
        if (stripHeight <= 0) return;

        var pixelData = new byte[pixelWidth * pixelHeight * 4];
        var backgroundColor = Colors.DarkBlue;

        for (int i = 0; i < pixelData.Length; i += 4)
        {
            pixelData[i] = backgroundColor.B;
            pixelData[i + 1] = backgroundColor.G;
            pixelData[i + 2] = backgroundColor.R;
            pixelData[i + 3] = 255;
        }

        for (int i = 0; i < stripsToDisplay.Count; i++)
        {
            int yStart = (int)(i * stripHeight);
            int yEnd = (int)((i + 1) * stripHeight);
            yEnd = Math.Min(yEnd, pixelHeight);

            var currentStripData = stripsToDisplay[^(i + 1)];
            if (currentStripData.Count == 0) continue;

            double cellWidth = Bounds.Width / currentStripData.Count;
            if (cellWidth < 1.0) cellWidth = 1.0;

            for (int j = 0; j < currentStripData.Count; j++)
            {
                int xStart = (int)(j * cellWidth);
                int xEnd = (int)((j + 1) * cellWidth);
                xEnd = Math.Min(xEnd, pixelWidth);

                var point = currentStripData[j];
                Color pointColor = GetColorForPowerHsl(point.Power);

                for (int py = yStart; py < yEnd; py++)
                {
                    for (int px = xStart; px < xEnd; px++)
                    {
                        int index = (py * pixelWidth + px) * 4;

                        pixelData[index] = pointColor.B;
                        pixelData[index + 1] = pointColor.G;
                        pixelData[index + 2] = pointColor.R;
                        pixelData[index + 3] = 255;
                    }
                }
            }
        }

        using (var framebuffer = _heatmapBitmap.Lock())
        {
            Marshal.Copy(pixelData, 0, framebuffer.Address, pixelData.Length);
        }
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (Bounds.Width <= 0 || Bounds.Height <= 0)
            return;

        GenerateBitmap();

        if (_heatmapBitmap != null) context.DrawImage(_heatmapBitmap, new Rect(0, 0, Bounds.Width, Bounds.Height));
        else context.DrawRectangle(new SolidColorBrush(Colors.DarkBlue), null, new Rect(0, 0, Bounds.Width, Bounds.Height));
    }

    private Color GetColorForPowerHsl(double power)
    {
        double min = _settings.GraphSettings!.GradientLevelMin;
        double max = _settings.GraphSettings!.GradientLevelMax;
        double range = max - min;

        double normalizedPower;
        if (range <= 0)        
            normalizedPower = power <= min ? 0.0 : 1.0;       
        else       
            normalizedPower = Math.Clamp((power - min) / range, 0.0, 1.0);        

        var startHsl = RgbToHsl(MinPowerColor);
        var endHsl = RgbToHsl(MaxPowerColor);

        double h = startHsl.H + (endHsl.H - startHsl.H) * normalizedPower;
        double s = startHsl.S + (endHsl.S - startHsl.S) * normalizedPower;
        double l = startHsl.L + (endHsl.L - startHsl.L) * normalizedPower;

        return HslToColor(h, s, l, MinPowerColor.A);
    }

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

    public static Color HslToColor(double h, double s, double l, byte alpha = 255)
    {
        double r, g, b;

        if (s == 0)
        {
            r = g = b = l;
        }
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

    public static double HueToRgb(double p, double q, double t)
    {
        if (t < 0) t += 1;
        if (t > 1) t -= 1;
        if (t < 1.0 / 6.0) return p + (q - p) * 6 * t;
        if (t < 1.0 / 2.0) return q;
        if (t < 2.0 / 3.0) return p + (q - p) * (2.0 / 3.0 - t) * 6;
        return p;
    }
}
