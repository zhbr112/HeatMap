using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace HeatMap;

public partial class App : Avalonia.Application
{
    public IServiceProvider? Provider { get; private set; }

    public static App? Instance { get; private set; }

    public static T GetRequiredService<T>() where T : class
        => Instance?.Provider?.GetRequiredService<T>() ??
           throw new InvalidOperationException($"Невозможно получить объект типа {typeof(T).FullName}!");

    #region Default

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        base.OnFrameworkInitializationCompleted();
        Instance = this;

        // Подключить настройки
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables()
            .Build();

        // Подключить DI
        var host = Host.CreateDefaultBuilder().ConfigureServices((context, collection) =>
        {
            collection.AddLogging();
            collection
                // Настройки
                .RegistrySettings(config)
                // Фоновые задания
                .RegistryBackground(config);
        }).Build();

        Provider = host.Services;
        host.StartAsync();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = new MainWindow();
    }

    #endregion
}