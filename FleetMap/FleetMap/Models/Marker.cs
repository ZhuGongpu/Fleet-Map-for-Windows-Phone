using AVOSCloud;

namespace FleetMap.Models
{
    /// <summary>
    ///     Marker实体类
    /// </summary>
    public class Marker
    {
        public static readonly string ParamObjectId = "objetId";
        public static readonly string ParamPeopleList = "peoplelist";
        public static readonly string ParamACL = "ACL";
        public static readonly string ParamContent = "content";
        public static readonly string ParamCommentList = "commnetlist";
        public static readonly string ParamType = "type";
        public static readonly string ParamPhoto = "photo";
        public static readonly string ParamLocation = "location";
        public static readonly string ParamUser = "user";
        public static readonly string ParamCreatedAt = "createdAt";
        public static readonly string ParamUpdatedAt = "updatedAt";

        public Marker(string markerId, string content, string type, AVFile photo, AVGeoPoint location)
        {
            MarkerId = markerId;
            Content = content;
            Type = type;
            Photo = photo;
            Location = location;
        }

        //TODO 添加其他数据

        public string MarkerId { get; private set; }
        public string Content { get; private set; }
        public string Type { get; private set; }
        public AVFile Photo { get; private set; }
        public AVGeoPoint Location { get; private set; }
    }
}