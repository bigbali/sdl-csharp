using sdl_csharp.Model;
using sdl_csharp.Model.Entry;

namespace sdl_csharp.Utility
{
    public static class EntryProgressParser
    {
        public static void ParseOutput(Entry entry, string output)
        {
            if (output is null)
            {
                return;
            }

            const string DOWNLOAD = "[download]";
            if (output.StartsWith(DOWNLOAD)) {
                HandleDownload(entry, Clean(output, DOWNLOAD));
            }

            if (entry.data is IEntryPlaylistData)
            {
                const string FETCH = "[youtube]";
                if (output.StartsWith(FETCH))
                {
                    HandleFetchingNewItem(entry, Clean(output, FETCH));
                }

                const string PROGRESS = "[progress]";
                if (output.StartsWith(PROGRESS))
                {
                    HandleProgress(entry, Clean(output, PROGRESS));
                }

                const string POSTPROCESS = "[postprocess]";
                if (output.StartsWith(POSTPROCESS))
                {
                    HandlePostProcess(entry, Clean(output, POSTPROCESS));
                }
            }
        }

        static void HandleDownload(Entry entry, string output) {
            if (output.StartsWith("Downloading item"))
            {
                ((IEntryPlaylistData)entry.data).PlaylistDownloadIndex++;
            }
            if (output.StartsWith("Finished downloading playlist"))
            {
                entry.Status = EntryStatus.DONE;
            }
        }

        static void HandleFetchingNewItem(Entry entry, string output) {
            SetMemberState(entry, PlaylistMemberState.FETCHING);
        }

        static void HandleProgress(Entry entry, string output) {
            IEntryPlaylistData data = (IEntryPlaylistData)entry.data;
            string[] pairs = output.Split('|');

            foreach (var pair in pairs)
            {
                string[] kv = pair.Split(':');

                if (kv.Length == 2)
                {
                    string key = kv[0];
                    string value = kv[1];

                    switch (key)
                    {
                        case "STATUS":
                            PlaylistMemberState state = value switch
                            {
                                "downloading" => PlaylistMemberState.DOWNLOADING,

                                // when download is finished we immediately start converting
                                "finished" => PlaylistMemberState.CONVERTING,
                                _ => PlaylistMemberState.UNKNOWN
                            };

                            SetMemberState(entry, state);
                            break;
                        case "DOWNLOADED":
                            if (int.TryParse(value, out int downloadedBytes))
                                data.CurrentMember.DownloadedBytes = downloadedBytes;
                            break;
                        case "TOTAL":
                            if (int.TryParse(value, out int totalBytes))
                                data.CurrentMember.TotalBytes = totalBytes;
                            break;
                        case "ETA":
                            if (int.TryParse(value, out int eta))
                                data.CurrentMember.TimeLeft = eta;
                            break;
                        case "SPEED":
                            if (float.TryParse(value, out float speed))
                                data.CurrentMember.DownloadSpeed = speed;
                            break;
                        case "ELAPSED":
                            if (float.TryParse(value, out float elapsed))
                                data.CurrentMember.TimeElapsed = elapsed;
                            break;
                    }
                }
            }

            // set total playlist download percent
            if (data.CurrentMember is PlaylistMember member)
            {
                float totalPercent = Utility.Math.Percent(data.PlaylistMemberCount, data.PlaylistDownloadIndex, member.DownloadPercent);
                data.PlaylistOverallDownloadPercent = totalPercent;
            }
        }

        static void HandlePostProcess(Entry entry, string output)
        {
            string[] kv = output.Split(':');

            if (kv[0] == "POSTPROCESS_STATUS" && kv[1] == "finished")
            {
                SetMemberState(entry, PlaylistMemberState.DONE);
            }
        }

        static void SetMemberState(Entry entry, PlaylistMemberState state)
        {
            IEntryPlaylistData data = entry.data as IEntryPlaylistData;
            data.CurrentMember.State = state;
        }

        static string Clean(string str, string toRemove) => str.Replace(toRemove, string.Empty).Trim();
    }
}
