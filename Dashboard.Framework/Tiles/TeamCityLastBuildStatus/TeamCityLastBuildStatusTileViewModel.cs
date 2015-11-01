﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using NoeticTools.Dashboard.Framework.Config;
using NoeticTools.Dashboard.Framework.Config.Commands;
using NoeticTools.Dashboard.Framework.Config.Parameters;
using NoeticTools.Dashboard.Framework.DataSources.TeamCity;
using NoeticTools.Dashboard.Framework.Time;


namespace NoeticTools.Dashboard.Framework.Tiles.TeamCityLastBuildStatus
{
    internal class TeamCityLastBuildStatusTileViewModel : ITileViewModel, ITimerListener
    {
        public const string TileTypeId = "TeamCity.Build.Status";

        private readonly Dictionary<string, Brush> _runningStatusBrushes = new Dictionary<string, Brush>
        {
            {"SUCCESS", Brushes.Yellow},
            {"FAILURE", Brushes.OrangeRed},
            {"UNKNOWN", Brushes.Gray}
        };

        private readonly Dictionary<string, Brush> _runningStatusTextBrushes = new Dictionary<string, Brush>
        {
            {"SUCCESS", Brushes.DarkSlateGray},
            {"FAILURE", Brushes.White},
            {"UNKNOWN", Brushes.White}
        };

        private readonly TeamCityService _service;
        private readonly IDashboardController _dashboardController;

        private readonly Dictionary<string, Brush> _statusBrushes = new Dictionary<string, Brush>
        {
            {"SUCCESS", (SolidColorBrush) (new BrushConverter().ConvertFrom("#FF448032"))},
            {"FAILURE", Brushes.Firebrick},
            {"UNKNOWN", Brushes.Gray}
        };

        private readonly Dictionary<string, Brush> _statusTextBrushes = new Dictionary<string, Brush>
        {
            {"SUCCESS", Brushes.White},
            {"FAILURE", Brushes.White},
            {"UNKNOWN", Brushes.White}
        };

        private readonly TileConfiguration _tileConfiguration;
        private readonly TimeSpan _updatePeriod = TimeSpan.FromSeconds(30);
        private readonly TimerToken _timerToken;
        private TeamCityBuildStatusTileControl _view;

        public TeamCityLastBuildStatusTileViewModel(TeamCityService service,
            DashboardTileConfiguration tileConfiguration, ITimerService timerService,
            IDashboardController dashboardController)
        {
            _service = service;
            _dashboardController = dashboardController;
            _tileConfiguration = new TileConfiguration(tileConfiguration, this);
            ConfigureServiceCommand = new TeamCityServiceConfigureCommand(service);
            _timerToken = timerService.QueueCallback(TimeSpan.FromDays(10000), this);
        }

        public ICommand ConfigureCommand { get; private set; }

        public FrameworkElement CreateView()
        {
            ConfigureCommand = new TileConfigureCommand("Last Build Status Configuration", _tileConfiguration,
                new IConfigurationView[]
                {
                    new ConfigurationParameter("Project", "", _tileConfiguration),
                    new ConfigurationParameter("Configuration", "", _tileConfiguration),
                    new ConfigurationParameter("Description", "", _tileConfiguration),
                    new ConfigurationHyperlink("TeamCity service", ConfigureServiceCommand)
                },
                _dashboardController);

            _view = new TeamCityBuildStatusTileControl {DataContext = this};

            _service.Connect();

            _timerToken.Requeue(TimeSpan.FromSeconds(1));
            return _view;
        }

        public void OnConfigurationChanged()
        {
            _timerToken.Requeue(TimeSpan.FromMilliseconds(100));
        }

        public void OnTimeElapsed(TimerToken token)
        {
            var projectName = _tileConfiguration.GetString("Project");
            var configurationName = _tileConfiguration.GetString("Configuration");

            var build = _service.GetRunningBuild(projectName, configurationName);
            var running = build != null;

            if (build == null)
            {
                build = _service.GetLastBuild(projectName, configurationName);
            }

            string status;

            if (build == null)
            {
                status = "UNKNOWN";
                _view.statusText.Text = "ERROR";
                _view.buildVer.Text = "";
                _view.headerText.Text = "";
            }
            else
            {
                status = build.Status;
                if (!_statusBrushes.ContainsKey(status))
                {
                    status = "UNKNOWN";
                }
                _view.statusText.Text = _tileConfiguration.GetString("Description");
                _view.buildVer.Text = build.Number;
                _view.headerText.Text = running ? "Running" : "";
            }

            _view.root.Background = running ? _runningStatusBrushes[status] : _statusBrushes[status];
            SetTextForeground(running, status);

            _timerToken.Requeue(_updatePeriod);
        }

        private ICommand ConfigureServiceCommand { get; }

        private void SetTextForeground(bool running, string status)
        {
            var textBrush = running ? _runningStatusTextBrushes[status] : _statusTextBrushes[status];
            _view.headerText.Foreground = textBrush;
            _view.statusText.Foreground = textBrush;
            _view.buildVer.Foreground = textBrush;
        }
    }
}