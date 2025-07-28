using System.Diagnostics.CodeAnalysis;
using PDS.Common.Jobs;
using PDS.Core.Api.Inject;
using PDS.Core.Api.Modules;
using PDS.SpaceBE.TIJ.Source.Module.Data;

namespace PDS.SpaceBE.TIJ.Source.Module
{
    /// <summary>
    /// The module reads continuously data from the Space Frontend Database of Regensburg and sends it to the Kafka Queue.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class Module : ConfigModule
    {
        /// <inheritdoc />
        public override void Configure(IInjectionConfig config)
        {
            config.Add<IDistributedJob, SpaceDataExtractor>(InjectionScopes.Singleton);
            config.Add<SpaceDao>(InjectionScopes.Singleton);
        }
    }
}
