using System;
using FireXamarin.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FireXamarin
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new ContactListPage()) { BarBackgroundColor = Color.FromHex("#F2F2F2") };
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
