using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using NoeticTools.TeamStatusBoard.Framework.Config.NamedValueRepositories;


namespace NoeticTools.TeamStatusBoard.Framework.Config
{
    public class ConfigurationNamedValueRepositoryDecorator : INamedValueRepository
    {
        private readonly INamedValueRepository _configurationRepository;
        private readonly INamedValueRepository _repository;

        public ConfigurationNamedValueRepositoryDecorator(INamedValueRepository configurationRepository, INamedValueRepository repository)
        {
            _configurationRepository = configurationRepository;
            _repository = repository;
        }

        public DateTime GetDateTime(string name)
        {
            return _repository.GetDateTime(GetValue(name));
        }

        public TimeSpan GetTimeSpan(string name)
        {
            return _repository.GetTimeSpan(GetValue(name));
        }

        public string GetString(string name)
        {
            return _repository.GetString(GetValue(name));
        }

        public string GetString(string name, string defaultValue)
        {
            return _repository.GetString(_configurationRepository.GetString(name, defaultValue), defaultValue);
        }

        public double GetDouble(string name, double defaultValue = 0)
        {
            return _repository.GetDouble(_configurationRepository.GetString(name, defaultValue.ToString()), defaultValue);
        }

        public bool GetBool(string name)
        {
            return _repository.GetBool(GetValue(name));
        }

        public object GetParameter(string name)
        {
            return _repository.GetParameter(GetValue(name));
        }

        public object GetParameter(string name, object defaultValue)
        {
            return _repository.GetParameter(name, defaultValue);
        }

        public void SetParameter(string name, object value)
        {
            _configurationRepository.SetParameter(name, value);
        }

        public Color GetColour(string name, string defaultValue)
        {
            return _repository.GetColour(GetValue(name), defaultValue);
        }

        public double[] GetDoubleArray(string name)
        {
            return GetValue(name).Split(',').Select(element => _repository.GetDouble(element)).ToArray();
        }

        private string GetValue(string name)
        {
            return _configurationRepository.GetString(name);
        }
    }
}