using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using AVOSCloud;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace FleetMap
{
    public partial class NewMarkerPage : PhoneApplicationPage
    {
        public const string ParamLatitude = "latitude";
        public const string ParamLongitude = "longitude";
        private Stream chosenPhoto;
        private string chosenPhotoName;
        private double latitude, longitude;

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

            // 解析当前坐标

            latitude = double.Parse(NavigationContext.QueryString[ParamLatitude]);
            longitude = double.Parse(NavigationContext.QueryString[ParamLongitude]);
        }

        private async void PostMarker_OnClick(object sender, EventArgs e)
        {
            //Post marker
            if (!string.IsNullOrEmpty(TextBox.Text))
            {
                //TODO Post

                Debug.WriteLine("POST Marker");

                var text = TextBox.Text;
                var avObject = new AVObject("Marker");
                if (chosenPhoto != null && chosenPhotoName != null)
                    avObject["photo"] = new AVFile(chosenPhotoName, chosenPhoto);
                avObject["content"] = text;
                avObject["type"] = "localTucao";

                Debug.WriteLine("saving...");
                await avObject.SaveAsync().ContinueWith(t =>                
                {
                    Debug.WriteLine("save done : " + t.IsFaulted);
                    //返回MainPage
                    Dispatcher.BeginInvoke(() => { NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative)); });
                });
            }
        }

        /// <summary>
        ///     选择照片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PickImage_OnClick(object sender, RoutedEventArgs e)
        {
            var photoChooserTask = new PhotoChooserTask();
            photoChooserTask.Completed += photoChooserTask_Completed;
            photoChooserTask.Show();
        }

        private void photoChooserTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                //Debug.WriteLine(e.OriginalFileName);
                Image.Source = new BitmapImage(new Uri(e.OriginalFileName));
                chosenPhotoName = e.OriginalFileName;

                chosenPhoto = e.ChosenPhoto;
            }
        }
    }
}