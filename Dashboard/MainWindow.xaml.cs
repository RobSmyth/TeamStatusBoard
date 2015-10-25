﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using NoeticTools.Dashboard.Framework;
using NoeticTools.Dashboard.Framework.Config;

namespace NoeticTools.TeamDashboard
{
    public partial class MainWindow : Window
    {
        private readonly DashboardController _controller;
        private readonly IDictionary<Key, Action<Key>> _keyDownHandlers;

        public MainWindow()
        {
            InitializeComponent();

            var runOptions = new RunOptions();
            _controller = new DashboardController(new DashboardConfigurationManager(), runOptions, new Clock());

            _keyDownHandlers = new Dictionary<Key, Action<Key>>
            {
                {Key.Home, key => _controller.ShowFirstDashboard()},
                {Key.End, key => _controller.ShowLastDashboard()},
                {Key.PageDown, key => _controller.NextDashboard()},
                {Key.PageUp, key => _controller.PrevDashboard()},
                {Key.F1, key => _controller.ShowHelp()},
                // F2 - used by tiles for tile edit
                {Key.F3, key => _controller.ShowNavigation()},
                // F5 - will use to force immediate refresh
                {Key.Escape, key => _controller.ShowCurrentDashboard()},
                // INS - will be used to insert a new tile
                // DEL - will be used to delete a tile
            };

            Loaded += LoadedHandler;
            Closed += ClosedHandler;
            KeyDown += MainWindow_KeyDown;
        }

        void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.IsDown)
            {
                if (Keyboard.Modifiers == ModifierKeys.None && _keyDownHandlers.ContainsKey(e.Key))
                {
                    _keyDownHandlers[e.Key](e.Key);
                }

                // TODO - CNTRL C, X, & V keys to cut & paste
                // TODO - SHIFT arrow keys to move tiles (maybe)

                // TODO - how to invoke dashboard configuration? ALT-F2? ... slide out panel?
            }
        }

        private void ClosedHandler(object sender, EventArgs e)
        {
            _controller.Stop();
        }

        private void LoadedHandler(object sender, RoutedEventArgs e)
        {
            _controller.Start(tileGrid, sidePanel);
        }
    }
}