﻿using NoeticTools.TeamStatusBoard.Framework.Plugins;
using NoeticTools.TeamStatusBoard.Framework.Services;
using NoeticTools.TeamStatusBoard.TeamCity.DataSources.TeamCity.Demo;


namespace NoeticTools.TeamStatusBoard.TeamCity.DataSources.TeamCity
{
    public sealed class TeamCityTestDataPlugin : IPlugin
    {
        private readonly string _teamCityServiceName;

        public TeamCityTestDataPlugin(string teamCityServiceName)
        {
            _teamCityServiceName = teamCityServiceName;
        }

        public int Rank => 10;

        public void Register(IServices services)
        {
            new TeamCityDemoService(services.GetService<ITeamCityService>(_teamCityServiceName), services);
        }
    }
}