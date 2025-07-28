using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PDS.Common.Utils;
using PDS.Space.Common.Data.PADSModel;
using PDS.SpaceNew.Common;
using PDS.SpaceNew.Common.Data.Config;
using PDS.SpaceNew.PADS.Module.Data.PADSModel;
using BaseData1ListRawValuesPads = PDS.SpaceNew.PADS.Module.Data.PADSModel.BaseData1ListRawValuesPads;

namespace PDS.SpaceNew.PADS.Module.Aggregations
{
    internal class SpaceBaseAggregation
    {
        private readonly string _siteKey;

        public SpaceBaseAggregation(string siteKey)
        {
            _siteKey = siteKey;
        }

        protected void AddBaseProperties<T>(IDictionary<string, object> dataFlatMetaData, SpaceE4A renamedAttributesE4A)
        {
            var baseFlatMetaDataPropertyNames = PropertyHelper.GetPropertyNames<T>();
            foreach (string propertyName in baseFlatMetaDataPropertyNames)
            {
                if (renamedAttributesE4A.SpaceAttributes.TryGetValue(propertyName, out object propertyValue))
                {
                    dataFlatMetaData[propertyName] = propertyValue;
                }
            }
        }

        public static void SetPropertiesFromDictionary<T>(T targetObject, IDictionary<string, object> attributeMapping)
        {
            var type = targetObject.GetType();
            var properties = type.GetProperties();

            var existingAttributeProperties = properties.Where(property => attributeMapping.ContainsKey(property.Name) && property.CanWrite);
            foreach (var attributeProperty in existingAttributeProperties)
            {
                string propertyName = attributeProperty.Name;
                object newPropertyValue = attributeMapping[propertyName];

                var propertyType = attributeProperty.PropertyType;
                if (newPropertyValue == null)
                {
                    attributeProperty.SetValue(targetObject, null);
                }
                else if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    // Handle nullable types
                    var underlyingType = Nullable.GetUnderlyingType(propertyType);
                    object value = Convert.ChangeType(newPropertyValue, underlyingType);
                    attributeProperty.SetValue(targetObject, value);
                }
                else
                {
                    object value = Convert.ChangeType(newPropertyValue, propertyType);
                    attributeProperty.SetValue(targetObject, value);
                }
            }
        }

        /// <summary>
        /// calculates measurement aggregates
        /// </summary>
        /// <param name="measurementRawValues"></param>
        /// <param name="violationRawValues">violationRawValues are the raw values of the e4adocument from the filesystem</param>
        /// <returns>returns the aggregates to the measurementaggregates subdocument</returns>
        public MeasurementAggregatesPads CreateMeasurementAggregates(int exeCount, IEnumerable<BaseData1ListRawValuesPads> measurementRawValues, IEnumerable<BaseData1ListRawValuesPads> violationRawValues)
        {
            var measurementValues = measurementRawValues.Select(it => it.Value).ToList();
            var samples = measurementRawValues.Select(it => it.SampleId).Distinct().ToList();
            var measurementAggregates = new MeasurementAggregatesPads
            {
                BaseCount = measurementValues.Count,
                ExecCount = exeCount,
                FlaggedCount = exeCount - measurementValues.Count
            };

            if (measurementValues.Count > 0)
            {
                measurementAggregates.Min = measurementValues.Min();
                measurementAggregates.Max = measurementValues.Max();
                measurementAggregates.Mean = measurementValues.Average();
                measurementAggregates.Sigma = GetSigma(measurementValues);
                measurementAggregates.Range = measurementAggregates.Max - measurementAggregates.Min;
                measurementAggregates.Q2 = AggregationUtils.Percentile(measurementValues, 0.02);
                measurementAggregates.Q5 = AggregationUtils.Percentile(measurementValues, 0.05);
                measurementAggregates.Q25 = AggregationUtils.Percentile(measurementValues, 0.25);
                measurementAggregates.Median = AggregationUtils.Percentile(measurementValues, 0.5);
                measurementAggregates.Q75 = AggregationUtils.Percentile(measurementValues, 0.75);
                measurementAggregates.Q95 = AggregationUtils.Percentile(measurementValues, 0.95);
                measurementAggregates.Q98 = AggregationUtils.Percentile(measurementValues, 0.98);
            }

            measurementAggregates.Samples = samples.Count > 0 ? string.Join(", ", samples) : null;

            var primaryViolations = violationRawValues
                .Select(it => it.PrimaryViolation).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
            measurementAggregates.PrimaryViolation = primaryViolations.Count > 0 ? string.Join(", ", primaryViolations) : null;

            var primaryViolComments = violationRawValues
                .Select(it => StringUtils.Normalize(it.PrimaryViolationComments, _siteKey)).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
            measurementAggregates.PrimaryViolationComments = primaryViolComments.Count > 0 ? string.Join(", ", primaryViolComments) : null;

            var violationComments = violationRawValues
                .Select(it => StringUtils.Normalize(it.ViolationComments, _siteKey)).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
            measurementAggregates.ViolationComments = violationComments.Count > 0 ? string.Join(", ", violationComments) : null;

            var violationList = GetViolationList(violationRawValues);
            measurementAggregates.ViolationList = violationList.Count > 0 ? string.Join(", ", violationList) : null;
            measurementAggregates.NumViolations = violationList.Count;
            measurementAggregates.Samples = string.Join(", ", samples.Distinct());
            return measurementAggregates;
        }

        protected void AddGofAggregates(MeasurementAggregatesPads aggregates, IEnumerable<IData1ListRawValuesPadsGof> rawValues)
        {
            if (!rawValues.Any())
                return;

            var gofValues = new List<double>();
            foreach (var e4aRawValues in rawValues)
            {
                if (e4aRawValues.GOF != null)
                {
                    gofValues.Add(SpaceAggregationUtilsLocally.DoubleParse(e4aRawValues.GOF));
                }
            }
            if (gofValues.Count > 0)
            {
                aggregates.GofMin = gofValues.Min();
                aggregates.GofMax = gofValues.Max();
                aggregates.GofMean = gofValues.Average();
            }
        }

        private double GetSigma(IList<double> values)
        {
            double sigma = 0.0;
            if (values.Count > 1)
            {
                double sum = values.Sum(d => (d - values.Average()) * (d - values.Average()));
                sigma = Math.Sqrt(sum / values.Count);
            }
            return sigma;
        }

        protected List<string> GetViolationList(IEnumerable<BaseData1ListRawValuesPads> rawvalues)
        {
            var violationList = new List<string>();
            var filteredRawValues = rawvalues.Where(r => r.ViolationList != null);
            foreach (var rawvalue in filteredRawValues)
            {
                violationList.AddRange(rawvalue.ViolationList.Split(","));
            }

            var uniqueViolations = violationList.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
            return uniqueViolations;
        }

        protected string GetId(string methodName, IdCreationConfig spaceIdCreationConfig,
            Dictionary<string, object> parameterValueDictionary, IDictionary<string, object> spaceAttributes)
        {
            if (!spaceIdCreationConfig.IdCreationMappings.ContainsKey(methodName))
                throw new ArgumentException($"{methodName} is not specified in config!");

            var allCreationParts = spaceIdCreationConfig.IdCreationMappings[methodName].Parts;
            var constantCreationParts = spaceIdCreationConfig.IdCreationMappings[methodName].ConstantParts;
            var creationPartsToAdd = allCreationParts.Except(constantCreationParts, StringComparer.OrdinalIgnoreCase).ToList();

            AddFlexibleIdPartsToDictionary(parameterValueDictionary, spaceAttributes, creationPartsToAdd);
            string id = CreateId(allCreationParts, constantCreationParts, parameterValueDictionary);
            return id;
        }

        private string CreateId(List<string> allCreationParts, List<string> constantCreationParts,
            Dictionary<string, object> parameterValueDictionary)
        {
            var stringBuilder = new StringBuilder();
            foreach (string creationPart in allCreationParts)
            {
                if (constantCreationParts.Contains(creationPart, StringComparer.OrdinalIgnoreCase))
                {
                    stringBuilder.Append($"{creationPart}:");
                }
                else if (parameterValueDictionary.ContainsKey(creationPart))
                {
                    stringBuilder.Append($"{parameterValueDictionary[creationPart]}:");
                }
                else
                {
                    stringBuilder.Append(":");
                }
            }

            if (stringBuilder.Length > 0)
            {
                stringBuilder.Length -= 1;
            }

            return stringBuilder.ToString();
        }

        private void AddFlexibleIdPartsToDictionary(IDictionary<string, object> parameterValueDictionary,
            IDictionary<string, object> spaceAttributes, List<string> creationPartsToAdd)
        {
            foreach (string creationPartToAdd in creationPartsToAdd)
            {
                if (spaceAttributes.ContainsKey(creationPartToAdd))
                {
                    if (!parameterValueDictionary.ContainsKey(creationPartToAdd))
                        parameterValueDictionary.Add(creationPartToAdd, spaceAttributes[creationPartToAdd]);
                    else
                        parameterValueDictionary[creationPartToAdd] = spaceAttributes[creationPartToAdd];
                }
            }
        }
    }
}
