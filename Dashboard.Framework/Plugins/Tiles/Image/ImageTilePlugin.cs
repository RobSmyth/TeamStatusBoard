﻿using System;
using System.Windows;
using NoeticTools.SystemsDashboard.Framework.Config;


namespace NoeticTools.SystemsDashboard.Framework.Plugins.Tiles.Image
{
    internal sealed class ImageTilePlugin : IPlugin, ITileControllerProvider
    {
        private readonly IDashboardController _dashboardController;
        private readonly IServices _services;
        private static readonly string TileTypeId = "Image";

        public ImageTilePlugin(IDashboardController dashboardController, IServices services)
        {
            _dashboardController = dashboardController;
            _services = services;
        }

        public int Rank => 0;

        public bool MatchesId(string id)
        {
            return id == TileTypeId;
        }

        public FrameworkElement CreateTile(TileConfiguration tile, TileLayoutController layoutController)
        {
            return new ImageTileControl { DataContext = new ImageViewModel(tile, _dashboardController, layoutController, _services) };
        }

        public TileConfiguration CreateDefaultConfiguration()
        {
            return new TileConfiguration
            {
                TypeId = TileTypeId,
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