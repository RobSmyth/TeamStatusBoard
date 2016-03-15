﻿using System.Windows;


namespace NoeticTools.SystemsDashboard.Framework.Dashboards
{
    public interface IDashboardController
    {
        void Start();
        void Stop();
        void ShowHelpPane();
        void ShowNavigationPane();
        void ShowOnSidePane(FrameworkElement viewController, string title);
        void Refresh();
        void ToggleGroupPanelsEditMode();
        void ShowInsertPanel();
    }
}