using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using FleetMap.Models;

namespace FleetMap
{
    public partial class Pushpin : UserControl
    {
        private Marker _marker;

        public Pushpin(Marker marker)
        {
            _marker = marker;

            InitializeComponent();

            //TODO 获取marker相关信息
            MarkerText.Text = marker.Content;
            //if (marker.Photo != null && marker.Photo.Url != null)
            //    MarkerImage.Source = new BitmapImage(marker.Photo.Url);
            //else
            //{
            //    MarkerImage.Visibility = Visibility.Collapsed; //隐藏image
            //}
        }
    }
}