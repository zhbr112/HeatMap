using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace HeatMap;

public static class RegistryExtension
{
    public static IServiceCollection RegistrySettings(this IServiceCollection source, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(configuration);

        // Читаем настройки из конфигурации
        var settings = configuration.GetSection("ApplicationSettings")
                               .Get<ApplicationSettings>()
                            ?? throw new InvalidOperationException("Невозможно подключить и подготовить набор сервисов ввиду отсутствия настроек в группе ApplicationSettings!");

        // Добавить в DI настройки
        source
            .AddSingleton(settings)
            // Контекст с данными для графика функций
            .AddSingleton<LinearPositionsContext>(x => new LinearPositionsContext(settings.GraphSettings!.TimeToHold));
        
        return source;
    }

    public static IServiceCollection RegistryBackground(this IServiceCollection source,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(configuration);

        // Добавление DI
        source
            .AddHostedService<DataContextBackgroundService>();

        return source;
    }
}