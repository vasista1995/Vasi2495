using Newtonsoft.Json;
using PDS.Common.E4AModel;
using PDS.Common.ExtractionLog;
using PDS.Common.Source;

namespace PDS.Space.Common.Data.E4AModel
{
    public class BaseSystemLogE4A : E4ASystemLog
    {
        /// <summary>
        /// Constructor is used for JSON deserialization
        /// </summary>
        [JsonConstructor]
        private BaseSystemLogE4A()
        {
        }

        public BaseSystemLogE4A(ISourceRecord sourceRecord, IExtractionJobRun jobRun)
            : base(sourceRecord, jobRun)
        {
        }
    }
}
