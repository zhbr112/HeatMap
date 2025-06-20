using Avalonia.Controls;

namespace HeatMap;

public partial class HeatMapViewer : Control
{
    // Настройки
    private readonly ApplicationSettings _settings;
    // Контекст с данными
    private readonly LinearPositionsContext _context;   

    public HeatMapViewer()
    {
        InitializeComponent();

        _context = App.GetRequiredService<LinearPositionsContext>();
        _settings = App.GetRequiredService<ApplicationSettings>();

        // Подключаю обработку событий
        _context.PropertyChanged += (obj, args) => InvalidateVisual();       
        _settings.PropertyChanged += (obj, args) => InvalidateVisual();      
    }
}