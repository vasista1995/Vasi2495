using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using PDS.Common.Utils;
using PDS.Core.Api.Utils;
using PDS.Queue.Api.Message;
using PDS.Space.Common.Data.E4AModel;
using PDS.Space.Common.Data.PADSModel;

namespace PDS.Space.Common.Aggregations
{
    /// <summary>
    /// Basic Lot / wafer aggregation
    /// </summary>
    public class BaseAggregation
    {
        public BaseAggregation([NotNull] BaseSpaceE4A e4aDocument, [NotNull] IQueueMessage message)
        {
            Message = Ensure.NotNull(message, nameof(message));
            E4aDocument = Ensure.NotNull(e4aDocument, nameof(e4aDocument));
        }

        public IQueueMessage Message { get; }

        public BaseSpaceE4A E4aDocument { get; }

        protected SystemLogPads CreateSystemLog()
        {
            return new SystemLogPads(E4aDocument.SystemLog, Message)
            {
                DocCreatedTimestampUtc = DateTime.UtcNow,
            };
        }

        protected SystemLogPads UpdateSystemLog(SystemLogPads padsSystemLog)
        {
            var log = new SystemLogPads(E4aDocument.SystemLog, Message)
            {
                DocUpdatedTimestampUtc = DateTime.UtcNow
            };
            log.DocCreatedTimestampUtc = padsSystemLog != null ? padsSystemLog.DocCreatedTimestampUtc : log.DocUpdatedTimestampUtc.Value;

            return log;
        }

        protected internal static MeasurementAggregatesPads CreateMeasurementAggregates(int execCount,
            IEnumerable<BaseData1ListRawValuesPads> measurementRawValues, IEnumerable<BaseData1ListRawValuesPads> violationRawValues)
        {
            var measurementValues = measurementRawValues.Select(it => it.Value).ToList();
            var samples = measurementRawValues.Select(it => it.SampleId).Distinct().ToList();
            var measurementAggregates = new MeasurementAggregatesPads
            {
                BaseCount = measurementValues.Count,
                ExecCount = execCount,
                FlaggedCount = execCount - measurementValues.Count
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

            var primaryViolComm = violationRawValues
                .Select(it => StringUtils.Normalize(it.PrimaryViolationComments, "RBG")).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
            measurementAggregates.PrimaryViolationComments = primaryViolComm.Count > 0 ? string.Join(", ", primaryViolComm) : null;

            var violationComments = violationRawValues
                .Select(it => StringUtils.Normalize(it.ViolationComments, "RBG")).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
            measurementAggregates.ViolationComments = violationComments.Count > 0 ? string.Join(", ", violationComments) : null;

            var violationList = GetViolationList(violationRawValues);
            measurementAggregates.ViolationList = violationList.Count > 0 ? string.Join(", ", violationList) : null;
            measurementAggregates.NumViolations = violationList.Count;
            measurementAggregates.Samples = string.Join(", ", samples.Distinct());
            return measurementAggregates;
        }

        protected internal static IList<string> GetViolationList(IEnumerable<BaseData1ListRawValuesPads> rawValues)
        {
            var violationList = new List<string>();
            foreach (var e4aRawValues in rawValues)
            {
                if (e4aRawValues.ViolationList != null)
                {
                    violationList.AddRange(e4aRawValues.ViolationList.Split(","));
                }
            }
            return violationList.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
        }

        protected internal static double GetSigma(IList<double> values)
        {
            double sigma = 0.0;
            if (values.Count > 1)
            {
                double sum = values.Sum(d => (d - values.Average()) * (d - values.Average()));
                sigma = Math.Sqrt(sum / values.Count);
            }
            return sigma;
        }
    }
}
