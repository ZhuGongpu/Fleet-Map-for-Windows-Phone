using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using AVOSCloud;

namespace FleetMap
{
    /// <summary>
    ///     封装所需要的LeanCloud API
    /// </summary>
    internal class LeanCloudHelper
    {
        #region LeanCloud

        /// <summary>
        ///     上传到LeanCloud
        /// </summary>
        private async void SaveAVObject()
        {
            //create object
            var football = new AVObject("Sport");
            football["totalTime"] = 90;
            football["name"] = "Football";
            await football.SaveAsync();
        }

        /// <summary>
        ///     从LeadCloud中加载数据
        /// </summary>
        private async void QueryAVObject()
        {
            //query
            var query = new AVQuery<AVObject>("Sport");
            //.whereNear;//不能写为query.WhereXXX，必须和上局相连
            Debug.WriteLine("Start");
            await query.FindAsync().ContinueWith(t =>
            {
                var enumerator = t.Result.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Debug.WriteLine(enumerator.Current.ObjectId + " " + enumerator.Current["totalTime"]);
                }
            });
            Debug.WriteLine("End");
        }

        /// <summary>
        ///     根据markerId获取marker详情
        /// </summary>
        /// <param name="markerId"></param>
        /// <param name="callback"></param>
        public static async void QueryMarker(string markerId, Action<Task<IEnumerable<AVObject>>> callback)
        {
            var query = new AVQuery<AVObject>("Marker");
            await query.FindAsync().ContinueWith(callback);
        }

        /// <summary>
        ///     获取附近的markers
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="continuationAction"></param>
        public static async void RetrieveSurroungingMarkers(double latitude, double longitude,
            Action<Task<IEnumerable<AVObject>>> continuationAction)
        {
            //query
            var query = new AVQuery<AVObject>("Marker").WhereNear("location", new AVGeoPoint(latitude, longitude));

            Debug.WriteLine("Start Query");

            await query.FindAsync().ContinueWith(continuationAction);

            //TODO sample
            //await query.FindAsync().ContinueWith(t =>
            //{
            //    var enumerator = t.Result.GetEnumerator();
            //    while (enumerator.MoveNext())
            //    {
            //        Debug.WriteLine(enumerator.Current.ObjectId + " " + enumerator.Current["location"]);
            //    }
            //});
            Debug.WriteLine("End Query");
        }

        #endregion
    }
}