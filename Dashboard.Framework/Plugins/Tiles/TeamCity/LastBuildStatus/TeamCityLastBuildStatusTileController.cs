﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using NoeticTools.Dashboard.Framework.Commands;
using NoeticTools.Dashboard.Framework.Config;
using NoeticTools.Dashboard.Framework.Config.Properties;
using NoeticTools.Dashboard.Framework.DataSources.TeamCity;
using NoeticTools.Dashboard.Framework.Tiles.TeamCity.LastBuildStatus;
using NoeticTools.Dashboard.Framework.Time;


namespace NoeticTools.Dashboard.Framework.Plugins.Tiles.TeamCity.LastBuildStatus
{
    internal sealed class TeamCityLastBuildStatusTileController : IViewController, ITimerListener
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
        private readonly TileConfiguration _tile;
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

        private readonly TileConfigurationConverter _tileConfigurationConverter;
        private readonly TimeSpan _updatePeriod = TimeSpan.FromSeconds(30);
        private readonly TimerToken _timerToken;
        private readonly TileLayoutController _layoutController;
        private readonly IServices _services;
        private TeamCityBuildStatusTileControl _view;

        public TeamCityLastBuildStatusTileController(TeamCityService service, TileConfiguration tile, IDashboardController dashboardController, TileLayoutController tileLayoutController, IServices services)
        {
            _service = service;
            _tile = tile;
            _dashboardController = dashboardController;
            _layoutController = tileLayoutController;
            _services = services;
            _tileConfigurationConverter = new TileConfigurationConverter(tile, this);
            ConfigureServiceCommand = new TeamCityServiceConfigureCommand(service);
            _timerToken = services.Timer.QueueCallback(TimeSpan.FromDays(10000), this);
        }

        public ICommand ConfigureCommand { get; private set; }

        public FrameworkElement CreateView()
        {
            var configurationParameters = GetConfigurationParameters();
            ConfigureCommand = new TileConfigureCommand(_tile, "Last Build Status Tile Configuration", configurationParameters, _dashboardController, _layoutController, _services);

            _view = new TeamCityBuildStatusTileControl {DataContext = this};

            _service.Connect();

            _timerToken.Requeue(TimeSpan.FromSeconds(0.1));
            return _view;
        }

        public void OnConfigurationChanged(TileConfigurationConverter converter)
        {
            _timerToken.Requeue(TimeSpan.FromMilliseconds(100));
        }

        public void OnTimeElapsed(TimerToken token)
        {
            var projectName = _tileConfigurationConverter.GetString("Project");
            var configurationName = _tileConfigurationConverter.GetString("Configuration");

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
                _view.statusText.Text = _tileConfigurationConverter.GetString("Description");
                _view.buildVer.Text = build.Number;
                _view.headerText.Text = running ? "Running" : "";
            }

            _view.root.Background = running ? _runningStatusBrushes[status] : _statusBrushes[status];
            SetTextForeground(running, status);

            _timerToken.Requeue(_updatePeriod);
        }

        private IPropertyViewModel[] GetConfigurationParameters()
        {
            var projectElementViewModel = new TeamCityProjectPropertyViewModel("Project", _tileConfigurationConverter, _service);
            var configurationParameters = new IPropertyViewModel[]
            {
                projectElementViewModel,
                new DependantPropertyViewModel("Configuration", "TextFromCombobox", _tileConfigurationConverter, projectElementViewModel,
                    x => x.Parameters = _service.GetConfigurationNames((string) projectElementViewModel.Value)),
                new PropertyViewModel("Description", "Text", _tileConfigurationConverter),
                new HyperlinkPropertyViewModel("TeamCity service", ConfigureServiceCommand)
            };
            return configurationParameters;
        }

        private void SetTextForeground(bool running, string status)
        {
            var textBrush = running ? _runningStatusTextBrushes[status] : _statusTextBrushes[status];
            _view.headerText.Foreground = textBrush;
            _view.statusText.Foreground = textBrush;
            _view.buildVer.Foreground = textBrush;
        }

        private ICommand ConfigureServiceCommand { get; }
    }
}