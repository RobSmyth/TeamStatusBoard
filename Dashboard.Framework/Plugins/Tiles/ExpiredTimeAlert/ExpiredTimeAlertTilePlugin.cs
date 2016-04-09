﻿using NoeticTools.SystemsDashboard.Framework.Plugins;
using NoeticTools.TeamStatusBoard.Framework.Services;


namespace NoeticTools.TeamStatusBoard.Framework.Plugins.Tiles.ExpiredTimeAlert
{
    public class ExpiredTimeAlertTilePlugin : IPlugin
    {
        public int Rank => 0;

        public void Register(IServices services)
        {
            services.TileProviders.Register(new ExpiredTimeAlertTileProvider(services.DashboardController, services));
        }
    }
}