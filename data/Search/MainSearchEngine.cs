using Data.Entities;
using Data.Enums;
using Data.Services;
using Data.Models;
using Realms;
using Realms.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Data.Interfaces;

namespace Data.Search
{
    internal class MainSearchEngine : SearchEngine
    {
        Expression<Func<Entities.Entry, bool>> StartsWithFilter(string inputText) => translation => translation.RawContents.Contains(inputText);
        Expression<Func<Entities.Entry, bool>> EntryFilter(string inputText) => entry => entry.RawContents.Contains(inputText);
        Expression<Func<Translation, bool>> TranslationFilter(string inputText) => entry => entry.RawContents.Contains(inputText);

        static string PreviousInputText;
        static Dictionary<int, IQueryable<Microsoft.Maui.Controls.Entry>> PreviousResults = new Dictionary<int, IQueryable<Microsoft.Maui.Controls.Entry>>();
        public MainSearchEngine(DataAccess dataAccess) : base(dataAccess) { }

        public async Task FindAsync(string inputText)
        {
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
        }
    }
}
