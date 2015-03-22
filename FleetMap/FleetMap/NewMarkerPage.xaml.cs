using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

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

        /// <summary>
        /// 选择照片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PickImage_OnClick(object sender, RoutedEventArgs e)
        {
            PhotoChooserTask photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += photoChooserTask_Completed;
            photoChooserTask.Show();
        }

        void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                //Debug.WriteLine(e.OriginalFileName);
                Image.Source = new BitmapImage(new Uri(e.OriginalFileName));
            }
        }
    }
}