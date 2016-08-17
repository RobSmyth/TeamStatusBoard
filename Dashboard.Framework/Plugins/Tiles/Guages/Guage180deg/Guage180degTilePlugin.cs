﻿using NoeticTools.TeamStatusBoard.Framework.Services;


namespace NoeticTools.TeamStatusBoard.Framework.Plugins.Tiles.Guages.Guage180deg
{
    public sealed class Guage180degTilePlugin : IPlugin
    {
        public int Rank => 0;

        public void Register(IServices services)
        {
            services.TileProviders.Register(new Guage180degTileProvider(services.DashboardController, services));
        }
    }
}