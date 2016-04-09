using System.Collections.Generic;
using System.Linq;
using NoeticTools.SystemsDashboard.Framework;


namespace NoeticTools.TeamStatusBoard.Framework.Registries
{
    public sealed class TileProviderRegistry : ITileProviderRegistry
    {
        private readonly IList<ITileControllerProvider> _providers = new List<ITileControllerProvider>();

        public IEnumerable<ITileControllerProvider> GetAll()
        {
            return _providers.ToArray();
        }

        public void Register(ITileControllerProvider provider)
        {
            _providers.Add(provider);
        }
    }
}