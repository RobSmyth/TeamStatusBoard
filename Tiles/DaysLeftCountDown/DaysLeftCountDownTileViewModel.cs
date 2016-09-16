using System;
using System.Windows.Input;
using NoeticTools.TeamStatusBoard.Framework;
using NoeticTools.TeamStatusBoard.Framework.Commands;
using NoeticTools.TeamStatusBoard.Framework.Config;
using NoeticTools.TeamStatusBoard.Framework.Config.Properties;
using NoeticTools.TeamStatusBoard.Framework.Dashboards;
using NoeticTools.TeamStatusBoard.Framework.Plugins.Tiles;
using NoeticTools.TeamStatusBoard.Framework.Services;
using NoeticTools.TeamStatusBoard.Framework.Services.TimeServices;


namespace NoeticTools.TeamStatusBoard.Tiles.DaysLeftCountDown
{
    internal sealed class DaysLeftCountDownTileViewModel : NotifyingViewModelBase, IConfigurationChangeListener, ITimerListener, ITileViewModel
    {
        private readonly IClock _clock;
        private readonly TimeSpan _tickPeriod = TimeSpan.FromSeconds(30);
        private readonly TileConfigurationConverter _tileConfigurationConverter;
        private readonly DaysLeftCoundDownTileView _view;
        private readonly ITimerToken _timerToken;
        private string _daysRemaining;
        private string _title;

        public DaysLeftCountDownTileViewModel(TileConfiguration tile, IClock clock, IDashboardController dashboardController, DaysLeftCoundDownTileView view, TileLayoutController tileLayoutController,
            IServices services)
        {
            _clock = clock;
            _view = view;
            _tileConfigurationConverter = new TileConfigurationConverter(tile, this);
            ConfigureCommand = new TileConfigureCommand(tile, "Days Count Down Configuration",
                new IPropertyViewModel[]
                {
                    new PropertyViewModel("Title", PropertyType.Text, _tileConfigurationConverter),
                    new PropertyViewModel("End_date", PropertyType.DateTime, _tileConfigurationConverter),
                    new PropertyViewModel("Disabled", PropertyType.Checkbox, _tileConfigurationConverter)
                },
                dashboardController, tileLayoutController, services);
            DaysRemaining = "";
            _view.DataContext = this;
            _timerToken = services.Timer.QueueCallback(TimeSpan.FromMilliseconds(10), this);
        }

        public ICommand ConfigureCommand { get; }

        public string Title
        {
            get { return _title; }
            set
            {
                if (string.IsNullOrWhiteSpace(_title) || !_title.Equals(value, StringComparison.InvariantCulture))
                {
                    _title = value;
                    OnPropertyChanged();
                }
            }
        }

        public string DaysRemaining
        {
            get { return _daysRemaining; }
            private set
            {
                if (_daysRemaining != value)
                {
                    _daysRemaining = value;
                    OnPropertyChanged();
                }
            }
        }

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
            DaysRemaining = disabled ? "-" : workingDaysRemaining.ToString();
            Title = _tileConfigurationConverter.GetString("Title");
        }

        void ITimerListener.OnTimeElapsed(TimerToken token)
        {
            UpdateView();
            _timerToken.Requeue(_tickPeriod);
        }
    }
}