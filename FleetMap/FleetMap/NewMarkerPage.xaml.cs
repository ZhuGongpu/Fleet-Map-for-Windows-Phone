using System;
using System.Diagnostics;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace FleetMap
{
    public partial class NewMarkerPage : PhoneApplicationPage
    {
        public const string ParamLatitude = "latitude";
        public const string ParamLongitude = "longitude";

        public NewMarkerPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Debug.WriteLine("OnNavigateTo Params:");
            foreach (var entry in NavigationContext.QueryString)
            {
                Debug.WriteLine("{0} : {1}", entry.Key, entry.Value);
            }
            Debug.WriteLine("OnNavigateTo End");

            //TODO 解析当前坐标
        }

        private void PostMarker_OnClick(object sender, EventArgs e)
        {
            //TODO Post
            if (!string.IsNullOrEmpty(TextBox.Text))
            {
            }

            //返回MainPage
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}