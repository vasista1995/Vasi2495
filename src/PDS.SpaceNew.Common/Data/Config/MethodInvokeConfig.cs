using System.Collections.Generic;

namespace PDS.SpaceNew.Common.Data.Config
{
    public class MethodInvokeConfig
    {
        /// <summary>
        /// Syntax: Dictionary<InputParameterCount, Dictionary<MethodName, Dictionary<AssignmentParameterName, List<InputParameterNames>>>>
        /// </summary>
        public Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> InputParameterCountMethodInvokeMapping { get; set; }
    }
}
