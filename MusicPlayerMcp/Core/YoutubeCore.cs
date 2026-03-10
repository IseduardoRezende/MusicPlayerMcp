using YoutubeExplode;
using MusicPlayerMcp.Tools;
using YoutubeExplode.Videos;
using Microsoft.Extensions.Logging;
using YoutubeExplode.Videos.Streams;

namespace MusicPlayerMcp.Core
{
    public class YoutubeCore
    {
        public static async Task<bool> SearchAndPlayAsync(string musicRequest, ILogger<MusicPlayerTool> logger, CancellationToken cancellationToken = default)
        {
            if (!ValidationUtils.IsValidUri(musicRequest))
            {
                musicRequest = await GetUrlAsync(musicRequest, cancellationToken);

                if (!ValidationUtils.IsValidUri(musicRequest))
                    return false;
            }

            if (FileUtils.TryGetFilePath(VideoId.TryParse(musicRequest), out string? filePath))
            {
                await AudioPlayerCore.PlayFromFilePathAsync(filePath, logger, cancellationToken);
                return true;
            }

            using var youtube = new YoutubeClient();

            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(musicRequest, cancellationToken);

            if (streamManifest is null)
                return false;

            var streamInfo = streamManifest.GetAudioStreams().GetWithHighestBitrate();

            await AudioPlayerCore.PlayFromUrlAsync(streamInfo.Url, logger, cancellationToken);
            return true;
        }

        public static async Task<string> SearchAndDownloadAsync(string musicRequest, ILogger<MusicPlayerTool> logger, CancellationToken cancellationToken)
        {
            if (!ValidationUtils.IsValidUri(musicRequest))
            {
                musicRequest = await GetUrlAsync(musicRequest, cancellationToken);

                if (!ValidationUtils.IsValidUri(musicRequest))
                    return string.Empty;
            }

            if (FileUtils.TryGetFilePath(VideoId.TryParse(musicRequest), out string? filePath))
                return filePath!;

            using var youtube = new YoutubeClient();

            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(musicRequest, cancellationToken);

            if (streamManifest is null)
                return string.Empty;

            var streamInfo = streamManifest.GetAudioStreams().GetWithHighestBitrate();

            filePath = FileUtils.GetFilePath(streamInfo, VideoId.TryParse(musicRequest));

            await youtube.Videos.Streams.DownloadAsync(streamInfo, filePath, cancellationToken: cancellationToken);
            return filePath;
        }

        private static async Task<string> GetUrlAsync(string musicRequest, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(musicRequest))
                return string.Empty;

            using var youtube = new YoutubeClient();

            var videos = youtube.Search.GetVideosAsync(musicRequest, cancellationToken);
            var firstMatch = await videos.FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (firstMatch is null)
                return string.Empty;

            return firstMatch.Url;
        }
    }
}
