using System;
using System.Windows.Input;
using NoeticTools.SystemsDashboard.Framework.Commands;
using NoeticTools.SystemsDashboard.Framework.Config;
using NoeticTools.SystemsDashboard.Framework.Config.Properties;
using NoeticTools.SystemsDashboard.Framework.Time;


namespace NoeticTools.SystemsDashboard.Framework.Plugins.Tiles.DaysLeftCountDown
{
    internal sealed class DaysLeftCountDownTileViewModel : NotifyingViewModelBase, IConfigurationChangeListener, ITimerListener
    {
        private readonly IClock _clock;
        private readonly TimeSpan _tickPeriod = TimeSpan.FromSeconds(30);
        private readonly TileConfigurationConverter _tileConfigurationConverter;
        private readonly DaysLeftCountDownTileView _view;
        private readonly TimerToken _timerToken;

        public DaysLeftCountDownTileViewModel(TileConfiguration tile, IClock clock, IDashboardController dashboardController, DaysLeftCountDownTileView view, TileLayoutController tileLayoutController,
            IServices services)
        {
            _clock = clock;
            _view = view;
            _tileConfigurationConverter = new TileConfigurationConverter(tile, this);
            ConfigureCommand = new TileConfigureCommand(tile, "Days Count Down Configuration",
                new IPropertyViewModel[]
                {
                    new PropertyViewModel("Title", "Text", _tileConfigurationConverter),
                    new PropertyViewModel("End_date", "DateTime", _tileConfigurationConverter),
                    new PropertyViewModel("Disabled", "Checkbox", _tileConfigurationConverter)
                },
                dashboardController, tileLayoutController, services);
            _view.DataContext = this;
            _timerToken = services.Timer.QueueCallback(TimeSpan.FromMilliseconds(10), this);
        }

        public ICommand ConfigureCommand { get; }

        public void OnConfigurationChanged(TileConfigurationConverter converter)
        {
            UpdateView();
        }

        private void UpdateView()
        {
            var disabled = _tileConfigurationConverter.GetBool("Disabled");
            var endDate = _tileConfigurationConverter.GetDateTime("End_date");
            var now = _clock.Now;
            var daysRemaining = Math.Abs((endDate - now).Days);
            var weeksRemaining = daysRemaining/7;
            var workingDaysRemaining = (weeksRemaining*5) + daysRemaining%7;
            if (now > endDate)
            {
                workingDaysRemaining = -workingDaysRemaining;
            }
            _view.days.Text = disabled ? "-" : workingDaysRemaining.ToString();
            _view.header.Text = _tileConfigurationConverter.GetString("Title");
        }

        void ITimerListener.OnTimeElapsed(TimerToken token)
        {
            UpdateView();
            _timerToken.Requeue(_tickPeriod);
        }
    }
}