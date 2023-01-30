using chldr_data.Entities;
using chldr_data.Enums;
using chldr_data.Services;
using chldr_data.Models;
using Realms;
using Realms.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using chldr_data.Interfaces;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using chldr_utils;
using chldr_utils.Services;
using chldr_utils.Models;

namespace chldr_data.Search
{
    public class MainSearchEngine : SearchEngine
    {
        Expression<Func<Entities.Entry, bool>> StartsWithFilter(string inputText) => translation => translation.RawContents.Contains(inputText);
        Expression<Func<Entities.Entry, bool>> EntryFilter(string inputText) => entry => entry.RawContents.Contains(inputText);
        Expression<Func<Translation, bool>> TranslationFilter(string inputText) => entry => entry.RawContents.Contains(inputText);

        public MainSearchEngine(IDataAccess dataAccess) : base(dataAccess){}

        public async Task FindAsync(string inputText, FiltrationFlags filtrationFlags)
        {
            var logger = new ConsoleService("GotNewSearchResults", false);
            logger.StartSpeedTest();

            inputText = inputText.Replace("1", "Ӏ").ToLower();

            if (inputText.Length < 3)
            {
                await DirectSearch(inputText, StartsWithFilter(inputText), 50);
            }
            else if (inputText.Length >= 3)
            {
                await DirectSearch(inputText, EntryFilter(inputText), 100);

                await ReverseSearch(inputText, TranslationFilter(inputText), 100);
            }
            logger.StopSpeedTest($"FindAsync finished");
        }
    }
}
