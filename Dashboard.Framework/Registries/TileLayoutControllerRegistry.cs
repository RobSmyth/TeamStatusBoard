﻿using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using NoeticTools.Dashboard.Framework.Tiles;


namespace NoeticTools.Dashboard.Framework.Registries
{
    public class TileLayoutControllerRegistry : ITileLayoutControllerRegistry
    {
        private readonly ITileControllerFactory _tileFactory;
        private readonly IList<ITileLayoutController> _layoutControllers = new List<ITileLayoutController>();

        public TileLayoutControllerRegistry(ITileControllerFactory tileFactory)
        {
            _tileFactory = tileFactory;
        }

        public ITileLayoutController GetNew(Grid tileGrid)
        {
            var layoutController = new TileLayoutController(tileGrid, _tileFactory, this, new Thickness(0));
            _layoutControllers.Add(layoutController);
            return layoutController;
        }

        public ITileLayoutController[] GetAll()
        {
            return _layoutControllers.ToArray();
        }

        public int Count => _layoutControllers.Count;
    }
}