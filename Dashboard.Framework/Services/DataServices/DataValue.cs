using System;
using System.Collections.Generic;
using System.Windows.Media;


namespace NoeticTools.TeamStatusBoard.Framework.Services.DataServices
{
    public sealed class DataValue : IDataValue
    {
        public const string DefaultString = "";
        public const string DefaultColour = "Gray";
        public const double DefaultDouble = 0.0;
        private object _instance;
        private readonly Action _notifyValueChanged;

        public DataValue(string name, object instance, PropertiesFlags flags, Action notifyValueChanged, params string[] tags)
        {
            Name = name;
            Flags = flags;
            _instance = instance;
            _notifyValueChanged = notifyValueChanged;
            Tags = new List<string>(tags);
            Broadcaster = new EventBroadcaster();
        }

        public EventBroadcaster Broadcaster { get; }

        public string Name { get; }
        public PropertiesFlags Flags { get; set; }
        public List<string> Tags { get; }

        public object Instance
        {
            get { return _instance; }
            set
            {
                if (_instance != null && _instance.Equals(value))
                {
                    return;
                }
                _instance = value;
                _notifyValueChanged();
                Broadcaster.Fire();
            }
        }

        public double Double
        {
            get { return Convert.ToDouble(Instance); }
            set { Instance = value; }
        }

        public Color Colour
        {
            get
            {
                try
                {
                    if (Instance is Color)
                    {
                        return (Color) Instance;
                    }
                    return (Color)ColorConverter.ConvertFromString(Convert.ToString(Instance));
                }
                catch (Exception)
                {
                    return (Color)ColorConverter.ConvertFromString(DefaultColour);
                }
            }
            set { Instance = value; }
        }

        public string String
        {
            get { return Convert.ToString(Instance); }
            set { Instance = value; }
        }

        public bool Boolean
        {
            get { return Convert.ToBoolean(Instance); }
            set { Instance = value; }
        }
    }
}