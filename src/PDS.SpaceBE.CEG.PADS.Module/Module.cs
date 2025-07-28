using System.Diagnostics.CodeAnalysis;
using PDS.Common.Jobs;
using PDS.Core.Api.Inject;
using PDS.Core.Api.Modules;
using PDS.SpaceBE.CEG.PADS.Module.Data;

namespace PDS.SpaceBE.CEG.PADS.Module
{
    [ExcludeFromCodeCoverage]
    public sealed class Module : ConfigModule
    {
        //private SpaceDataLoader _spaceQueueReader;

        /// <inheritdoc />
        public override void Configure(IInjectionConfig config)
        {
            config.Add<IDistributedJob, SpaceDataLoader>(InjectionScopes.Singleton);
            config.Add<IPadsDao, PadsDao>(InjectionScopes.Singleton);
        }
    }
}
