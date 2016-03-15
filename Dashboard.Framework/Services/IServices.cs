using NoeticTools.SystemsDashboard.Framework.Dashboards;
using NoeticTools.SystemsDashboard.Framework.DataSources;
using NoeticTools.SystemsDashboard.Framework.Input;
using NoeticTools.SystemsDashboard.Framework.Registries;
using NoeticTools.SystemsDashboard.Framework.Registries;
using NoeticTools.SystemsDashboard.Framework.Services.DataServices;
using NoeticTools.SystemsDashboard.Framework.Services.TimeServices;


namespace NoeticTools.SystemsDashboard.Framework
{
    public interface IServices
    {
        KeyboardHandler KeyboardHandler { get; }
        ITileProviderRegistry TileProviders { get; }
        IPropertyEditControlRegistry PropertyEditControlProviders { get; }
        ITimerService Timer { get; }
        IDataService DataService { get; }
        IPropertiesDataService Properties { get; }
        IClock Clock { get; set; }
        IDashboardController DashboardController { get; set; }
    }
}