﻿using System.Threading.Tasks;
using TeamCitySharp.DomainEntities;


namespace NoeticTools.SystemsDashboard.Framework.DataSources.TeamCity
{
    internal interface ITeamCityChannel
    {
        string[] ProjectNames { get; }
        bool IsConnected { get; }
        void Connect();
        Task<Build> GetLastBuild(string projectName, string buildConfigurationName);
        Task<Build> GetLastSuccessfulBuild(string projectName, string buildConfigurationName);
        Task<Build> GetRunningBuild(string projectName, string buildConfigurationName, string branchName);
        Task<Build> GetRunningBuild(string projectName, string buildConfigurationName);
        Task<string[]> GetConfigurationNames(string projectName);
    }
}