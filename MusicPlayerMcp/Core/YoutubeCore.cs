using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace MusicPlayerMcp.Core
{
    public class YoutubeCore
    {
        public static async Task<string> DownloadAudioAsync(string musicRequest, CancellationToken cancellationToken = default)
        {
            if (!ValidationUtils.IsValidUri(musicRequest))
            {
                musicRequest = await GetUriAsync(musicRequest, cancellationToken);

                if (!ValidationUtils.IsValidUri(musicRequest))
                    return string.Empty;
            }

            using var youtube = new YoutubeClient();

            // Get manifest and select highest quality muxed stream
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(musicRequest, cancellationToken);

            if (streamManifest is null)
                return string.Empty;

            var streamInfo = streamManifest.GetAudioStreams().GetWithHighestBitrate();

            var path = FileUtils.GetMusicsFolder();
            var filePath = FileUtils.GetFilePath(streamInfo, path);

            // Download the stream
            await youtube.Videos.Streams.DownloadAsync(streamInfo, filePath, cancellationToken: cancellationToken);
            return filePath;
        }

        private static async Task<string> GetUriAsync(string musicRequest, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(musicRequest))
                return string.Empty;

            using var youtube = new YoutubeClient();

            // Get the first batch of video search results
            var videos = youtube.Search.GetVideosAsync(musicRequest, cancellationToken);
            var firstMatch = await videos.FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (firstMatch is null)
                return string.Empty;

            return firstMatch.Url;
        }
    }
}
