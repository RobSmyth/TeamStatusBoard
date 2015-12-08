﻿using System;
using NoeticTools.SystemsDashboard.Framework;
using NoeticTools.SystemsDashboard.Framework.Commands;
using NoeticTools.SystemsDashboard.Framework.Config;
using NoeticTools.SystemsDashboard.Framework.DataSources.TeamCity;


namespace NoeticTools.SystemsDashboard.Framework.Plugins.Tiles.TeamCity.LastBuildStatus
{
    public sealed class TeamCityLastBuildStatusTilePlugin : IPlugin, ITileControllerProvider
    {
        private readonly TeamCityService _service;
        private readonly IDashboardController _dashboardController;
        private readonly IServices _services;

        public TeamCityLastBuildStatusTilePlugin(TeamCityService service, IDashboardController dashboardController, IServices services)
        {
            _service = service;
            _dashboardController = dashboardController;
            _services = services;
        }

        public int Rank => 0;

        public string Name => "TeamCity build status";

        public bool MatchesId(string id)
        {
            return id == TeamCityLastBuildStatusTileController.TileTypeId || id.Equals("0FFACE9A-8B68-4DBC-8B42-0255F51368B5", StringComparison.InvariantCultureIgnoreCase);
        }

        public IViewController CreateTileController(TileConfiguration tile, TileLayoutController layoutController)
        {
            return new TeamCityLastBuildStatusTileController(_service, tile, _dashboardController, layoutController, _services);
        }

        public TileConfiguration CreateDefaultConfiguration()
        {
            return new TileConfiguration
            {
                TypeId = TeamCityLastBuildStatusTileController.TileTypeId,
                Id = Guid.NewGuid(),
                Tiles = new TileConfiguration[0]
            };
        }

        public void Register(IServices services)
        {
            services.TileProviders.Register(this);
        }
    }
}