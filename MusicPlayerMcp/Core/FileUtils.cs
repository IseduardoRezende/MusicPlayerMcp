using YoutubeExplode.Videos.Streams;

namespace MusicPlayerMcp.Core
{
    public class FileUtils
    {
        const string FolderName = "MusicPlayerMcp";

        public static string GetMusicsFolder()
        {
            var path = Path.Combine(Path.GetTempPath(), FolderName);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

        public static string GetFilePath(IStreamInfo streamInfo, string path)
        {
            if (streamInfo is null || string.IsNullOrWhiteSpace(path))
                return string.Empty;

            return Path.Combine(path, $"audio_{Guid.CreateVersion7():N}.{streamInfo.Container}");
        }
    }
}
