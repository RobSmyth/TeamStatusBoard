using System.Threading.Tasks;
using NoeticTools.TeamStatusBoard.TeamCity.Plugins.DataSources.TeamCity.Agents;


namespace NoeticTools.TeamStatusBoard.TeamCity.Plugins.DataSources.TeamCity.Channel
{
    internal class ChannelStoppedState : ITeamCityChannelState
    {
        public ChannelStoppedState()
        {
            ProjectNames = new string[0];
            IsConnected = false;
        }

        public string[] ProjectNames { get; }
        public bool IsConnected { get; }

        public void Connect() {}

        public void Disconnect() {}

        public IBuildAgent[] GetAgents()
        {
            return new IBuildAgent[0];
        }

        public Task<IBuildAgent> GetAgent(string name)
        {
            return Task.Run(() => (IBuildAgent) null);
        }

        void ITeamCityChannelState.Leave() {}

        void ITeamCityChannelState.Enter() {}
    }
}