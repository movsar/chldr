using Microsoft.AspNetCore.Components;

namespace chldr_ui.ViewModels
{
    public class DonationsPageViewModel : ViewModelBase
    {
        public int Amount { get; set; }
        public string SelectedItem { get; set; }

        new Dictionary<int, string> _itemsMap = new Dictionary<int, string>()
            {
                {2, "Siskal"},
                {4, "Chorpa"},
                {8, "Khingal"},
                {16, "Chepalg"},
                {32, "Chursh"},
                {64, "Vurgolsh"},
                {128, "celikom"},
                {256, "Vurgolash"},
                {512, "Siskal"},
                {1024, "Khingal"},
            };

        public void SetNewAmountChanged(ChangeEventArgs eventArgs)
        {
            int selectedValue = Convert.ToInt32(eventArgs.Value);

            if (!_itemsMap.ContainsKey(selectedValue))
            {
                foreach (var item in _itemsMap.Keys)
                {
                    if (selectedValue < item)
                    {
                        selectedValue = item;
                        break;
                    }
                }
                StateHasChanged();
            }

            Amount = selectedValue;
            SelectedItem = _itemsMap[Amount];
        }
    }
}
