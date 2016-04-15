using System.Collections.Generic;
using System.Linq;
using NoeticTools.SystemsDashboard.Framework.Services.DataServices;


namespace NoeticTools.TeamStatusBoard.Framework.Services.DataServices
{
    public class DataRepositoy : IDataSource
    {
        private readonly IDictionary<string, object> _values = new Dictionary<string, object>();
        private readonly IList<IDataChangeListener> _listeners = new List<IDataChangeListener>();

        public DataRepositoy(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public void AddListener(IDataChangeListener listener)
        {
            _listeners.Add(listener);
        }

        public IEnumerable<string> GetAllNames()
        {
            return _values.Keys.ToArray();
        }

        public void Write<T>(string name, T value)
        {
            _values[name] = value;
            NotifyValueChanged();
        }

        private void NotifyValueChanged()
        {
            var listeners = _listeners.ToArray();
            foreach (var listener in listeners)
            {
                listener.OnChanged();
            }
        }

        public T Read<T>(string name)
        {
            if (!_values.ContainsKey(name))
            {
                _values.Add(name, default(T));
            }
            return (T)_values[name];
        }
    }
}