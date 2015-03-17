using System.Windows.Controls;
using AVOSCloud;
using FleetMap.Models;

namespace FleetMap
{
    public partial class Pushpin : UserControl
    {
        private Marker _marker = null;

        public Pushpin(Marker marker)
        {
            this._marker = marker;

            InitializeComponent();

            //TODO 获取marker相关信息
            Content.Text = marker.Content;
        }
    }
}