﻿using System;
using System.Collections.Generic;
using System.Linq;
using Atlassian.Jira;
using NoeticTools.TeamStatusBoard.Framework.Services.TimeServices;


namespace NoeticTools.TeamStatusBoard.DataSource.Jira.DataSource
{
    public class JiraChannelState : IJiraChannel
    {
        private const int MaxIssuesPerRequest = 100;
        private readonly string _username;
        private readonly string _password;
        private readonly string _url;
        private readonly IClock _clock;
        private readonly TimeCachedArray<JiraNamedEntity> _filters;
        private readonly TimeCachedArray<JiraNamedEntity> _projects;
        private readonly Dictionary<string, TimeCachedArray<Issue>> _filterItems;
        private Atlassian.Jira.Jira _jira;

        public JiraChannelState(string username, string password, string url, IClock clock)
        {
            _username = username;
            _password = password;
            _url = url;
            _clock = clock;
            _filters = new TimeCachedArray<JiraNamedEntity>(() => _jira.GetFilters(), TimeSpan.FromMinutes(5), clock);
            _projects = new TimeCachedArray<JiraNamedEntity>(() => _jira.GetProjects(), TimeSpan.FromMinutes(5), clock);
            _filterItems = new Dictionary<string, TimeCachedArray<Issue>>();
        }

        public JiraNamedEntity[] Filters => _filters.Items;

        public JiraNamedEntity[] Projects => _projects.Items;

        public void Connect()
        {
            //_jira = Atlassian.Jira.Jira.CreateSoapClient(_url, _username, _password);
            _jira = Atlassian.Jira.Jira.CreateRestClient(_url, _username, _password);
            _jira.MaxIssuesPerRequest = MaxIssuesPerRequest;
            _filters.Start();
            _projects.Start();
            foreach (var filterItem in _filterItems)
            {
                filterItem.Value.Start();
            }
        }

        public IEnumerable<CustomField> GetCustomFields()
        {
            return _jira.GetCustomFields();
        }

        public IEnumerable<Issue> GetIssuesFromFilter(string filterName)
        {
            if (!_filterItems.ContainsKey(filterName))
            {
                var reader = new JiraFilterIssuesReader(_jira, filterName);
                _filterItems.Add(filterName, new TimeCachedArray<Issue>(() => reader.GetIssuesFromFilter(), TimeSpan.FromSeconds(3), _clock));
            }
            return _filterItems[filterName].Items;
        }

        public IEnumerable<Issue> GetIssues(string projectName)
        {
            return _jira.Issues.Where(x => x.Project.Equals(projectName, StringComparison.InvariantCulture)).ToArray();
        }

        public IEnumerable<Issue> GetIssuesQuery(string query)
        {
            return _jira.GetIssuesFromJql(query).ToArray();
        }

        public void Disconnect()
        {
            _filters.Stop();
            _projects.Stop();
            foreach (var filterItem in _filterItems)
            {
                filterItem.Value.Stop();
            }
        }
    }
}