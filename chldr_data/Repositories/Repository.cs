using chldr_data.Dto;
using chldr_data.Entities;
using chldr_data.Factories;
using chldr_data.Interfaces;
using chldr_data.Interfaces.DatabaseEntities;
using chldr_data.Models;
using chldr_data.Models.Words;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Realms;
using System.Runtime.CompilerServices;

namespace chldr_data.Repositories
{
    public abstract class Repository
    {
        public IDataAccess DataAccess { get; }
        public Realm Database => DataAccess.GetActiveDataservice().GetDatabase();
        public Repository(IDataAccess dataAccess)
        {
            DataAccess = dataAccess;
        }
        protected void SetPropertyValue(object obj, string propertyName, object value)
        {
            var propertyInfo = obj.GetType().GetProperty(propertyName);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(obj, value);
            }
        }

        protected async Task Sync(List<ChangeSetModel>? changeSets = null)
        {
            var changeSetsToApply = changeSets;
            if (changeSetsToApply == null)
            {
                // TODO: Get latest changesets based on...?
            }

            Database.Write(() => {
           
                
                foreach (var changeSet in changeSetsToApply)
                {
                    var changes = JsonConvert.DeserializeObject<List<ChangeDto>>(changeSet.RecordChanges);
                    if (changes == null || changes.Count == 0)
                    {
                        continue;
                    }

                    // Apply changes to the local database
                    if (changeSet.RecordType == Enums.RecordType.Word)
                    {
                        try
                        {
                            var realmWord = Database.Find<RealmWord>(changeSet.RecordId);
                            if (realmWord == null)
                            {
                                throw new NullReferenceException();
                            }

                            foreach (var change in changes)
                            {
                                SetPropertyValue(realmWord, change.Property, change.NewValue);
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }


            });
        }
    }
}