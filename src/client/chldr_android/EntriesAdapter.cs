namespace chldr_android
{
    using Android.Views;
    using Android.Widget;
    using AndroidX.RecyclerView.Widget;
    using core.DatabaseObjects.Models;
    using System.Collections.Generic;
    using System.Linq;

    public class EntriesAdapter : RecyclerView.Adapter
    {
        private List<EntryModel> _entries;

        public EntriesAdapter(List<EntryModel> entries)
        {
            _entries = entries;
            NotifyDataSetChanged();
        }
        public void UpdateEntries(List<EntryModel> newEntries)
        {
            _entries = newEntries;
            NotifyDataSetChanged();
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context)!.Inflate(Resource.Layout.phrases_exp_group, parent, false)!;
            return new EntryViewHolder(itemView);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var entry = _entries[position];
            var viewHolder = (EntryViewHolder)holder;

            viewHolder.Phrase.Text = entry.Header;
            viewHolder.Source.Text = entry.Subheader;
            if (entry.SubEntries.Any())
            {
                viewHolder.Forms.Text = $"[ {string.Join(", ", entry.SubEntries.Select(e => e.Content).ToArray())} ]";
            }
            else
            {
                viewHolder.Forms.Visibility = ViewStates.Gone;
            }

            viewHolder.TranslationsAdapter = new TranslationsAdapter(entry.Translations);
            viewHolder.TranslationsView.SetAdapter(viewHolder.TranslationsAdapter);

            // Set layout manager for nested RecyclerView
            if (viewHolder.TranslationsView.GetLayoutManager() == null)
            {
                viewHolder.TranslationsView.SetLayoutManager(new LinearLayoutManager(holder.ItemView.Context));
            }
        }

        public override int ItemCount => _entries.Count;

        class EntryViewHolder : RecyclerView.ViewHolder
        {
            public TextView Phrase { get; private set; }
            public TextView Source { get; private set; }
            public TextView Forms { get; private set; }
            public RecyclerView TranslationsView { get; private set; }
            public TranslationsAdapter TranslationsAdapter { get; set; }

            public EntryViewHolder(View itemView) : base(itemView)
            {
                Phrase = itemView.FindViewById<TextView>(Resource.Id.tvPhrase)!;
                Source = itemView.FindViewById<TextView>(Resource.Id.tvSource)!;
                Forms = itemView.FindViewById<TextView>(Resource.Id.tvForms)!;
                TranslationsView = itemView.FindViewById<RecyclerView>(Resource.Id.rvTranslations)!;
            }
        }
    }
}
