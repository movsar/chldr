namespace chldr_android
{
    using Android.Views;
    using Android.Widget;
    using AndroidX.RecyclerView.Widget;
    using chldr_data.DatabaseObjects.Models;
    using System.Collections.Generic;
    using System.Linq;

    public class EntriesAdapter : RecyclerView.Adapter
    {
        private List<EntryModel> _entries;

        public EntriesAdapter(List<EntryModel> entries)
        {
            _entries = entries;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.phrases_exp_group, parent, false);
            return new EntryViewHolder(itemView);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var entry = _entries[position];
            var viewHolder = holder as EntryViewHolder;

            viewHolder.Phrase.Text = entry.Content;
            viewHolder.TranslationsAdapter = new TranslationsAdapter(entry.Translations);
            viewHolder.TranslationsRecyclerView.SetAdapter(viewHolder.TranslationsAdapter);

            // Set layout manager for nested RecyclerView
            if (viewHolder.TranslationsRecyclerView.GetLayoutManager() == null)
            {
                viewHolder.TranslationsRecyclerView.SetLayoutManager(new LinearLayoutManager(holder.ItemView.Context));
            }
        }

        public override int ItemCount => _entries.Count;

        class EntryViewHolder : RecyclerView.ViewHolder
        {
            public TextView Phrase { get; private set; }
            public RecyclerView TranslationsRecyclerView { get; private set; }
            public TranslationsAdapter TranslationsAdapter { get; set; }

            public EntryViewHolder(View itemView) : base(itemView)
            {
                Phrase = itemView.FindViewById<TextView>(Resource.Id.tvPhrase);
                TranslationsRecyclerView = itemView.FindViewById<RecyclerView>(Resource.Id.rvTranslations);
            }
        }
    }
}
