using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using Com.AMap.Api.Maps;
using Com.AMap.Api.Maps.Model;
using Microsoft.Phone.Controls;

namespace FleetMap
{
    public partial class MainPage : PhoneApplicationPage
    {
        /// <summary>
        ///     用于标记当前定位精度圈
        /// </summary>
        private AMapCircle currentLocationCircle;

        /// <summary>
        ///     用于标记当前用户位置
        /// </summary>
        private AMapMarker currentLocationMarker;

        /// <summary>
        /// 用于定位
        /// </summary>
        private AMapGeolocator geolocator = null;


        private AMap map;
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            InitMap();
        }

        /// <summary>
        ///     初始化地图控件
        /// </summary>
        private void InitMap()
        {
            //加入地图控件
            map = new AMap();
            ContentPanel.Children.Add(map);
            ShowSatelliteMap();
            ShowTrafficMap();

            map.Loaded += OnMapLoaded;
            map.Unloaded += OnMapUnloaded;

            //添加Listener
            map.MarkerClickListener += ShowMarkerInfo;
        }

        private void OnMapUnloaded(object sender, RoutedEventArgs e)
        {
            if (geolocator != null)
            {
                geolocator.PositionChanged -= OnPositionChanged;
                geolocator.Stop();
            }
        }

        private async void OnMapLoaded(object sender, RoutedEventArgs e)
        {
            //TODO
            //AddMarker();


            //定位
            geolocator = new AMapGeolocator();
            geolocator.Start();//开启定位
            geolocator.PositionChanged += OnPositionChanged;

            //var location = await geolocator.GetGeopositionAsync();
            //Debug.WriteLine("CurrentLocation:{0}", location.CivicAddress);
        }

        /// <summary>
        ///     监听位置变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnPositionChanged(AMapGeolocator sender, AMapPositionChangedEventArgs args)
        {
            Debug.WriteLine("OnPositionChanged");

            Dispatcher.BeginInvoke(() =>
            {
                if (currentLocationMarker == null)
                {
                    //添加定位精度圈，为覆盖物中的圆 
                    currentLocationCircle = map.AddCircle(new AMapCircleOptions
                    {
                        Center = args.LngLat, //圆点位置 
                        Radius = (float) args.Accuracy, //半径 
                        FillColor = Color.FromArgb(80, 100, 150, 255),
                        StrokeWidth = 2, //边框粗细 
                        StrokeColor = Color.FromArgb(80, 0, 0, 255) //边框颜色 
                    });

                    currentLocationMarker = map.AddMarker(new AMapMarkerOptions
                    {
                        Position = args.LngLat, //图标的位置 
                        Title = "我的位置",
                        IconUri = new Uri("Images/current_location.png", UriKind.Relative), //图标的URL 
                        //Anchor = new Point(0.5, 0.5) //图标中心点 
                    });
                }
                else
                {
                    //更新当前位置
                    currentLocationMarker.Position = args.LngLat;
                    currentLocationCircle.Center = args.LngLat;
                    currentLocationCircle.Radius = (float) args.Accuracy;
                }
                //设置当前地图的经纬度和缩放级别 
                map.MoveCamera(CameraUpdateFactory.NewLatLngZoom(args.LngLat, 15));
                Debug.WriteLine("定位精度:{0}米", args.Accuracy);
                Debug.WriteLine("定位经纬度:{0}", args.LngLat);
            });
        }

        /// <summary>
        ///     显示卫星地图
        /// </summary>
        private void ShowSatelliteMap()
        {
            map.MapType = AMap.AMapType.Aerial;
        }

        /// <summary>
        ///     显示实时交通地图
        /// </summary>
        private void ShowTrafficMap()
        {
            map.TrafficEnabled = true;
        }

        /// <summary>
        ///     向地图中添加marker
        /// </summary>
        private void AddMarker()
        {
            //TODO 
            //map.AddMarker(new AMapMarkerOptions()
            //{
            //    Position = map.Center,
            //    Title = "Title",
            //    Snippet = "Snippet",
            //    IconUri = new Uri("Images/baidu.png", UriKind.Relative)
            //});

            
        }

        /// <summary>
        ///     显示/隐藏弹出信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ShowMarkerInfo(AMapMarker sender, AMapEventArgs args)
        {
            if (sender.IsInfoWindowShown())
            {
                //隐藏弹出信息
                sender.HideInfoWindow();
            }
            else
            {
                //显示弹出信息
                MarkerInfo info = new MarkerInfo();
                sender.ShowInfoWindow(info, new Point(0, 0));
            }
        }
    }
}