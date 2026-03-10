using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace MusicPlayerMcp.Core
{
    public class FileUtils
    {
        const string FolderName = "MusicPlayerMcp";

        private static string GetMusicsFolder()
        {
            var path = Path.Combine(Path.GetTempPath(), FolderName);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

        public static bool TryGetFilePath(VideoId? videoId, out string? path)
        {
            path = null;

            if (videoId is null)
                return false;

            var files = Directory.GetFiles(GetMusicsFolder());

            if (files is null || files.Length == 0)
                return false;

            path = files.FirstOrDefault(c => c.Contains(videoId));
            return path is not null;
        }

        public static string GetFilePath(IStreamInfo streamInfo, VideoId? videoId)
        {
            if (streamInfo is null || videoId is null)
                return string.Empty;

            if (TryGetFilePath(videoId, out string? path))
                return path!;

            return Path.Combine(GetMusicsFolder(), $"{videoId}.{streamInfo.Container}");
        }
    }
}
