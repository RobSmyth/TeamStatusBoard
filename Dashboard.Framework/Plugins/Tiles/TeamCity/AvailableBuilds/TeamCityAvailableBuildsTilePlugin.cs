﻿using System;
using NoeticTools.Dashboard.Framework.Config;
using NoeticTools.Dashboard.Framework.DataSources.TeamCity;
using NoeticTools.Dashboard.Framework.Tiles;
using NoeticTools.Dashboard.Framework.Tiles.TeamCityAvailableBuilds;
using NoeticTools.Dashboard.Framework.Time;


namespace NoeticTools.Dashboard.Framework.Plugins.Tiles.TeamCity.AvailableBuilds
{
    public class TeamCityLAvailbleBuildSTilePlugin : IPlugin, ITileControllerProvider
    {
        private readonly TeamCityService _service;
        private readonly ITimerService _timerService;
        private readonly IDashboardController _dashboardController;

        public TeamCityLAvailbleBuildSTilePlugin(TeamCityService service, ITimerService timerService, IDashboardController dashboardController)
        {
            _service = service;
            _timerService = timerService;
            _dashboardController = dashboardController;
        }

        public string Name => "TeamCity available builds";

        public bool MatchesId(string id)
        {
            return id == TeamCityAvailableBuildsTileController.TileTypeId || id.Equals("0FFACE9A-8B68-4DBC-8B42-0255F51368B6", StringComparison.InvariantCultureIgnoreCase);
        }

        public IViewController CreateTileController(TileConfiguration tileConfiguration, TileLayoutController tileLayoutController)
        {
            return new TeamCityAvailableBuildsTileController(_service, tileConfiguration, _timerService, _dashboardController, tileLayoutController);
        }

        public TileConfiguration CreateDefaultConfiguration()
        {
            return new TileConfiguration
            {
                TypeId = TeamCityAvailableBuildsTileController.TileTypeId,
                Id = Guid.NewGuid(),
                Tiles = new TileConfiguration[0]
            };
        }

        public void Register(IServices services)
        {
            services.TileProviderRegistry.Register(this);
        }
    }
}