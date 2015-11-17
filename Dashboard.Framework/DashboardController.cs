﻿using System.Windows;
using System.Windows.Controls;
using NoeticTools.Dashboard.Framework.Config;
using NoeticTools.Dashboard.Framework.Tiles;
using NoeticTools.Dashboard.Framework.Tiles.Dashboards;
using NoeticTools.Dashboard.Framework.Tiles.Help;
using NoeticTools.Dashboard.Framework.Tiles.InsertTile;
using NoeticTools.Dashboard.Framework.Time;


namespace NoeticTools.Dashboard.Framework
{
    public class DashboardController : IDashboardController
    {
        private readonly DashboardConfigurations _config;
        private readonly IDashboardNavigator _dashboardNavigator;
        private readonly DashboardConfigurationManager _configurationManager;
        private readonly ITimerService _timerService;
        private readonly DockPanel _sidePanel;

        public DashboardController(DashboardConfigurationManager configurationManager, ITimerService timerService, DockPanel sidePanel, DashboardConfigurations config, IDashboardNavigator dashboardNavigator)
        {
            _configurationManager = configurationManager;
            _timerService = timerService;
            _sidePanel = sidePanel;
            _config = config;
            _dashboardNavigator = dashboardNavigator;
        }

        public void Start()
        {
        }

        public void Stop()
        {
            Save();
        }

        public void ShowHelpPane()
        {
            ShowOnSidePane(new HelpViewController(), "Help");
        }

        public void ShowNavigationPane()
        {
            ShowOnSidePane(new DashboardsNavigationViewController(_config, _dashboardNavigator), "Dashboards Navigation");
        }

        public void ShowOnSidePane(IViewController viewController, string title)
        {
            _sidePanel.Visibility = Visibility.Collapsed;
            _sidePanel.Children.Clear();

            var view = viewController.CreateView();

            _sidePanel.Children.Add(view);
            view.IsVisibleChanged += View_IsVisibleChanged;

            _sidePanel.Visibility = Visibility.Visible;
        }

        public void Refresh()
        {
            _timerService.FireAll();
        }

        public void ToggleGroupPanelsEditMode()
        {
            _dashboardNavigator.ToggleShowGroupPanelsDetailsMode();
            //ShowOnSidePane(new GroupPanelsEditTileViewModel(_config, _tileRegistryConduit));
        }

        public void ShowInsertPanel()
        {
            ShowOnSidePane(new InsertTileController(), "Help");
        }

        private void Save()
        {
            _configurationManager.Save(_config);
        }

        private void View_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var view = (UIElement) sender;

            if (_sidePanel.Children.Contains(view))
            {
                if (view.Visibility == Visibility.Collapsed)
                {
                    _sidePanel.Children.Remove(view);
                }
                if (_sidePanel.Children.Count == 0)
                {
                    _sidePanel.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}