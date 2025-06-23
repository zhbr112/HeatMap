using Avalonia.Media;

namespace HeatMap.Tests;

[TestFixture]
public class HeatMapViewerTests
{
    private const double Tolerance = 0.001;

    #region Color Conversion Tests (RGB <-> HSL)

    [Test]
    public void RgbToHsl_ShouldConvertRedCorrectly()
    {
        // Подготовка
        var red = Colors.Red;

        // Действие
        var (h, s, l) = HeatMapViewer.RgbToHsl(red);

        // Проверка
        Assert.That(h, Is.EqualTo(0.0).Within(Tolerance));
        Assert.That(s, Is.EqualTo(1.0).Within(Tolerance));
        Assert.That(l, Is.EqualTo(0.5).Within(Tolerance));
    }

    [Test]
    public void RgbToHsl_ShouldConvertGrayCorrectly()
    {
        // Подготовка
        var gray = Colors.Gray; // R=128, G=128, B=128

        // Действие
        var (h, s, l) = HeatMapViewer.RgbToHsl(gray);

        // Проверка
        Assert.That(h, Is.EqualTo(0.0).Within(Tolerance)); // Hue не имеет значения для серого, но обычно равен 0
        Assert.That(s, Is.EqualTo(0.0).Within(Tolerance)); // Saturation (насыщенность) должна быть 0
        Assert.That(l, Is.EqualTo(0.501).Within(Tolerance)); 
    }

    [Test]
    public void HslToRgb_ShouldConvertRedCorrectly()
    {
        // Подготовка
        double h = 0.0, s = 1.0, l = 0.5;

        // Действие
        var color = HeatMapViewer.HslToColor(h, s, l);

        // Проверка
        Assert.That(color, Is.EqualTo(Colors.Red));
    }

    [Test]
    [TestCase(255, 0, 0)]   // Red
    [TestCase(0, 255, 0)]   // Green
    [TestCase(0, 0, 255)]   // Blue
    [TestCase(255, 255, 0)] // Yellow
    [TestCase(0, 255, 255)] // Cyan
    [TestCase(128, 128, 128)]// Gray
    [TestCase(0, 0, 0)]     // Black
    [TestCase(255, 255, 255)]// White
    [TestCase(123, 45, 200)] // Random color
    public void RoundTrip_ColorConversion_WorksCorrectly(byte r, byte g, byte b)
    {
        // Подготовка
        var originalColor = Color.FromRgb(r, g, b);

        var (h, s, l) = HeatMapViewer.RgbToHsl(originalColor);
        var finalColor = HeatMapViewer.HslToColor(h, s, l);

        // Проверка
        Assert.That(finalColor.R, Is.EqualTo(originalColor.R).Within(1), "Red component mismatch");
        Assert.That(finalColor.G, Is.EqualTo(originalColor.G).Within(1), "Green component mismatch");
        Assert.That(finalColor.B, Is.EqualTo(originalColor.B).Within(1), "Blue component mismatch");
    }

    #endregion

    

    #region HueToRgb Tests

    [Test]
    public void HueToRgb_WhenTIsInThirdSegment_CalculatesCorrectly()
    {
        // Подготовка
        double t = 0.6;
        double expected = 0.45;

        // Действие
        var result = HeatMapViewer.HueToRgb(0.25, 0.75, t);

        // Проверка
        Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
    }

    [Test]
    public void HueToRgb_WhenTIsInFirstSegment_CalculatesCorrectly()
    {
        // Подготовка
        double t = 1.0 / 12.0;
        double expected = 0.5;

        // Действие
        var result = HeatMapViewer.HueToRgb(0.25, 0.75, t);

        // Проверка
        Assert.That(result, Is.EqualTo(expected).Within(Tolerance));
    }

    #endregion
}