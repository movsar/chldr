using dosham.Stores;

namespace dosham
{
    public partial class MainPage : ContentPage
    {
        private  ContentStore _contentStore;
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();
            _contentStore = this.Handler.MauiContext.Services.GetRequiredService<ContentStore>();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }

}
