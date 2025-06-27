using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using HeatMap;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace HeatMap;

public partial class HeatMapViewer : Control
{
    private Color MinPowerColor { get; set; }
    private Color MaxPowerColor { get; set; }

    private int CountRows { get; set; }

    // Настройки
    private readonly ApplicationSettings _settings;

    // Контекст с данными
    private readonly LinearPositionsContext _context;

    // Кэш для отрисованной тепловой карты
    private WriteableBitmap? _heatmapBitmap;

    public HeatMapViewer()
    {
        InitializeComponent();

        _context = App.GetRequiredService<LinearPositionsContext>();
        _settings = App.GetRequiredService<ApplicationSettings>();

        MinPowerColor = ColorConverts.HexToColor(_settings.GraphSettings!.MinPowerColor);
        MaxPowerColor = ColorConverts.HexToColor(_settings.GraphSettings!.MaxPowerColor);

        // Подключаю обработку событий
        _context.PropertyChanged += (obj, args) => InvalidateVisual();
        _settings.PropertyChanged += (obj, args) => InvalidateVisual();
    }


    /// <summary>
    /// Генерирует битмап тепловой карты на основе текущих данных и размеров
    /// </summary>
    private void GenerateBitmap()
    {
        var allStripsData = _context.ToList();

        if (Bounds.Width < 1 || Bounds.Height < 1 || allStripsData.Count == 0)
        {
            return;
        }

        CountRows = Bounds.Height < _settings.GraphSettings!.CountRowsHeatmap ? (int)Bounds.Height : _settings.GraphSettings!.CountRowsHeatmap;

        var pixelWidth = (int)Bounds.Width;
        var pixelHeight = (int)Bounds.Height;

        double displayFreqStart = _settings.GraphSettings!.ChartXLevelMin;
        double displayFreqEnd = _settings.GraphSettings!.ChartXLevelMax;
        double displayFreqRange = displayFreqEnd - displayFreqStart;

        _heatmapBitmap = new WriteableBitmap(
            new PixelSize(pixelWidth, pixelHeight),
            new Vector(96, 96),
            PixelFormat.Bgra8888,
            AlphaFormat.Premul);

        var stripsToDisplay = allStripsData.Count > CountRows
            ? allStripsData.Skip(allStripsData.Count - CountRows).ToList()
            : allStripsData;

        double stripHeight = Bounds.Height / CountRows;
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
                var point = currentStripData[j];

                double pointFreqStart = point.Frequency;
                double pointFreqEnd = j + 1 < currentStripData.Count ? currentStripData[j + 1].Frequency : 2 * point.Frequency - currentStripData[j - 1].Frequency;

                if (pointFreqEnd < displayFreqStart || pointFreqStart > displayFreqEnd)
                {
                    continue;
                }

                int xStart = (int)(((pointFreqStart - displayFreqStart) / displayFreqRange) * pixelWidth);
                int xEnd = (int)(((pointFreqEnd - displayFreqStart) / displayFreqRange) * pixelWidth);

                if (xStart == xEnd) xEnd = xStart + 1;

                xEnd = Math.Min(xEnd, pixelWidth);

                Color pointColor = ColorConverts.GetColorForPower(
                    point.Power,
                    _settings.GraphSettings!.GradientLevelMin,
                    _settings.GraphSettings!.GradientLevelMax,
                    MinPowerColor,
                    MaxPowerColor);

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

    /// <summary>
    /// Render User Control
    /// </summary>
    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (Bounds.Width <= 0 || Bounds.Height <= 0)
            return;

        GenerateBitmap();

        if (_heatmapBitmap != null) context.DrawImage(_heatmapBitmap, new Rect(0, 0, Bounds.Width, Bounds.Height));
        else context.DrawRectangle(new SolidColorBrush(Colors.DarkBlue), null, new Rect(0, 0, Bounds.Width, Bounds.Height));
    }
}
