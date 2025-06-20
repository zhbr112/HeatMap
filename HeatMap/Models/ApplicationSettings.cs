using System.ComponentModel;

namespace HeatMap;

public class ApplicationSettings: AbstractNotifyProperyChanged
{
    private GraphSettings? _graphSettings;  

    public GraphSettings? GraphSettings
    {
        get => _graphSettings;
        set
        {
            if (value != _graphSettings)
            {
                _graphSettings = value;
                OnPropertyChanged();
            }
        }
    }    
}