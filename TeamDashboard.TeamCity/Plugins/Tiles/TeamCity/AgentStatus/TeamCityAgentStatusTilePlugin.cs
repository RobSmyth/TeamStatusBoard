﻿using NoeticTools.TeamStatusBoard.Framework.Plugins.DataSources.TeamCity;
using NoeticTools.TeamStatusBoard.Framework.Services;
using NoeticTools.TeamStatusBoard.TeamCity.Plugins.TeamCity;


namespace NoeticTools.TeamStatusBoard.Framework.Plugins.Tiles.TeamCity.AgentStatus
{
    public sealed class TeamCityAgentStatusTilePlugin : IPlugin
    {
        public int Rank => 0;

        public void Register(IServices services)
        {
            services.TileProviders.Register(new TeamCityAgentStatusTileProvider(services.GetService<TeamCityService>("TeamCity"), services.DashboardController, services));
        }
    }
}