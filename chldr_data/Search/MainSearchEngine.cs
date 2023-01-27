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

namespace chldr_data.Search
{
    public class MainSearchEngine : SearchEngine
    {
        private readonly ILogger<IDataAccess> _logger;

        Expression<Func<Entities.Entry, bool>> StartsWithFilter(string inputText) => translation => translation.RawContents.Contains(inputText);
        Expression<Func<Entities.Entry, bool>> EntryFilter(string inputText) => entry => entry.RawContents.Contains(inputText);
        Expression<Func<Translation, bool>> TranslationFilter(string inputText) => entry => entry.RawContents.Contains(inputText);

        public MainSearchEngine(IDataAccess dataAccess, ILogger<IDataAccess> logger) : base(dataAccess)
        {
            _logger = logger;
        }

        public async Task FindAsync(string inputText)
        {
            var sw = Stopwatch.StartNew();

            inputText = inputText.Replace("1", "Ӏ")
                                 .ToLower();

            if (inputText.Length < 3)
            {
                await DirectSearch(inputText, StartsWithFilter(inputText), 50);
            }
            else if (inputText.Length >= 3)
            {
                await DirectSearch(inputText, EntryFilter(inputText), 100);

                await ReverseSearch(inputText, TranslationFilter(inputText), 100);
            }

            sw.Stop();
            _logger.LogInformation($"SW: FindAsync: {sw.ElapsedMilliseconds}");
        }
    }
}
