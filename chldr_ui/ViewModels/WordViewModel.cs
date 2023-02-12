using chldr_data.Models;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson;

namespace chldr_ui.ViewModels
{
    public class WordViewModel : EntryViewModelBase
    {
        public WordModel? Word { get; set; }

        public string? Header => Word?.Content;
        public string? Subheader => CreateSubheader();
        public void DeleteEntry()
        {
            ContentStore.DeleteEntry(Entry!.Id);
        }
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (Word == null && Entry != null)
            {
                Word = Entry as WordModel;

                Translations = Entry.Translations;
                Source = ParseSource(Entry.Source.Name);
            }
        }
        private string CreateSubheader()
        {
            if (Word?.NounDeclensions?.Count == 0 && Word?.VerbTenses?.Count == 0)
            {
                return null;
            }

            List<string> allForms = new List<string>();

            foreach (var item in Word!.VerbTenses.Values.Union(Word.NounDeclensions.Values))
            {
                if (item.Length > 0)
                {
                    allForms.Add(item);
                }
            }

            allForms.Remove(Word!.Content);

            if (allForms.Count == 0)
            {
                return null;
            }

            string part1 = String.Join(", ", allForms);

            string part2 = $" {chldr_data.Entities.Word.ParseGrammaticalClass(Word.GrammaticalClass)} ";

            return $"[ {part1}{part2}]";
        }
    }
}
