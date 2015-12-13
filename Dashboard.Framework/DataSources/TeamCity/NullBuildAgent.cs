﻿namespace NoeticTools.SystemsDashboard.Framework.DataSources.TeamCity
{
    public class NullBuildAgent : IBuildAgent
    {
        public NullBuildAgent(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public BuildAgentStatus Status => BuildAgentStatus.Unknown;
        public bool IsRunning => false;
    }
}