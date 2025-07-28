using System.Diagnostics.CodeAnalysis;
using PDS.Common.Jobs;
using PDS.Core.Api.Inject;
using PDS.Core.Api.Modules;
using PDS.SpaceBE.RBG.PADS.Module.Data;

namespace PDS.SpaceBE.RBG.PADS.Module
{
    [ExcludeFromCodeCoverage]
    public sealed class Module : ConfigModule
    {
        /// <inheritdoc />
        public override void Configure(IInjectionConfig config)
        {
            config.Add<IDistributedJob, SpaceDataLoader>(InjectionScopes.Singleton);
            config.Add<IPadsDao,PadsDao>(InjectionScopes.Singleton);
        }
    }
}
