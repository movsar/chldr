using Android.Views;
using AndroidX.RecyclerView.Widget;
using chldr_data.DatabaseObjects.Models;

namespace chldr_android
{
    public class TranslationsAdapter : RecyclerView.Adapter
    {
        private List<TranslationModel> _translations;

        public TranslationsAdapter(List<TranslationModel> translations)
        {
            _translations = translations;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.phrases_exp_child, parent, false);
            return new TranslationViewHolder(itemView);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var translation = _translations[position];
            var viewHolder = holder as TranslationViewHolder;
            viewHolder.Translation.Text = $"{translation.Content} ({translation.LanguageCode})";
        }

        public override int ItemCount => _translations.Count;

        class TranslationViewHolder : RecyclerView.ViewHolder
        {
            public TextView Translation { get; private set; }

            public TranslationViewHolder(View itemView) : base(itemView)
            {
                Translation = itemView.FindViewById<TextView>(Resource.Id.tvTranslation);
            }
        }
    }

}
