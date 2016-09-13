﻿using System;
using System.Windows.Input;
using System.Windows.Media;
using NoeticTools.TeamStatusBoard.Framework.Commands;
using NoeticTools.TeamStatusBoard.Framework.Config;
using NoeticTools.TeamStatusBoard.Framework.Config.Properties;
using NoeticTools.TeamStatusBoard.Framework.Dashboards;
using NoeticTools.TeamStatusBoard.Framework.Services;
using NoeticTools.TeamStatusBoard.Framework.Services.TimeServices;


namespace NoeticTools.TeamStatusBoard.Framework.Plugins.Tiles.Guages.GuageAngular
{
    internal sealed class GuageAngularTileViewModel : NotifyingViewModelBase, IConfigurationChangeListener, ITileViewModel, ITimerListener
    {
        private readonly TimeSpan _updatePeriod = TimeSpan.FromSeconds(30);
        private readonly IServices _services;
        private readonly TileProperties _tileProperties;
        private double _value;
        private string _label = "";
        private double _maximum = 1.0;
        private double _minimum = -1.0;
        private string _format = "{0}";
        private double _sectionsInnerRadius = 0.8;
        private Color _foreground = Colors.White;
        private Color _ticksForeground = Colors.Gray;

        public GuageAngularTileViewModel(TileConfiguration tile, IDashboardController dashboardController, ITileLayoutController layoutController, IServices services)
        {
            _services = services;
            _tileProperties = new TileProperties(tile, this, services);
            Formatter = x => string.Format(Format, x);

            var parameters = GetConfigurationParameters();
            ConfigureCommand = new TileConfigureCommand(tile, "Data Value Tile Configuration", parameters, dashboardController, layoutController, services);

            _services.Timer.QueueCallback(TimeSpan.FromMilliseconds(100), this);

            Update();
        }

        private IPropertyViewModel[] GetConfigurationParameters()
        {
            var parameters = new IPropertyViewModel[]
            {
                new AutoCompleteTextPropertyViewModel("Label", _tileProperties.Properties, _services),
                new AutoCompleteTextPropertyViewModel("Value", _tileProperties.Properties, _services),
                new AutoCompleteTextPropertyViewModel("Minimum", _tileProperties.Properties, _services),
                new AutoCompleteTextPropertyViewModel("Maximum", _tileProperties.Properties, _services),
                new TextPropertyViewModel("Format", _tileProperties.Properties),
            };
            return parameters;
        }

        public string Label
        {
            get { return _label; }
            set
            {
                if (_label != null && _label.Equals(value))
                {
                    return;
                }
                _label = value;
                OnPropertyChanged();
            }
        }

        public Func<double, string> Formatter { get; set; }

        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        public double Maximum
        {
            get { return _maximum; }
            set
            {
                if (double.IsNaN(value) || value <= Minimum)
                {
                    return;
                }
                _maximum = value;
                OnPropertyChanged();
            }
        }

        public double Minimum
        {
            get { return _minimum; }
            set
            {
                if (double.IsNaN(value) || value >= Maximum)
                {
                    return;
                }
                _minimum = value;
                OnPropertyChanged();
            }
        }

        public string Format
        {
            get { return _format; }
            private set
            {
                if (_format != null && _format.Equals(value))
                {
                    return;
                }
                _format = value;
                OnPropertyChanged();
                OnPropertyChanged("Minimum");
                OnPropertyChanged("Maximum");
            }
        }

        public ICommand ConfigureCommand { get; }

        public void OnConfigurationChanged(TileConfigurationConverter converter)
        {
            Update();
        }

        private void Update()
        {
            var namedValueReader = _tileProperties.NamedValueReader;
            Label = namedValueReader.GetString("Label", "Label");
            Minimum = namedValueReader.GetDouble("Minimum");
            Maximum = namedValueReader.GetDouble("Maximum", 100.0);
            Value = namedValueReader.GetDouble("Value");
            Format = namedValueReader.GetString("Format", "{0} %");
        }

        void ITimerListener.OnTimeElapsed(TimerToken token)
        {
            Update();
            _services.Timer.QueueCallback(_updatePeriod, this);
        }
    }
}