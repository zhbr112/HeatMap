using Microsoft.Extensions.Hosting;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HeatMap;

public class DataContextBackgroundService : BackgroundService
{
    private readonly ApplicationSettings _settings;
    private readonly IServiceProvider _services;
    private CancellationTokenSource _cts = new();
    private TaskCompletionSource<bool> _completionTaskSource = new();

    public DataContextBackgroundService(ApplicationSettings settings, IServiceProvider services)
    {
        _settings = settings;
        _services = services;
        // Установим режим работу фоновой задачи
        _completionTaskSource.TrySetResult(_settings.GraphSettings!.RealTime);
        // Подписка на изменение настроек
        _settings.PropertyChanged += SettingsOnPropertyChanged;
    }

    private void SettingsOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        _completionTaskSource.TrySetResult(_settings.GraphSettings!.RealTime);
    }

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        while (!_cts.Token.IsCancellationRequested)
        {
            await ProcessLoop();
            await _completionTaskSource.Task;
            _completionTaskSource = new TaskCompletionSource<bool>();
        }
    }

    private async Task ProcessLoop()
    {
        while (!_cts.Token.IsCancellationRequested && _settings.GraphSettings!.RealTime)
        {
            try
            {
                var _context = App.GetRequiredService<LinearPositionsContext>();

                _context.Points = LinearPositionsContextHelper.CreateRandomContext().Points.ToList();
            }
            finally
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100), _cts.Token);
            }
        }
    }

    public override void Dispose()
    {
        _settings.PropertyChanged -= SettingsOnPropertyChanged;
        _cts.Cancel();
        _cts.Dispose();
        base.Dispose();
    }
}

