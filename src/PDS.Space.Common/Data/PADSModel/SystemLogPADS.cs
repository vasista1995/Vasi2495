using Newtonsoft.Json;
using PDS.Common.E4AModel;
using PDS.Common.PADSModel;
using PDS.Queue.Api.Message;

namespace PDS.Space.Common.Data.PADSModel
{
    public class SystemLogPads : PadsSystemLog
    {
        /// <summary>
        /// Constructor is used for JSON deserialization
        /// </summary>
        [JsonConstructor]
        protected SystemLogPads()
        {
        }

        public SystemLogPads(IReadOnlyE4aSystemLog e4aSystemLog, IQueueMessage queueMessage) : base(e4aSystemLog, queueMessage)
        {
        }
    }
}
