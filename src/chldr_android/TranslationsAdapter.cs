using Android.Views;
using AndroidX.RecyclerView.Widget;
using core.DatabaseObjects.Models;

namespace chldr_android
{
    public class TranslationsAdapter : RecyclerView.Adapter
    {
        const string TranslationIndentation = "       ";
        private List<TranslationModel> _translations;

        public TranslationsAdapter(List<TranslationModel> translations)
        {
            _translations = translations;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context)!.Inflate(Resource.Layout.phrases_exp_child, parent, false)!;
            return new TranslationViewHolder(itemView);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var translation = _translations[position];
            var viewHolder = (TranslationViewHolder)holder;

            viewHolder.Translation.Text = $"{TranslationIndentation}{translation.Content}";
            viewHolder.LanguageCode.Text = translation.LanguageCode;

            if (!string.IsNullOrEmpty(translation.Notes))
            {
                viewHolder.Notes.Text = translation.Notes;
            }
            else
            {
                viewHolder.Notes.Visibility = ViewStates.Gone;
            }
        }

        public override int ItemCount => _translations.Count;

        class TranslationViewHolder : RecyclerView.ViewHolder
        {
            public TextView Translation { get; private set; }
            public TextView LanguageCode { get; private set; }
            public TextView Notes { get; private set; }

            public TranslationViewHolder(View itemView) : base(itemView)
            {
                Translation = itemView.FindViewById<TextView>(Resource.Id.tvTranslation)!;
                LanguageCode = itemView.FindViewById<TextView>(Resource.Id.tvExpChildLg)!;
                Notes = itemView.FindViewById<TextView>(Resource.Id.tvTranslationNotes)!;
            }
        }
    }

}
