using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MongoDB.Driver.Linq;
using PDS.SpaceNew.Common;
using PDS.SpaceNew.Source.Module.Data.SpaceModel;

namespace PDS.SpaceNew.Source.Module
{
    /// <summary>
    /// Converts Space source records into E4A documents.
    /// </summary>
    public class SpaceEntryConverter
    {
        /// <summary>
        /// This method is to Convert the source record into e4a file to be published in the kafka producer.
        /// </summary>
        /// <param name="spaceDatabaseEntries"></param>
        /// <param name="siteKey"></param>
        /// <param name="lotAttributeKey"></param>
        public IEnumerable<SpaceEntry> Convert(IEnumerable<IDictionary<string, object>> spaceDatabaseEntries, string siteKey, Dictionary<string,string> keyValuePairs,string spaceInstanceName)
        {
            string lotAttributeKey = keyValuePairs["lotAttribute"];
            var spaceDatabaseEntryList = spaceDatabaseEntries.ToList();
            var validLotDatabaseEntries = GetValidLotDatabaseEntries(spaceDatabaseEntryList, lotAttributeKey).ToList();
            var cleanedDatabaseEntries = FilterEmptyProperties(validLotDatabaseEntries);
            var dataBaseEntries = FilterProperties(validLotDatabaseEntries,keyValuePairs); 
            var updatedDatabaseEntries = SpecifyDateTimeKindToUtc(cleanedDatabaseEntries).ToList();
            var groupedDictionaries = updatedDatabaseEntries.GroupBy(dictionary => GetSpaceEntryKey(dictionary, siteKey, dataBaseEntries, spaceInstanceName))
                                       .ToDictionary(group => group.Key, group => group.ToList());

            var spaceEntries = GetSpaceEntries(groupedDictionaries);
            return spaceEntries;
        }

        private Dictionary<string, string> FilterProperties(List<IDictionary<string, object>> propertyMappings, Dictionary<string, string> keyValuePairs)
        {
            var filteredProperties = new Dictionary<string, string>();

            foreach (var keyValue in keyValuePairs)
            {
                var property = propertyMappings
                    .SelectMany(propertyMapping => propertyMapping)
                    .FirstOrDefault(property => string.Equals(property.Key, keyValue.Value, StringComparison.OrdinalIgnoreCase) && property.Value is string);

                if (property.Value is string propertyValue)
                {
                    var filteredValue = property.Value.ToString() == "-" ? string.Empty : property.Value.ToString();
                    filteredProperties.Add(keyValue.Key, filteredValue);
                }
                else
                {
                    filteredProperties.Add(keyValue.Key, string.Empty);
                }
            }
            return filteredProperties;
        }
        public IEnumerable<IDictionary<string, object>> SpecifyDateTimeKindToUtc(IEnumerable<IDictionary<string, object>> spaceDatabaseEntries)
        {
            foreach (var spaceDatabaseEntry in spaceDatabaseEntries)
            {
                foreach (var attribute in spaceDatabaseEntry.ToList()) // ToList() to avoid modifying the collection while iterating
                {
                    if (attribute.Value is DateTime dateTime)
                    {
                        spaceDatabaseEntry[attribute.Key] = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
                    }
                }

                yield return spaceDatabaseEntry;
            }
        }

        private IEnumerable<IDictionary<string, object>> GetValidLotDatabaseEntries(List<IDictionary<string, object>> spaceDatabaseEntries, string lotAttributeKey)
        {
            // LotAttributeKey must exist, if not then it's fine if an exception is thrown here (Query/Code is then incorrect)
            var validLotDatabaseEntries = spaceDatabaseEntries.Where(e => e.ContainsKey(lotAttributeKey) &&
                                                                          e[lotAttributeKey] != null &&
                                                                          !string.IsNullOrWhiteSpace((string) e[lotAttributeKey]) &&
                                                                          !string.Equals((string) e[lotAttributeKey], "-", StringComparison.OrdinalIgnoreCase)
                                                                     );
            return validLotDatabaseEntries;
        }

        private IEnumerable<SpaceEntry> GetSpaceEntries(Dictionary<string, List<IDictionary<string, object>>> spaceEntryKeyRecordsMapping)
        {
            var spaceEntries = new List<SpaceEntry>();
            foreach (KeyValuePair<string, List<IDictionary<string, object>>> spaceEntryKeyRecordsPair in spaceEntryKeyRecordsMapping)
            {
                string spaceEntryKey = spaceEntryKeyRecordsPair.Key;
                var spaceEntryRecords = spaceEntryKeyRecordsPair.Value;
                var spaceEntry = new SpaceEntry(spaceEntryRecords, spaceEntryKey);
                spaceEntries.Add(spaceEntry);
            }

            return spaceEntries;
        }

        private string GetSpaceEntryKey(IDictionary<string, object> dictionary, string siteKey,Dictionary<string,string> keyValuePairs, string spaceInstanceName)
        {
            string primaryKey = dictionary.GetValueOrThrow(RequiredRawAttributes.PKey).ToString();
            return $"{siteKey}:{spaceInstanceName}:{keyValuePairs["lotAttribute"]}:{keyValuePairs["parameterFacility"]}:{keyValuePairs["parameterOper"]}:{primaryKey}";
        }

        private IEnumerable<IDictionary<string, object>> FilterEmptyProperties(List<IDictionary<string, object>> propertyMappings)
        {
            var dynamicList = new List<IDictionary<string, object>>();
            foreach (var propertyMapping in propertyMappings)
            {
                var sourceDataDictionary = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);

                foreach (var property in propertyMapping)
                {
                    var propertyValue = property.Value;
                    if (propertyValue != null && !string.IsNullOrWhiteSpace(propertyValue.ToString()) && propertyValue.ToString() != "-")
                    {
                        sourceDataDictionary[property.Key] = propertyValue;
                    }
                }

                dynamicList.Add(sourceDataDictionary);
            }

            return dynamicList;
        }
    }
}
