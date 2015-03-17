using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Diagnostics;
using System.Windows;
using Windows.Devices.Geolocation;
using FleetMap.Models;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;

namespace FleetMap
{
    public partial class MainPage : PhoneApplicationPage
    {
        private Geolocator geolocator;

        public MainPage()
        {
            InitializeComponent();

            InitMapView();
            InitGeolocator();
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
            //RetrieveSurroungingMarkers(latitude, longitude, t =>
            //{
            //    var enumerator = t.Result.GetEnumerator();
            //    while (enumerator.MoveNext())
            //    {
            //        AVGeoPoint geoPoint = (AVGeoPoint) enumerator.Current["location"];
            //        Debug.WriteLine(enumerator.Current.ObjectId + " " + geoPoint);
            //    }
            //    //TODO load data into map

            //});
        }


        /// <summary>
        ///     Add a pushpin to the map
        /// </summary>
        private void AddMapOverlay(Geocoordinate geoCoordinate)
        {
            //TODO store the map layer

            //var pushpin = new Pushpin();

            ////creating a map overlayou and adding the pushpin to it
            //var mapOverlay = new MapOverlay();
            //mapOverlay.Content = pushpin;
            //mapOverlay.GeoCoordinate = new GeoCoordinate(geoCoordinate.Latitude, geoCoordinate.Longitude);
            //mapOverlay.PositionOrigin = new Point(0, 0.5);

            ////creating a map layer and adding the map overlayou to it
            //var mapLayer = new MapLayer();
            //mapLayer.Add(mapOverlay);
            //MapView.Layers.Add(mapLayer);
        }

        #endregion

        #region Location

        /// <summary>
        ///     Init and Configure Geo Locator
        /// </summary>
        private void InitGeolocator()
        {
            if (geolocator == null) geolocator = new Geolocator();
            geolocator.DesiredAccuracy = PositionAccuracy.High;
            geolocator.MovementThreshold = 100; //the units are meters
            geolocator.StatusChanged += GeolocatorOnStatusChanged;
            geolocator.PositionChanged += GeolocatorOnPositionChanged;
        }


        /// <summary>
        ///     Change Map View Center
        /// </summary>
        /// <param name="geoposition"></param>
        private void ChangeMapViewCenter(Geoposition geoposition)
        {
            MapView.Center = new GeoCoordinate(geoposition.Coordinate.Latitude, geoposition.Coordinate.Longitude);

            //加载markers
            AddMapOverlay(geoposition.Coordinate);
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
            if (geolocator == null)
                geolocator = new Geolocator();

            geolocator.DesiredAccuracy = PositionAccuracy.Default;
            geolocator.DesiredAccuracyInMeters = 50;

            try
            {
                Debug.WriteLine("locating...");
                var geoposition = await geolocator.GetGeopositionAsync(
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