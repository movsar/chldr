﻿using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace chldr_data.Models
{
    public class Change
    {
        public string Property { get; set; }
        public dynamic OldValue { get; set; }
        public dynamic NewValue { get; set; }

        public static List<Change> GetChanges<T>(T updated, T existing)
        {
            // This method compares the two dto's and returns the changed properties with their names and values

            var changes = new List<Change>();

            // Get all properties except for the class typed fields, i.e. references to other objects
            var properties = typeof(T).GetProperties()
                                      .Where(p => !p.PropertyType.IsClass || p.PropertyType == typeof(string));

            foreach (var property in properties)
            {
                // Get currenta and old values, use empty string if they're null
                var newValue = property.GetValue(updated) ?? "";
                var oldValue = property.GetValue(existing) ?? "";

                // ! Serialization allows comparision between complex objects, it might slow down the process though and worth reconsidering
                if (!Equals(JsonConvert.SerializeObject(newValue), JsonConvert.SerializeObject(oldValue)))
                {
                    changes.Add(new Change()
                    {
                        Property = property.Name,
                        OldValue = oldValue,
                        NewValue = newValue,
                    });
                }
            }

            return changes;
        }
    }
}
