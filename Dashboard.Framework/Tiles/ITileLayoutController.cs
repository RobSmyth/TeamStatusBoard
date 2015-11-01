﻿using NoeticTools.Dashboard.Framework.Config;


namespace NoeticTools.Dashboard.Framework.Tiles
{
    public interface ITileLayoutController
    {
        void AddTile(DashboardTileConfiguration tileConfiguration);
        void Clear();
        void ToggleDisplayMode();
    }
}