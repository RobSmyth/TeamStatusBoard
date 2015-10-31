﻿using System;
using System.Xml.Serialization;
using Dashboard.Config;

namespace NoeticTools.Dashboard.Framework.Config
{
    [XmlType("dashboard")]
    public class DashboardConfiguration : IDashboardConfiguration
    {
        public DashboardConfiguration()
        {
            Name = string.Empty;
            DisplayName = string.Empty;
        }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "displayName")]
        public string DisplayName { get; set; }

        [XmlElement(ElementName = "root")]
        public DashboardTileConfiguration RootTile { get; set; }
    }
}