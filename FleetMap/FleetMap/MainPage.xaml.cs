using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Diagnostics;
using System.Windows;
using Windows.Devices.Geolocation;
using AVOSCloud;
using FleetMap.Models;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;

namespace FleetMap
{
    public partial class MainPage : PhoneApplicationPage
    {
        /// <summary>
        ///     当前位置
        /// </summary>
        private Geoposition _currentGeoposition;

        private Geolocator _geolocator;

        public MainPage()
        {
            InitializeComponent();

            InitMapView();
            InitGeolocator();
        }

        /// <summary>
        /// 跳转到新建marker页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewMarker_OnClick(object sender, EventArgs e)
        {
            //当定位未完成时，不能跳转
            if (_currentGeoposition == null)
            {
                MessageBox.Show("请稍候，正在定位...");
                return;
            }


            // 传入当前坐标
            NavigationService.Navigate(
                new Uri(
                    "/NewMarkerPage.xaml?" + NewMarkerPage.ParamLatitude + "=" + _currentGeoposition.Coordinate.Latitude +
                    "&" +
                    NewMarkerPage.ParamLongitude + "=" + _currentGeoposition.Coordinate.Longitude
                    , UriKind.Relative));
        }

        #region Map

        private void InitMapView()
        {
            MapView.CartographicMode = MapCartographicMode.Road;
            MapView.ZoomLevel = 16;

            MapView.Pitch = 45; //倾斜角度

            MapView.LandmarksEnabled = true;
            MapView.PedestrianFeaturesEnabled = true;
        }

        /// <summary>
        ///     将附近的markers加载到地图上
        /// </summary>
        /// <param name="markers">附近的marker</param>
        private void LoadMarkersToMap(List<Marker> markers)
        {
            foreach (var marker in markers)
            {
                AddPushpin(marker);
            }
        }

        /// <summary>
        ///     根据mark信息向地图中添加一个pushpin
        /// </summary>
        /// <param name="marker"></param>
        private void AddPushpin(Marker marker)
        {
            var pushpin = new Pushpin(marker);

            //creating a map overlay and adding the pushpin to it.
            var mapOverlay = new MapOverlay();
            mapOverlay.Content = pushpin;
            mapOverlay.GeoCoordinate = new GeoCoordinate(marker.Location.Latitude, marker.Location.Longitude);
            mapOverlay.PositionOrigin = new Point(0, 0.5);

            //creating a map layer and adding the map overlay to it.
            var mapLayer = new MapLayer();
            mapLayer.Add(mapOverlay);
            MapView.Layers.Add(mapLayer);
        }

        #endregion

        #region Location

        /// <summary>
        ///     Init and Configure Geo Locator
        /// </summary>
        private void InitGeolocator()
        {
            if (_geolocator == null) _geolocator = new Geolocator();
            _geolocator.DesiredAccuracy = PositionAccuracy.High;
            _geolocator.MovementThreshold = 100; //the units are meters
            _geolocator.StatusChanged += GeolocatorOnStatusChanged;
            _geolocator.PositionChanged += GeolocatorOnPositionChanged;
        }


        /// <summary>
        ///     Change Map View Center
        /// </summary>
        /// <param name="geoposition"></param>
        private void ChangeMapViewCenter(Geoposition geoposition)
        {
            _currentGeoposition = geoposition;

            MapView.Center = new GeoCoordinate(geoposition.Coordinate.Latitude, geoposition.Coordinate.Longitude);

            //load nearby markers
            LeanCloudHelper.RetrieveSurroungingMarkers(geoposition.Coordinate.Latitude, geoposition.Coordinate.Longitude,
                task =>
                {
                    //load markers to map
                    var markers = new List<Marker>();
                    foreach (var avObjetct in task.Result)
                    {
                        //Debug.WriteLine("Keys:");
                        //foreach (var key in avObjetct.Keys)
                        //{
                        //    Debug.WriteLine(key);
                        //}
                        //Debug.WriteLine("---End Keys");
                        var markerId = avObjetct.ObjectId;
                        var content = "";
                        var type = "";
                        AVFile photo = null;
                        AVGeoPoint point;

                        if (avObjetct.ContainsKey(Marker.ParamContent))
                            content = avObjetct.Get<String>(Marker.ParamContent);
                        if (avObjetct.ContainsKey(Marker.ParamType))
                            type = avObjetct.Get<String>(Marker.ParamType);
                        if (avObjetct.ContainsKey(Marker.ParamPhoto))
                            photo = avObjetct.Get<AVFile>(Marker.ParamPhoto);
                        //一定存在位置信息，因此不需要检查
                        point = avObjetct.Get<AVGeoPoint>(Marker.ParamLocation);
                        markers.Add(new Marker(markerId, content, type, photo, point));
                    }

                    Dispatcher.BeginInvoke(() => { LoadMarkersToMap(markers); });
                });
        }

        /// <summary>
        ///     On Position Changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void GeolocatorOnPositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            var geoposition = args.Position;
            Debug.WriteLine("CurrentLocation:({0},{1})", geoposition.Coordinate.Latitude,
                geoposition.Coordinate.Longitude);
            //调整地图中心点位置
            Dispatcher.BeginInvoke(() => { ChangeMapViewCenter(geoposition); });
        }

        /// <summary>
        ///     On Status Changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void GeolocatorOnStatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            var status = "";

            switch (args.Status)
            {
                case PositionStatus.Disabled:
                    status = "location is disabled in phone settings";
                    break;
                case PositionStatus.Initializing:
                    status = "initializing";
                    break;
                case PositionStatus.NoData:
                    status = "no data";
                    break;
                case PositionStatus.Ready:
                    status = "ready";
                    break;
                case PositionStatus.NotAvailable:
                    status = "not available";
                    break;
                case PositionStatus.NotInitialized:
                    //the initial state of the geolocator, once the tracking operation is stopped by the user, the geolocator moves back to this state
                    break;
            }

            Debug.WriteLine("Status:{0}" + status);
        }

        /// <summary>
        ///     Get Current Location
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void GetCurrentLocation_OnClick(object sender, EventArgs e)
        {
            if (_geolocator == null)
                _geolocator = new Geolocator();

            _geolocator.DesiredAccuracy = PositionAccuracy.Default;
            _geolocator.DesiredAccuracyInMeters = 50;

            try
            {
                Debug.WriteLine("locating...");
                var geoposition = await _geolocator.GetGeopositionAsync(
                    TimeSpan.FromMinutes(5), //maximun age of cache
                    TimeSpan.FromSeconds(60) //time out
                    );

                if (geoposition == null || geoposition.Coordinate == null) return;

                var latitude = geoposition.Coordinate.Latitude;
                var longitude = geoposition.Coordinate.Longitude;

                Debug.WriteLine("CurrentLocation:({0},{1})", latitude, longitude);

                Dispatcher.BeginInvoke(() => { ChangeMapViewCenter(geoposition); });
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Location is disabled in phone settings");
            }
        }

        #endregion
    }
}