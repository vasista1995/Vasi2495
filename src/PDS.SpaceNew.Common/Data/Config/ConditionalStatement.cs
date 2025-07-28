using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDS.SpaceNew.Common.Data.Config
{
    public class ConditionalStatement
    {
        public string Condition { get; set; }
        public string Attribute { get; set; }
        public string Value { get; set; }
        public bool IsConstantValue { get; set; }
    }
}
