using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FireXamarin.ViewModels;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;

namespace FireXamarin.Views
{
    public partial class BottomSheetView : PopupPage
    {
        BottomSheetViewModel _viewModel;
        public event EventHandler<ItemTappedEventArgs> SeletectItem;
        public Dictionary<string, object> Parameters { get; set; }

        public BottomSheetView(string title, List<BottomSheetItem> options)
        {
            InitializeComponent();
            _viewModel = new BottomSheetViewModel(title, options);
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ListItems.ItemTapped += ItemTapped;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ListItems.ItemTapped -= ItemTapped;
        }

        private void ItemTapped(object sender, ItemTappedEventArgs e)
        {
            _ = _viewModel.SelectItem(e.Item as BottomSheetItem);
            SeletectItem?.Invoke(sender, e);
        }

        protected new virtual Task OnAppearingAnimationEnd => Content.FadeTo(0.5);

        protected new virtual Task OnDisappearingAnimationBegin => Content.FadeTo(1);
    }
}
