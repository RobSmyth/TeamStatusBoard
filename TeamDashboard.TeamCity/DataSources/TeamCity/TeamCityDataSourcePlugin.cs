﻿using System;
using NoeticTools.TeamStatusBoard.Framework;
using NoeticTools.TeamStatusBoard.Framework.Config;
using NoeticTools.TeamStatusBoard.Framework.Plugins;
using NoeticTools.TeamStatusBoard.Framework.Services;
using NoeticTools.TeamStatusBoard.Framework.Services.DataServices;
using NoeticTools.TeamStatusBoard.TeamCity.DataSources.TeamCity.Agents;
using NoeticTools.TeamStatusBoard.TeamCity.DataSources.TeamCity.Channel;
using NoeticTools.TeamStatusBoard.TeamCity.DataSources.TeamCity.Configurations;
using NoeticTools.TeamStatusBoard.TeamCity.DataSources.TeamCity.Projects;
using NoeticTools.TeamStatusBoard.TeamCity.DataSources.TeamCity.TcSharpInterop;
using TeamCitySharp;


namespace NoeticTools.TeamStatusBoard.TeamCity.DataSources.TeamCity
{
    public class TeamCityDataSourcePlugin : IPlugin
    {
        public int Rank => 90;

        public void Register(IServices services)
        {
            var channelStateBroadcaster = new ChannelConnectionStateBroadcaster(new EventBroadcaster(), new EventBroadcaster());
            var verySlowConnectedTicker = new ConnectedStateTicker(new EventBroadcaster(), services.Timer, TimeSpan.FromMinutes(10), channelStateBroadcaster);
            var slowConnectedTicker = new ConnectedStateTicker(new EventBroadcaster(), services.Timer, TimeSpan.FromMinutes(1), channelStateBroadcaster);
            var fastConnectedTicker = new ConnectedStateTicker(new EventBroadcaster(), services.Timer, TimeSpan.FromSeconds(30), channelStateBroadcaster);

            var dataSource = new DataRepositoryFactory().Create("TeamCity");
            var configuration = new DataSourceConfiguration(services.Configuration.Services.GetService("TeamCity"));
            var teamCityClient = new TcSharpTeamCityClient(new TeamCityClient(configuration.Url));

            var buildAgentRepository = GetBuildAgentRepository(services, fastConnectedTicker, teamCityClient, channelStateBroadcaster, slowConnectedTicker, dataSource);

            var buildConfigurationRepositoryFactory = new BuildConfigurationRepositoryFactory(teamCityClient, services, channelStateBroadcaster, verySlowConnectedTicker);
            var projectFactory = new ProjectFactory(buildConfigurationRepositoryFactory);
            var projectRepository = new ProjectRepository(teamCityClient, projectFactory, slowConnectedTicker);
            var stateEngine = new ChannelStateEngine(services, teamCityClient, projectRepository, buildAgentRepository, dataSource, configuration, channelStateBroadcaster);

            var conduit = new ConfigurationChangeListenerConduit();
            var tileProperties = new TileProperties(configuration, conduit, services);
            var channel = new TeamCityChannel(services, dataSource, configuration, stateEngine, buildAgentRepository, channelStateBroadcaster, tileProperties);
            conduit.SetTarget(channel);

            var teamCityDataService = new TeamCityDataService(channel, channelStateBroadcaster, fastConnectedTicker, projectRepository, buildAgentRepository);
            services.Register(teamCityDataService);

            services.DataService.Register(teamCityDataService.Name, dataSource);

            RegisterProjectsDataSource(services, projectRepository, dataSource);

            // todo - add data sources for agents
        }

        private static BuildAgentRepository GetBuildAgentRepository(IServices services, ConnectedStateTicker fastConnectedTicker, ITcSharpTeamCityClient teamCityClient, IChannelConnectionStateBroadcaster channelStateBroadcaster, IConnectedStateTicker slowConnectedTicker, IDataSource dataSource)
        {
            var buildAgentFactory = new BuildAgentViewModelFactory(services, dataSource, fastConnectedTicker, teamCityClient);
            var buildAgentRepository = new BuildAgentRepository(dataSource, teamCityClient, channelStateBroadcaster, slowConnectedTicker, buildAgentFactory);
            return buildAgentRepository;
        }

        private static void RegisterProjectsDataSource(IServices services, IProjectRepository projectRepository, IDataSource dataSource)
        {
            var dataService = new ProjectRepositoryDataService(dataSource, projectRepository);
            services.Register(dataService);
        }
    }
}