using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;

namespace FireXamarin.ViewModels
{
    public class BottomSheetViewModel : BaseViewModel
    {
        public ObservableCollection<BottomSheetItem> Options { get; set; }
        public object ExtraParams { get; set; }

        int _height;
        public int Height
        {
            get => _height;
            set => SetProperty(ref _height, value);
        }

        private bool _iconSpace;
        public bool IconSpace
        {
            get => _iconSpace;
            set => SetProperty(ref _iconSpace, value);
        }


        public BottomSheetViewModel(string title, List<BottomSheetItem> options)
        {
            Options = new ObservableCollection<BottomSheetItem>();
            PageName = title;
            foreach (var item in options)
            {
                if (!string.IsNullOrWhiteSpace(item.Icon))
                    IconSpace = true;
                Options.Add(item);
            }

            Height = Options.Count * 65;
        }

        public async Task SelectItem(BottomSheetItem item)
        {
            await Application.Current.MainPage.Navigation.PopPopupAsync();
        }
    }

    public class BottomSheetItem
    {
        public string Icon { get; set; }
        public string Name { get; set; }
    }
}
