using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using PDS.SpaceNew.Common;
using PDS.SpaceNew.Common.Data.Config;
using SharpCompress.Common;
using NCalc;
using System.Text.RegularExpressions;

namespace PDS.SpaceNew.Source.Module
{
    internal class AttributeChanger
    {
        internal void AddCalculatedAttributes(SpaceE4A spaceE4A, AttributeCalculationConfig config)
        {
            foreach (List<ConditionalStatement> statementBlock in config.AttributeAssignmentConfig)
            {
                foreach (ConditionalStatement statement in statementBlock)
                {
                    string replacedCondition = ReplaceVariablesInCondition(statement.Condition, spaceE4A.SpaceAttributes);
                    if (replacedCondition == null)
                    {
                        continue; // This will skip to the next iteration of the loop
                    }
                    var expression = new NCalc.Expression(replacedCondition);

                    // Add the metadata attributes dictionary as a parameter to the expression
                    expression.Parameters["entry"] = spaceE4A.SpaceAttributes;

                    var result = expression.Evaluate();
                    if ((bool) result)
                    {
                        if (statement.IsConstantValue)
                        {
                            AddEntryToDictionary(spaceE4A.SpaceAttributes, statement.Attribute, statement.Value);
                        }
                        else if (spaceE4A.SpaceAttributes.ContainsKey(statement.Value))
                        {
                            AddEntryToDictionary(spaceE4A.SpaceAttributes, statement.Attribute, spaceE4A.SpaceAttributes[statement.Value]);
                        }
                        else
                        {
                            throw new ArgumentException($"{statement.Value} was not found in dictionary");
                        }
                    }
                }
            }
        }

        private string ReplaceVariablesInCondition(string condition, IDictionary<string, object> spaceAttributes)
        {
            var placeholders = Regex.Matches(condition, @"\{([^}]+)\}");
            var relevantKeys = placeholders.Cast<Match>().Select(m => m.Groups[1].Value);

            if (!relevantKeys.All(spaceAttributes.ContainsKey))
            {
                var missingKeys = relevantKeys.Except(spaceAttributes.Keys);
                return null;
                //throw new ArgumentException($"Variables '{string.Join(", ", missingKeys)}' not found in dictionary");
            }

                foreach (string key in relevantKeys)
                {
                    condition = condition.Replace("{" + key + "}", spaceAttributes[key].ToString());
                }

            return condition;
        }

        private void AddEntryToDictionary(IDictionary<string, object> spaceAttributes, string attributeName, object value)
        {
            if (value != null && value.ToString() == "null")
            {
                value = null;
            }

            if (spaceAttributes.ContainsKey(attributeName))
            {
                spaceAttributes[attributeName] = value;
            }
            else
            {
                spaceAttributes.Add(attributeName, value);
            }
        }

        internal void RemoveAttributes(SpaceE4A spaceE4A, AttributeCalculationConfig spaceAttributeCalculationConfig)
        {
            foreach (string attributeToDelete in spaceAttributeCalculationConfig.AttributesToDelete)
            {
                if (spaceE4A.SpaceAttributes.ContainsKey(attributeToDelete))
                    spaceE4A.SpaceAttributes.Remove(attributeToDelete);
            }
        }
    }
}
