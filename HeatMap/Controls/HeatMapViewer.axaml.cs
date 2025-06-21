using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeatMap;

public partial class HeatMapViewer : Control
{
    public Color MinPowerColor { get; set; } = Colors.Blue;
    public Color MaxPowerColor { get; set; } = Colors.Yellow;
    public int DisplayedRowCount { get; set; } = 200;

    private readonly ApplicationSettings _settings;
    private readonly GraphSettings _graphSettings;

    private readonly LinearPositionsContext _context;

    public HeatMapViewer()
    {
        InitializeComponent();

        _context = App.GetRequiredService<LinearPositionsContext>();
        _settings = App.GetRequiredService<ApplicationSettings>();
        _graphSettings = _settings.GraphSettings;

        _context.PropertyChanged += (obj, args) => InvalidateVisual();       
        _settings.PropertyChanged += (obj, args) => InvalidateVisual();        
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (Bounds.Width <= 0 || Bounds.Height <= 0 || DisplayedRowCount <= 0)
            return;

        context.DrawRectangle(new SolidColorBrush(Colors.DarkBlue), null, new Rect(0, 0, Bounds.Width, Bounds.Height));

        var allStripsData = _context.ToList() ?? new List<List<LinearPosition>>();

        double stripHeight = Bounds.Height / DisplayedRowCount;
        if (stripHeight <= 0) stripHeight = 1.0;

        List<List<LinearPosition>> stripsToDisplay;

        if (allStripsData.Count > DisplayedRowCount) 
            stripsToDisplay = allStripsData.Skip(allStripsData.Count - DisplayedRowCount).ToList();        
        else stripsToDisplay = allStripsData;        

        for (int i = 0; i < stripsToDisplay.Count; i++)
        {
            double yPos = i * stripHeight;

            List<LinearPosition>  currentStripData = stripsToDisplay[^(i+1)];

            double cellWidth = Bounds.Width / currentStripData.Count;
            if (cellWidth < 1.0) cellWidth = 1.0;

            for (int j = 0; j < currentStripData.Count; j++)
            {
                var point = currentStripData[j];
                double xPos = j * cellWidth;

                Color pointColor = GetColorForPower(point.Power);
                var brush = new SolidColorBrush(pointColor);

                context.DrawRectangle(brush, null, new Rect(xPos, yPos, cellWidth, stripHeight));
            }
        }
    }

    private Color GetColorForPower(double power)
    {
        double normalizedPower;
        
        if (power <= _graphSettings.GradientLevelMin)
        {
            normalizedPower = 0.0;
        }
        else if (power >= _graphSettings.GradientLevelMax)
        {
            normalizedPower = 1.0;
        }
        else
        {
            normalizedPower = Math.Clamp((power - _graphSettings.GradientLevelMin) / (_graphSettings.GradientLevelMax - _graphSettings.GradientLevelMin), 0.0, 1.0);
        }

        byte r = (byte)(MinPowerColor.R * (1 - normalizedPower) + MaxPowerColor.R * normalizedPower);
        byte g = (byte)(MinPowerColor.G * (1 - normalizedPower) + MaxPowerColor.G * normalizedPower);
        byte b = (byte)(MinPowerColor.B * (1 - normalizedPower) + MaxPowerColor.B * normalizedPower);

        return Color.FromRgb(r, g, b);
    }
}
