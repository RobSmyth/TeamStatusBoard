using NoeticTools.TeamStatusBoard.DataSource.TeamCity.DataSources.TeamCity.Projects;


namespace NoeticTools.TeamStatusBoard.DataSource.TeamCity.DataSources.TeamCity.Configurations
{
    public interface IBuildConfigurationRepositoryFactory
    {
        IBuildConfigurationRepository Create(IProject project);
    }
}