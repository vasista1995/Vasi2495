using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDS.SpaceNew.Common.Data.Config
{
    public class AttributeCalculationConfig
    {
        public List<List<ConditionalStatement>> AttributeAssignmentConfig { get; set; }
        public List<string> AttributesToDelete { get; set; }
    }
}
