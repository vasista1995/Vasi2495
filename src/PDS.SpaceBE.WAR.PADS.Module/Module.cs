using System.Diagnostics.CodeAnalysis;
using PDS.Common.Jobs;
using PDS.Core.Api.Inject;
using PDS.Core.Api.Modules;
using PDS.SpaceBE.WAR.PADS.Module.Data;

namespace PDS.SpaceBE.WAR.PADS.Module
{
    [ExcludeFromCodeCoverage]
    public sealed class Module : IModule
    {
        //private SpaceDataLoader _spaceQueueReader;

        /// <inheritdoc />
        public void Configure(IInjectionConfig config)
        {
            config.Add<IDistributedJob, SpaceDataLoader>(InjectionScopes.Singleton);
            config.Add<IPadsDao, PadsDao>(InjectionScopes.Singleton);
        }

        /// <inheritdoc />
        public void Start(IInjector injector)
        {
            //_spaceQueueReader = injector.GetInstance<SpaceDataLoader>();
        }

        /// <inheritdoc />
        public void Stop()
        {
            //_spaceQueueReader.Dispose();
        }
    }
}
