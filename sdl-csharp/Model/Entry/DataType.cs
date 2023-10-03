using sdl_csharp.Utility;
using System;
using System.Collections.Specialized;
using System.Web;
using static sdl_csharp.Model.Entry.Entry;

namespace sdl_csharp.Model.Entry
{
    public partial class Data : NotifyPropertyChanged
    {
        public static VideoType? GetType(string url, ref string vId, ref string pId)
        {
            Console.WriteLine("GETTYPE");
            try
            {
                UriBuilder uri = new(url);
                NameValueCollection queryParams = HttpUtility.ParseQueryString(uri.Query);

                if (Single(url, queryParams, ref vId))            return VideoType.SINGLE;
                if (Member(url, queryParams, ref vId, ref pId))   return VideoType.MEMBER;
                if (Playlist(url, queryParams, ref vId, ref pId)) return VideoType.PLAYLIST;
            }
            catch (Exception e)
            {
                Console.WriteLine($"DEBUG_GET_VIDEO_KIND_BY_URL_EXCEPTION: {e.Message}");
            }

            return null;
        }
        private static bool Single(string url, NameValueCollection queryParams, ref string vId)
        {
            if (url.StartsWith("https://www.youtube.com/watch"))
            {
                if (queryParams["v"] is not null && queryParams["list"] is null)
                {
                    vId = queryParams["v"];

                    return true;
                }
            }

            if (url.StartsWith("https://youtu.be"))
            {
                if (queryParams["v"] is null && queryParams["list"] is null)
                {
                    vId = url.Remove(0, "https://youtu.be/".Length);
                    return true;
                }
            }

            return false;
        }
        private static bool Member(string url, NameValueCollection queryParams, ref string vId, ref string pId)
        {
            if (url.StartsWith("https://www.youtube.com/watch"))
            {
                if (queryParams["v"] is not null
                    && queryParams["list"] is not null
                    && queryParams["index"] is not null)
                {
                    vId = queryParams["v"];
                    pId = queryParams["list"];

                    return true;
                }
            }

            if (url.StartsWith("https://youtu.be"))
            {
                if (queryParams["v"] is null && queryParams["list"] is not null)
                {
                    string vIdFirstPartRemoved = url.Remove(0, "https://youtu.be/".Length);
                    int vIdListParamStartIndex = vIdFirstPartRemoved.IndexOf('?'); // Presume ?list= starts at this point

                    vId = vIdFirstPartRemoved.Remove(vIdListParamStartIndex);
                    pId = queryParams["list"];

                    return true;
                }
            }

            return false;
        }

        private static bool Playlist(string url, NameValueCollection queryParams, ref string vId, ref string pId)
        {
            if (url.StartsWith("https://www.youtube.com/watch"))
            {
                if (queryParams["list"] is not null)
                {
                    Console.WriteLine("PLAYLIST");

                    pId = queryParams["list"];

                    return true;
                }
            }

            if (url.StartsWith("https://www.youtube.com/playlist")
                && queryParams["list"] is not null)
            {
                pId = queryParams["list"];
                return true;
            }

            return false;
        }
    }
}
